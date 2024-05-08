namespace Pathway.Core.Infrastructure.PerPathway.Server {
    public class ServerProcessCountView {
        public string ServerClass { get; set; }
        public long ProcessCount { get; set; }
        public double CPUBusy { get; set; }
        public long TcpTransaction { get; set; }
        public long LinkmonTransaction { get; set; }
        public double AverageRt { get; set; }
    }
}