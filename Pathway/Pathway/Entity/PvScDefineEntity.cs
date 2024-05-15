using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Entity
{
    public class PvScDefineEntity
    {
        public virtual DateTime FromTimestamp { get; set; }
        public virtual DateTime ToTimestamp { get; set; }
        public virtual string PathwayName { get; set; }
        public virtual string ScName { get; set; }
        public virtual string DefineName { get; set; }
        public virtual string MapFileName { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            PvScDefineEntity other = (PvScDefineEntity)obj;

            if (other == null)
                return false;
            if (FromTimestamp == other.FromTimestamp &&
                ToTimestamp == other.ToTimestamp &&
                PathwayName == other.PathwayName &&
                ScName == other.ScName &&
                DefineName == other.DefineName)
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
                    DefineName.ToString()
                ).GetHashCode();
        }
    }
}
