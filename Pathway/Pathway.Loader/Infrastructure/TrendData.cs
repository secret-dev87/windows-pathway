using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pathway.Loader.Infrastructure {
    public class TrendData {
        public DateTime Interval { get; set; }
        public string PathwayName { get; set; }
        public double CpuBusy { get; set; }
		public double PeakCPUBusy { get; set; }
		public double PeakLinkmonTransaction { get; set; }
		public double AverageLinkmonTransaction { get; set; }
		public double PeakTCPTransaction { get; set; }
		public double AverageTCPTransaction { get; set; }
		public int CpuCounter { get; set; }
        public long ServerTransactions { get; set; }
    }
}
