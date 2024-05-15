using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Entity
{
    public class PvScAssignEntity
    {
        public virtual DateTime FromTimestamp { get; set; }
        public virtual DateTime ToTimestamp { get; set; }
        public virtual string PathwayName { get; set; }
        public virtual string ScName { get; set; }
        public virtual string LogicalFile { get; set; }
        public virtual string GuardianFile { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            PvScAssignEntity other = (PvScAssignEntity)obj;

            if (other == null)
                return false;
            if (FromTimestamp == other.FromTimestamp &&
                ToTimestamp == other.ToTimestamp &&
                PathwayName == other.PathwayName &&
                ScName == other.ScName &&
                LogicalFile == other.LogicalFile)
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
                    LogicalFile.ToString()
                ).GetHashCode();
        }
    }
}
