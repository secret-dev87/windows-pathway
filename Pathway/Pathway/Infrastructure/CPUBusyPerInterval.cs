using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pathway.Core.Infrastructure {
    public class CPUBusyPerInterval {
        public string ScName { get; set; }
        public double TotalBusyTime { get; set; }
        public DateTime FromTimestamp { get; set; }
    }
}
