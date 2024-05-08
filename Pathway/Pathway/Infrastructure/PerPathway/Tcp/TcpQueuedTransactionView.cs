using System;

namespace Pathway.Core.Infrastructure.PerPathway.Tcp {
    public class TcpQueuedTransactionView {
        public string Tcp { get; set; }
        public long PeakQueueLength { get; set; }
        public DateTime PeakTime { get; set; }
        public long Instances { get; set; }

        public long TermCount { get; set; }
    }
}