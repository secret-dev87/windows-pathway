using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Entity
{
    public class PvCollectEntity
    {
        public virtual DateTime FromTimestamp { get; set; }
        public virtual DateTime ToTimestamp { get; set;}
        public virtual string IntervalNn { get; set; }
        public virtual string PathwayCountDefined { get; set; }
        public virtual string PathmonCountDefined { get; set; }
        public virtual DateTime ActualStartTime { get; set; }
        public virtual DateTime ActualStopTime { get; set; }
        public virtual string LastTimePcHour { get; set; }
        public virtual string LastTimePcMinute { get; set; }
        public virtual string CollectionStatus { get; set; }
        public virtual string Pathmon01 { get; set; }
        public virtual string Pathmon02 { get; set; }
        public virtual string Pathmon03 { get; set; }
        public virtual string Pathmon04 { get; set; }
        public virtual string Pathmon05 { get; set; }
        public virtual string Pathmon06 { get; set; }
        public virtual string Pathmon07 { get; set; }
        public virtual string Pathmon08 { get; set; }
        public virtual string Pathmon09 { get; set; }
        public virtual string Pathmon10 { get; set; }
        public virtual string IntervalHOrM { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            PvCollectEntity other = (PvCollectEntity)obj;

            if (other == null)
                return false;
            if (FromTimestamp == other.FromTimestamp && ToTimestamp == other.ToTimestamp)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            return (
                    FromTimestamp.ToString() + "|" +
                    ToTimestamp.ToString()
                ).GetHashCode();
        }
    }
}
