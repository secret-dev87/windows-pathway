namespace Pathway.Core.Infrastructure.PerPathway.Tcp {
    public class TcpTransactionView {
        public string Tcp { get; set; }
        public long TermTranscaction { get; set; }
        public double CPUBusy { get; set; }
        public long ServerTransaction { get; set; }
        public long TermCount { get; set; }
        public double AverageRt { get; set; }
    }
}