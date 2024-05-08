namespace Pathway.Core.Infrastructure {
    public class AlertView {
        public string PathwayName { get; set; }
        public long TermHiMaxRt { get; set; }
        public long TermHiAvgRt { get; set; }
        public long TermUnused { get; set; }
        public long TermErrorList { get; set; }
        public long TcpQueuedTransactions { get; set; }
        public long TcpLowTermPool { get; set; }
        public long TcpLowServerPool { get; set; }
        public long TcpUnused { get; set; }
        public long TcpErrorList { get; set; }
        public long ServerHiMaxRt { get; set; }
        public long ServerHiAvgRt { get; set; }
        public long ServerPendingClass { get; set; }
        public long ServerPendingProcess { get; set; }
        public long ServerQueueTcp { get; set; }
        public long ServerQueueLinkmon { get; set; }
        public long ServerUnusedClass { get; set; }
        public long ServerUnusedProcess { get; set; }
        public long ServerMaxLinks { get; set; }
        public long DirectoryOnLinks { get; set; }
        public long HighDynamicServers { get; set; }
        public long ServerErrorList { get; set; }
    }
}