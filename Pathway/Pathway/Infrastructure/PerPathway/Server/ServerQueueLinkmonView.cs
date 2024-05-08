using System;

namespace Pathway.Core.Infrastructure.PerPathway.Server {
    public class ServerQueueLinkmonView {
        public string Server { get; set; }
        public long PeakQueueLength { get; set; }
        public string Linkmon { get; set; }
        public DateTime PeakTime { get; set; }
        public long Instances { get; set; }
    }
}