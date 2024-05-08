using System;

namespace Pathway.Core.Infrastructure {
    public class Alert {
        public string PathwayName { get; set; }

        public long Count { get; set; }

        public DateTime FromTimestamp { get; set; }
        public DateTime ToTimestamp { get; set; }
    }
}
