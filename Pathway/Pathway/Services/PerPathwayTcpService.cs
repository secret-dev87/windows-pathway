using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Pathway.Core.Abstract.Services;
using Pathway.Core.Abstract.Repositories;
using Pathway.Core.Infrastructure;
using Pathway.Core.Infrastructure.PerPathway.Server;
using Pathway.Core.Infrastructure.PerPathway.Tcp;
using Pathway.Core.Repositories;

namespace Pathway.Core.Services {
    public class PerPathwayTcpService : IPerPathwayTcpService {
        private readonly long _intervalInSec;
        public PerPathwayTcpService(long intervalInSec)
        {
            _intervalInSec = intervalInSec;
        }

        public List<TcpTransactionView> GetTcpTransactionFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName) {
            IPvTcpInfoRepository tcpinfo = new PvTcpInfoRepository();
            IPvCPUManyRepository cpuMany = new PvCPUManyRepository();

            double cpuElapse = cpuMany.GetCPUElapse(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)),
                toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));

            var termList = tcpinfo.GetTermTransaction(pathwayName,
                fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)),
                toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));

            var tcpTrans = new List<TcpTransactionView>();

            if (termList.Count > 0) {
                tcpTrans = termList.Select(view => new TcpTransactionView {
                    Tcp = view.Key,
                    TermTranscaction = view.Value
                }).ToList();

                var cpuBusyList = tcpinfo.GetTermCPUBusy(pathwayName,
                    fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)),
                    toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));
                foreach (var view in cpuBusyList) {
                    var match = tcpTrans.Where(x => x.Tcp == view.Key).ToList();
                    foreach (var mat in match) {
                        mat.CPUBusy = Convert.ToSingle(Math.Round(view.Value * 100 / cpuElapse, 3));
                    }
                }

                var serverTrans = tcpinfo.GetTermServerTransaction(pathwayName,
                    fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)),
                    toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));
                foreach (var view in serverTrans) {
                    var match = tcpTrans.Where(x => x.Tcp == view.Key).ToList();
                    foreach (var mat in match) {
                        mat.ServerTransaction = view.Value;
                    }
                }

                var termCount = tcpinfo.GetTermCount(pathwayName,
                    fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)),
                    toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));
                foreach (var view in termCount) {
                    var match = tcpTrans.Where(x => x.Tcp == view.Key).ToList();
                    foreach (var mat in match) {
                        mat.TermCount = view.Value;
                    }
                }

                var avgRt = tcpinfo.GetTermAverageResponseTime(pathwayName,
                    fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)),
                    toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));
                foreach (var view in avgRt) {
                    var match = tcpTrans.Where(x => x.Tcp == view.Key).ToList();
                    foreach (var mat in match) {
                        mat.AverageRt = Math.Round(view.Value / Convert.ToInt64(ConfigurationManager.AppSettings["ResponseTime"]), 2);
                    }
                }
            }
            return tcpTrans;
        }

        public Dictionary<string, List<TcpQueuedTransactionView>> GetTcpQueuedTransactionFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, Enums.IntervalTypes intervalTypes) {
            IPvTcpStusRepository tcpStus = new PvTcpStusRepository();
            IPvTcpInfoRepository tcpinfo = new PvTcpInfoRepository();

            var tcpQueuedView = new Dictionary<string, List<TcpQueuedTransactionView>>();

            for (var dtStart = fromTimestamp; dtStart.Date < toTimestamp.Date; dtStart = IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec)) {
                var queued = tcpStus.GetTcpQueuedTransactions(dtStart.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), 
                                                            IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec).AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), 
                                                            pathwayName);

                var termCount = tcpinfo.GetTermCount(pathwayName,
                                                    dtStart.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)),
                                                    IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec).AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));
                foreach (var view in termCount) {
                    var match = queued.Where(x => x.Tcp == view.Key).ToList();
                    foreach (var mat in match) {
                        mat.TermCount = view.Value;
                    }
                }

                if (queued.Count > 0) {
                    if (Enums.IntervalTypes.Hourly == intervalTypes)
                        tcpQueuedView.Add(dtStart.ToString("HH:mm"), queued);
                    else
                        tcpQueuedView.Add(dtStart.ToString("yyyy-MM-dd"), queued);
                }
            }
            return tcpQueuedView;
        }

        public Dictionary<string, List<ServerQueueTcpSubView>> GetQueueTCPSubFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, Enums.IntervalTypes intervalTypes) {
            IPvTcpStusRepository tcpStus = new PvTcpStusRepository();

            var queueTCPSubs = new Dictionary<string, List<ServerQueueTcpSubView>>();
            for (var dtStart = fromTimestamp; dtStart.Date < toTimestamp.Date; dtStart = IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec)) {
                var queueTCPSub = tcpStus.GetQueueTCPSub(dtStart.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)),
                                                                    IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec).AddSeconds(_intervalInSec *
                                                                    Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);

                if (queueTCPSub.Count > 0) {
                    if (Enums.IntervalTypes.Hourly == intervalTypes)
                        queueTCPSubs.Add(dtStart.ToString("HH:mm"), queueTCPSub);
                    else
                        queueTCPSubs.Add(dtStart.ToString("yyyy-MM-dd"), queueTCPSub);
                }
            }

            return queueTCPSubs;
        }

        public Dictionary<string, List<ServerUnusedServerClassView>> GetTcpUnusedFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, Enums.IntervalTypes intervalTypes) {
            IPvTermStatRepository termStat = new PvTermStatRepository();
            IPvTcpInfoRepository tcpinfo = new PvTcpInfoRepository();

            var tcpUnusedView = new Dictionary<string, List<ServerUnusedServerClassView>>();

            for (var dtStart = fromTimestamp; dtStart.Date < toTimestamp.Date; dtStart = IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec)) {
                var queued = termStat.GetTcpUnused(dtStart.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)),
                                                            IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec).AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])),
                                                            pathwayName);

                var termCount = tcpinfo.GetTermCount(pathwayName,
                                                    dtStart.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)),
                                                    IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec).AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));
                foreach (var view in termCount) {
                    var match = queued.Where(x => x.ServerClass == view.Key).ToList();
                    foreach (var mat in match) {
                        mat.TermCount = view.Value;
                    }
                }

                if (queued.Count > 0) {
                    if (Enums.IntervalTypes.Hourly == intervalTypes)
                        tcpUnusedView.Add(dtStart.ToString("HH:mm"), queued);
                    else
                        tcpUnusedView.Add(dtStart.ToString("yyyy-MM-dd"), queued);
                }
            }
            return tcpUnusedView;
        }
    }
}
