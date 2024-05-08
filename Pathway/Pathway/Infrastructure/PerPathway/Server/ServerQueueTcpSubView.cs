using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pathway.Core.Infrastructure.PerPathway.Server {
    public class ServerQueueTcpSubView {
        public string ServerClass { get; set; }
        public string TcpName { get; set; }
        public long RequestCount { get; set; }
        public double PercentWait { get; set; }
        public double PercentDynamic { get; set; }
        public DateTime Time { get; set; }

        public long MaxWaits { get; set; }
        public double AvgWaits { get; set; }
        public long SentRequestCount { get; set; }
    }
}
