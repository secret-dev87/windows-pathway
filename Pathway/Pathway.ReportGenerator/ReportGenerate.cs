using log4net;
using OfficeOpenXml;
using Pathway.Core.Abstract;
using Pathway.Core.Abstract.Services;
using Pathway.Core.Infrastructure;
using Pathway.Core.RemoteAnalyst.Concrete;
using Pathway.Core.Services;
using Pathway.ReportGenerator.Excel;
using Pathway.ReportGenerator.Infrastructure;
using Pathway.ReportGenerator.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Pathway.ReportGenerator
{
    public class ReportGenerate : ReportNames
    {
        private readonly string _connectionStringMain = "";
        private readonly string _connectionStringSystem = "";
        private readonly DateTime _fromTimestamp;
        private readonly long _intervalInSec;
        private readonly object _misValue = System.Reflection.Missing.Value;
        private readonly ILog _log;
        private readonly List<string> _pathwayList;
        private readonly string _saveLocation;
        private readonly DateTime _toTimestamp;
        private readonly string _systemSerial;
        private readonly string _systemName;
        private readonly int _ipu;
        private readonly int _reportDownloadId;
        private readonly bool _isLocalAnalyst;
        private ReportDownloadLogs _reportDownloadLogs;

        public ReportGenerate(ILog log, string saveLocation, string connectionString, string connectionStringSystem, DateTime fromTimestamp, DateTime toTimestamp, List<string> pathwayList, long intervalInSec, string systemSerial, int reportDownloadId, bool isLocalAnalyst)
        {
            _log = log;
            _connectionStringMain = connectionString;
            _connectionStringSystem = connectionStringSystem;
            _fromTimestamp = fromTimestamp;
            _toTimestamp = toTimestamp;
            _pathwayList = pathwayList;
            _intervalInSec = intervalInSec;
            _saveLocation = saveLocation + @"\Pathway_" + DateTime.Now.Ticks;
            _systemSerial = systemSerial;

            var systemTbl = new SystemTbl();
            _systemName = systemTbl.GetSystemName(systemSerial).Replace('\\', ' ').Trim();

            var cpu = new CPU();
            var tableName = cpu.GetLatestTableName();
            _ipu = cpu.GetIPU(tableName);
            _reportDownloadId = reportDownloadId;

            _isLocalAnalyst = isLocalAnalyst;

            StringBuilder sbLog = new StringBuilder();
            sbLog.AppendLine("_connectionStringMain: {0}");
            sbLog.AppendLine("_connectionStringSystem: {1}");
            sbLog.AppendLine("_fromTimestamp: {2}");
            sbLog.AppendLine("_toTimestamp: {3}");
            sbLog.AppendLine("_intervalInSec: {4}");
            sbLog.AppendLine("_saveLocation: {5}");
            sbLog.AppendLine("_reportDownloadId: {6}");
            _log.InfoFormat(sbLog.ToString(),
                ReportGenerate.RemovePassword(_connectionStringMain),
                ReportGenerate.RemovePassword(_connectionStringSystem),
                _fromTimestamp, _toTimestamp, _intervalInSec,
                _saveLocation, _reportDownloadId);

            foreach (var pathway in _pathwayList)
            {
                _log.InfoFormat("_pathwayList: {0}", pathway);
            }

            if (!Directory.Exists(_saveLocation)) Directory.CreateDirectory(_saveLocation);

            //Move images to directory.
            Resource1.collapse.Save(_saveLocation + @"\collapse.gif");
            Resource1.expand.Save(_saveLocation + @"\expand.gif");
            Resource1.view.Save(_saveLocation + @"\view.gif");
            Resource1.home.Save(_saveLocation + @"\home.png");
            Resource1.navigate_left.Save(_saveLocation + @"\navigate_left.png");
            Resource1.navigate_left2.Save(_saveLocation + @"\navigate_left2.png");
            Resource1.navigate_right.Save(_saveLocation + @"\navigate_right.png");
            Resource1.navigate_right2.Save(_saveLocation + @"\navigate_right2.png");
            Resource1.help.Save(_saveLocation + @"\help.gif");

        }

        public static string RemovePassword(string connectionString)
        {
            try
            {
                if (String.IsNullOrEmpty(connectionString))
                {
                    return connectionString;
                }
                if ((connectionString.Contains("PASSWORD") && connectionString.Contains(";")) || (connectionString.Contains("password") && connectionString.Contains(";")))
                {
                    List<string> strlist = connectionString.Split(';').ToList();
                    for (int i = 0; i < strlist.Count; i++)
                    {
                        if (strlist[i].Contains("PASSWORD") || connectionString.Contains("password"))
                        {
                            strlist.Remove(strlist[i]);
                            break;
                        }
                    }
                    string concat = String.Join(";", strlist.ToArray());
                    return concat;
                }
                else
                {
                    return connectionString;
                }
            }
            catch (Exception e)
            {
                return connectionString;
            }
        }

        public string CreateExcelReport()
        {
            IPathwayAlertService alertListService = new PathwayAlertService();
            var alertList = alertListService.GetAllAlertsFor();
            //GeneratePerPathwayDetailMultiDays(alertList);
            //GenerateAllPathwayCollection(alertList);

            _log.InfoFormat("-------------------Generate Daily report at: {0}", DateTime.Now);

            IPvCollectsService collectService = new PvCollectsService();
            bool isMultiDays = false;
            var tsSpan = _toTimestamp - _fromTimestamp;
            if (tsSpan.TotalDays > 1)
            {
                isMultiDays = true;
            }
            //Generate Daily report first.
            //If request dates > 1 day skip DetailDaily
            if (!isMultiDays)
            {
                for (var dtStart = _fromTimestamp; dtStart.Date < _toTimestamp.Date; dtStart = dtStart.AddDays(1))
                {
                    //Check if ther is a data on selected date.
                    var exits = collectService.IsDuplictedFor(dtStart, dtStart.AddDays(1));
                    if (!exits) continue;

                    //GeneratePerPathwayDetailDaily(alertList, dtStart, dtStart.AddDays(1));

                    GeneratePerPathwayDetailDailyInParallel(alertList, dtStart, dtStart.AddDays(1));
                }
            }
            _log.InfoFormat("----------------------Finished Generating Daily report at: {0}", DateTime.Now);
            _log.InfoFormat("isMultiDays: {0}", isMultiDays);

            if (_pathwayList.Count == 1 && isMultiDays)
            {
                _log.InfoFormat("Calling GeneratePerPathwayDetailMultiDays at: {0}", DateTime.Now);
                GeneratePerPathwayDetailMultiDays(alertList);
            }
            else if (_pathwayList.Count > 1 && !isMultiDays)
            {
                _log.InfoFormat("Calling GenerateAllPathwayCollection at: {0}", DateTime.Now);
                _reportDownloadLogs = new ReportDownloadLogs();
                _reportDownloadLogs.InsertNewLog(_reportDownloadId, DateTime.Now, "Generating Collection report");
                GenerateAllPathwayCollection(alertList, isMultiDays);
            }
            else if (_pathwayList.Count > 1 && isMultiDays)
            {
                //Don't need multidays RA-1487
                //_log.InfoFormat("Calling GeneratePerPathwayDetailMultiDays at: {0}", DateTime.Now);
                //
                //GeneratePerPathwayDetailMultiDays(alertList);

                _log.InfoFormat("Calling GenerateAllPathwayCollection at: {0}", DateTime.Now);
                _reportDownloadLogs = new ReportDownloadLogs();
                _reportDownloadLogs.InsertNewLog(_reportDownloadId, DateTime.Now, "Generating Collection report");
                GenerateAllPathwayCollection(alertList, isMultiDays);
            }
            return _saveLocation;
        }

        public string CreateExcelAlertReport()
        {
            IPathwayAlertService alertListService = new PathwayAlertService();
            var alertList = alertListService.GetAlertsFor();
            //var alertList = new List<string> { "ServerPendingClass", "ServerPendingProcess", "ServerQueueTCP", "ServerQueueLinkmon", "ServerUnusedClass", "ServerUnusedProcess", "ServerMaxLinks", "CheckDirectoryOn", "HighDynamicServers", "ServerErrorList" };

            string alertReportLocation = GenerateAllPathwayCollectionAlert(alertList);
            return alertReportLocation;
        }

        internal string GenerateAllPathwayCollectionAlert(List<string> alertList)
        {

            ExcelPackage xlWorkPackage = new ExcelPackage();
            var worksheetCount = 1;

            IPvAlertService alertService = new PvAlertService(_intervalInSec);


            _reportDownloadLogs = new ReportDownloadLogs();
            _reportDownloadLogs.InsertNewLog(_reportDownloadId, DateTime.Now, "Fetching list of Alerts");
            var excelChart = new ExcelChartAlert(_reportDownloadId, _reportDownloadLogs, _log);
            //Create Collection.
            foreach (var alert in alertList)
            {
                _log.InfoFormat("GetCollectionAlertFor: {0}", alert);
            }
            var alerts = alertService.GetCollectionAlertFor(alertList, _fromTimestamp, _toTimestamp);

            _log.InfoFormat("Alert Count: {0}", alerts.Count);


            var worksheetNames = new Dictionary<string, Collection>();
            //Get worksheet names.
            foreach (var pathway in alerts)
            {
                var reports = new Collection
                {
                    ServerPendingClass = pathway.Key + PathwayCollectionServerPendingClass,
                    ServerPendingProcess = pathway.Key + PathwayCollectionServerPendingProcess,
                    ServerQueuedTCP = pathway.Key + PathwayCollectionServerQueTCP,
                    ServerQueuedLinkmon = pathway.Key + PathwayCollectionServerQueLinkmon,
                    ServerUnusedServerClasses = pathway.Key + PathwayCollectionServerUnuseClass,
                    ServerUnusedServerProcesses = pathway.Key + PathwayCollectionServerUnusePro,
                    ServerMaxLinks = pathway.Key + PathwayCollectionServerMaxLinks,
                    CheckDirectoryOnLinks = pathway.Key + PathwayCollectionCheckDirectoryOnLinks,
                    HighDynamicServers = pathway.Key + PathwayCollectionHighDynamicServers,
                    ServerErrorList = pathway.Key + PathwayCollectionServerErrorList
                };
                worksheetNames.Add(pathway.Key, reports);
            }
            string dateRange = "";
            if (!_fromTimestamp.Date.Equals(_toTimestamp.Date))
                dateRange = _fromTimestamp.ToString("yyyy-MM-dd") + " through " + _toTimestamp.AddDays(-1).ToString("yyyy-MM-dd");
            else
                dateRange = _fromTimestamp.ToString("yyyy-MM-dd");

            excelChart.InsertWorksheetAlert(xlWorkPackage.Workbook, _saveLocation, worksheetCount, alerts, CollectionAlerts, CollectionAlertsTitle + dateRange, worksheetNames, _isLocalAnalyst);
            worksheetCount++;

            var alertsWithData = alerts.Where(x => x.Value.ServerPendingClass > 0 ||
                                                   x.Value.ServerPendingProcess > 0 ||
                                                   x.Value.ServerQueueTcp > 0 ||
                                                   x.Value.ServerQueueLinkmon > 0 ||
                                                   x.Value.ServerUnusedClass > 0 ||
                                                   x.Value.ServerUnusedProcess > 0 ||
                                                   x.Value.ServerMaxLinks > 0 ||
                                                   x.Value.DirectoryOnLinks > 0 ||
                                                   x.Value.HighDynamicServers > 0 ||
                                                   x.Value.ServerErrorList > 0).ToDictionary(x => x.Key, x => x.Value);

            var errorList = new List<long>();

            int pathwayCount = 0;

            //Get list of days with data.
            IPvCollectsService collectService = new PvCollectsService();
            var datesWithData = collectService.GetDatesWithDataFor(_fromTimestamp, _toTimestamp);
            StringBuilder sbLog = new StringBuilder();
            //Create Daily Alerts.
            foreach (var pathway in alertsWithData)
            {
                _reportDownloadLogs.InsertNewLog(_reportDownloadId, DateTime.Now, "Generating report for " + pathway.Key + " (" + (pathwayCount + 1) + " of " + alertsWithData.Count + ")");
                string nextWorksheet = "";
                string previousWorksheet = "";
                string nextPathway = "";
                string proviousPathway = "";

                #region Get Next & Previous Pathway Link
                //Get Next Pathway
                if ((pathwayCount + 1) < alertsWithData.Count)
                {
                    //Get next Pathway.
                    var nextKey = alertsWithData.Keys.ElementAt(pathwayCount + 1);
                    var nextPathwayInfo = alertsWithData[nextKey];

                    if (nextPathwayInfo.ServerPendingClass > 0)
                        nextPathway = nextKey + PathwayCollectionServerPendingClass;
                    else if (nextPathwayInfo.ServerPendingProcess > 0)
                        nextPathway = nextKey + PathwayCollectionServerPendingProcess;
                    else if (nextPathwayInfo.ServerQueueTcp > 0)
                        nextPathway = nextKey + PathwayCollectionServerQueTCP;
                    else if (nextPathwayInfo.ServerQueueLinkmon > 0)
                        nextPathway = nextKey + PathwayCollectionServerQueLinkmon;
                    else if (nextPathwayInfo.ServerUnusedClass > 0)
                        nextPathway = nextKey + PathwayCollectionServerUnuseClass;
                    else if (nextPathwayInfo.ServerUnusedProcess > 0)
                        nextPathway = nextKey + PathwayCollectionServerUnusePro;
                    else if (nextPathwayInfo.ServerMaxLinks > 0)
                        nextPathway = nextKey + PathwayCollectionServerMaxLinks;
                    else if (nextPathwayInfo.DirectoryOnLinks > 0)
                        nextPathway = nextKey + PathwayCollectionCheckDirectoryOnLinks;
                    else if (nextPathwayInfo.HighDynamicServers > 0)
                        nextPathway = nextKey + PathwayCollectionHighDynamicServers;
                    else if (nextPathwayInfo.ServerErrorList > 0)
                        nextPathway = nextKey + PathwayCollectionServerErrorList;
                }
                else nextWorksheet = "";

                //Get Previous Pathway
                if (pathwayCount != 0)
                {
                    var nextKey = alertsWithData.Keys.ElementAt(pathwayCount - 1);
                    var nextPathwayInfo = alertsWithData[nextKey];

                    if (nextPathwayInfo.ServerPendingClass > 0)
                        proviousPathway = nextKey + PathwayCollectionServerPendingClass;
                    else if (nextPathwayInfo.ServerPendingProcess > 0)
                        proviousPathway = nextKey + PathwayCollectionServerPendingProcess;
                    else if (nextPathwayInfo.ServerQueueTcp > 0)
                        proviousPathway = nextKey + PathwayCollectionServerQueTCP;
                    else if (nextPathwayInfo.ServerQueueLinkmon > 0)
                        proviousPathway = nextKey + PathwayCollectionServerQueLinkmon;
                    else if (nextPathwayInfo.ServerUnusedClass > 0)
                        proviousPathway = nextKey + PathwayCollectionServerUnuseClass;
                    else if (nextPathwayInfo.ServerUnusedProcess > 0)
                        proviousPathway = nextKey + PathwayCollectionServerUnusePro;
                    else if (nextPathwayInfo.ServerMaxLinks > 0)
                        proviousPathway = nextKey + PathwayCollectionServerMaxLinks;
                    else if (nextPathwayInfo.DirectoryOnLinks > 0)
                        proviousPathway = nextKey + PathwayCollectionCheckDirectoryOnLinks;
                    else if (nextPathwayInfo.HighDynamicServers > 0)
                        proviousPathway = nextKey + PathwayCollectionHighDynamicServers;
                    else if (nextPathwayInfo.ServerErrorList > 0)
                        proviousPathway = nextKey + PathwayCollectionServerErrorList;
                }
                #endregion
                sbLog.Clear();
                sbLog.AppendLine("Pathway: {0}");
                sbLog.AppendLine("ServerPendingClass: {1}");
                sbLog.AppendLine("ServerPendingProcess: {2}");
                sbLog.AppendLine("ServerQueueTcp: {3}");
                sbLog.AppendLine("ServerQueueLinkmon: {4}");
                sbLog.AppendLine("ServerUnusedClass: {5}");
                sbLog.AppendLine("ServerUnusedProcess: {6}");
                sbLog.AppendLine("ServerMaxLinks: {7}");
                sbLog.AppendLine("DirectoryOnLinks: {8}");
                sbLog.AppendLine("HighDynamicServers: {9}");
                sbLog.AppendLine("ServerErrorList: {10}");
                _log.InfoFormat(sbLog.ToString(), pathway.Key,
                    pathway.Value.ServerPendingClass, pathway.Value.ServerPendingProcess,
                    pathway.Value.ServerQueueTcp, pathway.Value.ServerQueueLinkmon,
                    pathway.Value.ServerUnusedClass, pathway.Value.ServerUnusedProcess,
                    pathway.Value.ServerMaxLinks, pathway.Value.DirectoryOnLinks,
                    pathway.Value.HighDynamicServers, pathway.Value.ServerErrorList);

                //Foreach alert, create Summery and Detail excel report.
                if (pathway.Value.ServerPendingClass > 0)
                {
                    #region ServerPendingClass
                    if (pathway.Value.ServerPendingProcess > 0)
                        nextWorksheet = pathway.Key + PathwayCollectionServerPendingProcess;
                    else if (pathway.Value.ServerQueueTcp > 0)
                        nextWorksheet = pathway.Key + PathwayCollectionServerQueTCP;
                    else if (pathway.Value.ServerQueueLinkmon > 0)
                        nextWorksheet = pathway.Key + PathwayCollectionServerQueLinkmon;
                    else if (pathway.Value.ServerUnusedClass > 0)
                        nextWorksheet = pathway.Key + PathwayCollectionServerUnuseClass;
                    else if (pathway.Value.ServerUnusedProcess > 0)
                        nextWorksheet = pathway.Key + PathwayCollectionServerUnusePro;
                    else if (pathway.Value.ServerMaxLinks > 0)
                        nextWorksheet = pathway.Key + PathwayCollectionServerMaxLinks;
                    else if (pathway.Value.DirectoryOnLinks > 0)
                        nextWorksheet = pathway.Key + PathwayCollectionCheckDirectoryOnLinks;
                    else if (pathway.Value.HighDynamicServers > 0)
                        nextWorksheet = pathway.Key + PathwayCollectionHighDynamicServers;
                    else if (pathway.Value.ServerErrorList > 0)
                        nextWorksheet = pathway.Key + PathwayCollectionServerErrorList;
                    else
                        nextWorksheet = "";

                    IPerPathwayServerService serverService = new PerPathwayServerService(_intervalInSec);
                    var pendingClass = serverService.GetServerPendingClassIntervalFor(_fromTimestamp, _toTimestamp, pathway.Key, Enums.IntervalTypes.Daily, datesWithData);
                    excelChart.InsertWorksheetPendingClassInterval(xlWorkPackage.Workbook, worksheetCount, pathway.Key, pendingClass, pathway.Key + PathwayCollectionServerPendingClass, _saveLocation,
                                                                        proviousPathway, "", nextWorksheet, nextPathway,
                                                                        CollectionAlerts, pathway.Key + PathwayCollectionServerPendingClassTitle + dateRange, _isLocalAnalyst);
                    worksheetCount++;

                    #endregion
                }

                if (pathway.Value.ServerPendingProcess > 0)
                {
                    #region ServerPendingProcess

                    if (pathway.Value.ServerPendingClass > 0)
                        previousWorksheet = pathway.Key + PathwayCollectionServerPendingClass;

                    if (pathway.Value.ServerQueueTcp > 0)
                        nextWorksheet = pathway.Key + PathwayCollectionServerQueTCP;
                    else if (pathway.Value.ServerQueueLinkmon > 0)
                        nextWorksheet = pathway.Key + PathwayCollectionServerQueLinkmon;
                    else if (pathway.Value.ServerUnusedClass > 0)
                        nextWorksheet = pathway.Key + PathwayCollectionServerUnuseClass;
                    else if (pathway.Value.ServerUnusedProcess > 0)
                        nextWorksheet = pathway.Key + PathwayCollectionServerUnusePro;
                    else if (pathway.Value.ServerMaxLinks > 0)
                        nextWorksheet = pathway.Key + PathwayCollectionServerMaxLinks;
                    else if (pathway.Value.DirectoryOnLinks > 0)
                        nextWorksheet = pathway.Key + PathwayCollectionCheckDirectoryOnLinks;
                    else if (pathway.Value.HighDynamicServers > 0)
                        nextWorksheet = pathway.Key + PathwayCollectionHighDynamicServers;
                    else if (pathway.Value.ServerErrorList > 0)
                        nextWorksheet = pathway.Key + PathwayCollectionServerErrorList;
                    else
                        nextWorksheet = "";

                    IPerPathwayServerService serverService = new PerPathwayServerService(_intervalInSec);
                    var unusedClass = serverService.GetServerPendingProcessIntervalFor(_fromTimestamp, _toTimestamp, pathway.Key, Enums.IntervalTypes.Daily, datesWithData);
                    excelChart.InsertWorksheetPendingProcessInterval(xlWorkPackage.Workbook, worksheetCount, pathway.Key, unusedClass, pathway.Key + PathwayCollectionServerPendingProcess, _saveLocation,
                                                                        proviousPathway, previousWorksheet, nextWorksheet, nextPathway,
                                                                        CollectionAlerts, pathway.Key + PathwayCollectionServerPendingProTitle + dateRange, _isLocalAnalyst);
                    worksheetCount++;

                    #endregion
                }

                if (pathway.Value.ServerQueueTcp > 0)
                {
                    #region ServerQueueTcp

                    if (pathway.Value.ServerPendingProcess > 0)
                        previousWorksheet = pathway.Key + PathwayCollectionServerPendingProcess;
                    else if (pathway.Value.ServerPendingClass > 0)
                        previousWorksheet = pathway.Key + PathwayCollectionServerPendingClass;

                    IPerPathwayServerService serverService = new PerPathwayServerService(_intervalInSec);
                    var queueTcp = serverService.GetServerQueueTCPIntervalFor(_fromTimestamp, _toTimestamp, pathway.Key, Enums.IntervalTypes.Daily, datesWithData);
                    var subDatas = alertService.GetQueueTCPSubFor(_fromTimestamp, _toTimestamp, pathway.Key, Enums.IntervalTypes.Daily, datesWithData);

                    //Check Next worksheet.
                    if (subDatas.Count(x => x.Value.Count > 0) > 0)
                    {
                        var tempData = subDatas.First(subData => subData.Value.Count > 0);
                        nextWorksheet = pathway.Key + PathwayCollectionServerQueTCPSub + Convert.ToDateTime(tempData.Key).ToString("yy-MM-dd");
                    }

                    excelChart.InsertWorksheetQueues(xlWorkPackage.Workbook, worksheetCount, pathway.Key, pathway.Key + PathwayCollectionServerQueTCP, _saveLocation,
                                                    proviousPathway, previousWorksheet, nextWorksheet, nextPathway,
                                                    CollectionAlerts, pathway.Key + PathwayCollectionServerQueTCPTitle + dateRange, subDatas, Enums.IntervalTypes.Daily, _isLocalAnalyst, queueTcp);
                    worksheetCount++;

                    var subCount = 0;
                    //Get only data with values.
                    var subDatasWithValue = subDatas.Where(subData => subData.Value.Count > 0).ToDictionary(x => x.Key, x => x.Value);

                    foreach (var subData in subDatasWithValue)
                    {
                        //Get Previous
                        if (subCount == 0)
                            previousWorksheet = pathway.Key + PathwayCollectionServerQueTCP;
                        else
                        {
                            string previouKey = subDatasWithValue.Keys.ElementAt(subCount - 1);
                            previousWorksheet = pathway.Key + PathwayCollectionServerQueTCPSub + Convert.ToDateTime(previouKey).ToString("yy-MM-dd");
                        }

                        //Get Next worksheet.
                        if ((subCount + 1) < subDatasWithValue.Count)
                        {
                            string nextKey = subDatasWithValue.Keys.ElementAt(subCount + 1);
                            nextWorksheet = pathway.Key + PathwayCollectionServerQueTCPSub + Convert.ToDateTime(nextKey).ToString("yy-MM-dd");
                        }
                        else
                        {
                            if (pathway.Value.ServerQueueLinkmon > 0)
                                nextWorksheet = pathway.Key + PathwayCollectionServerQueLinkmon;
                            else if (pathway.Value.ServerUnusedClass > 0)
                                nextWorksheet = pathway.Key + PathwayCollectionServerUnuseClass;
                            else if (pathway.Value.ServerUnusedProcess > 0)
                                nextWorksheet = pathway.Key + PathwayCollectionServerUnusePro;
                            else if (pathway.Value.ServerMaxLinks > 0)
                                nextWorksheet = pathway.Key + PathwayCollectionServerMaxLinks;
                            else if (pathway.Value.DirectoryOnLinks > 0)
                                nextWorksheet = pathway.Key + PathwayCollectionCheckDirectoryOnLinks;
                            else if (pathway.Value.HighDynamicServers > 0)
                                nextWorksheet = pathway.Key + PathwayCollectionHighDynamicServers;
                            else if (pathway.Value.ServerErrorList > 0)
                                nextWorksheet = pathway.Key + PathwayCollectionServerErrorList;
                            else
                                nextWorksheet = "";

                        }

                        excelChart.InsertWorksheetQueueSubs(xlWorkPackage.Workbook, worksheetCount, pathway.Key, pathway.Key + PathwayCollectionServerQueTCPSub + Convert.ToDateTime(subData.Key).ToString("yy-MM-dd"), _saveLocation,
                                                            proviousPathway, previousWorksheet, nextWorksheet, nextPathway,
                                                            CollectionAlerts, pathway.Key + PathwayCollectionServerQueTCPTitleSub + subData.Key, subData.Value, false, _isLocalAnalyst);
                        worksheetCount++;
                        subCount++;
                    }

                    //Get previous worksheet for the ServerQueueLinkmon 
                    if (subDatas.Count(x => x.Value.Count > 0) > 0)
                    {
                        var tempData = subDatas.Last(subData => subData.Value.Count > 0);
                        previousWorksheet = pathway.Key + PathwayCollectionServerQueTCPSub + Convert.ToDateTime(tempData.Key).ToString("yy-MM-dd");
                    }
                    //else previousWorksheet = "";

                    #endregion
                }

                if (pathway.Value.ServerQueueLinkmon > 0)
                {
                    #region ServerQueueLinkmon

                    if (pathway.Value.ServerQueueTcp == 0)
                    {
                        if (pathway.Value.ServerPendingProcess > 0)
                            previousWorksheet = pathway.Key + PathwayCollectionServerPendingProcess;
                        else if (pathway.Value.ServerPendingClass > 0)
                            previousWorksheet = pathway.Key + PathwayCollectionServerPendingClass;
                    }

                    IPerPathwayServerService serverService = new PerPathwayServerService(_intervalInSec);
                    var queueLinkmon = serverService.GetServerQueueLinkmonIntervalFor(_fromTimestamp, _toTimestamp, pathway.Key, Enums.IntervalTypes.Daily, datesWithData);
                    var subDatas = alertService.GetQueueLinkmonSubFor(_fromTimestamp, _toTimestamp, pathway.Key, Enums.IntervalTypes.Daily, datesWithData);
                    //Check Next worksheet.
                    if (subDatas.Count(x => x.Value.Count > 0) > 0)
                    {
                        var tempData = subDatas.First(subData => subData.Value.Count > 0);
                        nextWorksheet = pathway.Key + PathwayCollectionServerQueLinkmonSub + Convert.ToDateTime(tempData.Key).ToString("yy-MM-dd");
                    }

                    excelChart.InsertWorksheetQueues(xlWorkPackage.Workbook, worksheetCount, pathway.Key, pathway.Key + PathwayCollectionServerQueLinkmon, _saveLocation,
                                                    proviousPathway, previousWorksheet, nextWorksheet, nextPathway,
                                                    CollectionAlerts, pathway.Key + PathwayCollectionServerQueLinkmonTitle + dateRange, subDatas, Enums.IntervalTypes.Daily, _isLocalAnalyst, null, queueLinkmon);
                    worksheetCount++;

                    var subCount = 0;
                    //Get only data with values.
                    var subDatasWithValue = subDatas.Where(subData => subData.Value.Count > 0).ToDictionary(x => x.Key, x => x.Value);

                    foreach (var subData in subDatasWithValue)
                    {
                        //Get Previous
                        if (subCount == 0)
                            previousWorksheet = pathway.Key + PathwayCollectionServerQueLinkmon;
                        else
                        {
                            string previouKey = subDatasWithValue.Keys.ElementAt(subCount - 1);
                            previousWorksheet = pathway.Key + PathwayCollectionServerQueLinkmonSub + Convert.ToDateTime(previouKey).ToString("yy-MM-dd");
                        }

                        //Get Next worksheet.
                        if ((subCount + 1) < subDatasWithValue.Count)
                        {
                            string nextKey = subDatasWithValue.Keys.ElementAt(subCount + 1);
                            nextWorksheet = pathway.Key + PathwayCollectionServerQueLinkmonSub + Convert.ToDateTime(nextKey).ToString("yy-MM-dd");
                        }
                        else
                        {
                            if (pathway.Value.ServerUnusedClass > 0)
                                nextWorksheet = pathway.Key + PathwayCollectionServerUnuseClass;
                            else if (pathway.Value.ServerUnusedProcess > 0)
                                nextWorksheet = pathway.Key + PathwayCollectionServerUnusePro;
                            else if (pathway.Value.ServerMaxLinks > 0)
                                nextWorksheet = pathway.Key + PathwayCollectionServerMaxLinks;
                            else if (pathway.Value.DirectoryOnLinks > 0)
                                nextWorksheet = pathway.Key + PathwayCollectionCheckDirectoryOnLinks;
                            else if (pathway.Value.HighDynamicServers > 0)
                                nextWorksheet = pathway.Key + PathwayCollectionHighDynamicServers;
                            else if (pathway.Value.ServerErrorList > 0)
                                nextWorksheet = pathway.Key + PathwayCollectionServerErrorList;
                            else
                                nextWorksheet = "";
                        }

                        excelChart.InsertWorksheetQueueSubs(xlWorkPackage.Workbook, worksheetCount, pathway.Key, pathway.Key + PathwayCollectionServerQueLinkmonSub + Convert.ToDateTime(subData.Key).ToString("yy-MM-dd"), _saveLocation,
                                                            proviousPathway, previousWorksheet, nextWorksheet, nextPathway,
                                                            CollectionAlerts, pathway.Key + PathwayCollectionServerQueLinkmonTitleSub + subData.Key, subData.Value, true, _isLocalAnalyst);
                        worksheetCount++;
                        subCount++;
                    }

                    //Get previous worksheet for the ServerUnusedClass 
                    if (subDatas.Count(x => x.Value.Count > 0) > 0)
                    {
                        var tempData = subDatas.Last(subData => subData.Value.Count > 0);
                        previousWorksheet = pathway.Key + PathwayCollectionServerQueLinkmonSub + Convert.ToDateTime(tempData.Key).ToString("yy-MM-dd");
                    }
                    #endregion
                }

                if (pathway.Value.ServerUnusedClass > 0)
                {
                    #region ServerUnusedClass

                    if (pathway.Value.ServerQueueTcp == 0)
                    {
                        if (pathway.Value.ServerPendingProcess > 0)
                            previousWorksheet = pathway.Key + PathwayCollectionServerPendingProcess;
                        else if (pathway.Value.ServerPendingClass > 0)
                            previousWorksheet = pathway.Key + PathwayCollectionServerPendingClass;
                    }

                    if (pathway.Value.ServerUnusedProcess > 0)
                        nextWorksheet = pathway.Key + PathwayCollectionServerUnusePro;
                    else if (pathway.Value.ServerMaxLinks > 0)
                        nextWorksheet = pathway.Key + PathwayCollectionServerMaxLinks;
                    else if (pathway.Value.DirectoryOnLinks > 0)
                        nextWorksheet = pathway.Key + PathwayCollectionCheckDirectoryOnLinks;
                    else if (pathway.Value.HighDynamicServers > 0)
                        nextWorksheet = pathway.Key + PathwayCollectionHighDynamicServers;
                    else if (pathway.Value.ServerErrorList > 0)
                        nextWorksheet = pathway.Key + PathwayCollectionServerErrorList;
                    else
                        nextWorksheet = "";

                    //There is no detail for Unused Class
                    var excelChartServer = new ExcelChartServer(_reportDownloadId, _reportDownloadLogs, _log);
                    IPerPathwayServerService serverService = new PerPathwayServerService(_intervalInSec);
                    var unusedClass = serverService.GetServerUnusedClassesIntervalFor(_fromTimestamp, _toTimestamp, pathway.Key, Enums.IntervalTypes.Daily, datesWithData);
                    excelChartServer.InsertWorksheetUnusedClassInterval(xlWorkPackage.Workbook, worksheetCount, pathway.Key, unusedClass, pathway.Key + PathwayCollectionServerUnuseClass, _saveLocation,
                                                                        proviousPathway, previousWorksheet, nextWorksheet, nextPathway,
                                                                        CollectionAlerts, pathway.Key + PathwayCollectionServerUnuseClassTitle + dateRange, _isLocalAnalyst);
                    worksheetCount++;
                    #endregion
                }

                if (pathway.Value.ServerUnusedProcess > 0)
                {
                    #region ServerUnusedProcess

                    if (pathway.Value.ServerQueueTcp == 0)
                    {
                        if (pathway.Value.ServerUnusedClass > 0)
                            previousWorksheet = pathway.Key + PathwayCollectionServerUnuseClass;
                        else if (pathway.Value.ServerPendingProcess > 0)
                            previousWorksheet = pathway.Key + PathwayCollectionServerPendingProcess;
                        else if (pathway.Value.ServerPendingClass > 0)
                            previousWorksheet = pathway.Key + PathwayCollectionServerPendingClass;
                    }

                    if (pathway.Value.ServerMaxLinks > 0)
                        nextWorksheet = pathway.Key + PathwayCollectionServerMaxLinks;
                    else if (pathway.Value.DirectoryOnLinks > 0)
                        nextWorksheet = pathway.Key + PathwayCollectionCheckDirectoryOnLinks;
                    else if (pathway.Value.HighDynamicServers > 0)
                        nextWorksheet = pathway.Key + PathwayCollectionHighDynamicServers;
                    else if (pathway.Value.ServerErrorList > 0)
                        nextWorksheet = pathway.Key + PathwayCollectionServerErrorList;
                    else
                        nextWorksheet = "";

                    //There is no detail for Unused Process
                    var unusedProcess = alertService.GetUnusedServerProcessesesFor(_fromTimestamp, _toTimestamp, pathway.Key, Enums.IntervalTypes.Daily, datesWithData);
                    excelChart.InsertWorksheetUnusedProcessInterval(xlWorkPackage.Workbook, worksheetCount, pathway.Key, unusedProcess, pathway.Key + PathwayCollectionServerUnusePro, _saveLocation,
                                                                    proviousPathway, previousWorksheet, nextWorksheet, nextPathway,
                                                                    CollectionAlerts, pathway.Key + PathwayCollectionServerUnuseProTitle + dateRange, _isLocalAnalyst);
                    worksheetCount++;
                    #endregion
                }

                if (pathway.Value.ServerMaxLinks > 0)
                {
                    #region ServerMaxLinks

                    if (pathway.Value.ServerQueueTcp == 0)
                    {
                        if (pathway.Value.ServerUnusedProcess > 0)
                            previousWorksheet = pathway.Key + PathwayCollectionServerUnusePro;
                        else if (pathway.Value.ServerUnusedClass > 0)
                            previousWorksheet = pathway.Key + PathwayCollectionServerUnuseClass;
                        else if (pathway.Value.ServerPendingProcess > 0)
                            previousWorksheet = pathway.Key + PathwayCollectionServerPendingProcess;
                        else if (pathway.Value.ServerPendingClass > 0)
                            previousWorksheet = pathway.Key + PathwayCollectionServerPendingClass;
                    }

                    if (pathway.Value.DirectoryOnLinks > 0)
                        nextWorksheet = pathway.Key + PathwayCollectionCheckDirectoryOnLinks;
                    else if (pathway.Value.HighDynamicServers > 0)
                        nextWorksheet = pathway.Key + PathwayCollectionHighDynamicServers;
                    else if (pathway.Value.ServerErrorList > 0)
                        nextWorksheet = pathway.Key + PathwayCollectionServerErrorList;
                    else
                        nextWorksheet = "";

                    //There is no detail for Unused Process
                    var serverMaxLinks = alertService.GetServerMaxLinksFor(_fromTimestamp, _toTimestamp, pathway.Key, Enums.IntervalTypes.Daily, datesWithData);
                    excelChart.InsertWorksheetServerMaxLinksInterval(xlWorkPackage.Workbook, worksheetCount, pathway.Key, serverMaxLinks, pathway.Key + PathwayCollectionServerMaxLinks, _saveLocation,
                                                                    proviousPathway, previousWorksheet, nextWorksheet, nextPathway,
                                                                    CollectionAlerts, pathway.Key + PathwayCollectionServerMaxLinksTitle + dateRange, _isLocalAnalyst);
                    worksheetCount++;
                    #endregion
                }

                if (pathway.Value.DirectoryOnLinks > 0)
                {
                    #region DirectoryOnLinks

                    if (pathway.Value.ServerQueueTcp == 0)
                    {
                        if (pathway.Value.ServerMaxLinks > 0)
                            previousWorksheet = pathway.Key + PathwayCollectionServerMaxLinks;
                        else if (pathway.Value.ServerUnusedProcess > 0)
                            previousWorksheet = pathway.Key + PathwayCollectionServerUnusePro;
                        else if (pathway.Value.ServerUnusedClass > 0)
                            previousWorksheet = pathway.Key + PathwayCollectionServerUnuseClass;
                        else if (pathway.Value.ServerPendingProcess > 0)
                            previousWorksheet = pathway.Key + PathwayCollectionServerPendingProcess;
                        else if (pathway.Value.ServerPendingClass > 0)
                            previousWorksheet = pathway.Key + PathwayCollectionServerPendingClass;
                    }

                    if (pathway.Value.HighDynamicServers > 0)
                        nextWorksheet = pathway.Key + PathwayCollectionHighDynamicServers;
                    else if (pathway.Value.ServerErrorList > 0)
                        nextWorksheet = pathway.Key + PathwayCollectionServerErrorList;
                    else
                        nextWorksheet = "";

                    //There is no detail for Unused Process
                    var checkDirectoryON = alertService.GetCheckDirectoryONFor(_fromTimestamp, _toTimestamp, pathway.Key, Enums.IntervalTypes.Daily, datesWithData);
                    excelChart.InsertWorksheetCheckDirectoryOnDetail(xlWorkPackage.Workbook, worksheetCount, pathway.Key, checkDirectoryON, pathway.Key + PathwayCollectionCheckDirectoryOnLinks, _saveLocation,
                                                                    proviousPathway, previousWorksheet, nextWorksheet, nextPathway,
                                                                    CollectionAlerts, pathway.Key + PathwayCollectionCheckDirectoryOnLinksTitle + dateRange, _isLocalAnalyst);
                    worksheetCount++;
                    #endregion
                }

                //High Dynamic Servers GetHighDynamicServersFor
                if (pathway.Value.HighDynamicServers > 0)
                {
                    #region HighDynamicServers

                    if (pathway.Value.ServerQueueTcp == 0)
                    {
                        if (pathway.Value.DirectoryOnLinks > 0)
                            previousWorksheet = pathway.Key + PathwayCollectionCheckDirectoryOnLinks;
                        else if (pathway.Value.ServerMaxLinks > 0)
                            previousWorksheet = pathway.Key + PathwayCollectionServerMaxLinks;
                        else if (pathway.Value.ServerUnusedProcess > 0)
                            previousWorksheet = pathway.Key + PathwayCollectionServerUnusePro;
                        else if (pathway.Value.ServerUnusedClass > 0)
                            previousWorksheet = pathway.Key + PathwayCollectionServerUnuseClass;
                        else if (pathway.Value.ServerPendingProcess > 0)
                            previousWorksheet = pathway.Key + PathwayCollectionServerPendingProcess;
                        else if (pathway.Value.ServerPendingClass > 0)
                            previousWorksheet = pathway.Key + PathwayCollectionServerPendingClass;
                    }

                    if (pathway.Value.ServerErrorList > 0)
                        nextWorksheet = pathway.Key + PathwayCollectionServerErrorList;
                    else
                        nextWorksheet = "";

                    //There is no detail for Unused Process
                    var checkDirectoryON = alertService.GetHighDynamicServersFor(_fromTimestamp, _toTimestamp, pathway.Key, Enums.IntervalTypes.Daily, datesWithData);
                    excelChart.InsertWorksheetHighDynamicServersDetail(xlWorkPackage.Workbook, worksheetCount, pathway.Key, checkDirectoryON, pathway.Key + PathwayCollectionHighDynamicServers, _saveLocation,
                                                                    proviousPathway, previousWorksheet, nextWorksheet, nextPathway,
                                                                    CollectionAlerts, pathway.Key + PathwayCollectionHighDynamicServersTitle + dateRange, _isLocalAnalyst);
                    worksheetCount++;
                    #endregion
                }

                if (pathway.Value.ServerErrorList > 0)
                {
                    #region ServerErrorList

                    if (pathway.Value.ServerQueueTcp == 0)  // if (previousWorksheet.Length == 0)
                    {
                        if (pathway.Value.HighDynamicServers > 0)
                            previousWorksheet = pathway.Key + PathwayCollectionHighDynamicServers;
                        else if (pathway.Value.DirectoryOnLinks > 0)
                            previousWorksheet = pathway.Key + PathwayCollectionCheckDirectoryOnLinks;
                        else if (pathway.Value.ServerMaxLinks > 0)
                            previousWorksheet = pathway.Key + PathwayCollectionServerMaxLinks;
                        else if (pathway.Value.ServerUnusedProcess > 0)
                            previousWorksheet = pathway.Key + PathwayCollectionServerUnusePro;
                        else if (pathway.Value.ServerUnusedClass > 0)
                            previousWorksheet = pathway.Key + PathwayCollectionServerUnuseClass;
                        else if (pathway.Value.ServerPendingProcess > 0)
                            previousWorksheet = pathway.Key + PathwayCollectionServerPendingProcess;
                        else if (pathway.Value.ServerPendingClass > 0)
                            previousWorksheet = pathway.Key + PathwayCollectionServerPendingClass;
                    }

                    //subDatas is Instance of errorLists
                    var errorLists = alertService.GetServerErrorListIntervalFor(_fromTimestamp, _toTimestamp, pathway.Key, Enums.IntervalTypes.Daily, datesWithData);
                    var subDatas = alertService.GetServerErrorListSubFor(_fromTimestamp, _toTimestamp, pathway.Key, Enums.IntervalTypes.Daily, datesWithData);
                    //Check Next worksheet.
                    if (subDatas.Count(x => x.Value.Count > 0) > 0)
                    {

                        var tempData = subDatas.First(subData => subData.Value.Count > 0);
                        nextWorksheet = pathway.Key + PathwayCollectionServerErrorListSub + Convert.ToDateTime(tempData.Key).ToString("yy-MM-dd");
                    }

                    excelChart.InsertWorksheetErrorList(xlWorkPackage.Workbook, worksheetCount, pathway.Key, errorLists, subDatas, pathway.Key + PathwayCollectionServerErrorList, _saveLocation,
                                                        proviousPathway, previousWorksheet, nextWorksheet, nextPathway,
                                                        CollectionAlerts, pathway.Key + PathwayCollectionServerErrorListTitle + dateRange, _isLocalAnalyst);
                    worksheetCount++;
                    var subCount = 0;
                    var subDatasWithValue = subDatas.Where(subData => subData.Value.Count > 0).ToDictionary(x => x.Key, x => x.Value);

                    foreach (var subData in subDatasWithValue)
                    {
                        //Get Error Numbers.
                        foreach (var sub in subData.Value.Where(sub => !errorList.Contains(sub.ErrorNumber)))
                        {
                            errorList.Add(sub.ErrorNumber);
                        }

                        //Get Previous
                        if (subCount == 0)
                            previousWorksheet = pathway.Key + PathwayCollectionServerErrorList;
                        else
                        {
                            string previouKey = subDatasWithValue.Keys.ElementAt(subCount - 1);
                            previousWorksheet = pathway.Key + PathwayCollectionServerErrorListSub + Convert.ToDateTime(previouKey).ToString("yy-MM-dd");
                        }
                        //Get Next worksheet.
                        if ((subCount + 1) < subDatasWithValue.Count)
                        {
                            string nextKey = subDatasWithValue.Keys.ElementAt(subCount + 1);
                            nextWorksheet = pathway.Key + PathwayCollectionServerErrorListSub + Convert.ToDateTime(nextKey).ToString("yy-MM-dd");
                        }
                        else nextWorksheet = "";

                        excelChart.InsertWorksheetErrorListSubs(xlWorkPackage.Workbook, worksheetCount, pathway.Key, subData.Value, pathway.Key + PathwayCollectionServerErrorListSub + Convert.ToDateTime(subData.Key).ToString("yy-MM-dd"), _saveLocation,
                                                                proviousPathway, previousWorksheet, nextWorksheet, nextPathway,
                                                                CollectionAlerts, pathway.Key + PathwayCollectionServerErrorListTitleSub + subData.Key, errorList, _isLocalAnalyst);


                        worksheetCount++;
                        subCount++;
                    }

                    #endregion
                }
                pathwayCount++;
            }

            //Create worksheet with Error information.
            if (errorList.Count > 0)
            {
                IPathwayAlertService alertListService = new PathwayAlertService();
                var errorLists = alertListService.GetErrorInfoFor(errorList);

                excelChart.InsertWorksheetErrorInfo(xlWorkPackage.Workbook, worksheetCount, errorLists.OrderBy(x => x.ErrorNumber).ToList(), _saveLocation, CollectionAlerts);
            }

            var macro = new Macros();

            xlWorkPackage.Workbook.CreateVBAProject();
            xlWorkPackage.Workbook.CodeModule.Code = macro.GenerateThisWorkbookPreviousWorksheet();

            var module = xlWorkPackage.Workbook.VbaProject.Modules.AddModule("PreviousWorksheet_Modules");
            module.Code = macro.GeneratePreviousWorksheetMacro();


            var reportName = @"\" + _systemName + "(" + _systemSerial + ") - Pathway Alert for " + dateRange + ".xlsm";
            var fileName = _saveLocation + reportName;

            if (File.Exists(fileName))
                File.Delete(fileName);

            string helpfile = _saveLocation + "\\Pathway.txt";

            //Delete existing Help file
            if (File.Exists(helpfile))
            {
                File.Delete(helpfile);
            }

            var data = Resource1.Pathway;
            using(var stream = new FileStream(helpfile, FileMode.Create))
            {
                stream.Write(data, 0, data.Count());
                stream.Flush();
            }

            FileInfo fileInfo = new FileInfo(fileName);
            xlWorkPackage.SaveAs(fileInfo);
            xlWorkPackage.Dispose();

            GC.GetTotalMemory(false);
            GC.Collect();
            GC.GetTotalMemory(true);

            return fileName;
        }

        /// <summary>
        /// This function will cover All Pathway Mulitiple Days Excel Reports
        /// </summary>
        /// <param name="alertList"></param>
        internal void GenerateAllPathwayCollection(List<string> alertList, bool isMultiDays)
        {
            //var xlApp = new Application();
            var excelChart = new ExcelChart(_reportDownloadId, _reportDownloadLogs, _log);



            _log.Info("********************************************************************");
            _log.InfoFormat("Starting GenerateAllPathwayCollection at: {0}", DateTime.Now);


            string worksheetNameToc = "Table Of Contents";
            // Workbook xlWorkBook = excelChart.CreateExcelObject(xlApp);
            ExcelPackage xlWorkPackage = new ExcelPackage();
            int worksheetCount = 1;

            //IPvAlertService alertService = new PvAlertService(_connectionStringSystem, _intervalInSec);
            IAllPathwayService service = new AllPathwayService(_intervalInSec);

            var dateList = new List<DateTime>();
            //Get the list of days that has a data.
            IPvCollectsService collectService = new PvCollectsService();
            for (var dtStart = _fromTimestamp; dtStart.Date < _toTimestamp.Date; dtStart = dtStart.AddDays(1))
            {
                var exits = collectService.IsDuplictedFor(dtStart, dtStart.AddDays(1));
                if (!exits) continue;

                dateList.Add(dtStart);
            }

            var category = new Dictionary<String, Collection>();
            string nextSection = "";
            var reports = new Collection
            {
                CPUDetail = CollectionCPUDetail,
                TransactionServer = CollectionTrans
            };
            //multidays add three more graphs
            if (isMultiDays)
            {
                reports = new Collection
                {
                    CPUDetail = CollectionCPUDetail,
                    TransactionServer = CollectionTrans,
                    TransactionTCP = CollectionHourlyTcpCounts,
                    TransactionLinkmon = CollectionHourlyLinkmonCounts,
                    PeakCPUBusyHourly = CollectionHourlyPeakCPUBusy,
                    CPUBusyHourly = CollectionHourlyAvgCPUBusy,
                    TransCountHourly = CollectionHourlyTransCounts
                };
            }

            //Generate Collection worksheets if there are data for more than one day.
            if (dateList.Count > 1)
            {


                var title = "";

                if (!_fromTimestamp.Date.Equals(_toTimestamp.Date))
                    title = _fromTimestamp.ToString("yyyy-MM-dd") + " through " + _toTimestamp.AddDays(-1).ToString("yyyy-MM-dd"); //Since we added extra day when submit the order, need to subtract one day when displaying time range.
                else
                    title = _fromTimestamp.ToString("yyyy-MM-dd");



                if (dateList.Count > 0)
                {
                    nextSection = dateList.First().ToString("yyyy-MM-dd") + DailyCPUDetail;
                    //if multidays, report does not have next section like "xxxx-xx-xx CPU" any more.
                    if (isMultiDays)
                    {
                        nextSection = "";
                    }
                }

                _log.InfoFormat("Calling CPU Detail at: {0}", DateTime.Now);

                //Get CPU Detail
                var detail = service.GetCPUBusyDetailFor(_fromTimestamp, _toTimestamp, _systemSerial, _ipu);
                excelChart.InsertWorksheetCPUDetailAllPathway(xlWorkPackage.Workbook, worksheetCount, detail, reports.CPUDetail, _saveLocation, "", "", reports.TransactionServer, nextSection, worksheetNameToc, CollectionCPUDetailTitle + title, _isLocalAnalyst);
                worksheetCount++;

                _log.InfoFormat("Calling Trans. Server at: {0}", DateTime.Now);

                //Get Trans. Server
                var transactionServer = service.GetTransactionServer(_fromTimestamp, _toTimestamp);
                var transactionTcp = service.GetTransactionTcp(_fromTimestamp, _toTimestamp);
                excelChart.InsertWorksheetTransactionServerAllPathway(xlWorkPackage.Workbook, worksheetCount, transactionServer, transactionTcp, reports.TransactionServer, _saveLocation, "", reports.CPUDetail, reports.CPUBusyHourly, nextSection, worksheetNameToc, CollectionTransTitle + title, _isLocalAnalyst);
                worksheetCount++;

                //Add Hourly Average CPU Busy per Pathway, Hourly Peak CPU Busy per Pathway, Hourly transaction counts RA-1487
                if (isMultiDays)
                {
                    //add peak CPU busy hourly
                    _log.InfoFormat("Calling " + " Hourly Peak CPU Busy per Pathway at: {0}", DateTime.Now);

                    var cpuAndTransHourly = service.GetPathwayHourly(_fromTimestamp, _toTimestamp);
                    excelChart.InsertPeakHourlyCPUBusyPerPathway(xlWorkPackage.Workbook, worksheetCount, reports.PeakCPUBusyHourly, _saveLocation, "", reports.TransactionServer, "", reports.CPUBusyHourly, worksheetNameToc, CollectionPeakCPUBusyHourly + title, _isLocalAnalyst, _fromTimestamp, _toTimestamp, _pathwayList, cpuAndTransHourly);
                    worksheetCount++;

                    _log.InfoFormat("Calling " + " Hourly CPU Busy per Pathway at: {0}", DateTime.Now);

                    excelChart.InsertHourlyCPUBusyPerPathway(xlWorkPackage.Workbook, worksheetCount, reports.CPUBusyHourly, _saveLocation, "", reports.PeakCPUBusyHourly, "", reports.TransactionLinkmon, worksheetNameToc, CollectionAvgCPUBusyHourly + title, _isLocalAnalyst, _fromTimestamp, _toTimestamp, _pathwayList, cpuAndTransHourly);
                    worksheetCount++;

                    //add LinkMon
                    _log.InfoFormat("Calling " + " Hourly Linkmon Trans Counts per Pathway at: {0}", DateTime.Now);

                    excelChart.InsertHourlyLinkmonCounts(xlWorkPackage.Workbook, worksheetCount, reports.TransactionLinkmon, _saveLocation, "", reports.CPUBusyHourly, "", reports.TransactionTCP, worksheetNameToc, CollectionLinkmonTransCountsHourly + title, _isLocalAnalyst, _fromTimestamp, _toTimestamp, _pathwayList, cpuAndTransHourly);
                    worksheetCount++;

                    //add Tcp
                    _log.InfoFormat("Calling " + " Hourly TCP Trans Counts per Pathway at: {0}", DateTime.Now);

                    excelChart.InsertHourlyTcpCounts(xlWorkPackage.Workbook, worksheetCount, reports.TransactionTCP, _saveLocation, "", reports.TransactionLinkmon, "", reports.TransCountHourly, worksheetNameToc, CollectionTcpTransCountsHourly + title, _isLocalAnalyst, _fromTimestamp, _toTimestamp, _pathwayList, cpuAndTransHourly);
                    worksheetCount++;

                    _log.InfoFormat("Calling " + " Hourly transaction count at: {0}", DateTime.Now);

                    excelChart.InsertHourlyTransactionCounts(xlWorkPackage.Workbook, worksheetCount, reports.TransCountHourly, _saveLocation, "", reports.CPUBusyHourly, "", "", worksheetNameToc, CollectionTransactionsCountHourly + title, _isLocalAnalyst, _fromTimestamp, _toTimestamp, _pathwayList, cpuAndTransHourly);
                    worksheetCount++;
                }

                category.Add("Collection", reports);
            }

            //if number of dates is greater then 1, generate summary pathway which removes sheet for specific date
            //for (var dtStart = _fromTimestamp; dtStart.Date < _toTimestamp.Date; dtStart = dtStart.AddDays(1)) {
            if (!isMultiDays)
            {
                foreach (var dtStart in dateList)
                {
                    /*string previousSection;
					if (dtStart.Equals(_fromTimestamp)) {
						previousSection = CollectionCPUDetail;
					}
					else {
						previousSection = dtStart.AddDays(-1).ToString("yyyy-MM-dd") + DailyCPUDetail;
					}
					nextSection = "";
					if (!dtStart.Date.Equals(_toTimestamp.Date)) {
						nextSection = dtStart.AddDays(1).ToString("yyyy-MM-dd") + DailyCPUDetail;
					}*/

                    var currentIndex = dateList.IndexOf(dtStart);

                    string previousSection;
                    if (currentIndex.Equals(0))
                    {
                        //previousSection = CollectionCPUDetail;
                        previousSection = "";
                    }
                    else
                    {
                        previousSection = dateList[currentIndex - 1].ToString("yyyy-MM-dd") + DailyCPUDetail;
                    }
                    nextSection = "";
                    if ((dateList.Count - 1) != currentIndex)
                    {
                        nextSection = dateList[currentIndex + 1].ToString("yyyy-MM-dd") + DailyCPUDetail;
                    }

                    reports = new Collection
                    {
                        //Alert = dtStart.ToString("yyyy-MM-dd") + DailyAlerts,
                        //CPUSummary = dtStart.ToString("yyyy-MM-dd") + DailyCPUSummary,
                        CPUDetail = dtStart.ToString("yyyy-MM-dd") + DailyCPUDetail,
                        TransactionServer = dtStart.ToString("yyyy-MM-dd") + DailyTrans
                    };


                    _log.InfoFormat("Calling {0} CPU Detail at: {1}", dtStart.ToString("yyyy-MM-dd"), DateTime.Now);

                    //Get CPU Detail
                    var detailDaily = service.GetCPUBusyDetailFor(dtStart, dtStart.AddDays(1), _systemSerial, _ipu);
                    excelChart.InsertWorksheetCPUDetailAllPathway(xlWorkPackage.Workbook, worksheetCount, detailDaily, reports.CPUDetail, _saveLocation, previousSection, "", reports.TransactionServer, nextSection, worksheetNameToc, CollectionCPUDetailTitle + dtStart.ToString("yyyy-MM-dd"), _isLocalAnalyst);
                    worksheetCount++;

                    _log.InfoFormat("Calling {0} Trans. Server at: {1}", dtStart.ToString("yyyy-MM-dd"), DateTime.Now);

                    //Get Trans. Server
                    var transactionServerDaily = service.GetTransactionServer(dtStart, dtStart.AddDays(1));
                    var transactionTcpDaily = service.GetTransactionTcp(dtStart, dtStart.AddDays(1));
                    excelChart.InsertWorksheetTransactionServerAllPathway(xlWorkPackage.Workbook, worksheetCount, transactionServerDaily, transactionTcpDaily, reports.TransactionServer, _saveLocation, previousSection, reports.CPUDetail, "", nextSection, worksheetNameToc, CollectionTransTitle + dtStart.ToString("yyyy-MM-dd"), _isLocalAnalyst);
                    worksheetCount++;

                    //Get Trans. TCP
                    //excelChart.InsertWorksheetTransactionTcpAllPathway(xlWorkBook, worksheetCount, transactionTcpDaily, reports.TransactionTCP, _saveLocation, previousSection, reports.TransactionServer, "", nextSection, worksheetNameToc, DailyTransTCPTitle + dtStart.ToString("yyyy-MM-dd"));
                    //worksheetCount++;

                    category.Add(dtStart.ToString("yyyy-MM-dd dddd"), reports);

                }
            }

            _log.InfoFormat("Build Table of Content at: {0}", DateTime.Now);

            //Build Table of Content.
            excelChart.InsertCollectionTOC(xlWorkPackage.Workbook, worksheetCount, _pathwayList, category, worksheetNameToc, _saveLocation, PerPathwayFileInfo, isMultiDays);

            _log.InfoFormat("Reverse the order of excel worksheet at: {0}", DateTime.Now);

            //Reverse the order of excel worksheet.

            xlWorkPackage.Workbook.Worksheets.MoveToStart(xlWorkPackage.Workbook.Worksheets.Count);

            _log.InfoFormat("Build Macros at: {0}", DateTime.Now);


            var macro = new Macros();

            xlWorkPackage.Workbook.CreateVBAProject();
            xlWorkPackage.Workbook.CodeModule.Code = macro.GenerateThisWorkbookCollection();

            var module = xlWorkPackage.Workbook.VbaProject.Modules.AddModule("Collection_Modules");
            module.Code = macro.GenerateCollectionMacro(worksheetNameToc);

            string fileName = _saveLocation + @"\" + _systemName + "(" + _systemSerial + ") - Collection" +
                                      " - " +
                                      _fromTimestamp.ToString("yyyy-MM-dd") + " to " +
                                      _toTimestamp.AddDays(-1).ToString("yyyy-MM-dd") + ".xlsm"; ;

            if (dateList.Count == 1)
            {
                fileName = _saveLocation + @"\" + _systemName + "(" + _systemSerial + ") - Collection" +
                                      " - " +
                                      _fromTimestamp.ToString("yyyy-MM-dd") + ".xlsm"; ;
            }
            if (!System.IO.Directory.Exists(_saveLocation))
                System.IO.Directory.CreateDirectory(_saveLocation);

            _log.InfoFormat("fileName: {0}", fileName);

            if (File.Exists(fileName))
                File.Delete(fileName);

            FileInfo fileInfo = new FileInfo(fileName);
            xlWorkPackage.SaveAs(fileInfo);
            xlWorkPackage.Dispose();
        }

        /// <summary>
        /// This report will cover Per Pathway Mulitiple Days and Per Pathway Per Day
        /// </summary>
        /// <param name="alertList"></param>
        internal void GeneratePerPathwayDetailMultiDays(List<string> alertList)
        {
            _log.Info("********************************************************************");
            _log.InfoFormat("Starting GeneratePerPathwayDetailMultiDays at: {0}", DateTime.Now);

            _reportDownloadLogs = new ReportDownloadLogs();
            foreach (var list in _pathwayList)
            {

                _reportDownloadLogs.InsertNewLog(_reportDownloadId, DateTime.Now, "Generating detail report for " + list);
                //Parallel.ForEach(_pathwayList, list => {
                //Check if there is a PathwayName on PvPwylist.
                IPvPwyListService checkPathway = new PvPwyListService();
                var exits = checkPathway.CheckPathwayNameFor(list, _fromTimestamp, _toTimestamp);
                if (exits)
                {

                    _log.InfoFormat("Starting Pathway {0} at: {1}", list, DateTime.Now);


                    #region Multiple Days

                    string worksheetNameToc = list + " - Table Of Contents";

                    //var xlApp = new Application();
                    var excelChart = new ExcelChart(_reportDownloadId, _reportDownloadLogs, _log);
                    var excelChartServer = new ExcelChartServer(_reportDownloadId, _reportDownloadLogs, _log);
                    var excelChartTerm = new ExcelChartTerm(_reportDownloadId, _reportDownloadLogs, _log);
                    var excelChartTCP = new ExcelChartTCP(_reportDownloadId, _reportDownloadLogs, _log);

                    //var xlWorkBook = excelChart.CreateExcelObject(xlApp);
                    ExcelPackage xlWorkPackage = new ExcelPackage();
                    var worksheetCount = 1;

                    var dateList = new List<DateTime>();
                    //Get the list of days that has a data.
                    for (var dtStart = _fromTimestamp; dtStart.Date < _toTimestamp.Date; dtStart = dtStart.AddDays(1))
                    {
                        exits = checkPathway.CheckPathwayNameFor(list, dtStart, dtStart.AddDays(1));
                        if (!exits) continue;

                        dateList.Add(dtStart);
                    }

                    var category = new Dictionary<String, Collection>();
                    //Generate Collection worksheets
                    var reports = new Collection
                    {
                        CPUDetail = list + PathwayCollectionCPUDetail,
                        TransactionServer = list + PathwayCollectionTransServer,
                        ServerCPUBusy = list + PathwayCollectionServerCPUBusy,
                        ServerTransactions = list + PathwayCollectionServerTransaction,
                        ServerProcessCount = list + PathwayCollectionServerProCount,
                        ServerQueuedTCP = list + PathwayCollectionServerQueTCP,
                        ServerQueuedLinkmon = list + PathwayCollectionServerQueLinkmon,
                        ServerUnusedServerClasses = list + PathwayCollectionServerUnuseClass,
                        ServerUnusedServerProcesses = list + PathwayCollectionServerUnusePro,
                        TermUnused = list + PathwayCollectionTermUnused,
                        TermTop20 = list + PathwayCollectionTermTop20,
                        TCPTransactions = list + PathwayCollectionTCPTransaction,
                        TCPQueuedTransactions = list + PathwayCollectionTCPQueTransaction,
                        TCPUnusedTCPs = list + PathwayCollectionTCPUnusedTCP
                    };
                    //var nextSection = list + "-" + _fromTimestamp.ToString("yy-MM-dd") + DailyCPUDetail;
                    var nextSection = list + "-" + dateList.First().ToString("yy-MM-dd") + DailyCPUDetail;

                    string dateRange = "";
                    if (!_fromTimestamp.Date.Equals(_toTimestamp.Date))
                        dateRange = _fromTimestamp.ToString("yyyy-MM-dd") + " through " + _toTimestamp.AddDays(-1).ToString("yyyy-MM-dd"); //Since we added extra day when submit the order, need to subtract one day when displaying time range.
                    else
                        dateRange = _fromTimestamp.ToString("yyyy-MM-dd");

                    _log.InfoFormat("Calling Basic Data at: {0}", DateTime.Now);


                    #region Basic Data

                    IPvAlertService alertService = new PvAlertService(_intervalInSec);
                    IPerPathwayService service = new PerPathwayService(_intervalInSec);

                    _log.InfoFormat("Calling Basic Data (CPU Busy Detail) at: {0}", DateTime.Now);

                    var detail = service.GetCPUBusyDetailFor(_fromTimestamp, _toTimestamp, list, _ipu, _systemSerial);
                    excelChart.InsertWorksheetCPUDetailPerPathway(xlWorkPackage.Workbook, worksheetCount, list, detail, reports.CPUDetail, _saveLocation, "", "", "", "", worksheetNameToc, list + PathwayCollectionCPUDetailTitle + dateRange, _isLocalAnalyst);
                    worksheetCount++;

                    _log.InfoFormat("Calling Basic Data (Transaction) at: {0}", DateTime.Now);

                    var transactionServer = service.GetIntervalTransactionServerFor(_fromTimestamp, _toTimestamp, list, Enums.IntervalTypes.Daily);
                    var transactionTcp = service.GetIntervalTransactionTCPFor(_fromTimestamp, _toTimestamp, list, Enums.IntervalTypes.Daily);
                    excelChart.InsertWorksheetTransactionPerPathwayInterval(xlWorkPackage.Workbook, worksheetCount, list, transactionServer, transactionTcp, reports.TransactionServer, _saveLocation, "", "", "", "", worksheetNameToc,
                        list + PathwayCollectionTransTitle + dateRange, _isLocalAnalyst);
                    worksheetCount++;

                    #endregion

                    _log.InfoFormat("Calling SERVER at: {0}", DateTime.Now);


                    #region SERVER

                    IPerPathwayServerService serverService = new PerPathwayServerService(_intervalInSec);

                    _log.InfoFormat("Calling SERVER (CPU BUSY) at: {0}", DateTime.Now);

                    //CPU BUSY
                    var serverCPUBusy = serverService.GetServerCPUBusyFor(_fromTimestamp, _toTimestamp, list, _ipu);
                    _log.InfoFormat("SERVER (CPU BUSY) Count: {0}", serverCPUBusy.Count);

                    if (serverCPUBusy.Count > 0)
                    {
                        excelChartServer.InsertWorksheetCPUBusy(xlWorkPackage.Workbook, worksheetCount, list, serverCPUBusy, reports.ServerCPUBusy, _saveLocation, "", "", "", "", worksheetNameToc, list + PathwayCollectionServerCPUBusyTitle + dateRange, true, _isLocalAnalyst);
                        worksheetCount++;
                    }

                    _log.InfoFormat("Calling SERVER (TRANSACTIONS) at: {0}", DateTime.Now);

                    //TRANSACTIONS
                    var serverTransactionCount = serverService.GetServerTransactionsFor(_fromTimestamp, _toTimestamp, list, _ipu);
                    _log.InfoFormat("SERVER (TRANSACTIONS) Count: {0}", serverTransactionCount.Count);

                    if (serverTransactionCount.Count > 0)
                    {
                        excelChartServer.InsertWorksheetCPUBusy(xlWorkPackage.Workbook, worksheetCount, list, serverTransactionCount, reports.ServerTransactions, _saveLocation, "", "", "", "", worksheetNameToc,
                            list + PathwayCollectionServerTransactionTitle + dateRange, false, _isLocalAnalyst);
                        worksheetCount++;
                    }

                    _log.InfoFormat("Calling SERVER (QUEUED TCP) at: {0}", DateTime.Now);


                    #region QUEUED TCP

                    var queueTcp = serverService.GetServerQueueTCPIntervalFor(_fromTimestamp, _toTimestamp, list, Enums.IntervalTypes.Daily);
                    var subDatasTcp = alertService.GetQueueTCPSubFor(_fromTimestamp, _toTimestamp, list, Enums.IntervalTypes.Daily);

                    string nextWorksheet = reports.ServerQueuedLinkmon;
                    //Check Next worksheet.
                    if (subDatasTcp.Count(x => x.Value.Count > 0) > 0)
                    {
                        var tempData = subDatasTcp.First(subData => subData.Value.Count > 0);
                        nextWorksheet = list + PathwayCollectionServerQueTCPSub + Convert.ToDateTime(tempData.Key).ToString("yy-MM-dd");
                    }

                    var excelChartAlert = new ExcelChartAlert(_reportDownloadId, _reportDownloadLogs, _log);
                    string previousWorksheet = reports.ServerQueuedTCP;

                    _log.InfoFormat("SERVER (QUEUED TCP) Count: {0}", queueTcp.Count);
                    _log.InfoFormat("SERVER (QUEUED TCP) Sub Count: {0}", subDatasTcp.Count);


                    if (queueTcp.Count > 0)
                    {
                        excelChartAlert.InsertWorksheetQueues(xlWorkPackage.Workbook, worksheetCount, list, reports.ServerQueuedTCP, _saveLocation,
                            "", "", "", "",
                            worksheetNameToc, list + PathwayCollectionServerQueTCPTitle + dateRange, subDatasTcp, Enums.IntervalTypes.Daily, _isLocalAnalyst, queueTcp, null, false);
                        worksheetCount++;
                        var subCountTcp = 0;
                        //Get only data with values.
                        var subDatasTcpWithValue = subDatasTcp.Where(subData => subData.Value.Count > 0).ToDictionary(x => x.Key, x => x.Value);
                        foreach (var subData in subDatasTcpWithValue)
                        {
                            //Get Previous
                            if (subCountTcp == 0)
                                previousWorksheet = list + PathwayCollectionServerQueTCPTitle + dateRange;
                            else
                            {
                                string previouKey = subDatasTcpWithValue.Keys.ElementAt(subCountTcp - 1);
                                previousWorksheet = list + PathwayCollectionServerQueTCPSub + Convert.ToDateTime(previouKey).ToString("yy-MM-dd");
                            }

                            //Get Next worksheet.
                            if ((subCountTcp + 1) < subDatasTcpWithValue.Count)
                            {
                                string nextKey = subDatasTcpWithValue.Keys.ElementAt(subCountTcp + 1);
                                nextWorksheet = list + PathwayCollectionServerQueTCPSub + Convert.ToDateTime(nextKey).ToString("yy-MM-dd");
                            }
                            else
                            {
                                nextWorksheet = reports.ServerQueuedLinkmon;
                            }
                            excelChartAlert.InsertWorksheetQueueSubs(xlWorkPackage.Workbook, worksheetCount, list, list + PathwayCollectionServerQueTCPSub + Convert.ToDateTime(subData.Key).ToString("yy-MM-dd"), _saveLocation,
                                "", "", "", "",
                                worksheetNameToc, list + PathwayCollectionServerQueTCPTitleSub + subData.Key, subData.Value, false, _isLocalAnalyst);
                            worksheetCount++;
                            subCountTcp++;
                        }
                    }

                    //Get previous worksheet for the Server QueueLinkmon
                    if (subDatasTcp.Count(x => x.Value.Count > 0) > 0)
                    {
                        var tempData = subDatasTcp.Last(subData => subData.Value.Count > 0);
                        previousWorksheet = list + PathwayCollectionServerQueTCPSub + Convert.ToDateTime(tempData.Key).ToString("yy-MM-dd");
                    }

                    #endregion

                    _log.InfoFormat("Calling SERVER (QUEUED LINKMON) at: {0}", DateTime.Now);


                    #region QUEUED LINKMON

                    var queueLinkmon = serverService.GetServerQueueLinkmonIntervalFor(_fromTimestamp, _toTimestamp, list, Enums.IntervalTypes.Daily);
                    var subDatasLinkmon = alertService.GetQueueLinkmonSubFor(_fromTimestamp, _toTimestamp, list, Enums.IntervalTypes.Daily);

                    //Check Next worksheet.
                    if (subDatasLinkmon.Count(x => x.Value.Count > 0) > 0)
                    {
                        var tempData = subDatasLinkmon.First(subData => subData.Value.Count > 0);
                        nextWorksheet = list + PathwayCollectionServerQueLinkmonSub + Convert.ToDateTime(tempData.Key).ToString("yy-MM-dd");
                    }
                    else
                    {
                        nextWorksheet = reports.ServerUnusedServerClasses;
                    }
                    _log.InfoFormat("SERVER (QUEUED LINKMON) Count: {0}", queueLinkmon.Count);
                    _log.InfoFormat("SERVER (QUEUED LINKMON) Sub Count: {0}", subDatasLinkmon.Count);


                    if (queueLinkmon.Count > 0)
                    {
                        excelChartAlert.InsertWorksheetQueues(xlWorkPackage.Workbook, worksheetCount, list, reports.ServerQueuedLinkmon, _saveLocation,
                            "", "", "", "",
                            worksheetNameToc, list + PathwayCollectionServerQueLinkmonTitle + dateRange, subDatasLinkmon, Enums.IntervalTypes.Daily, _isLocalAnalyst, null, queueLinkmon, false);
                        worksheetCount++;
                        var subCountLinkmon = 0;
                        //Get only data with values.
                        var subDatasLinkmonWithValue = subDatasLinkmon.Where(subData => subData.Value.Count > 0).ToDictionary(x => x.Key, x => x.Value);
                        foreach (var subData in subDatasLinkmonWithValue)
                        {
                            //Get Previous
                            if (subCountLinkmon == 0)
                                previousWorksheet = reports.ServerQueuedLinkmon;
                            else
                            {
                                string previouKey = subDatasLinkmonWithValue.Keys.ElementAt(subCountLinkmon - 1);
                                previousWorksheet = list + PathwayCollectionServerQueLinkmonSub + Convert.ToDateTime(previouKey).ToString("yy-MM-dd");
                            }
                            //Get Next worksheet.
                            if ((subCountLinkmon + 1) < subDatasLinkmonWithValue.Count)
                            {
                                string nextKey = subDatasLinkmonWithValue.Keys.ElementAt(subCountLinkmon + 1);
                                nextWorksheet = list + PathwayCollectionServerQueLinkmonSub + Convert.ToDateTime(nextKey).ToString("yy-MM-dd");
                            }
                            else
                            {
                                nextWorksheet = reports.ServerUnusedServerClasses;
                            }
                            excelChartAlert.InsertWorksheetQueueSubs(xlWorkPackage.Workbook, worksheetCount, list, list + PathwayCollectionServerQueLinkmonSub + Convert.ToDateTime(subData.Key).ToString("yy-MM-dd"), _saveLocation,
                                "", "", "", "",
                                worksheetNameToc, list + PathwayCollectionServerQueLinkmonTitleSub + subData.Key, subData.Value, true, _isLocalAnalyst);
                            worksheetCount++;
                            subCountLinkmon++;
                        }
                    }

                    //Get previous worksheet for the ServerUnusedClass 
                    if (subDatasLinkmon.Count(x => x.Value.Count > 0) > 0)
                    {
                        var tempData = subDatasLinkmon.Last(subData => subData.Value.Count > 0);
                        previousWorksheet = list + PathwayCollectionServerQueLinkmonSub + Convert.ToDateTime(tempData.Key).ToString("yy-MM-dd");
                    }

                    #endregion

                    _log.InfoFormat("Calling SERVER (UNUSED CLASSES) at: {0}", DateTime.Now);

                    //UNUSED CLASSES
                    var unusedClass = serverService.GetServerUnusedClassesIntervalFor(_fromTimestamp, _toTimestamp, list, Enums.IntervalTypes.Daily);
                    _log.InfoFormat("SERVER (UNUSED CLASSES) Count: {0}", unusedClass.Count);

                    if (unusedClass.Count > 0)
                    {
                        excelChartServer.InsertWorksheetUnusedClassInterval(xlWorkPackage.Workbook, worksheetCount, list, unusedClass, reports.ServerUnusedServerClasses, _saveLocation,
                            "", "", "", "",
                            worksheetNameToc, list + PathwayCollectionServerUnuseClassTitle + dateRange, _isLocalAnalyst);
                        worksheetCount++;
                    }

                    _log.InfoFormat("Calling SERVER (UNUSED PROCESSES) at: {0}", DateTime.Now);

                    //UNUSED PROCESSES
                    var unusedProcess = serverService.GetServerUnusedProcessesIntervalFor(_fromTimestamp, _toTimestamp, list, Enums.IntervalTypes.Daily);
                    _log.InfoFormat("SERVER (UNUSED PROCESSES) Count: {0}", unusedProcess.Count);

                    if (unusedProcess.Count > 0)
                    {
                        excelChartServer.InsertWorksheetUnusedProcessInterval(xlWorkPackage.Workbook, worksheetCount, list, unusedProcess, reports.ServerUnusedServerProcesses, _saveLocation,
                            "", "", "", "",
                            worksheetNameToc, list + PathwayCollectionServerUnuseProTitle + dateRange, _isLocalAnalyst);
                        worksheetCount++;
                    }

                    #endregion

                    _log.InfoFormat("Calling TCP at: {0}", DateTime.Now);


                    #region TCP

                    IPerPathwayTcpService tcpService = new PerPathwayTcpService(_intervalInSec);
                    _log.InfoFormat("Calling TCP (TRANSACTION) at: {0}", DateTime.Now);


                    //TRANSACTION
                    var tcpTransaction = tcpService.GetTcpTransactionFor(_fromTimestamp, _toTimestamp, list);
                    _log.InfoFormat("TCP (TRANSACTION) Count: {0}", tcpTransaction.Count);

                    if (tcpTransaction.Count > 0)
                    {
                        excelChartTCP.InsertWorksheetTcpTransaction(xlWorkPackage.Workbook, worksheetCount, reports.TCPTransactions, tcpTransaction, reports.TCPTransactions, _saveLocation,
                            "", "", "", "",
                            worksheetNameToc, list + PathwayCollectionTCPQueTransactionTitle + dateRange, _isLocalAnalyst);
                        worksheetCount++;
                    }

                    _log.InfoFormat("Calling TCP (QUEUED TRANSACTIONS) at: {0}", DateTime.Now);


                    #region QUEUED TRANSACTIONS

                    var tcpQueued = tcpService.GetTcpQueuedTransactionFor(_fromTimestamp, _toTimestamp, list, Enums.IntervalTypes.Daily);
                    var subDatas = tcpService.GetQueueTCPSubFor(_fromTimestamp, _toTimestamp, list, Enums.IntervalTypes.Daily);
                    _log.InfoFormat("tcpQueued Count: {0}", tcpQueued.Count);
                    _log.InfoFormat("subDatas Count: {0}", subDatas.Count);


                    nextWorksheet = reports.TCPUnusedTCPs;
                    //Check Next worksheet.
                    if (subDatas.Count(x => x.Value.Count > 0) > 0)
                    {
                        var tempData = subDatas.First(subData => subData.Value.Count > 0);
                        nextWorksheet = list + PathwayCollectionTCPQueTransactionSub + Convert.ToDateTime(tempData.Key).ToString("yy-MM-dd");
                    }

                    if (tcpQueued.Count > 0)
                    {
                        excelChartTCP.InsertWorksheetQueues(xlWorkPackage.Workbook, worksheetCount, list, reports.TCPQueuedTransactions, _saveLocation,
                            "", "", "", "", worksheetNameToc,
                            list + PathwayCollectionTCPQueTransactionTitle + dateRange, tcpQueued, subDatas, Enums.IntervalTypes.Daily, _isLocalAnalyst);
                        worksheetCount++;
                        var subCount = 0;
                        //Get only data with values.
                        var subDatasWithValue = subDatas.Where(subData => subData.Value.Count > 0).ToDictionary(x => x.Key, x => x.Value);
                        previousWorksheet = reports.TCPTransactions;
                        foreach (var subData in subDatasWithValue)
                        {
                            //Get Previous
                            if (subCount == 0)
                                previousWorksheet = reports.TCPQueuedTransactions;
                            else
                            {
                                string previouKey = subDatasWithValue.Keys.ElementAt(subCount - 1);
                                previousWorksheet = list + PathwayCollectionTCPQueTransactionSub + Convert.ToDateTime(previouKey).ToString("yy-MM-dd");
                            }
                            //Get Next worksheet.
                            if ((subCount + 1) < subDatasWithValue.Count)
                            {
                                string nextKey = subDatasWithValue.Keys.ElementAt(subCount + 1);
                                nextWorksheet = list + PathwayCollectionTCPQueTransactionSub + Convert.ToDateTime(nextKey).ToString("yy-MM-dd");
                            }
                            else
                            {
                                nextWorksheet = reports.TCPUnusedTCPs;
                            }
                            excelChartTCP.InsertWorksheetQueueSubs(xlWorkPackage.Workbook, worksheetCount, list, list + PathwayCollectionTCPQueTransactionSub + Convert.ToDateTime(subData.Key).ToString("yy-MM-dd"), _saveLocation,
                                "", "", "", "",
                                worksheetNameToc, list + PathwayCollectionServerQueTCPTitleSub + subData.Key, subData.Value, _isLocalAnalyst);
                            worksheetCount++;
                            subCount++;
                        }
                    }

                    //Get previous worksheet for the Server QueueLinkmon
                    if (subDatas.Count(x => x.Value.Count > 0) > 0)
                    {
                        var tempData = subDatas.Last(subData => subData.Value.Count > 0);
                        previousWorksheet = list + PathwayCollectionTCPQueTransactionSub + Convert.ToDateTime(tempData.Key).ToString("yy-MM-dd");
                    }

                    #endregion

                    //UNUSED TCP
                    var tcpUnused = tcpService.GetTcpUnusedFor(_fromTimestamp, _toTimestamp, list, Enums.IntervalTypes.Daily);
                    _log.InfoFormat("TCP (UNUSED TCP) Count: {0}", tcpUnused.Count);

                    if (tcpUnused.Count > 0)
                    {
                        excelChartServer.InsertWorksheetUnusedClassInterval(xlWorkPackage.Workbook, worksheetCount, list, tcpUnused, reports.TCPUnusedTCPs, _saveLocation,
                            "", "", "", "", worksheetNameToc,
                            list + PathwayCollectionTCPUnusedTCPTitle + dateRange, true);
                        worksheetCount++;
                    }

                    #endregion

                    _log.InfoFormat("Starting TERM at: {0}", DateTime.Now);


                    #region TERM

                    _log.InfoFormat("Calling TCP (TERM Top 20) at: {0}", DateTime.Now);

                    IPerPathwayTermService termService = new PerPathwayTermService(_intervalInSec);
                    var termTop20 = termService.GetTermTop20(_fromTimestamp, _toTimestamp, list, Enums.IntervalTypes.Daily);
                    _log.InfoFormat("TERM (Term Top 20) Count: {0}", termTop20.Count);

                    if (termTop20.Count > 0)
                    {
                        excelChartTerm.InsertWorksheetTermTop20(xlWorkPackage.Workbook, worksheetCount, list, termTop20, reports.TermTop20, _saveLocation,
                            "", "", "", "",
                            worksheetNameToc, list + PathwayCollectionTermTop20Title + dateRange, _isLocalAnalyst);
                        worksheetCount++;
                    }

                    _log.InfoFormat("Calling TCP (Term Unused) at: {0}", DateTime.Now);

                    var termUnused = termService.GetTermUnused(_fromTimestamp, _toTimestamp, list, Enums.IntervalTypes.Daily);
                    _log.InfoFormat("TERM (Term Unused) Count: {0}", termUnused.Count);

                    if (termUnused.Count > 0)
                    {
                        excelChartTerm.InsertWorksheetTermUnused(xlWorkPackage.Workbook, worksheetCount, list, termUnused, reports.TermUnused, _saveLocation,
                            "", "", "", "",
                            worksheetNameToc, list + PathwayCollectionTermUnusedTitle + dateRange, _isLocalAnalyst);
                        worksheetCount++;
                    }

                    #endregion


                    category.Add(list + " - " +
                                 _fromTimestamp.ToString("yyyy-MM-dd") + " to " +
                                 _toTimestamp.AddDays(-1).ToString("yyyy-MM-dd"), reports);

                    foreach (var dtStart in dateList)
                    {
                        var currentIndex = dateList.IndexOf(dtStart);
                        string previousSection;
                        if (currentIndex.Equals(0))
                        {
                            previousSection = list + PathwayCollectionCPUDetail;
                        }
                        else
                        {
                            previousSection = list + "-" + dateList[currentIndex - 1].ToString("yy-MM-dd") + DailyCPUDetail;
                        }
                        nextSection = "";
                        if ((dateList.Count - 1) != currentIndex)
                        {
                            nextSection = list + "-" + dateList[currentIndex + 1].ToString("yy-MM-dd") + DailyCPUDetail;
                        }

                        #region Per Daily - This will generate tab with pathwayName + date + column name

                        reports = new Collection
                        {
                            CPUDetail = list + "-" + dtStart.ToString("yy-MM-dd") + DailyCPUDetail,
                            TransactionServer = list + "-" + dtStart.ToString("yy-MM-dd") + DailyTrans,
                            ServerCPUBusy = list + "-" + dtStart.ToString("yy-MM-dd") + DailyServerCPUBusy,
                            ServerTransactions = list + "-" + dtStart.ToString("yy-MM-dd") + DailyServerTransaction,
                            ServerProcessCount = list + "-" + dtStart.ToString("yy-MM-dd") + DailyServerProCount,
                            ServerQueuedTCP = list + "-" + dtStart.ToString("yy-MM-dd") + DailyServerQueTCP,
                            ServerQueuedLinkmon = list + "-" + dtStart.ToString("yy-MM-dd") + DailyServerQueLinkmon,
                            ServerUnusedServerClasses = list + "-" + dtStart.ToString("yy-MM-dd") + DailyServerUnuseClass,
                            ServerUnusedServerProcesses = list + "-" + dtStart.ToString("yy-MM-dd") + DailyServerUnusePro,
                            TermTop20 = list + "-" + dtStart.ToString("yy-MM-dd") + DailyTermTop20,
                            TermUnused = list + "-" + dtStart.ToString("yy-MM-dd") + DailyTermUnused,
                            TCPTransactions = list + "-" + dtStart.ToString("yy-MM-dd") + DailyTCPTransaction,
                            TCPQueuedTransactions = list + "-" + dtStart.ToString("yy-MM-dd") + DailyTCPQueTransaction,
                            TCPUnusedTCPs = list + "-" + dtStart.ToString("yy-MM-dd") + DailyTCPUnusedTCP
                        };

                        _log.InfoFormat("Starting Per Daily Basic Data at: {0}", DateTime.Now);


                        #region Basic Data

                        var hourlyDetail = service.GetCPUBusyDetailFor(dtStart, dtStart.AddDays(1), list, _ipu, _systemSerial);
                        _log.InfoFormat("Per Daily Basic Data (CPU BUSY) Count: {0}", hourlyDetail.Count);

                        if (hourlyDetail.Count > 0)
                        {
                            excelChart.InsertWorksheetCPUDetailPerPathway(xlWorkPackage.Workbook, worksheetCount, list, hourlyDetail, reports.CPUDetail, _saveLocation,
                                "", "", "", "",
                                worksheetNameToc, list + PathwayCollectionCPUDetailTitle + dtStart.ToString("yy-MM-dd"), _isLocalAnalyst);
                            worksheetCount++;
                        }

                        #endregion

                        _log.InfoFormat("Starting Per Daily SERVER at: {0}", DateTime.Now);


                        #region SERVER

                        _log.InfoFormat("Starting Per Daily SERVER (CPU Busy) at: {0}", DateTime.Now);

                        serverCPUBusy = serverService.GetServerCPUBusyFor(dtStart, dtStart.AddDays(1), list, _ipu);
                        _log.InfoFormat("Per Daily SERVER (CPU BUSY) Count: {0}", serverCPUBusy.Count);

                        if (serverCPUBusy.Count > 0)
                        {
                            excelChartServer.InsertWorksheetCPUBusy(xlWorkPackage.Workbook, worksheetCount, list, serverCPUBusy, reports.ServerCPUBusy, _saveLocation,
                                "", "", "", "",
                                worksheetNameToc, list + PathwayCollectionServerCPUBusyTitle + dtStart.ToString("yy-MM-dd"), true, _isLocalAnalyst);
                            worksheetCount++;
                        }
                        _log.InfoFormat("Starting Per Daily SERVER (Transaction Count) at: {0}", DateTime.Now);

                        serverTransactionCount = serverService.GetServerTransactionsFor(dtStart, dtStart.AddDays(1), list, _ipu);
                        _log.InfoFormat("Per Daily SERVER (Transaction Count) Count: {0}", serverTransactionCount.Count);

                        if (serverTransactionCount.Count > 0)
                        {
                            excelChartServer.InsertWorksheetCPUBusy(xlWorkPackage.Workbook, worksheetCount, list, serverTransactionCount, reports.ServerTransactions, _saveLocation,
                                "", "", "", "",
                                worksheetNameToc, list + PathwayCollectionServerTransactionTitle + dtStart.ToString("yy-MM-dd"), false, _isLocalAnalyst);
                            worksheetCount++;
                        }

                        #endregion

                        _log.InfoFormat("Starting Per Daily TCP at: {0}", DateTime.Now);


                        #region TCP

                        //TRANSACTION
                        tcpTransaction = tcpService.GetTcpTransactionFor(dtStart, dtStart.AddDays(1), list);
                        _log.InfoFormat("Per Daily TCP (TRANSACTION) Count: {0}", tcpTransaction.Count);

                        if (tcpTransaction.Count > 0)
                        {
                            excelChartTCP.InsertWorksheetTcpTransaction(xlWorkPackage.Workbook, worksheetCount, reports.TCPTransactions, tcpTransaction, reports.TCPTransactions, _saveLocation,
                                "", "", "", "",
                                worksheetNameToc, list + PathwayCollectionTCPQueTransactionTitle + dtStart.ToString("yy-MM-dd"), _isLocalAnalyst);
                            worksheetCount++;
                        }

                        #endregion

                        if (hourlyDetail.Count > 0 ||
                            serverCPUBusy.Count > 0 ||
                            serverTransactionCount.Count > 0 ||
                            tcpTransaction.Count > 0)
                            category.Add(list + " - " + dtStart.ToString("yyyy-MM-dd dddd"), reports);

                        #endregion
                    }

                    _log.InfoFormat("Build Table of Content at {0}", DateTime.Now);

                    //Build Table of Content.
                    excelChart.InsertPathwayTOC(xlWorkPackage.Workbook, worksheetCount, list, category, worksheetNameToc, _saveLocation, PerPathwayFileInfo);

                    _log.InfoFormat("Build MACROS at {0}", DateTime.Now);


                    var macro = new Macros();

                    xlWorkPackage.Workbook.CreateVBAProject();
                    xlWorkPackage.Workbook.CodeModule.Code = macro.GenerateThisWorkbookPathway();

                    var module = xlWorkPackage.Workbook.VbaProject.Modules.AddModule("Pathway_Modules");
                    module.Code = macro.GeneratePathwayMacroInterval(worksheetNameToc);


                    _log.InfoFormat("Reverse the order of excel worksheet at {0}", DateTime.Now);


                    xlWorkPackage.Workbook.Worksheets.MoveToStart(xlWorkPackage.Workbook.Worksheets.Count);
                    excelChart.GetWorksheetNavigation(xlWorkPackage.Workbook, _saveLocation);


                    string fileName = _saveLocation + @"\" + _systemName + "(" + _systemSerial + ") - " + list +
                                      " - " +
                                      _fromTimestamp.ToString("yyyy-MM-dd") + " to " +
                                      _toTimestamp.AddDays(-1).ToString("yyyy-MM-dd") + ".xlsm"; //Since we are adding 1 extra day when customer submits the order, we need to remove extra day when displaying the end date.

                    _log.InfoFormat("fileName: {0}", fileName);

                    if (File.Exists(fileName))
                        File.Delete(fileName);

                    FileInfo fileInfo = new FileInfo(fileName);
                    xlWorkPackage.SaveAs(fileInfo);
                    xlWorkPackage.Dispose();

                    var excelFileInfo = new ExcelFileInfo
                    {
                        ExcelReportDate = DateTime.MinValue,
                        FileName = list +
                                   " - " +
                                   _fromTimestamp.ToString("yyyy-MM-dd") + " to " +
                                   _toTimestamp.AddDays(-1).ToString("yyyy-MM-dd") + ".xlsm",
                        AlertWorksheetName = list + PathwayCollectionAlerts,
                        CPUSummaryWorksheetName = list + PathwayCollectionCPUSummary,
                        CPUDetailWorksheetName = list + PathwayCollectionCPUDetail,
                        ServerTransactionWorksheetName = list + PathwayCollectionTransServer,
                        TCPTransactionWorksheetName = list + PathwayCollectionTransTCP
                    };

                    //Save the FileName for hyperlink from other pages.
                    if (PerPathwayFileInfo == null)
                        PerPathwayFileInfo = new Dictionary<string, List<ExcelFileInfo>>();

                    if (!PerPathwayFileInfo.ContainsKey(list))
                    {
                        PerPathwayFileInfo.Add(list, new List<ExcelFileInfo> { excelFileInfo });
                    }
                    else
                    {
                        PerPathwayFileInfo[list].Add(excelFileInfo);
                    }

                    #endregion
                }

                //});
            }
        }


        internal void GeneratePerPathwayDetailDailyInParallel(List<string> alertList, DateTime fromTimestamp, DateTime toTimestamp)
        {
            _log.Info("********************************************************************");
            _log.InfoFormat("Starting GeneratePerPathwayDetailDailyInParallel at: {0}", DateTime.Now);

            _reportDownloadLogs = new ReportDownloadLogs();
            var reportCount = 1;

            foreach (var list in _pathwayList)
            {
                //Parallel.ForEach(_pathwayList, list => {
                //Check if there is a PathwayName on PvPwylist.
                var checkPathway = new PvPwyListService();
                var exits = checkPathway.CheckPathwayNameFor(list, fromTimestamp, toTimestamp);
                if (exits)
                {
                    _reportDownloadLogs.InsertNewLog(_reportDownloadId, DateTime.Now, "Generating daily report for " + list + " (" + reportCount + " of " + _pathwayList.Count + ")");
                    _log.Info("------------------------------------------------------------------------");
                    _log.InfoFormat("Starting Pathway {0} at: {1}", list, DateTime.Now);


                    #region Per Day

                    var worksheetNameToc = list + " - Table Of Contents";

                    //var xlApp = new Application();
                    var excelChart = new ExcelChart(_reportDownloadId, _reportDownloadLogs, _log);
                    var excelChartServer = new ExcelChartServer(_reportDownloadId, _reportDownloadLogs, _log);
                    var excelChartTerm = new ExcelChartTerm(_reportDownloadId, _reportDownloadLogs, _log);
                    var excelChartTCP = new ExcelChartTCP(_reportDownloadId, _reportDownloadLogs, _log);

                    //var xlWorkBook = excelChart.CreateExcelObject(xlApp);
                    ExcelPackage xlWorkPackage = new ExcelPackage();
                    var worksheetCount = 1;

                    var category = new Dictionary<String, Collection>();
                    //Generate Collection worksheets
                    var reports = new Collection
                    {
                        //Alert = list + PathwayCollectionAlerts,
                        //CPUSummary = list + PathwayCollectionCPUSummary,
                        CPUDetail = list + PathwayCollectionCPUDetail,
                        TransactionServer = list + PathwayCollectionTransServer,
                        //TransactionTCP = list + PathwayCollectionTransTCP,
                        ServerCPUBusy = list + PathwayCollectionServerCPUBusy,
                        ServerTransactions = list + PathwayCollectionServerTransaction,
                        //ServerProcessCount = list + PathwayCollectionServerProCount,
                        ServerQueuedTCP = list + PathwayCollectionServerQueTCP,
                        ServerQueuedLinkmon = list + PathwayCollectionServerQueLinkmon,
                        ServerUnusedServerClasses = list + PathwayCollectionServerUnuseClass,
                        ServerUnusedServerProcesses = list + PathwayCollectionServerUnusePro,
                        TermTop20 = list + PathwayCollectionTermTop20,
                        TermUnused = list + PathwayCollectionTermUnused,
                        TCPTransactions = list + PathwayCollectionTCPTransaction,
                        TCPQueuedTransactions = list + PathwayCollectionTCPQueTransaction,
                        TCPUnusedTCPs = list + PathwayCollectionTCPUnusedTCP
                    };

                    //var nextSection = list + "-" + fromTimestamp.ToString("HHmm") + DailyCPUDetail;

                    var dateRange = fromTimestamp.ToString("yyyy-MM-dd dddd");

                    _log.InfoFormat("Calling Basic Data at {0}", DateTime.Now);


                    #region Basic Data

                    IPvAlertService alertService = new PvAlertService(_intervalInSec);
                    IPerPathwayService service = new PerPathwayService(_intervalInSec);

                    _log.InfoFormat("Calling Basic (CPU Detail Per Pathway) at {0}", DateTime.Now);

                    _reportDownloadLogs.InsertNewLog(_reportDownloadId, DateTime.Now, "    Generating CPU Detail Per Pathway");
                    var detail = service.GetCPUBusyDetailFor(fromTimestamp, toTimestamp, list, _ipu, _systemSerial);
                    excelChart.InsertWorksheetCPUDetailPerPathway(xlWorkPackage.Workbook, worksheetCount, list, detail, reports.CPUDetail, _saveLocation, "", "", "", "", worksheetNameToc,
                        list + PathwayCollectionCPUDetailTitle + dateRange, _isLocalAnalyst);
                    worksheetCount++;

                    _log.InfoFormat("Calling Basic (Transaction Per Pathway Interval) at {0}", DateTime.Now);

                    _reportDownloadLogs.InsertNewLog(_reportDownloadId, DateTime.Now, "    Generating Transaction Per Pathway Interval");
                    var transactionServer = service.GetIntervalTransactionServerFor(fromTimestamp, toTimestamp, list, Enums.IntervalTypes.Hourly);
                    var transactionTcp = service.GetIntervalTransactionTCPFor(fromTimestamp, toTimestamp, list, Enums.IntervalTypes.Hourly);
                    excelChart.InsertWorksheetTransactionPerPathwayInterval(xlWorkPackage.Workbook, worksheetCount, list, transactionServer, transactionTcp, reports.TransactionServer, _saveLocation, "", "", "", "",
                        worksheetNameToc, list + PathwayCollectionTransTitle + dateRange, _isLocalAnalyst);
                    worksheetCount++;

                    #endregion


                    _log.InfoFormat("Calling SERVER at {0}", DateTime.Now);


                    #region SERVER

                    IPerPathwayServerService serverService = new PerPathwayServerService(_intervalInSec);

                    _log.InfoFormat("Calling SERVER (CPU BUSY) at {0}", DateTime.Now);

                    //CPU BUSY
                    _reportDownloadLogs.InsertNewLog(_reportDownloadId, DateTime.Now, "    Generating SERVER (CPU BUSY)");
                    var serverCPUBusy = serverService.GetServerCPUBusyFor(fromTimestamp, toTimestamp, list, _ipu);
                    _log.InfoFormat("SERVER (CPU BUSY) Count: {0}",  serverCPUBusy.Count);

                    if (serverCPUBusy.Count > 0)
                    {
                        excelChartServer.InsertWorksheetCPUBusy(xlWorkPackage.Workbook, worksheetCount, list, serverCPUBusy, reports.ServerCPUBusy, _saveLocation, "", "", "", "", worksheetNameToc,
                            list + PathwayCollectionServerCPUBusyTitle + dateRange, true, _isLocalAnalyst);
                        worksheetCount++;
                    }

                    _log.InfoFormat("Calling SERVER (TRANSACTIONS) at {0}", DateTime.Now);

                    //TRANSACTIONS
                    _reportDownloadLogs.InsertNewLog(_reportDownloadId, DateTime.Now, "    Generating SERVER (CPU TRANSACTIONS)");
                    var serverTransactionCount = serverService.GetServerTransactionsFor(fromTimestamp, toTimestamp, list, _ipu);
                    _log.InfoFormat("SERVER (TRANSACTIONS) Count: {0}",  serverTransactionCount.Count);

                    if (serverTransactionCount.Count > 0)
                    {
                        excelChartServer.InsertWorksheetCPUBusy(xlWorkPackage.Workbook, worksheetCount, list, serverTransactionCount, reports.ServerTransactions, _saveLocation, "", "", "", "", worksheetNameToc,
                            list + PathwayCollectionServerTransactionTitle + dateRange, false, _isLocalAnalyst);
                        worksheetCount++;
                    }

                    _log.InfoFormat("Calling SERVER (QUEUED TCP) at {0}", DateTime.Now);


                    _reportDownloadLogs.InsertNewLog(_reportDownloadId, DateTime.Now, "    Generating SERVER (QUEUED TCP)");
                    #region QUEUED TCP

                    var queueTcp = serverService.GetServerQueueTCPIntervalFor(fromTimestamp, toTimestamp, list, Enums.IntervalTypes.Hourly);
                    var subDatasTcp = alertService.GetQueueTCPSubFor(fromTimestamp, toTimestamp, list, Enums.IntervalTypes.Hourly);

                    string nextWorksheet = reports.ServerQueuedLinkmon;
                    //Check Next worksheet.
                    if (subDatasTcp.Count(x => x.Value.Count > 0) > 0)
                    {
                        var tempData = subDatasTcp.First(subData => subData.Value.Count > 0);
                        nextWorksheet = list + PathwayCollectionServerQueTCPSub + Convert.ToDateTime(tempData.Key).ToString("HHmm");
                    }

                    var excelChartAlert = new ExcelChartAlert(_reportDownloadId, _reportDownloadLogs, _log);
                    string previousWorksheet = reports.ServerQueuedTCP;

                    _log.InfoFormat("SERVER (QUEUED TCP) Count: {0}",  queueTcp.Count);
                    _log.InfoFormat("SERVER (QUEUED TCP) Sub Count: {0}",  subDatasTcp.Count);

                    if (queueTcp.Count > 0)
                    {
                        excelChartAlert.InsertWorksheetQueues(xlWorkPackage.Workbook, worksheetCount, list, reports.ServerQueuedTCP, _saveLocation,
                            "", "", "", "",
                            worksheetNameToc, list + PathwayCollectionServerQueTCPTitle + dateRange, subDatasTcp, Enums.IntervalTypes.Hourly, _isLocalAnalyst, queueTcp, null, false);
                        worksheetCount++;

                        var subCountTcp = 0;
                        //Get only data with values.
                        var subDatasTcpWithValue = subDatasTcp.Where(subData => subData.Value.Count > 0).ToDictionary(x => x.Key, x => x.Value);

                        foreach (var subData in subDatasTcpWithValue)
                        {
                            //Get Previous
                            if (subCountTcp == 0)
                                previousWorksheet = list + PathwayCollectionServerQueTCPTitle + dateRange;
                            else
                            {
                                string previouKey = subDatasTcpWithValue.Keys.ElementAt(subCountTcp - 1);
                                previousWorksheet = list + PathwayCollectionServerQueTCPSub + Convert.ToDateTime(previouKey).ToString("HHmm");
                            }

                            //Get Next worksheet.
                            if ((subCountTcp + 1) < subDatasTcpWithValue.Count)
                            {
                                string nextKey = subDatasTcpWithValue.Keys.ElementAt(subCountTcp + 1);
                                nextWorksheet = list + PathwayCollectionServerQueTCPSub + Convert.ToDateTime(nextKey).ToString("HHmm");
                            }
                            else
                            {
                                nextWorksheet = reports.ServerQueuedLinkmon;
                            }

                            excelChartAlert.InsertWorksheetQueueSubs(xlWorkPackage.Workbook, worksheetCount, list, list + PathwayCollectionServerQueTCPSub + Convert.ToDateTime(subData.Key).ToString("HHmm"), _saveLocation,
                                "", "", "", "",
                                worksheetNameToc, list + PathwayCollectionServerQueTCPTitleSub + subData.Key, subData.Value, false, _isLocalAnalyst);
                            worksheetCount++;
                            subCountTcp++;
                        }
                    }

                    //Get previous worksheet for the Server QueueLinkmon
                    if (subDatasTcp.Count(x => x.Value.Count > 0) > 0)
                    {
                        var tempData = subDatasTcp.Last(subData => subData.Value.Count > 0);
                        previousWorksheet = list + PathwayCollectionServerQueTCPSub + Convert.ToDateTime(tempData.Key).ToString("HHmm");
                    }


                    #endregion

                    _log.InfoFormat("Calling SERVER (QUEUED LINKMON) at {0}", DateTime.Now);


                    _reportDownloadLogs.InsertNewLog(_reportDownloadId, DateTime.Now, "    Generating SERVER (QUEUED LINKMON)");
                    #region QUEUED LINKMON

                    var queueLinkmon = serverService.GetServerQueueLinkmonIntervalFor(fromTimestamp, toTimestamp, list, Enums.IntervalTypes.Hourly);
                    var subDatasLinkmon = alertService.GetQueueLinkmonSubFor(fromTimestamp, toTimestamp, list, Enums.IntervalTypes.Hourly);

                    //Check Next worksheet.
                    if (subDatasLinkmon.Count(x => x.Value.Count > 0) > 0)
                    {
                        var tempData = subDatasLinkmon.First(subData => subData.Value.Count > 0);
                        nextWorksheet = list + PathwayCollectionServerQueLinkmonSub + Convert.ToDateTime(tempData.Key).ToString("HHmm");
                    }
                    else
                    {
                        nextWorksheet = reports.ServerUnusedServerClasses;
                    }
                    _log.InfoFormat("SERVER (QUEUED LINKMON) Count: {0}",  queueLinkmon.Count);
                    _log.InfoFormat("SERVER (QUEUED LINKMON) Sub Count: {0}",  subDatasLinkmon.Count);


                    if (queueLinkmon.Count > 0)
                    {
                        excelChartAlert.InsertWorksheetQueues(xlWorkPackage.Workbook, worksheetCount, list, reports.ServerQueuedLinkmon, _saveLocation,
                            "", "", "", "",
                            worksheetNameToc, list + PathwayCollectionServerQueLinkmonTitle + dateRange, subDatasLinkmon, Enums.IntervalTypes.Hourly, _isLocalAnalyst, null, queueLinkmon, false);
                        worksheetCount++;

                        var subCountLinkmon = 0;
                        //Get only data with values.
                        var subDatasLinkmonWithValue = subDatasLinkmon.Where(subData => subData.Value.Count > 0).ToDictionary(x => x.Key, x => x.Value);

                        foreach (var subData in subDatasLinkmonWithValue)
                        {
                            //Get Previous
                            if (subCountLinkmon == 0)
                                previousWorksheet = reports.ServerQueuedLinkmon;
                            else
                            {
                                string previouKey = subDatasLinkmonWithValue.Keys.ElementAt(subCountLinkmon - 1);
                                previousWorksheet = list + PathwayCollectionServerQueLinkmonSub + Convert.ToDateTime(previouKey).ToString("HHmm");
                            }
                            //Get Next worksheet.
                            if ((subCountLinkmon + 1) < subDatasLinkmonWithValue.Count)
                            {
                                string nextKey = subDatasLinkmonWithValue.Keys.ElementAt(subCountLinkmon + 1);
                                nextWorksheet = list + PathwayCollectionServerQueLinkmonSub + Convert.ToDateTime(nextKey).ToString("HHmm");
                            }
                            else
                            {
                                nextWorksheet = reports.ServerUnusedServerClasses;
                            }

                            excelChartAlert.InsertWorksheetQueueSubs(xlWorkPackage.Workbook, worksheetCount, list, list + PathwayCollectionServerQueLinkmonSub + Convert.ToDateTime(subData.Key).ToString("HHmm"), _saveLocation,
                                "", "", "", "",
                                worksheetNameToc, list + PathwayCollectionServerQueLinkmonTitleSub + subData.Key, subData.Value, true, _isLocalAnalyst);
                            worksheetCount++;
                            subCountLinkmon++;
                        }
                    }

                    //Get previous worksheet for the ServerUnusedClass 
                    if (subDatasLinkmon.Count(x => x.Value.Count > 0) > 0)
                    {
                        var tempData = subDatasLinkmon.Last(subData => subData.Value.Count > 0);
                        previousWorksheet = list + PathwayCollectionServerQueLinkmonSub + Convert.ToDateTime(tempData.Key).ToString("HHmm");
                    }

                    #endregion

                    _log.InfoFormat("Calling SERVER (UNUSED CLASSES) at {0}", DateTime.Now);

                    _reportDownloadLogs.InsertNewLog(_reportDownloadId, DateTime.Now, "    Generating SERVER (UNUSED CLASSES)");
                    //UNUSED CLASSES
                    var unusedClass = serverService.GetServerUnusedClassesIntervalFor(fromTimestamp, toTimestamp, list, Enums.IntervalTypes.Hourly);
                    _log.InfoFormat("SERVER (UNUSED CLASSES) Count: {0}",  unusedClass.Count);


                    if (unusedClass.Count > 0)
                    {
                        excelChartServer.InsertWorksheetUnusedClassInterval(xlWorkPackage.Workbook, worksheetCount, list, unusedClass, reports.ServerUnusedServerClasses, _saveLocation,
                            "", "", "", "",
                            worksheetNameToc, list + PathwayCollectionServerUnuseClassTitle + dateRange, _isLocalAnalyst);
                        worksheetCount++;
                    }

                    _log.InfoFormat("Calling SERVER (UNUSED PROCESSES) at {0}", DateTime.Now);


                    _reportDownloadLogs.InsertNewLog(_reportDownloadId, DateTime.Now, "    Generating SERVER (UNUSED PROCESSES)");
                    //UNUSED PROCESSES
                    var unusedProcess = serverService.GetServerUnusedProcessesIntervalFor(fromTimestamp, toTimestamp, list, Enums.IntervalTypes.Hourly);
                    _log.InfoFormat("SERVER (UNUSED PROCESSES) Count: {0}",  unusedProcess.Count);

                    if (unusedProcess.Count > 0)
                    {
                        excelChartServer.InsertWorksheetUnusedProcessInterval(xlWorkPackage.Workbook, worksheetCount, list, unusedProcess, reports.ServerUnusedServerProcesses, _saveLocation,
                            "", "", "", "",
                            worksheetNameToc, list + PathwayCollectionServerUnuseProTitle + dateRange, _isLocalAnalyst);
                        worksheetCount++;
                    }


                    #endregion

                    _log.InfoFormat("Calling TCP at {0}", DateTime.Now);


                    #region TCP

                    _log.InfoFormat("Calling TCP (TRANSACTION) at {0}", DateTime.Now);

                    _reportDownloadLogs.InsertNewLog(_reportDownloadId, DateTime.Now, "    Generating TCP (TRANSACTION)");
                    IPerPathwayTcpService tcpService = new PerPathwayTcpService(_intervalInSec);
                    //TRANSACTION
                    var tcpTransaction = tcpService.GetTcpTransactionFor(fromTimestamp, toTimestamp, list);
                    _log.InfoFormat("TCP (TRANSACTION) Count: {0}",  tcpTransaction.Count);

                    if (tcpTransaction.Count > 0)
                    {
                        excelChartTCP.InsertWorksheetTcpTransaction(xlWorkPackage.Workbook, worksheetCount, reports.TCPTransactions, tcpTransaction, reports.TCPTransactions, _saveLocation,
                            "", "", "", "",
                            worksheetNameToc, list + PathwayCollectionTCPQueTransactionTitle + dateRange, _isLocalAnalyst);
                        worksheetCount++;
                    }

                    _log.InfoFormat("Calling TCP (QUEUED TRANSACTIONS) at {0}", DateTime.Now);

                    _reportDownloadLogs.InsertNewLog(_reportDownloadId, DateTime.Now, "    Generating TCP (QUEUED TRANSACTIONS)");

                    #region QUEUED TRANSACTIONS

                    var tcpQueued = tcpService.GetTcpQueuedTransactionFor(fromTimestamp, toTimestamp, list, Enums.IntervalTypes.Hourly);
                    var subDatas = tcpService.GetQueueTCPSubFor(fromTimestamp, toTimestamp, list, Enums.IntervalTypes.Hourly);

                    _log.InfoFormat("tcpQueued Count: {0}",  tcpQueued.Count);
                    _log.InfoFormat("subDatas Count: {0}",  subDatas.Count);

                    nextWorksheet = reports.TCPUnusedTCPs;
                    //Check Next worksheet.
                    if (subDatas.Count(x => x.Value.Count > 0) > 0)
                    {
                        var tempData = subDatas.First(subData => subData.Value.Count > 0);
                        nextWorksheet = list + PathwayCollectionTCPQueTransactionSub + Convert.ToDateTime(tempData.Key).ToString("HHmm");
                    }

                    if (tcpQueued.Count > 0)
                    {
                        excelChartTCP.InsertWorksheetQueues(xlWorkPackage.Workbook, worksheetCount, list, reports.TCPQueuedTransactions, _saveLocation,
                            "", "", "", "", worksheetNameToc,
                            list + PathwayCollectionTCPQueTransactionTitle + dateRange, tcpQueued, subDatas, Enums.IntervalTypes.Hourly, _isLocalAnalyst);
                        worksheetCount++;

                        var subCount = 0;
                        //Get only data with values.
                        var subDatasWithValue = subDatas.Where(subData => subData.Value.Count > 0).ToDictionary(x => x.Key, x => x.Value);

                        previousWorksheet = reports.TCPTransactions;

                        foreach (var subData in subDatasWithValue)
                        {
                            //Get Previous
                            if (subCount == 0)
                                previousWorksheet = reports.TCPQueuedTransactions;
                            else
                            {
                                string previouKey = subDatasWithValue.Keys.ElementAt(subCount - 1);
                                previousWorksheet = list + PathwayCollectionTCPQueTransactionSub + Convert.ToDateTime(previouKey).ToString("HHmm");
                            }

                            //Get Next worksheet.
                            if ((subCount + 1) < subDatasWithValue.Count)
                            {
                                string nextKey = subDatasWithValue.Keys.ElementAt(subCount + 1);
                                nextWorksheet = list + PathwayCollectionTCPQueTransactionSub + Convert.ToDateTime(nextKey).ToString("HHmm");
                            }
                            else
                            {
                                nextWorksheet = reports.TCPUnusedTCPs;
                            }

                            excelChartTCP.InsertWorksheetQueueSubs(xlWorkPackage.Workbook, worksheetCount, list, list + PathwayCollectionTCPQueTransactionSub + Convert.ToDateTime(subData.Key).ToString("HHmm"), _saveLocation,
                                "", "", "", "",
                                worksheetNameToc, list + PathwayCollectionServerQueTCPTitleSub + subData.Key, subData.Value, _isLocalAnalyst);
                            worksheetCount++;
                            subCount++;
                        }
                    }
                    //Get previous worksheet for the Server QueueLinkmon
                    if (subDatas.Count(x => x.Value.Count > 0) > 0)
                    {
                        var tempData = subDatas.Last(subData => subData.Value.Count > 0);
                        previousWorksheet = list + PathwayCollectionTCPQueTransactionSub + Convert.ToDateTime(tempData.Key).ToString("HHmm");
                    }

                    #endregion

                    _log.InfoFormat("Calling TCP (UNUSED TCP) at {0}", DateTime.Now);

                    _reportDownloadLogs.InsertNewLog(_reportDownloadId, DateTime.Now, "    Generating TCP (UNUSED TCP)");
                    //UNUSED TCP
                    var tcpUnused = tcpService.GetTcpUnusedFor(fromTimestamp, toTimestamp, list, Enums.IntervalTypes.Hourly);
                    _log.InfoFormat("TCP (UNUSED TCP) Count: {0}",  tcpUnused.Count);


                    if (tcpUnused.Count > 0)
                    {
                        excelChartServer.InsertWorksheetUnusedClassInterval(xlWorkPackage.Workbook, worksheetCount, list, tcpUnused, reports.TCPUnusedTCPs, _saveLocation,
                            "", "", "", "", worksheetNameToc,
                            list + PathwayCollectionTCPUnusedTCPTitle + dateRange, true);
                        worksheetCount++;
                    }

                    #endregion


                    _log.InfoFormat("Calling TERM at {0}", DateTime.Now);


                    #region TERM

                    _log.InfoFormat("Calling TERM (Term Top 20) at {0}", DateTime.Now);

                    _reportDownloadLogs.InsertNewLog(_reportDownloadId, DateTime.Now, "    Generating TERM (Term Top 20)");
                    IPerPathwayTermService termService = new PerPathwayTermService(_intervalInSec);
                    var termTop20 = termService.GetTermTop20(fromTimestamp, toTimestamp, list, Enums.IntervalTypes.Hourly);
                    _log.InfoFormat("TERM (Term Top 20) Count: {0}",  termTop20.Count);


                    if (termTop20.Count > 0)
                    {
                        excelChartTerm.InsertWorksheetTermTop20(xlWorkPackage.Workbook, worksheetCount, list, termTop20, reports.TermTop20, _saveLocation,
                            "", "", "", "",
                            worksheetNameToc, list + PathwayCollectionTermTop20Title + dateRange, _isLocalAnalyst);
                        worksheetCount++;
                    }

                    _log.InfoFormat("Calling TERM (Term Unused) at {0}", DateTime.Now);

                    _reportDownloadLogs.InsertNewLog(_reportDownloadId, DateTime.Now, "    Generating TERM (Term Unused)");
                    var termUnused = termService.GetTermUnused(fromTimestamp, toTimestamp, list, Enums.IntervalTypes.Hourly);
                    _log.InfoFormat("TERM (Term Unused) Count: {0}",  termUnused.Count);

                    if (termUnused.Count > 0)
                    {
                        excelChartTerm.InsertWorksheetTermUnused(xlWorkPackage.Workbook, worksheetCount, list, termUnused, reports.TermUnused, _saveLocation,
                            "", "", "", "",
                            worksheetNameToc, list + PathwayCollectionTermUnusedTitle + dateRange, _isLocalAnalyst);
                        worksheetCount++;
                    }

                    #endregion

                    category.Add(list + " - " + fromTimestamp.ToString("yyyy-MM-dd dddd"), reports);

                    for (var dtStart = fromTimestamp; dtStart.Date < toTimestamp.Date; dtStart = dtStart.AddSeconds(_intervalInSec))
                    {
                        _log.InfoFormat("Calling Per Interval");


                        #region Per Interval

                        reports = new Collection
                        {
                            CPUDetail = list + "-" + dtStart.ToString("HHmm") + DailyCPUDetail,
                            TransactionServer = list + "-" + dtStart.ToString("HHmm") + DailyTrans,
                            ServerCPUBusy = list + "-" + dtStart.ToString("HHmm") + DailyServerCPUBusy,
                            ServerTransactions = list + "-" + dtStart.ToString("HHmm") + DailyServerTransaction,
                            ServerQueuedTCP = list + "-" + dtStart.ToString("HHmm") + DailyServerQueTCP,
                            ServerQueuedLinkmon = list + "-" + dtStart.ToString("HHmm") + DailyServerQueLinkmon,
                            ServerUnusedServerClasses = list + "-" + dtStart.ToString("HHmm") + DailyServerUnuseClass,
                            ServerUnusedServerProcesses = list + "-" + dtStart.ToString("HHmm") + DailyServerUnusePro,
                            TermTop20 = list + "-" + dtStart.ToString("HHmm") + DailyTermTop20,
                            TermUnused = list + "-" + dtStart.ToString("HHmm") + DailyTermUnused,
                            TCPTransactions = list + "-" + dtStart.ToString("HHmm") + DailyTCPTransaction,
                            TCPQueuedTransactions = list + "-" + dtStart.ToString("HHmm") + DailyTCPQueTransaction,
                            TCPUnusedTCPs = list + "-" + dtStart.ToString("HHmm") + DailyTCPUnusedTCP
                        };

                        dateRange = dtStart.ToString("yyyy-MM-dd HH:mm");

                        #region Basic Data

                        _log.InfoFormat("Calling Per Interval Basic Data (CPU BUSY) at {0}", DateTime.Now);

                        var hourlyDetail = service.GetCPUBusyDetailFor(dtStart, dtStart.AddSeconds(_intervalInSec), list, _ipu, _systemSerial);

                        _log.InfoFormat("Per Interval Basic Data (CPU BUSY) Count: {0}",  hourlyDetail.Count);

                        if (hourlyDetail.Count > 0)
                        {
                            excelChart.InsertWorksheetCPUDetailPerPathway(xlWorkPackage.Workbook, worksheetCount, list, hourlyDetail, reports.CPUDetail, _saveLocation,
                                "", "", "", "",
                                worksheetNameToc, list + PathwayCollectionCPUDetailTitle + dateRange, _isLocalAnalyst);
                            worksheetCount++;
                        }

                        #endregion

                        #region SERVER

                        _log.InfoFormat("Calling Per Interval SERVER (CPU BUSY) at {0}", DateTime.Now);

                        serverCPUBusy = serverService.GetServerCPUBusyFor(dtStart, dtStart.AddSeconds(_intervalInSec), list, _ipu);
                        _log.InfoFormat("Per Interval SERVER (CPU BUSY) Count: {0}",  serverCPUBusy.Count);

                        if (serverCPUBusy.Count > 0)
                        {
                            excelChartServer.InsertWorksheetCPUBusy(xlWorkPackage.Workbook, worksheetCount, list, serverCPUBusy, reports.ServerCPUBusy, _saveLocation,
                                "", "", "", "",
                                worksheetNameToc, list + PathwayCollectionServerCPUBusyTitle + dateRange, true, _isLocalAnalyst);
                            worksheetCount++;
                        }

                        _log.InfoFormat("Calling Per Interval SERVER (Transaction Count) at {0}", DateTime.Now);

                        serverTransactionCount = serverService.GetServerTransactionsFor(dtStart, dtStart.AddSeconds(_intervalInSec), list, _ipu);
                        _log.InfoFormat("Per Interval SERVER (Transaction Count) Count: {0}",  serverTransactionCount.Count);

                        if (serverTransactionCount.Count > 0)
                        {
                            excelChartServer.InsertWorksheetCPUBusy(xlWorkPackage.Workbook, worksheetCount, list, serverTransactionCount, reports.ServerTransactions, _saveLocation,
                                "", "", "", "",
                                worksheetNameToc, list + PathwayCollectionServerTransactionTitle + dateRange, false, _isLocalAnalyst);
                            worksheetCount++;
                        }

                        #endregion

                        #region TCP

                        _log.InfoFormat("Calling Per Interval TCP (TRANSACTION) at {0}", DateTime.Now);

                        //TRANSACTION
                        tcpTransaction = tcpService.GetTcpTransactionFor(dtStart, dtStart.AddSeconds(_intervalInSec), list);
                        _log.InfoFormat("Per Interval TCP (TRANSACTION) Count: {0}",  tcpTransaction.Count);

                        if (tcpTransaction.Count > 0)
                        {
                            excelChartTCP.InsertWorksheetTcpTransaction(xlWorkPackage.Workbook, worksheetCount, reports.TCPTransactions, tcpTransaction, reports.TCPTransactions, _saveLocation,
                                "", "", "", "",
                                worksheetNameToc, list + PathwayCollectionTCPQueTransactionTitle + dateRange, _isLocalAnalyst);
                            worksheetCount++;
                        }

                        #endregion

                        if (hourlyDetail.Count > 0 ||
                            serverCPUBusy.Count > 0 ||
                            serverTransactionCount.Count > 0 ||
                            tcpTransaction.Count > 0)
                            category.Add(list + " - " + dtStart.ToString("HH:mm"), reports);

                        #endregion
                    }

                    _log.InfoFormat("Build Table of Content at {0}", DateTime.Now);

                    //Build Table of Content.
                    excelChart.InsertPathwayTOC(xlWorkPackage.Workbook, worksheetCount, list, category, worksheetNameToc, _saveLocation);

                    _log.InfoFormat("Add MACROS at {0}", DateTime.Now);


                    var macro = new Macros();

                    xlWorkPackage.Workbook.CreateVBAProject();
                    xlWorkPackage.Workbook.CodeModule.Code = macro.GenerateThisWorkbookPathway();

                    var module = xlWorkPackage.Workbook.VbaProject.Modules.AddModule("Pathway_Modules");
                    module.Code = macro.GeneratePathwayMacroInterval(worksheetNameToc);


                    _log.InfoFormat("Reverse the order of excel worksheet at {0}", DateTime.Now);


                    //Reverse the order of excel worksheet, skip toc.
                    xlWorkPackage.Workbook.Worksheets.MoveToStart(xlWorkPackage.Workbook.Worksheets.Count);

                    //Generate new Navigation.
                    excelChart.GetWorksheetNavigation(xlWorkPackage.Workbook, _saveLocation);

                    string fileName = _saveLocation + @"\" + _systemName + "(" + _systemSerial + ") - " + list +
                                      " - " + fromTimestamp.ToString("yyyy-MM-dd") + ".xlsm";

                    _log.InfoFormat("fileName: {0}", fileName);

                    if (File.Exists(fileName))
                        File.Delete(fileName);

                    FileInfo fileInfo = new FileInfo(fileName);
                    xlWorkPackage.SaveAs(fileInfo);
                    xlWorkPackage.Dispose();

                    var excelFileInfo = new ExcelFileInfo
                    {
                        ExcelReportDate = fromTimestamp,
                        FileName = _systemName + "(" + _systemSerial + ") - " + list + " - " + fromTimestamp.ToString("yyyy-MM-dd") + ".xlsm",
                        AlertWorksheetName = list + PathwayCollectionAlerts,
                        CPUSummaryWorksheetName = list + PathwayCollectionCPUSummary,
                        CPUDetailWorksheetName = list + PathwayCollectionCPUDetail,
                        ServerTransactionWorksheetName = list + PathwayCollectionTransServer,
                        TCPTransactionWorksheetName = list + PathwayCollectionTransTCP
                    };

                    //Save the FileName for hyperlink from other pages.
                    if (PerPathwayFileInfo == null)
                        PerPathwayFileInfo = new Dictionary<string, List<ExcelFileInfo>>();

                    if (!PerPathwayFileInfo.ContainsKey(list))
                    {
                        PerPathwayFileInfo.Add(list, new List<ExcelFileInfo> { excelFileInfo });
                    }
                    else
                    {
                        PerPathwayFileInfo[list].Add(excelFileInfo);
                    }

                    #endregion
                }
                reportCount++;
                _log.InfoFormat("Line 2234: Processing {0} on thread {1}", list,
                    Thread.CurrentThread.ManagedThreadId);
                //});
            }
        }


    }
}