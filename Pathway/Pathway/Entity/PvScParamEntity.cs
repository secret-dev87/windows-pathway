using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Entity
{
    public class PvScParamEntity
    {
        public virtual DateTime FromTimestamp { get; set; }
        public virtual DateTime ToTimestamp { get; set; }
        public virtual string PathwayName { get; set; }
        public virtual string ScName { get; set; }
        public virtual string ParamName { get; set; }
        public virtual string SFiller { get; set; }
        public virtual string ParamArea { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            PvScParamEntity other = (PvScParamEntity)obj;

            if (other == null)
                return false;
            if (FromTimestamp == other.FromTimestamp &&
                ToTimestamp == other.ToTimestamp &&
                PathwayName == other.PathwayName &&
                ScName == other.ScName &&
                ParamName == other.ParamName)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            return (
                    FromTimestamp.ToString() + "|" +
                    ToTimestamp.ToString() + "|" +
                    ParamName.ToString() + "|" +
                    ScName.ToString() + "|" +
                    ParamName.ToString()
                ).GetHashCode();
        }
    }
}
