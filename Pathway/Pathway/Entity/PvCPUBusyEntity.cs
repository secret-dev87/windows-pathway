using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Entity
{
    public class PvCPUBusyEntity
    {
        public virtual DateTime FromTimestamp { get; set; }
        public virtual DateTime ToTimestamp { get; set; }
        public virtual string Pathway { get; set; }
        public virtual double CPUBusy { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            PvCPUBusyEntity other = (PvCPUBusyEntity)obj;

            if (other == null)
                return false;
            if (FromTimestamp == other.FromTimestamp && ToTimestamp == other.ToTimestamp && Pathway == other.Pathway)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            return (
                    FromTimestamp.ToString() + "|" +
                    ToTimestamp.ToString() + "|" +
                    Pathway.ToString()
                ).GetHashCode();
        }
    }
}
