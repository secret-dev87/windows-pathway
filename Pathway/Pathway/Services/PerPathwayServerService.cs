using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Pathway.Core.Abstract;
using Pathway.Core.Concrete;
using Pathway.Core.Infrastructure;
using Pathway.Core.Infrastructure.PerPathway.Server;

namespace Pathway.Core.Services {
    public class PerPathwayServerService : IPerPathwayServerService {
        private readonly string _connectionString = "";
        private readonly long _intervalInSec;

        public PerPathwayServerService(string connectionString, long intervalInSec) {
            _connectionString = connectionString;
            _intervalInSec = intervalInSec;
        }
        public List<ServerCPUBusyView> GetServerCPUBusyFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, int ipu) {
            IPvCPUMany cpuMany = new PvCPUMany(_connectionString);
            IPvScInfo scInfo = new PvScInfo(_connectionString);
            IPvScTStat scTStat = new PvScTStat(_connectionString);
            IPvScLStat scLStat = new PvScLStat(_connectionString);
            IPvScPrStus scPrStus = new PvScPrStus(_connectionString);

            double cpuElapse = cpuMany.GetCPUElapse(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));
            if (cpuElapse == 0)
                cpuElapse = 0.01;

            var scNames = scInfo.GetServerCPUBusy(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName, 20);
            var serverCPUBusy = scNames.Select(view => new ServerCPUBusyView {
                ServerClass = view.CPUNumber,
                CPUBusy = Convert.ToSingle(Math.Round(Convert.ToDouble(view.Value) * 100 / (cpuElapse * 1000 * ipu), 3))
            }).ToList();

