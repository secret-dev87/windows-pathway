namespace Pathway.Core.Infrastructure.PerPathway.Term {
    public class TermTop20View {
        public string TransactionTerm { get; set; }
        public string TransactionTcp { get; set; }
        public long TransactionValue { get; set; }


        public string AvgResponseTerm { get; set; }
        public string AvgResponseTcp { get; set; }
        public double AvgResponseValue { get; set; }


        public string MaxResponseTerm { get; set; }
        public string MaxResponseTcp { get; set; }
        public double MaxResponseValue { get; set; }
    }
}