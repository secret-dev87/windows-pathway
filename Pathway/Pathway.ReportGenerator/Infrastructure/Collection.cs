using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.ReportGenerator.Infrastructure {
    class Collection {
        public string Alert { get; set; }
        public string CPUSummary { get; set; }
        public string CPUDetail { get; set; }
        public string TransactionServer { get; set; }
        public string TransactionTCP { get; set; }
        public string TransactionLinkmon { get; set; }
        public string PeakCPUBusyHourly { get; set; }
        public string CPUBusyHourly { get; set; }

        public string TransCountHourly { get; set; }

        public string ServerCPUBusy { get; set; }

        public string ServerTransactions { get; set; }

        public string ServerProcessCount { get; set; }

        public string ServerPendingClass { get; set; }

        public string ServerPendingProcess { get; set; }

        public string ServerQueuedTCP { get; set; }

        public string ServerQueuedLinkmon { get; set; }

        public string ServerUnusedServerClasses { get; set; }

        public string ServerUnusedServerProcesses { get; set; }

        public string ServerMaxLinks { get; set; }

        public string CheckDirectoryOnLinks { get; set; }

        public string HighDynamicServers { get; set; }

        public string ServerErrorList { get; set; }

        public string TermTop20 { get; set; }

        public string TermUnused { get; set; }

        public string TCPTransactions { get; set; }

        public string TCPQueuedTransactions { get; set; }

        public string TCPUnusedTCPs { get; set; }
    }
}
