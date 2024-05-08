using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pathway.Core.Infrastructure {
    public class ErrorInfo {
        public long ErrorNumber { get; set; }

        public string Message { get; set; }
        public string Cause { get; set; }
        public string Effect { get; set; }
        public string Recovery { get; set; }
    }
}
