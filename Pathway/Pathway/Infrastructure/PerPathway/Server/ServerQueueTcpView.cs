using System;

namespace Pathway.Core.Infrastructure.PerPathway.Server {
    public class ServerQueueTcpView {
        public string Server { get; set; }
        public long PeakQueueLength { get; set; }
        public string Tcp { get; set; }
        public DateTime PeakTime { get; set; }
        public long Instances { get; set; }
    }
}