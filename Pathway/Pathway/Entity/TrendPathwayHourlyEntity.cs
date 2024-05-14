using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Entity
{
    public class TrendPathwayHourlyEntity
    {
        public virtual DateTime Interval { get; set; }
        public virtual string PathwayName { get; set; }
        public virtual double PeakCPUBusy { get; set; }
        public virtual double CpuBusy { get; set; }
        public virtual double PeakLinkmonTransaction { get; set; }
        public virtual double AverageLinkmonTransaction { get; set; }
        public virtual double PeakTCPTransaction { get; set; }
        public virtual double AverageTCPTransaction { get; set; }
        public virtual double ServerTransaction { get; set; }
    }
}
