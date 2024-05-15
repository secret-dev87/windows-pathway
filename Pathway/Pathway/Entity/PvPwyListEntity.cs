using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Entity
{
    public class PvPwyListEntity
    {
        public virtual DateTime FromTimestamp { get; set; }
        public virtual DateTime ToTimestamp { get; set; }
        public virtual string PathwayName { get; set; }
        public virtual string PFiller { get; set; }
        public virtual DateTime StartTime { get; set; }
        public virtual DateTime LastTime { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            PvPwyListEntity other = (PvPwyListEntity)obj;

            if (other == null)
                return false;
            if (FromTimestamp == other.FromTimestamp && ToTimestamp == other.ToTimestamp && PathwayName == other.PathwayName)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            return (
                    FromTimestamp.ToString() + "|" +
                    ToTimestamp.ToString() + "|" +
                    PathwayName.ToString()
                ).GetHashCode();
        }
    }
}
