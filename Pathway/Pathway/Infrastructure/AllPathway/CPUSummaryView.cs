using System.Collections.Generic;

namespace Pathway.Core.Infrastructure.AllPathway {
    public class CPUSummaryView {
        public double ElapsedTime { get; set; }
        public long BusyTime { get; set; }

        public Dictionary<string, double> AllPathways { get; set; }

        public Dictionary<string, double> AllPathwaysWithoutFree { get; set; }

        public Dictionary<string, double> OnlyPathways { get; set; }
    }
}
