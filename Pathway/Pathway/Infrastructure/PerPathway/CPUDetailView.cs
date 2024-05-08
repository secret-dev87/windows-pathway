using System.Collections.Generic;

namespace Pathway.Core.Infrastructure.PerPathway {
    public class CPUDetailView {
        [System.ComponentModel.DefaultValue(0)]
        public double Pathmon { get; set; }

        [System.ComponentModel.DefaultValue(0)]
        public double Tcp { get; set; }

        [System.ComponentModel.DefaultValue(0)]
        public double Servers { get; set; }
    }
}