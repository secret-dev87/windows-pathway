using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Entity
{
    public class PvLmStusEntity
    {
        public virtual DateTime FromTimestamp { get; set; }
        public virtual DateTime ToTimestamp { get; set; }
        public virtual string LinkmonName { get; set; }
        public virtual string LFiller { get; set; }
        public virtual double LmStatusTime { get; set; }
        public virtual double LmProcessTime { get; set; }
        public virtual double DeltaTime { get; set; }
        public virtual double DeltaProcTime { get; set; }
        public virtual string LmCpu { get; set; }
        public virtual double CurPages { get; set; }
        public virtual double RecQueue { get; set; }
        public virtual double PageFaults { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            PvLmStusEntity other = (PvLmStusEntity)obj;

            if (other == null)
                return false;
            if (FromTimestamp == other.FromTimestamp &&
                ToTimestamp == other.ToTimestamp &&
                LinkmonName == other.LinkmonName)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            return (
                    FromTimestamp.ToString() + "|" +
                    ToTimestamp.ToString() + "|" +
                    LinkmonName.ToString()
                ).GetHashCode();
        }
    }
}
