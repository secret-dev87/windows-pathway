using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Entity
{
    public class PvErrInfoEntity
    {
        public virtual DateTime FromTimestamp { get; set; }
        public virtual DateTime ToTimestamp { get; set; }
        public virtual string PathwayName { get; set; }
        public virtual string ErrCurrentEntity { get; set; }
        public virtual string ErrCommand { get; set; }
        public virtual string ErrSpiStatus { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            PvErrInfoEntity other = (PvErrInfoEntity)obj;

            if (other == null)
                return false;
            if (FromTimestamp == other.FromTimestamp &&
                ToTimestamp == other.ToTimestamp &&
                PathwayName == other.PathwayName &&
                ErrCurrentEntity == other.ErrCurrentEntity &&
                ErrCommand == other.ErrCommand)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            return (
                    FromTimestamp.ToString() + "|" +
                    ToTimestamp.ToString() + "|" +
                    PathwayName.ToString() + "|" +
                    ErrCurrentEntity.ToString() + "|" +
                    ErrCommand.ToString()
                ).GetHashCode();
        }
    }
}
