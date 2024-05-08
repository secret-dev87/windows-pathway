using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pathway.Core.Abstract;
using Pathway.Core.Infrastructure;
using Pathway.Core.Infrastructure.PerPathway;
using Pathway.Core.Infrastructure.PerPathway.Server;
using Pathway.Core.Infrastructure.PerPathway.Tcp;
using Pathway.Core.Infrastructure.PerPathway.Term;
using Pathway.Core.Services;

namespace Pathway.ReportGenerator.Infrastructure {
    class PerPathway {
        private readonly string _connectionStringSystem;
        public PerPathway(string connectionStringSystem) {
            _connectionStringSystem = connectionStringSystem;
        }

        #region Generic Data
        /*internal Dictionary<string, AlertView> GetAlerts(string pathwayName, DateTime fromTimestamp, DateTime toTimestamp) {
            //Get All the Alerts.
            var alertList = new List<string> {
                "TermHiMaxRT",
                "TermHiAvgRT",
                "TermUnused",
                "TermErrorList",
                "TCPQueuedTransactions",
                "TCPLowTermPool",
                "TCPLowServerPool",
                "TCPUnused",
                "TCPErrorList",
                "ServerHiMaxRT",
                "ServerHiAvgRT",
                "ServerQueueTCP",
                "ServerQueueLinkmon",
                "ServerUnusedClass",
                "ServerUnusedProcess",
                "ServerErrorList"
            };

            //For each Pathway Name, Loop throuth the From and To Timestamp using Interval and insert empty data.
            IPvAlertService alert = new PvAlertService(_connectionStringSystem);
            var alerts = alert.GetCollectionAlertFor(alertList, fromTimestamp, toTimestamp, pathwayName);
            return alerts;
        }*/

        internal List<CPUSummaryView> GetCPUBusySummary() {
            var summary = new List<CPUSummaryView>();

            return summary;
        }

        internal List<CPUDetailView> GetCPUBusyDetail() {
            var detail = new List<CPUDetailView>();

            return detail;
        }

        internal List<TransactionServerView> GetTransactionsServer() {
            var transactionServer = new List<TransactionServerView>();

            return transactionServer;
        }

        internal List<TransactionTcpView> GetTransactionsTcp() {
            var transactionTcp = new List<TransactionTcpView>();

            return transactionTcp;
        }

        #endregion

        #region Server
        internal List<ServerCPUBusyView> GetServerCPUBusy() {
            var serverCPUBusy = new List<ServerCPUBusyView>();

            return serverCPUBusy;
        }

        internal List<ServerTransactionView> GetServerTransactions() {
            var serverTransaction = new List<ServerTransactionView>();

            return serverTransaction;
        }

        internal List<ServerProcessCountView> GetServerProcessCount() {
            var serverProcessCount = new List<ServerProcessCountView>();

            return serverProcessCount;
        }

        internal List<ServerQueueTcpView> GetServerQueueTcp() {
            var serverQueueTcp = new List<ServerQueueTcpView>();

            return serverQueueTcp;
        }

        internal List<ServerQueueLinkmonView> GetServerQueueLinkmon() {
            var serverQeueuLinkmon = new List<ServerQueueLinkmonView>();

            return serverQeueuLinkmon;
        }

        internal List<ServerUnusedServerClassView> GetServerUnusedServerClasses() {
            var serverUnusedServerClass = new List<ServerUnusedServerClassView>();

            return serverUnusedServerClass;
        }

        internal List<ServerUnusedServerProcessesView> GetServerUnusedServerProcesses() {
            var serverUnusedServerProcess = new List<ServerUnusedServerProcessesView>();

            return serverUnusedServerProcess;
        }
        #endregion

        #region Term
        internal List<TermTop20View> GetTermTop20Transaction() {
            var termTransaction = new List<TermTop20View>();

            return termTransaction;
        }
        internal List<TermTop20View> GetTermTop20AvgResponseTime() {
            var termAvgResponseTime = new List<TermTop20View>();

            return termAvgResponseTime;
        }
        internal List<TermTop20View> GetTermTop20MaxResponseTime() {
            var termMaxResponseTime = new List<TermTop20View>();

            return termMaxResponseTime;
        }
        internal void GetTermUnused() {

        }
        #endregion

        #region Tcp
        internal List<TcpTransactionView> GetTcpTransactions() {
            var tcpTransaction = new List<TcpTransactionView>();

            return tcpTransaction;
        }
        internal List<TcpQueuedTransactionView> GetTcpQueueTransactions() {
            var tcpQueuedTransaction = new List<TcpQueuedTransactionView>();

            return tcpQueuedTransaction;
        }
        internal List<TcpUnusedView> GetTcpUnusedTcp() {
            var tcpUnused = new List<TcpUnusedView>();

            return tcpUnused;
        }
        #endregion

    }
}
