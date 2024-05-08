using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pathway.Core.Infrastructure.PerPathway.Server {
    public class ServerErrorListView {
        public string ServerClass { get; set; }
        public DateTime MostRecentTime { get; set; }
        public long Instances { get; set; }

        public long ErrorCount { get; set; }
    }

    public class ServerErrorView {
        public string ServerClass { get; set; }
        public DateTime MostRecentTime { get; set; }
        public long ErrorNumber { get; set; }
    }
}
