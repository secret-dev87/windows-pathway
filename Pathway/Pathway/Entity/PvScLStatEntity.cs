using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Entity
{
    public class PvScLStatEntity
    {
        public virtual DateTime FromTimestamp { get; set; }
        public virtual DateTime ToTimestamp { get; set; }
        public virtual string PathwayName { get; set; }
        public virtual string ScName { get; set; }
        public virtual string ScLmName { get; set; }
        public virtual string SFiller { get; set; }
        public virtual double QIReqCnt { get; set; }
        public virtual double QIWaits { get; set; }
        public virtual double QIMaxWaits { get; set; }
        public virtual double QIAggregateWaits { get; set; }
        public virtual double QIDynamicLinks { get; set; }
        public virtual double? IsReqCnt { get; set; }
        public virtual double IsMaxSize { get; set; }
        public virtual double IsAggregateSize { get; set; }
        public virtual double IsIOs { get; set; }
        public virtual double? IrReqCnt { get; set; }
        public virtual double IrMaxSize { get; set; }
        public virtual double IrAggregateSize { get; set; }
        public virtual double IrIOs { get; set; }
        public virtual int TotalMeasCount { get; set; }
        public virtual double AvgResp { get; set; }
        public virtual double MaxResp { get; set; }
        public virtual double MinResp { get; set; }
        public virtual double SumOfSquares { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            PvScLStatEntity other = (PvScLStatEntity)obj;

            if (other == null)
                return false;
            if (FromTimestamp == other.FromTimestamp && ToTimestamp == other.ToTimestamp && PathwayName == other.PathwayName && ScName == other.ScName && ScLmName == other.ScLmName)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            return (
                    FromTimestamp.ToString() + "|" +
                    ToTimestamp.ToString() + "|" +
                    PathwayName.ToString() + "|" +
                    ScName.ToString() + "|" +
                    ScLmName.ToString()
                ).GetHashCode();
        }
    }
}
