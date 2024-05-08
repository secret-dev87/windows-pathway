namespace Pathway.Core.Infrastructure.PerPathway.Server {
    public class ServerTransactionView {
        public string ServerClass { get; set; }
        public long TotalTransaction { get; set; }
        public long TcpTransaction { get; set; }
        public long LinkmonTransaction { get; set; }
        public float CPUBusy { get; set; }
        public long ProcessCount { get; set; }

        public long ProcessMaxCount { get; set; }

        public long NumStatic { get; set; }

        public long MaxServers { get; set; }
        public double AverageRt { get; set; }
    }
}