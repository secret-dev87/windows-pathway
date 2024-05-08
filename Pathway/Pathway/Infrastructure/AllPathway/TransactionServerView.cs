using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pathway.Core.Infrastructure;

namespace Pathway.Core.Infrastructure.AllPathway {
    public class TransactionServerView {
        public Dictionary<string, LinkMonToServer> LinkmonToServer { get; set; }

        public Dictionary<string, TcpToServer> TcptoServer { get; set; }
    }
}
