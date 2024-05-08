using System;

namespace Pathway.Core.Infrastructure.PerPathway.Server {
    public class ServerCPUBusyView {
        public string ServerClass { get; set; }

        [System.ComponentModel.DefaultValue(0)]
        public double CPUBusy { get; set; }

        [System.ComponentModel.DefaultValue(0)]
        public long TotalTransaction { get; set; }

        [System.ComponentModel.DefaultValue(0)]
        public long TcpTransaction { get; set; }

        [System.ComponentModel.DefaultValue(0)]
        public long LinkmonTransaction { get; set; }

        [System.ComponentModel.DefaultValue(0)]
        public long ProcessCount { get; set; }

        [System.ComponentModel.DefaultValue(0)]
        public long ProcessMaxCount { get; set; }

        [System.ComponentModel.DefaultValue(0)]
        public long NumStatic { get; set; }

        [System.ComponentModel.DefaultValue(0)]
        public long MaxServers { get; set; }

        [System.ComponentModel.DefaultValue(0)]
        public double AverageRt { get; set; }

        public DateTime FromTimestamp { get; set; }
    }
}