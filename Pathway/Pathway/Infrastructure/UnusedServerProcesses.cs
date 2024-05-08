using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pathway.Core.Infrastructure {
    public class UnusedServerProcesses {
        public string ServerClass { get; set; }

        public string Process { get; set; }

        public int MaxServers { get; set; }

        public int NumStatic { get; set; }
    }

    public class ServerMaxLinks
    {
        public string ServerClass { get; set; }

        public int LinksUsed { get; set; }

        public int MaxLinks { get; set; }
    }

    public class HighDynamicServers
    {
        public string ServerClass { get; set; }

        public int ProcessCount { get; set; }

        public int NumStatic { get; set; }

        public int MaxServers { get; set; }
    }
    
    public class CheckDirectoryON
    {
        public string ServerClass { get; set; }
    }
}
