using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Pathway.Core.Abstract;
using Pathway.Core.Concrete;
using Pathway.Core.Infrastructure;
using Pathway.Core.Infrastructure.PerPathway;
using Pathway.Core.RemoteAnalyst.Concrete;

namespace Pathway.Core.Services {
    public class PerPathwayService : IPerPathwayService {
        private readonly string _connectionString = "";
        private readonly long _intervalInSec;

        public PerPathwayService(string connectionString, long intervalInSec) {
            _connectionString = connectionString;
            _intervalInSec = intervalInSec;
        }

        public Dictionary<string, CPUDetailView> GetCPUBusyDetailFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, int ipu, string systemSerial) {
            var cpuBusyDetail = new Dictionary<string, CPUDetailView>();

            IPvCPUMany cpuMany = new PvCPUMany(_connectionString);
            IPvPwyMany pwyMany = new PvPwyMany(_connectionString);
            IPvTcpStus tcpStus = new PvTcpStus(_connectionString);
            IPvScPrStus scPrStus = new PvScPrStus(_connectionString);


            var cpuElapse = cpuMany.GetCPUElapsePerCPU(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));

            //Get all the CPU Numbers
            var cpuCount = cpuMany.GetCPUCount(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));
            foreach (var cpu in cpuCount.Where(cpu => !cpuBusyDetail.ContainsKey(cpu))) {
                cpuBusyDetail.Add(cpu, new CPUDetailView());
            }

            var process = new Process(_connectionString);
            var processTableName = systemSerial + "_PROCESS_" + fromTimestamp.Year + "_" + fromTimestamp.Month + "_" + fromTimestamp.Day;
            var pathmonData = process.GetPathmonProcessBusyPerCPU(processTableName, fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);

            //var pathmonData = pwyMany.GetPathmonProcessBusyPerCPU(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);
            foreach (var view in pathmonData) {
                if (view.CPUNumber != "\0\0") {
                    if (!cpuBusyDetail.ContainsKey(view.CPUNumber)) {
                        cpuBusyDetail.Add(view.CPUNumber, new CPUDetailView());
                    }
                    cpuBusyDetail[view.CPUNumber].Pathmon = (100 * Convert.ToDouble(view.Value)) / (cpuElapse[view.CPUNumber] * 1000 * ipu);
                }
            }

            var tcpData = tcpStus.GetTcpBusyPerCPU(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);
            foreach (var view in tcpData) {
                if (view.CPUNumber != "\0\0") {
                    if (!cpuBusyDetail.ContainsKey(view.CPUNumber)) {
                        cpuBusyDetail.Add(view.CPUNumber, new CPUDetailView());
                    }
                    cpuBusyDetail[view.CPUNumber].Tcp = (100 * Convert.ToDouble(view.Value)) / (cpuElapse[view.CPUNumber] * 1000 * ipu);
                }
            }

            var serverData = scPrStus.GetServerBusyPerCPU(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);
            foreach (var view in serverData) {
                if (view.CPUNumber != "\0\0") {
                    if (!cpuBusyDetail.ContainsKey(view.CPUNumber)) {
                        cpuBusyDetail.Add(view.CPUNumber, new CPUDetailView());
                    }
                    cpuBusyDetail[view.CPUNumber].Servers = (100*Convert.ToDouble(view.Value))/(cpuElapse[view.CPUNumber] * 1000 * ipu);
                }
            }

            return cpuBusyDetail;
        }

        public CPUSummaryView GetCPUSummaryFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, string systemSerial) {
            IPvCPUMany cpuMany = new PvCPUMany(_connectionString);
            IPvPwyMany pwyMany = new PvPwyMany(_connectionString);
            IPvTcpStus tcpStus = new PvTcpStus(_connectionString);
            IPvScPrStus scPrStus = new PvScPrStus(_connectionString);

            var cpuElapse = cpuMany.GetCPUElapse(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));
            if (cpuElapse == 0)
                cpuElapse = 0.01;

            var process = new Process(_connectionString);
            var processTableName = systemSerial + "_PROCESS_" + fromTimestamp.Year + "_" + fromTimestamp.Month + "_" + fromTimestamp.Day;
            double pathmon = Convert.ToDouble(process.GetPathmonProcessBusy(processTableName, fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName));

            //double pathmon = Convert.ToDouble(pwyMany.GetPathmonProcessBusy(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName) / cpuElapse);
            double tcp = Convert.ToDouble(tcpStus.GetTcpBusy(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName) / cpuElapse);
            double servers = Convert.ToDouble(scPrStus.GetServerBusy(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName) / cpuElapse);

            var totalValue = pathmon + tcp + servers;
            if (totalValue == 0)
                totalValue = 0.01;

            var cpuSummary = new CPUSummaryView {
                Pathmon = Math.Round((pathmon * 100) / totalValue, 2),
                Tcp = Math.Round((tcp * 100) / totalValue, 2),
                Servers = Math.Round((servers * 100) / totalValue, 2)
            };
            return cpuSummary;
        }

        public Dictionary<string, CPUSummaryView> GetIntervalCPUSummaryFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, Enums.IntervalTypes intervalTypes, string systemSerial) {
            IPvCPUMany cpuMany = new PvCPUMany(_connectionString);
            IPvPwyMany pwyMany = new PvPwyMany(_connectionString);
            IPvTcpStus tcpStus = new PvTcpStus(_connectionString);
            IPvScPrStus scPrStus = new PvScPrStus(_connectionString);

            var summaryInterval = new Dictionary<string, CPUSummaryView>();
                var process = new Process(_connectionString);

            for (var dtStart = fromTimestamp; dtStart.Date < toTimestamp.Date; dtStart = IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec)) {
                var cpuElapse = cpuMany.GetCPUElapse(dtStart.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec).AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));
                if (cpuElapse == 0)
                    cpuElapse = 0.01;

                var processTableName = systemSerial + "_PROCESS_" + fromTimestamp.Year + "_" + fromTimestamp.Month + "_" + fromTimestamp.Day;
                double pathmon = Convert.ToDouble(process.GetPathmonProcessBusy(processTableName, dtStart.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec).AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName) / cpuElapse);

                //double pathmon = Convert.ToDouble(pwyMany.GetPathmonProcessBusy(dtStart.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec).AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName) / cpuElapse);
                double tcp = Convert.ToDouble(tcpStus.GetTcpBusy(dtStart.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec).AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName) / cpuElapse);
                double servers = Convert.ToDouble(scPrStus.GetServerBusy(dtStart.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec).AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName) / cpuElapse);

                var totalValue = pathmon + tcp + servers;
                if (totalValue == 0)
                    totalValue = 0.01;

                var cpuSummary = new CPUSummaryView {
                    Pathmon = Math.Round((pathmon * 100) / totalValue, 2),
                    Tcp = Math.Round((tcp * 100) / totalValue, 2),
                    Servers = Math.Round((servers * 100) / totalValue, 2)
                };

                if (intervalTypes == Enums.IntervalTypes.Hourly)
                    summaryInterval.Add(dtStart.ToString("HH:mm"), cpuSummary);
                else
                    summaryInterval.Add(dtStart.ToString("yyyy-MM-dd"), cpuSummary);
            }

            return summaryInterval;
        }

        public TransactionServerView GetTransactionServerFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName) {
            IPvScLStat scLStat = new PvScLStat(_connectionString);
            IPvScTStat scTStat = new PvScTStat(_connectionString);

            var transactionServerView = new TransactionServerView {
                LinkmonToServer = scLStat.GetLinkmonToServer(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName),
                TcptoServer = scTStat.GetTcpToServer(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName)
            };

            return transactionServerView;
        }

        public Dictionary<string, TransactionServerView> GetIntervalTransactionServerFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, Enums.IntervalTypes intervalTypes) {
            IPvScLStat scLStat = new PvScLStat(_connectionString);
            IPvScTStat scTStat = new PvScTStat(_connectionString);

            var serverTransactions = new Dictionary<string, TransactionServerView>();
            for (var dtStart = fromTimestamp; dtStart.Date < toTimestamp.Date; dtStart = IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec)) {
                var transactionServerView = new TransactionServerView {
                    LinkmonToServer = scLStat.GetLinkmonToServer(dtStart.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec).AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName),
                    TcptoServer = scTStat.GetTcpToServer(dtStart.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec).AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName)
                };

                if (intervalTypes == Enums.IntervalTypes.Hourly)
                    serverTransactions.Add(dtStart.ToString("HH:mm"), transactionServerView);
                else
                    serverTransactions.Add(dtStart.ToString("yyyy-MM-dd"), transactionServerView);
            }
            return serverTransactions;
        }

        public TransactionTcpView GetTransactionTCPFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName) {
            IPvTermStat termStat = new PvTermStat(_connectionString);
            IPvScTStat scTStat = new PvScTStat(_connectionString);

            var transactionTcpView = new TransactionTcpView {
                TermToTcp = termStat.GetTermToTcp(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName),
                TcpToServer = scTStat.GetTcpToServer(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName)
            };

            return transactionTcpView;
        }

        public Dictionary<string, TransactionTcpView> GetIntervalTransactionTCPFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, Enums.IntervalTypes intervalTypes) {
            IPvTermStat termStat = new PvTermStat(_connectionString);
            IPvScTStat scTStat = new PvScTStat(_connectionString);
            var tcpTransactions = new Dictionary<string, TransactionTcpView>();

            for (var dtStart = fromTimestamp; dtStart.Date < toTimestamp.Date; dtStart = IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec).AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]))) {
                var transactionTcpView = new TransactionTcpView {
                    TermToTcp = termStat.GetTermToTcp(dtStart.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec).AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName),
                    TcpToServer = scTStat.GetTcpToServer(dtStart.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec).AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName)
                };

                if (intervalTypes == Enums.IntervalTypes.Hourly)
                    tcpTransactions.Add(dtStart.ToString("HH:mm"), transactionTcpView);
                else
                    tcpTransactions.Add(dtStart.ToString("yyyy-MM-dd"), transactionTcpView);
            }
            return tcpTransactions;
        }
    }
}
