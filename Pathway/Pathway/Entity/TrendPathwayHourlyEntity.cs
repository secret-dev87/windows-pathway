using Google.Protobuf.WellKnownTypes;
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

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            TrendPathwayHourlyEntity other = (TrendPathwayHourlyEntity)obj;

            if (other == null)
                return false;
            if (Interval == other.Interval &&
                PathwayName == other.PathwayName)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            return (
                    Interval.ToString() + "|" +
                    PathwayName.ToString()
                ).GetHashCode();
        }
    }
}
