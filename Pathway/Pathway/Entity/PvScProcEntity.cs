using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Entity
{
    public class PvScProcEntity
    {
        public virtual DateTime FromTimestamp { get; set; }
        public virtual DateTime ToTimestamp { get; set; }
        public virtual string PathwayName { get; set; }
        public virtual string ScName { get; set; }
        public virtual string ScProcessName { get; set; }
        public virtual int PrimaryCpu { get; set; }
        public virtual int BackupCpu { get; set; }
        public virtual int ScPriority { get; set; }
        public virtual string ProcAssociative { get; set; }
        public virtual string Debug { get; set; }
        public virtual string GuardianSwap { get; set; }
        public virtual string HomeTermSc { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            PvScProcEntity other = (PvScProcEntity)obj;

            if (other == null)
                return false;
            if (FromTimestamp == other.FromTimestamp &&
                ToTimestamp == other.ToTimestamp &&
                PathwayName == other.PathwayName &&
                ScName == other.ScName &&
                ScProcessName == other.ScProcessName)
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
                    ScProcessName.ToString()
                ).GetHashCode();
        }
    }
}
