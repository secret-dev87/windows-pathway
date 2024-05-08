using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pathway.Core.Infrastructure {
	public class TcpToServer {
		public long TotalIsReqCnt { get; set; }
		public long PeakReqCnt { get; set; }
		public double AverageReqCnt { get; set; }
	}
}