            var scStatic = scInfo.GetNumStaticMaxServers(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)),
                                                    toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);
            foreach (var tcpStatic in scStatic) {
                var match = serverCPUBusy.Where(x => x.ServerClass == tcpStatic.CPUNumber).ToList();
                foreach (var mat in match) {
                    mat.NumStatic = tcpStatic.Value;
                    mat.MaxServers = tcpStatic.Value2;
                }
            }

            var tcpTrans = scTStat.GetServerCPUBusyTcpTrans(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);
            foreach (var tcpTran in tcpTrans) {
                var match = serverCPUBusy.Where(x => x.ServerClass == tcpTran.CPUNumber).ToList();
                foreach (var mat in match) {
                    mat.TcpTransaction = tcpTran.Value;
                }
            }

            var linkmonTrans = scLStat.GetServerCPUBusyLinkmonTrans(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);
            foreach (var linkmon in linkmonTrans) {
                var match = serverCPUBusy.Where(x => x.ServerClass == linkmon.CPUNumber).ToList();
                foreach (var mat in match) {
                    mat.LinkmonTransaction = linkmon.Value;
                    mat.TotalTransaction = mat.TcpTransaction + mat.LinkmonTransaction;
                }
            }

            var processCount = scPrStus.GetServerCPUBusyProcessCount(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);
            foreach (var process in processCount) {
                var match = serverCPUBusy.Where(x => x.ServerClass == process.CPUNumber).ToList();
                foreach (var mat in match) {
                    mat.ProcessCount = process.Value;
                    mat.ProcessMaxCount = process.Value2;
                }
            }

            var avgRt = scTStat.GetServerCPUBusyAvgRt(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);
            foreach (var avg in avgRt) {
                var match = serverCPUBusy.Where(x => x.ServerClass == avg.CPUNumber).ToList();
                foreach (var mat in match) {
                    mat.AverageRt = Math.Round(avg.Value / Convert.ToDouble(ConfigurationManager.AppSettings["ResponseTime"]), 3);
                }
            }

            return serverCPUBusy;
        }

        public List<ServerCPUBusyView> GetServerProcessCountFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName) {
            IPvCPUMany cpuMany = new PvCPUMany(_connectionString);
            IPvScInfo scInfo = new PvScInfo(_connectionString);
            IPvScTStat scTStat = new PvScTStat(_connectionString);
            IPvScLStat scLStat = new PvScLStat(_connectionString);
            //var scPrStus = new PvScPrStus(_connectionString);

            double cpuElapse = cpuMany.GetCPUElapse(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));
            if (cpuElapse == 0)
                cpuElapse = 0.01;

            var scNames = scInfo.GetServerProcessCount(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName, 20);
            var serverCPUBusy = scNames.Select(view => new ServerCPUBusyView {
                ServerClass = view.CPUNumber,
                //CPUBusy = Convert.ToSingle(Math.Round(Convert.ToDouble(view.Value)*100/cpuElapse, 3))
                ProcessCount = view.Value
            }).ToList();

            var tcpTrans = scTStat.GetServerCPUBusyTcpTrans(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);
            foreach (var tcpTran in tcpTrans) {
                var match = serverCPUBusy.Where(x => x.ServerClass == tcpTran.CPUNumber).ToList();
                foreach (var mat in match) {
                    mat.TcpTransaction = tcpTran.Value;
                }
            }

            var linkmonTrans = scLStat.GetServerCPUBusyLinkmonTrans(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);
            foreach (var linkmon in linkmonTrans) {
                var match = serverCPUBusy.Where(x => x.ServerClass == linkmon.CPUNumber).ToList();
                foreach (var mat in match) {
                    mat.LinkmonTransaction = linkmon.Value;
                }
            }

            var cpuBusy = scInfo.GetServerCPUBusy(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName, 0);
            foreach (var busy in cpuBusy) {
                var match = serverCPUBusy.Where(x => x.ServerClass == busy.CPUNumber).ToList();
                foreach (var mat in match) {
                    mat.CPUBusy = Convert.ToSingle(Math.Round(Convert.ToDouble(busy.Value) * 100 / cpuElapse, 3));
                }
            }

            var avgRt = scTStat.GetServerCPUBusyAvgRt(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);
            foreach (var avg in avgRt) {
                var match = serverCPUBusy.Where(x => x.ServerClass == avg.CPUNumber).ToList();
                foreach (var mat in match) {
                    mat.AverageRt = Math.Round(avg.Value / Convert.ToDouble(ConfigurationManager.AppSettings["ResponseTime"]), 3);
                }
            }

            return serverCPUBusy;
        }

        public List<ServerQueueTcpView> GetServerQueueTCPFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName) {
            IPvScTStat sctStat = new PvScTStat(_connectionString);

            var queueTcp = sctStat.GetServerQueueTCP(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)),
                                                    toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])),
                                                    pathwayName);

            return queueTcp;
        }

        public Dictionary<string, List<string>> GetServerPendingClassIntervalFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, Enums.IntervalTypes intervalTypes, List<DateTime> datesWithData = null) {
            IPvScStus sctStat = new PvScStus(_connectionString);

            var pendingClasses = new Dictionary<string, List<string>>();

            for (var dtStart = fromTimestamp; dtStart.Date < toTimestamp.Date; dtStart = IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec)) {
                if (datesWithData != null)
                    if (!datesWithData.Contains(dtStart.Date)) continue;

                var pendingClass = sctStat.GetServerPendingClass(dtStart.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)),
                                                                IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec).AddSeconds(_intervalInSec *
                                                                Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);

                if (pendingClass.Count > 0) {
                    if (Enums.IntervalTypes.Hourly == intervalTypes)
                        pendingClasses.Add(dtStart.ToString("HH:mm"), pendingClass);
                    else
                        pendingClasses.Add(dtStart.ToString("yyyy-MM-dd"), pendingClass);
                }
            }

            return pendingClasses;
        }

        public Dictionary<string, List<ServerUnusedServerProcessesView>> GetServerPendingProcessIntervalFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, Enums.IntervalTypes intervalTypes, List<DateTime> datesWithData = null) {
            IPvScPrStus sctStat = new PvScPrStus(_connectionString);

            var pendingProcesses = new Dictionary<string, List<ServerUnusedServerProcessesView>>();

            for (var dtStart = fromTimestamp; dtStart.Date < toTimestamp.Date; dtStart = IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec)) {
                if (datesWithData != null)
                    if (!datesWithData.Contains(dtStart.Date)) continue;

                var pendingProcess = sctStat.GetServerPendingProcess(dtStart.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)),
                                                                IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec).AddSeconds(_intervalInSec *
                                                                Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);

                if (pendingProcess.Count > 0) {
                    if (Enums.IntervalTypes.Hourly == intervalTypes)
                        pendingProcesses.Add(dtStart.ToString("HH:mm"), pendingProcess);
                    else
                        pendingProcesses.Add(dtStart.ToString("yyyy-MM-dd"), pendingProcess);
                }
            }

            return pendingProcesses;
        }

        public Dictionary<string, List<ServerQueueTcpView>> GetServerQueueTCPIntervalFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, Enums.IntervalTypes intervalTypes, List<DateTime> datesWithData = null) {
            IPvScTStat sctStat = new PvScTStat(_connectionString);

            var tcpQueues = new Dictionary<string, List<ServerQueueTcpView>>();

            for (var dtStart = fromTimestamp; dtStart.Date < toTimestamp.Date; dtStart = IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec)) {
                if (datesWithData != null)
                    if (!datesWithData.Contains(dtStart.Date)) continue;

                var queueTcp = sctStat.GetServerQueueTCP(dtStart.AddSeconds(_intervalInSec*(Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])*-1)),
                    IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec).AddSeconds(_intervalInSec*
                                                                                                 Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);

                if (queueTcp.Count > 0) {
                    if (Enums.IntervalTypes.Hourly == intervalTypes)
                        tcpQueues.Add(dtStart.ToString("HH:mm"), queueTcp);
                    else
                        tcpQueues.Add(dtStart.ToString("yyyy-MM-dd"), queueTcp);
                }
            }

            return tcpQueues;
        }

        public List<ServerQueueLinkmonView> GetServerQueueLinkmonFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName) {
            IPvScLStat sclStat = new PvScLStat(_connectionString);

            var queueLinkmon = sclStat.GetServerQueueLinkmon(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), 
                                                            toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), 
                                                            pathwayName);

            return queueLinkmon;
        }

        public Dictionary<string, List<ServerQueueLinkmonView>> GetServerQueueLinkmonIntervalFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, Enums.IntervalTypes intervalTypes, List<DateTime> datesWithData = null) {
            IPvScLStat sclStat = new PvScLStat(_connectionString);

            var linkmonQueues = new Dictionary<string, List<ServerQueueLinkmonView>>();
            for (var dtStart = fromTimestamp; dtStart.Date < toTimestamp.Date; dtStart = IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec)) {
                if (datesWithData != null)
                    if (!datesWithData.Contains(dtStart.Date)) continue;

                var queueLinkmon = sclStat.GetServerQueueLinkmon(dtStart.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)),
                                                                    IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec).AddSeconds(_intervalInSec *
                                                                    Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);

                if (queueLinkmon.Count > 0) {
                    if (Enums.IntervalTypes.Hourly == intervalTypes) {
                        if (!linkmonQueues.ContainsKey(dtStart.ToString("HH:mm"))) {
                            linkmonQueues.Add(dtStart.ToString("HH:mm"), queueLinkmon);
                        }
                        else {
                            //linkmonQueues[dtStart.ToString("HH:mm")].AddRange(queueLinkmon);
                            foreach (var linkmon in queueLinkmon) {
                                if (linkmonQueues[dtStart.ToString("HH:mm")].Any(x => x.Server.Equals(linkmon.Server)) &&
                                    linkmonQueues[dtStart.ToString("HH:mm")].Any(x => x.Linkmon.Equals(linkmon.Linkmon))) {
                                        var currentValue = linkmonQueues[dtStart.ToString("HH:mm")].
                                            Where(x => x.Server.Equals(linkmon.Server)).First(x => x.Linkmon.Equals(linkmon.Linkmon));

                                    if (linkmon.PeakQueueLength > currentValue.PeakQueueLength) {
                                        currentValue.Instances = linkmon.Instances;
                                        currentValue.PeakQueueLength = linkmon.PeakQueueLength;
                                        currentValue.PeakTime = linkmon.PeakTime;
                                    }
                                }
                                else {
                                    linkmonQueues[dtStart.ToString("HH:mm")].AddRange(queueLinkmon);
                                }
                            }
                        }
                    }
                    else {
                        if (!linkmonQueues.ContainsKey(dtStart.ToString("yyyy-MM-dd"))) {
                            linkmonQueues.Add(dtStart.ToString("yyyy-MM-dd"), queueLinkmon);
                        }
                        else {
                            foreach (var linkmon in queueLinkmon) {
                                if (linkmonQueues[dtStart.ToString("yyyy-MM-dd")].Any(x => x.Server.Equals(linkmon.Server)) &&
                                    linkmonQueues[dtStart.ToString("yyyy-MM-dd")].Any(x => x.Linkmon.Equals(linkmon.Linkmon))) {
                                        var currentValue = linkmonQueues[dtStart.ToString("yyyy-MM-dd")].
                                        Where(x => x.Server.Equals(linkmon.Server)).First(x => x.Linkmon.Equals(linkmon.Linkmon));

                                    if (linkmon.PeakQueueLength > currentValue.PeakQueueLength) {
                                        currentValue.Instances = linkmon.Instances;
                                        currentValue.PeakQueueLength = linkmon.PeakQueueLength;
                                        currentValue.PeakTime = linkmon.PeakTime;
                                    }
                                }
                                else {
                                    linkmonQueues[dtStart.ToString("yyyy-MM-dd")].AddRange(queueLinkmon);
                                }
                            }
                        }
                    }
                }
            }

            return linkmonQueues;
        }

        public Dictionary<string, List<ServerCPUBusyView>> GetServerTransactionsIntervalFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, Enums.IntervalTypes intervalTypes) {
            IPvCPUMany cpuMany = new PvCPUMany(_connectionString);
            IPvScInfo scInfo = new PvScInfo(_connectionString);

            double cpuElapse = cpuMany.GetCPUElapse(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)),
                                                    toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));
            if (cpuElapse == 0)
                cpuElapse = 0.01;

            var views = new Dictionary<string, List<ServerCPUBusyView>>();
            for (var dtStart = fromTimestamp; dtStart.Date < toTimestamp.Date; dtStart = IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec)) {
                var transactionViews = scInfo.GetServerTransactions(dtStart.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)),
                                                                    IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec).AddSeconds(_intervalInSec *
                                                                    Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])),
                                                                    pathwayName);

                var cpuBusy = scInfo.GetServerCPUBusy(dtStart.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)),
                                                      IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec).AddSeconds(_intervalInSec *
                                                      Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])),
                                                      pathwayName);
                foreach (var view in cpuBusy) {
                    var match = transactionViews.Where(x => x.ServerClass == view.CPUNumber).ToList();
                    foreach (var mat in match) {
                        mat.CPUBusy = Convert.ToSingle(Math.Round(Convert.ToDouble(view.Value) * 100 / cpuElapse, 3));
                    }
                }

                var numStatic = scInfo.GetNumStaticMaxServers(dtStart.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)),
                                                      IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec).AddSeconds(_intervalInSec *
                                                      Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])),
                                                      pathwayName);
                foreach (var view in numStatic) {
                    var match = transactionViews.Where(x => x.ServerClass == view.CPUNumber).ToList();
                    foreach (var mat in match) {
                        mat.NumStatic = Convert.ToInt64(view.Value);
                        mat.MaxServers = Convert.ToInt64(view.Value2);
                    }
                }

                var processCount = scInfo.GetServerProcessCount(dtStart.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)),
                                                                IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec).AddSeconds(_intervalInSec *
                                                                Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])),
                                                                pathwayName);
                foreach (var view in processCount) {
                    var match = transactionViews.Where(x => x.ServerClass == view.CPUNumber).ToList();
                    foreach (var mat in match) {
                        mat.ProcessCount = Convert.ToInt64(view.Value);
                        mat.ProcessMaxCount = Convert.ToInt64(view.Value2);
                    }
                }

                var avgResTime = scInfo.GetServerAverageResponseTime(dtStart.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)),
                                                                    IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec).AddSeconds(_intervalInSec *
                                                                    Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])),
                                                                    pathwayName);
                foreach (var view in avgResTime) {
                    var match = transactionViews.Where(x => x.ServerClass == view.CPUNumber).ToList();
                    foreach (var mat in match) {
                        mat.AverageRt = Convert.ToSingle(Math.Round(view.DoubleValue, 3));
                    }
                }

                if (Enums.IntervalTypes.Hourly == intervalTypes)
                    views.Add(dtStart.ToString("HH:mm"), transactionViews);
                else
                    views.Add(dtStart.ToString("yyyy-MM-dd"), transactionViews);
            }

            return views;
        }

        public List<ServerCPUBusyView> GetServerTransactionsFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, int ipu) {
            IPvCPUMany cpuMany = new PvCPUMany(_connectionString);
            IPvScInfo scInfo = new PvScInfo(_connectionString);
            var scTStat = new PvScTStat(_connectionString);
            var scLStat = new PvScLStat(_connectionString);
            var scPrStus = new PvScPrStus(_connectionString);

            double cpuElapse = cpuMany.GetCPUElapse(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));
            if (cpuElapse == 0)
                cpuElapse = 0.01;

            var transactionViews = scInfo.GetServerTransactions(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);

            var cpuBusy = scInfo.GetServerCPUBusy(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);
            foreach (var view in cpuBusy) {
                var match = transactionViews.Where(x => x.ServerClass == view.CPUNumber).ToList();
                foreach (var mat in match) {
                    mat.CPUBusy = Convert.ToSingle(Math.Round(Convert.ToDouble(view.Value) * 100 / (cpuElapse * 1000 * ipu), 3));
                }
            }

            var processCount = scInfo.GetServerProcessCount(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);
            foreach (var view in processCount) {
                var match = transactionViews.Where(x => x.ServerClass == view.CPUNumber).ToList();
                foreach (var mat in match) {
                    mat.ProcessCount = Convert.ToInt64(view.Value);
                }
            }

            var avgResTime = scInfo.GetServerAverageResponseTime(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);
            foreach (var view in avgResTime) {
                var match = transactionViews.Where(x => x.ServerClass == view.CPUNumber).ToList();
                foreach (var mat in match) {
                    mat.AverageRt = Convert.ToSingle(Math.Round(view.DoubleValue, 3));
                }
            }

            return transactionViews;
        }

        public List<ServerCPUBusyView> GetServerTransactionsPerIntervalFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName) {
            IPvCPUMany cpuMany = new PvCPUMany(_connectionString);
            IPvScInfo scInfo = new PvScInfo(_connectionString);

            double cpuElapse = cpuMany.GetCPUElapse(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));
            if (cpuElapse == 0)
                cpuElapse = 0.01;

            var transactionViews = scInfo.GetServerTransactionsPerInterval(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);

            var cpuBusy = scInfo.GetServerCPUBusy(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);
            foreach (var view in cpuBusy) {
                var match = transactionViews.Where(x => x.ServerClass == view.CPUNumber).ToList();
                foreach (var mat in match) {
                    mat.CPUBusy = Convert.ToSingle(Math.Round(Convert.ToDouble(view.Value) * 100 / cpuElapse, 3));
                }
            }

            var processCount = scInfo.GetServerProcessCount(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);
            foreach (var view in processCount) {
                var match = transactionViews.Where(x => x.ServerClass == view.CPUNumber).ToList();
                foreach (var mat in match) {
                    mat.ProcessCount = Convert.ToInt64(view.Value);
                }
            }

            var avgResTime = scInfo.GetServerAverageResponseTime(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);
            foreach (var view in avgResTime) {
                var match = transactionViews.Where(x => x.ServerClass == view.CPUNumber).ToList();
                foreach (var mat in match) {
                    mat.AverageRt = Convert.ToSingle(Math.Round(view.DoubleValue, 3));
                }
            }

            return transactionViews;
        }

        public List<ServerUnusedServerClassView> GetServerUnusedClassesFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName) {
            IPvScLStat scLStat = new PvScLStat(_connectionString);

            var serverClassCollection = scLStat.GetServerUnusedServerClass(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), 
                                                                toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])),
                                                                pathwayName);
            return serverClassCollection;
        }

        public Dictionary<string, List<ServerUnusedServerClassView>> GetServerUnusedClassesIntervalFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, Enums.IntervalTypes intervalTypes, List<DateTime> datesWithData = null) {
            IPvScLStat scLStat = new PvScLStat(_connectionString);

            var serverClasses = new Dictionary<string, List<ServerUnusedServerClassView>>();

            var serverClassCollection = GetServerUnusedClassesFor(fromTimestamp, toTimestamp, pathwayName);
            if (serverClassCollection.Count > 0) 
                serverClasses.Add("Collection", serverClassCollection);

            for (var dtStart = fromTimestamp; dtStart.Date < toTimestamp.Date; dtStart = IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec)) {
                if (datesWithData != null)
                    if (!datesWithData.Contains(dtStart.Date)) continue;

                var serverClass = scLStat.GetServerUnusedServerClass(dtStart.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)),
                                                                    IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec).AddSeconds(_intervalInSec *
                                                                    Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);

                if (serverClass.Count <= 0) continue;
                if (Enums.IntervalTypes.Hourly == intervalTypes) {
                    if (!serverClasses.ContainsKey(dtStart.ToString("HH:mm")))
                        serverClasses.Add(dtStart.ToString("HH:mm"), serverClass);
                    else {
                        foreach (var serverClassView in serverClass) {
                            if (serverClasses[dtStart.ToString("HH:mm")].Any(x => x.ServerClass.Equals(serverClassView.ServerClass))) {
                                var updateServerClass = serverClasses[dtStart.ToString("HH:mm")].Where(x => x.ServerClass.Equals(serverClassView.ServerClass)).First();
                                updateServerClass.TermCount += serverClassView.TermCount;
                                updateServerClass.Unused += serverClassView.Unused;
                            }
                            else {
                                serverClasses[dtStart.ToString("HH:mm")].Add(serverClassView);
                            }
                        }
                    }
                }
                else {
                    if (!serverClasses.ContainsKey(dtStart.ToString("yyyy-MM-dd")))
                        serverClasses.Add(dtStart.ToString("yyyy-MM-dd"), serverClass);
                    else {
                        foreach (var serverClassView in serverClass) {
                            if (serverClasses[dtStart.ToString("yyyy-MM-dd")].Any(x => x.ServerClass.Equals(serverClassView.ServerClass))) {
                                var updateServerClass = serverClasses[dtStart.ToString("yyyy-MM-dd")].Where(x => x.ServerClass.Equals(serverClassView.ServerClass)).First();
                                updateServerClass.TermCount += serverClassView.TermCount;
                                updateServerClass.Unused += serverClassView.Unused;
                            }
                            else {
                                serverClasses[dtStart.ToString("yyyy-MM-dd")].Add(serverClassView);
                            }
                        }
                    }
                }
            }
            return serverClasses;
        }
        
        public List<ServerUnusedServerProcessesView> GetServerUnusedProcessesFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName) {
            IPvScPrStus scPrStus = new PvScPrStus(_connectionString);
            var serverProcess = scPrStus.GetServerUnusedServerProcessPerPathway(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)),
                                                                toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])),
                                                                pathwayName);

            return serverProcess;
        }

        public Dictionary<string, List<ServerUnusedServerProcessesView>> GetServerUnusedProcessesIntervalFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, Enums.IntervalTypes intervalTypes) {
            IPvScPrStus scPrStus = new PvScPrStus(_connectionString);

            var serverProcesses = new Dictionary<string, List<ServerUnusedServerProcessesView>>();

            var serverProcessCollection = GetServerUnusedProcessesFor(fromTimestamp, toTimestamp, pathwayName);
            if (serverProcessCollection.Count > 0)
                serverProcesses.Add("Collection", serverProcessCollection);

            for (var dtStart = fromTimestamp; dtStart.Date < toTimestamp.Date; dtStart = IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec)) {
                var serverProcess = scPrStus.GetServerUnusedServerProcessPerPathway(dtStart.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec).AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);

                if (serverProcess.Count > 0) {
                    if (Enums.IntervalTypes.Hourly == intervalTypes)
                        serverProcesses.Add(dtStart.ToString("HH:mm"), serverProcess);
                    else
                        serverProcesses.Add(dtStart.ToString("yyyy-MM-dd"), serverProcess);
                }
            }

            return serverProcesses;
        }
    }
}
