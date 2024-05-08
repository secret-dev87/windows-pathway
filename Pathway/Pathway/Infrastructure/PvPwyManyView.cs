using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pathway.Core.Infrastructure {
    public class PvPwyManyView {
        public DateTime FromTimestamp { get; set; }
        public DateTime ToTimestamp { get; set; }
        public string PathwayName { get; set; }
        public string PwyCpu { get; set; }
        public long DeltaProcTime { get; set; }
    }
}
