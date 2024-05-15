using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Pathway.Core.Abstract.Services;
using Pathway.Core.Concrete;
using Pathway.Core.Infrastructure;
using Pathway.Core.Infrastructure.PerPathway.Server;
using Pathway.Core.Abstract.Services;
using Pathway.Core.Abstract.Repositories;
using Pathway.Core.Repositories;

namespace Pathway.Core.Services
{
    public class PvAlertService : IPvAlertService
    {
        private readonly string _connectionString = "";
        private readonly long _intervalInSec;

        public PvAlertService(string connectionString, long intervalInSec)
        {
            _connectionString = connectionString;
            _intervalInSec = intervalInSec;
        }

        public void InsertEmptyDataFor(string pathwayName, DateTime fromTimestamp, DateTime toTimestamp)
        {
            IPvAlertsRepository alerts = new PvAlertsRepository();
            alerts.InsertEmptyData(pathwayName, fromTimestamp, toTimestamp);
        }

        public List<Alert> GetHourlyAlertFor(string alertName, DateTime fromTimestamp, DateTime toTimestamp)
        {
            var alertView = new List<Alert>();
            IPvAlertsRepository alerts = new PvAlertsRepository();

            switch (alertName)
            {
                case "TermHiMaxRT":
                    alertView = alerts.GetTermHiMaxRtHourly(
                        fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)),
                        toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));
                    break;
                case "TermHiAvgRT":
                    alertView = alerts.GetTermHiAvgRtHourly(
                        fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)),
                        toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));
                    break;
                case "TermUnused":
                    alertView = alerts.GetTermUnusedHourly(
                        fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)),
                        toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));
                    break;
                case "TermErrorList":
                    alertView = alerts.GetTermErrorListHourly(
                        fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)),
                        toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));
                    break;
                case "TCPQueuedTransactions":
                    alertView = alerts.GetTcpQueuedTransactionsHourly(
                        fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)),
                        toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));
                    break;
                case "TCPLowTermPool":
                    alertView = alerts.GetTcpLowTermPoolHourly(
                        fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)),
                        toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));
                    break;
                case "TCPLowServerPool":
                    alertView = alerts.GetTcpLowServerPoolHourly(
                        fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)),
                        toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));
                    break;
                case "TCPErrorList ":
                    alertView = alerts.GetTcpErrorListHourly(
                        fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)),
                        toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));
                    break;
                case "TCPUnused":
                    alertView = alerts.GetTcpUnusedHourly(
                        fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)),
                        toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));
                    break;
                case "ServerHiMaxRT":
                    alertView = alerts.GetServerHiMaxRtHourly(
                        fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)),
                        toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));
                    break;
                case "ServerHiAvgRT":
                    alertView = alerts.GetServerHiAvgRtHourly(
                        fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)),
                        toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));
                    break;
                case "ServerQueueTCP":
                    alertView = alerts.GetServerQueueTcpHourly(
                        fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)),
                        toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));
                    break;
                case "ServerQueueLinkmon":
                    alertView = alerts.GetServerQueueLinkmonHourly(
                        fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)),
                        toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));
                    break;
                case "ServerUnusedClass":
                    alertView = alerts.GetServerUnusedClassHourly(
                        fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)),
                        toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));
                    break;
                case "ServerUnusedProcess":
                    alertView = alerts.GetServerUnusedProcessHourly(
                        fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)),
                        toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));
                    break;
                case "ServerErrorList":
                    alertView = alerts.GetServerErrorListHourly(
                        fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)),
                        toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));
                    break;
            }
            return alertView;
        }

        public Dictionary<string, AlertView> GetLastIntervalAlertFor(List<string> alertList, DateTime fromTimestamp, DateTime toTimestamp)
        {
            //1. Go to PvCollects and get interval.
            IPvCollectsService collectService = new PvCollectsService();
            var intervalInSec = collectService.GetIntervalFor(fromTimestamp, toTimestamp);

            //2. using ToTimestamp, get last interval.
            DateTime newFromTimestamp = toTimestamp.AddSeconds(intervalInSec * -1);

            //3. With new FromTimestamp and ToTimestamp, call GetCollectionAlertFor
            var alertsInfo = GetCollectionAlertFor(alertList, newFromTimestamp, toTimestamp, "");

            return alertsInfo;

        }

        public Dictionary<string, Dictionary<DateTime, AlertView>> GetCollectionAlertForAllIntervals(List<string> alertList, DateTime fromTimestamp, DateTime toTimestamp)
        {
            var collectionAlerts = new Dictionary<string, Dictionary<DateTime, AlertView>>();



            IPvPwyListRepository pwyList = new PvPwyListRepository();
            var pwyLists = pwyList.GetPathwayNames(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));
            foreach (var pathway in pwyLists)
            {
                if (!collectionAlerts.ContainsKey(pathway))
                {
                    
                    var alerts = GetIntervalAlertFor(alertList, fromTimestamp, toTimestamp, pathway, Enums.IntervalTypes.Hourly);
                    //var alertsAfterSort = alerts.OrderByDescending(x => x.Key);
                    var items = from alert in alerts
                                orderby alert.Key descending
                                select alert;

                    //Only include the pathway which has at least one interval
                    if (items.Count() > 0)
                    {
                        collectionAlerts.Add(pathway, new Dictionary<DateTime, AlertView>());
                        collectionAlerts[pathway] = items.Select(t => new { t.Key, t.Value }).ToDictionary(t => t.Key, t => t.Value);
                    }
                    
                }
            }
            return collectionAlerts;
        }

        public Dictionary<string, AlertView> GetCollectionAlertFor(List<string> alertList, DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "")
        {
            var collectionAlerts = new Dictionary<string, AlertView>();

            if (pathwayName.Length == 0)
            {
                IPvPwyListRepository pwyList = new PvPwyListRepository();
                var pwyLists = pwyList.GetPathwayNames(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));
                foreach (var pathway in pwyLists)
                {
                    if (!collectionAlerts.ContainsKey(pathway))
                    {
                        collectionAlerts.Add(pathway, new AlertView());
                    }
                }
            }

            foreach (var pvAlert in alertList)
            {
                var alertView = GetCollectionAlertFor(pathwayName, pvAlert, fromTimestamp, toTimestamp);

                foreach (Alert view in alertView)
                {
                    if (!collectionAlerts.ContainsKey(view.PathwayName))
                    {
                        collectionAlerts.Add(view.PathwayName, new AlertView());
                    }

                    switch (pvAlert) 
                    {
                        case "TermHiMaxRT":
                            collectionAlerts[view.PathwayName].TermHiMaxRt = view.Count;
                            break;
                        case "TermHiAvgRT":
                            collectionAlerts[view.PathwayName].TermHiAvgRt = view.Count;
                            break;
                        case "TermUnused":
                            collectionAlerts[view.PathwayName].TermUnused = view.Count;
                            break;
                        case "TermErrorList":
                            collectionAlerts[view.PathwayName].TermErrorList = view.Count;
                            break;
                        case "TCPQueuedTransactions":
                            collectionAlerts[view.PathwayName].TcpQueuedTransactions = view.Count;
                            break;
                        case "TCPLowTermPool":
                            collectionAlerts[view.PathwayName].TcpLowTermPool = view.Count;
                            break;
                        case "TCPLowServerPool":
                            collectionAlerts[view.PathwayName].TcpLowServerPool = view.Count;
                            break;
                        case "TCPErrorList ":
                            collectionAlerts[view.PathwayName].TcpErrorList = view.Count;
                            break;
                        case "TCPUnused":
                            collectionAlerts[view.PathwayName].TcpUnused = view.Count;
                            break;
                        case "ServerHiMaxRT":
                            collectionAlerts[view.PathwayName].ServerHiMaxRt = view.Count;
                            break;
                        case "ServerHiAvgRT":
                            collectionAlerts[view.PathwayName].ServerHiAvgRt = view.Count;
                            break;
                        case "ServerPendingClass":
                            collectionAlerts[view.PathwayName].ServerPendingClass = view.Count;
                            break;
                        case "ServerPendingProcess":
                            collectionAlerts[view.PathwayName].ServerPendingProcess = view.Count;
                            break;
                        case "ServerQueueTCP":
                            collectionAlerts[view.PathwayName].ServerQueueTcp = view.Count;
                            break;
                        case "ServerQueueLinkmon":
                            collectionAlerts[view.PathwayName].ServerQueueLinkmon = view.Count;
                            break;
                        case "ServerUnusedClass":
                            collectionAlerts[view.PathwayName].ServerUnusedClass = view.Count;
                            break;
                        case "ServerUnusedProcess":
                            collectionAlerts[view.PathwayName].ServerUnusedProcess = view.Count;
                            break;
                        case "ServerMaxLinks":
                            collectionAlerts[view.PathwayName].ServerMaxLinks = view.Count;
                            break;
                        case "CheckDirectoryOn":
                            collectionAlerts[view.PathwayName].DirectoryOnLinks = view.Count;
                            break;
                        case "HighDynamicServers":
                            collectionAlerts[view.PathwayName].HighDynamicServers = view.Count;
                            break;
                        case "ServerErrorList":
                            collectionAlerts[view.PathwayName].ServerErrorList = view.Count;
                            break;
                    }
                }
            }

            return collectionAlerts;
        }

        public void UpdateAlertFor(string alertName, List<Alert> alert)
        {
            IPvAlertsRepository alerts = new PvAlertsRepository();

            foreach (Alert alertView in alert)
            {
                //Since the from and to time is not always same, give 10% allow time.
                alerts.UpdateAlert(alertName, alertView.PathwayName, alertView.Count,
                    alertView.FromTimestamp.AddSeconds(_intervalInSec * -0.10),
                    alertView.ToTimestamp.AddSeconds(_intervalInSec * 0.10));
            }
        }

        public List<Alert> GetCollectionAlertFor(string pathwayName, string alertName, DateTime fromTimestamp, DateTime toTimestamp)
        {
            var alertView = new List<Alert>();
            IPvAlertsRepository alerts = new PvAlertsRepository();

            switch (alertName) 
            {
                case "TermHiMaxRT":
                    alertView = alerts.GetTermHiMaxRt(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);
                    break;
                case "TermHiAvgRT":
                    alertView = alerts.GetTermHiAvgRt(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);
                    break;
                case "TermUnused":
                    alertView = alerts.GetTermUnused(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);
                    break;
                case "TermErrorList":
                    alertView = alerts.GetTermErrorList(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);
                    break;
                case "TCPQueuedTransactions":
                    alertView = alerts.GetTcpQueuedTransactions(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);
                    break;
                case "TCPLowTermPool":
                    alertView = alerts.GetTcpLowTermPool(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);
                    break;
                case "TCPLowServerPool":
                    alertView = alerts.GetTcpLowServerPool(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);
                    break;
                case "TCPErrorList ":
                    alertView = alerts.GetTcpErrorList(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);
                    break;
                case "TCPUnused":
                    alertView = alerts.GetTcpUnused(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);
                    break;
                case "ServerHiMaxRT":
                    alertView = alerts.GetServerHiMaxRt(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);
                    break;
                case "ServerHiAvgRT":
                    alertView = alerts.GetServerHiAvgRt(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);
                    break;
                case "ServerPendingClass":
                    alertView = alerts.GetServerPendingClass(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);
                    break;
                case "ServerPendingProcess":
                    alertView = alerts.GetServerPendingProcess(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);
                    break;
                case "ServerQueueTCP":
                    alertView = alerts.GetServerQueueTcp(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);
                    break;
                case "ServerQueueLinkmon":
                    alertView = alerts.GetServerQueueLinkmon(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);
                    break;
                case "ServerUnusedClass":
                    alertView = alerts.GetServerUnusedClass(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);
                    break;
                case "ServerUnusedProcess":
                    alertView = alerts.GetServerUnusedProcess(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);
                    break;
                case "ServerMaxLinks":
                    alertView = alerts.GetServerMaxLinks(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);
                    break;
                case "CheckDirectoryOn":
                    alertView = alerts.GetCheckDirectoryOn(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);
                    break;
                case "HighDynamicServers":
                    alertView = alerts.GetHighDynamicServers(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);
                    break;
                case "ServerErrorList":
                    alertView = alerts.GetServerErrorList(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);
                    break;
            }
            return alertView;
        }

        public List<Alert> GetCollectionAlertForInterval(string pathwayName, string alertName, DateTime fromTimestamp, DateTime toTimestamp)
        {
            var alertView = new List<Alert>();
            IPvAlertsRepository alerts = new PvAlertsRepository();

            switch (alertName)
            {                
                case "ServerPendingClass":
                    alertView = alerts.GetServerPendingClass(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);
                    break;
                case "ServerPendingProcess":
                    alertView = alerts.GetServerPendingProcess(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);
                    break;
                case "ServerQueueLinkmon":
                    alertView = alerts.GetServerQueueLinkmon(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);
                    break;
                case "ServerErrorList":
                    alertView = alerts.GetServerErrorList(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);
                    break;
            }
            return alertView;
        }

        public Dictionary<DateTime, AlertView> GetIntervalAlertFor(List<string> alertList, DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, Enums.IntervalTypes intervalTypes)
        {
            var collectionAlerts = new Dictionary<DateTime, AlertView>();
//            alertList.Add("PathwayName");
            for (var dtStart = fromTimestamp; dtStart.Date < toTimestamp.Date; dtStart = IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec))
            {
                foreach (var pvAlert in alertList)
                {
                    var alertView = GetCollectionAlertForInterval(pathwayName, pvAlert, dtStart, IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec));

                    var displayValue = dtStart;
                    //if (intervalTypes == Enums.IntervalTypes.Hourly)
                    //    displayValue = dtStart.ToString("HH:mm");
                    //else
                    //    displayValue = dtStart.ToString("yyyy-MM-dd");

                    //If alertView is empty, insert 0 value.
                    if (alertView.Count == 0) {
                        continue;
                        #region Insert Empty Data - OLD CODE

                        /*if (!collectionAlerts.ContainsKey(displayValue))
                        {
                            collectionAlerts.Add(displayValue, new AlertView());
                        }
                        switch (pvAlert)
                        {
                            case "TermHiMaxRT":
                                collectionAlerts[displayValue].TermHiMaxRt = 0;
                                break;
                            case "TermHiAvgRT":
                                collectionAlerts[displayValue].TermHiAvgRt = 0;
                                break;
                            case "TermUnused":
                                collectionAlerts[displayValue].TermUnused = 0;
                                break;
                            case "TermErrorList":
                                collectionAlerts[displayValue].TermErrorList = 0;
                                break;
                            case "TCPQueuedTransactions":
                                collectionAlerts[displayValue].TcpQueuedTransactions = 0;
                                break;
                            case "TCPLowTermPool":
                                collectionAlerts[displayValue].TcpLowTermPool = 0;
                                break;
                            case "TCPLowServerPool":
                                collectionAlerts[displayValue].TcpLowServerPool = 0;
                                break;
                            case "TCPErrorList ":
                                collectionAlerts[displayValue].TcpErrorList = 0;
                                break;
                            case "TCPUnused":
                                collectionAlerts[displayValue].TcpUnused = 0;
                                break;
                            case "ServerHiMaxRT":
                                collectionAlerts[displayValue].ServerHiMaxRt = 0;
                                break;
                            case "ServerHiAvgRT":
                                collectionAlerts[displayValue].ServerHiAvgRt = 0;
                                break;
                            case "ServerQueueTCP":
                                collectionAlerts[displayValue].ServerQueueTcp = 0;
                                break;
                            case "ServerQueueLinkmon":
                                collectionAlerts[displayValue].ServerQueueLinkmon = 0;
                                break;
                            case "ServerUnusedClass":
                                collectionAlerts[displayValue].ServerUnusedClass = 0;
                                break;
                            case "ServerUnusedProcess":
                                collectionAlerts[displayValue].ServerUnusedProcess = 0;
                                break;
                            case "ServerPendingClass":
                                collectionAlerts[displayValue].ServerPendingClass = 0;
                                break;
                            case "ServerPendingProcess":
                                collectionAlerts[displayValue].ServerPendingProcess = 0;
                                break;
                            case "ServerErrorList":
                                collectionAlerts[displayValue].ServerErrorList = 0;
                                break;
                        }*/

                        #endregion
                    }
                    #region Insert Data
                    foreach (var view in alertView)
                    {
                        if (!collectionAlerts.ContainsKey(displayValue))
                        {                           
                             collectionAlerts.Add(displayValue, new AlertView());                                                      
                        }

                        switch (pvAlert)
                        {
                            case "TermHiMaxRT":
                                collectionAlerts[displayValue].TermHiMaxRt = view.Count;
                                break;
                            case "TermHiAvgRT":
                                collectionAlerts[displayValue].TermHiAvgRt = view.Count;
                                break;
                            case "TermUnused":
                                collectionAlerts[displayValue].TermUnused = view.Count;
                                break;
                            case "TermErrorList":
                                collectionAlerts[displayValue].TermErrorList = view.Count;
                                break;
                            case "TCPQueuedTransactions":
                                collectionAlerts[displayValue].TcpQueuedTransactions = view.Count;
                                break;
                            case "TCPLowTermPool":
                                collectionAlerts[displayValue].TcpLowTermPool = view.Count;
                                break;
                            case "TCPLowServerPool":
                                collectionAlerts[displayValue].TcpLowServerPool = view.Count;
                                break;
                            case "TCPErrorList ":
                                collectionAlerts[displayValue].TcpErrorList = view.Count;
                                break;
                            case "TCPUnused":
                                collectionAlerts[displayValue].TcpUnused = view.Count;
                                break;
                            case "ServerHiMaxRT":
                                collectionAlerts[displayValue].ServerHiMaxRt = view.Count;
                                break;
                            case "ServerHiAvgRT":
                                collectionAlerts[displayValue].ServerHiAvgRt = view.Count;
                                break;
                            case "ServerQueueTCP":
                                collectionAlerts[displayValue].ServerQueueTcp = view.Count;
                                break;
                            case "ServerQueueLinkmon":
                                collectionAlerts[displayValue].ServerQueueLinkmon = view.Count;
                                break;
                            case "ServerUnusedClass":
                                collectionAlerts[displayValue].ServerUnusedClass = view.Count;
                                break;
                            case "ServerUnusedProcess":
                                collectionAlerts[displayValue].ServerUnusedProcess = view.Count;
                                break;
                            case "ServerPendingClass":
                                collectionAlerts[displayValue].ServerPendingClass = view.Count;
                                break;
                            case "ServerPendingProcess":
                                collectionAlerts[displayValue].ServerPendingProcess = view.Count;
                                break;
                            case "ServerErrorList":
                                collectionAlerts[displayValue].ServerErrorList = view.Count;
                                break;
                        }
                    }
                    #endregion

                    //if there is no alert in this interval, then do not include this interval
                    if ((collectionAlerts[displayValue].ServerPendingClass+collectionAlerts[displayValue].ServerPendingProcess+
                        collectionAlerts[displayValue].ServerQueueLinkmon+collectionAlerts[displayValue].ServerErrorList)==0)
                    {
                        collectionAlerts.Remove(displayValue);
                    }
                }
            }

            return collectionAlerts;
        }

        public Dictionary<string, List<UnusedServerProcesses>> GetUnusedServerProcessesesFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, Enums.IntervalTypes intervalTypes, List<DateTime> datesWithData = null)
        {
            IPvScPrStusRepository scPrStus = new PvScPrStusRepository();

            var serverProcesses = new Dictionary<string, List<UnusedServerProcesses>>();

            for (var dtStart = fromTimestamp; dtStart.Date < toTimestamp.Date; dtStart = IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec))
            {
                if (datesWithData != null)
                    if (!datesWithData.Contains(dtStart.Date)) continue;

                var serverProcess = scPrStus.GetUnusedServerProcesseses(dtStart.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)),
                                                                        IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec).AddSeconds(_intervalInSec *
                                                                        Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);

                if (serverProcess.Count > 0)
                {
                    if (intervalTypes == Enums.IntervalTypes.Hourly)
                        serverProcesses.Add(dtStart.ToString("HH:mm"), serverProcess);
                    else
                        serverProcesses.Add(dtStart.ToString("yyyy-MM-dd"), serverProcess);
                }
            }

            return serverProcesses;
        }

        public Dictionary<string, List<ServerMaxLinks>> GetServerMaxLinksFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, Enums.IntervalTypes intervalTypes, List<DateTime> datesWithData = null)
        {
            IPvScPrStusRepository scPrStus = new PvScPrStusRepository();

            var serverMaxLinks = new Dictionary<string, List<ServerMaxLinks>>();

            for (var dtStart = fromTimestamp; dtStart.Date < toTimestamp.Date; dtStart = IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec))
            {
                if (datesWithData != null)
                    if (!datesWithData.Contains(dtStart.Date)) continue;

                var serverMaxLink = scPrStus.GetServerMaxLinks(dtStart.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)),
                                                               IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec).AddSeconds(_intervalInSec *
                                                               Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);

                if (serverMaxLink.Count > 0)
                {
                    if (intervalTypes == Enums.IntervalTypes.Hourly)
                        serverMaxLinks.Add(dtStart.ToString("HH:mm"), serverMaxLink);
                    else
                        serverMaxLinks.Add(dtStart.ToString("yyyy-MM-dd"), serverMaxLink);
                }
            }

            return serverMaxLinks;
        }

        public Dictionary<string, List<CheckDirectoryON>> GetCheckDirectoryONFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, Enums.IntervalTypes intervalTypes, List<DateTime> datesWithData = null)
        {
            IPvScPrStusRepository scPrStus = new PvScPrStusRepository();

            var checkDirectoryON = new Dictionary<string, List<CheckDirectoryON>>();

            for (var dtStart = fromTimestamp; dtStart.Date < toTimestamp.Date; dtStart = IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec))
            {
                if (datesWithData != null)
                    if (!datesWithData.Contains(dtStart.Date)) continue;

                var serverMaxLink = scPrStus.GetCheckDirectoryOnDetail(dtStart.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)),
                                                                       IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec).AddSeconds(_intervalInSec *
                                                                       Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);

                if (serverMaxLink.Count > 0)
                {
                    if (intervalTypes == Enums.IntervalTypes.Hourly)
                        checkDirectoryON.Add(dtStart.ToString("HH:mm"), serverMaxLink);
                    else
                        checkDirectoryON.Add(dtStart.ToString("yyyy-MM-dd"), serverMaxLink);
                }
            }

            return checkDirectoryON;
        }

        public Dictionary<string, List<HighDynamicServers>> GetHighDynamicServersFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, Enums.IntervalTypes intervalTypes, List<DateTime> datesWithData = null)
        {
            IPvScPrStusRepository scPrStus = new PvScPrStusRepository();
            var highDynamicServers = new Dictionary<string, List<HighDynamicServers>>();

            for (var dtStart = fromTimestamp; dtStart.Date < toTimestamp.Date; dtStart = IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec))
            {
                if (datesWithData != null)
                    if (!datesWithData.Contains(dtStart.Date)) continue;

                var highDynamicServer = scPrStus.GetHighDynamicServersDetail(dtStart.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)),
                                                                         IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec).AddSeconds(_intervalInSec *
                                                                         Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);

                if (highDynamicServer.Count > 0)
                {
                    if (intervalTypes == Enums.IntervalTypes.Hourly)
                        highDynamicServers.Add(dtStart.ToString("HH:mm"), highDynamicServer);
                    else
                        highDynamicServers.Add(dtStart.ToString("yyyy-MM-dd"), highDynamicServer);
                }
            }

            return highDynamicServers;
        }
        
        public Dictionary<string, List<ServerErrorListView>> GetServerErrorListIntervalFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, Enums.IntervalTypes intervalTypes, List<DateTime> datesWithData = null)
        {
            IPvScStusRepository sclStat = new PvScStusRepository();

            var errorLists = new Dictionary<string, List<ServerErrorListView>>();
            for (var dtStart = fromTimestamp; dtStart.Date < toTimestamp.Date; dtStart = IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec))
            {
                if (datesWithData != null)
                    if (!datesWithData.Contains(dtStart.Date)) continue;

                var errorList = sclStat.GetServerErrorLists(dtStart.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)),
                                                                    IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec).AddSeconds(_intervalInSec *
                                                                    Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);

                if (errorList.Count > 0)
                {
                    if (intervalTypes == Enums.IntervalTypes.Hourly) {
                        if (!errorLists.ContainsKey(dtStart.ToString("HH:mm"))) {
                            errorLists.Add(dtStart.ToString("HH:mm"), errorList);
                        }
                        else {
                            foreach (var list in errorList) {
                                if (errorLists[dtStart.ToString("HH:mm")].Any(x => x.ServerClass.Equals(list.ServerClass))) {
                                    var errors = errorLists[dtStart.ToString("HH:mm")].Where(x => x.ServerClass.Equals(list.ServerClass)).First();
                                    if (list.MostRecentTime > errors.MostRecentTime) {
                                        errors.Instances = list.Instances;
                                        errors.MostRecentTime = list.MostRecentTime;
                                    }
                                }
                                else {
                                    errorLists[dtStart.ToString("HH:mm")].AddRange(errorList);
                                }
                            }
                        }
                    }
                    else {
                        if (!errorLists.ContainsKey(dtStart.ToString("yyyy-MM-dd"))) {
                            errorLists.Add(dtStart.ToString("yyyy-MM-dd"), errorList);
                        }
                        else {
                            foreach (var list in errorList) {
                                if (errorLists[dtStart.ToString("yyyy-MM-dd")].Any(x => x.ServerClass.Equals(list.ServerClass))) {
                                    var errors = errorLists[dtStart.ToString("yyyy-MM-dd")].Where(x => x.ServerClass.Equals(list.ServerClass)).First();
                                    if (list.MostRecentTime > errors.MostRecentTime) {
                                        errors.Instances = list.Instances;
                                        errors.MostRecentTime = list.MostRecentTime;
                                    }
                                }
                                else {
                                    errorLists[dtStart.ToString("yyyy-MM-dd")].AddRange(errorList);
                                }
                            }
                        }
                    }
                }
            }

            return errorLists;
        }

        public Dictionary<string, List<ServerQueueTcpSubView>> GetQueueTCPSubFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, Enums.IntervalTypes intervalTypes, List<DateTime> datesWithData = null)
        {
            IPvScTStatRepository sctStat = new PvScTStatRepository();

            var queueTCPSubs = new Dictionary<string, List<ServerQueueTcpSubView>>();
            for (var dtStart = fromTimestamp; dtStart.Date < toTimestamp.Date; dtStart = IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec))
            {
                if (datesWithData != null)
                    if (!datesWithData.Contains(dtStart.Date)) continue;

                var queueTCPSub = sctStat.GetQueueTCPSub(dtStart.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)),
                                                                    IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec).AddSeconds(_intervalInSec *
                                                                    Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);

                if (queueTCPSub.Count > 0)
                {
                    if (intervalTypes == Enums.IntervalTypes.Hourly)
                        queueTCPSubs.Add(dtStart.ToString("HH:mm"), queueTCPSub);
                    else
                        queueTCPSubs.Add(dtStart.ToString("yyyy-MM-dd"), queueTCPSub);
                }
            }

            return queueTCPSubs;
        }

        public Dictionary<string, List<ServerQueueTcpSubView>> GetQueueLinkmonSubFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, Enums.IntervalTypes intervalTypes, List<DateTime> datesWithData = null)
        {
            IPvScLStatRepository sclStat = new PvScLStatRepository();

            var queueLinkmonSubs = new Dictionary<string, List<ServerQueueTcpSubView>>();
            for (var dtStart = fromTimestamp; dtStart.Date < toTimestamp.Date; dtStart = IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec))
            {
                if (datesWithData != null)
                    if (!datesWithData.Contains(dtStart.Date)) continue;

                var queueLinkmonSub = sclStat.GetQueueLinkmonSub(dtStart.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)),
                                                                    IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec).AddSeconds(_intervalInSec *
                                                                    Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);

                if (queueLinkmonSub.Count > 0)
                {
                    if (intervalTypes == Enums.IntervalTypes.Hourly)
                        queueLinkmonSubs.Add(dtStart.ToString("HH:mm"), queueLinkmonSub);
                    else
                        queueLinkmonSubs.Add(dtStart.ToString("yyyy-MM-dd"), queueLinkmonSub);
                }
            }

            return queueLinkmonSubs;
        }

        public Dictionary<string, List<ServerErrorView>> GetServerErrorListSubFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, Enums.IntervalTypes intervalTypes, List<DateTime> datesWithData = null)
        {
            IPvScStusRepository sclStat = new PvScStusRepository();

            var errorLists = new Dictionary<string, List<ServerErrorView>>();
            for (var dtStart = fromTimestamp; dtStart.Date < toTimestamp.Date; dtStart = IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec))
            {
                if (datesWithData != null)
                    if (!datesWithData.Contains(dtStart.Date)) continue;

                var errorList = sclStat.GetServerErrorListSub(dtStart.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)),
                                                                    IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec).AddSeconds(_intervalInSec *
                                                                    Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);

                if (errorList.Count > 0)
                {
                    if (intervalTypes == Enums.IntervalTypes.Hourly) {
                        if (!errorLists.ContainsKey(dtStart.ToString("HH:mm"))) {
                            errorLists.Add(dtStart.ToString("HH:mm"), errorList);
                        }
                        else {
                            foreach (var list in errorList) {
                                if (errorLists[dtStart.ToString("HH:mm")].Any(x => x.ServerClass.Equals(list.ServerClass))) {
                                    var errors = errorLists[dtStart.ToString("HH:mm")].Where(x => x.ServerClass.Equals(list.ServerClass)).First();
                                    if (list.MostRecentTime > errors.MostRecentTime) {
                                        errors.ErrorNumber = list.ErrorNumber;
                                        errors.MostRecentTime = list.MostRecentTime;
                                    }
                                }
                                else {
                                    errorLists[dtStart.ToString("HH:mm")].AddRange(errorList);
                                }
                            }
                        }
                    }
                    else {
                        if (!errorLists.ContainsKey(dtStart.ToString("yyyy-MM-dd"))) {
                            errorLists.Add(dtStart.ToString("yyyy-MM-dd"), errorList);
                        }
                        else {
                            foreach (var list in errorList) {
                                if (errorLists[dtStart.ToString("yyyy-MM-dd")].Any(x => x.ServerClass.Equals(list.ServerClass))) {
                                    var errors = errorLists[dtStart.ToString("yyyy-MM-dd")].Where(x => x.ServerClass.Equals(list.ServerClass)).First();
                                    if (list.MostRecentTime > errors.MostRecentTime) {
                                        errors.ErrorNumber = list.ErrorNumber;
                                        errors.MostRecentTime = list.MostRecentTime;
                                    }
                                }
                                else {
                                    errorLists[dtStart.ToString("yyyy-MM-dd")].AddRange(errorList);
                                }
                            }
                        }
                    }
                }
            }

            return errorLists;
        }
    }
}