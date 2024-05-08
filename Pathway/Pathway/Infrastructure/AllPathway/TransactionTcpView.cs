using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pathway.Core.Infrastructure;

namespace Pathway.Core.Infrastructure.AllPathway {
    public class TransactionTcpView {
        public Dictionary<string, long> TermToTcp { get; set; }

        public Dictionary<string, TcpToServer> TcpToServer { get; set; }
    }
}
