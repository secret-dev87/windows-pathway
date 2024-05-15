using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Entity
{
    public class PvCPUOnceEntity
    {
        public virtual DateTime FromTimestamp { get; set; }
        public virtual DateTime ToTimestamp { get; set; }
        public virtual string CpuNumber { get; set; }
        public virtual int ProcessorType { get; set; }
        public virtual string SoftwareVersion { get; set; }
        public virtual int PageSize { get; set; }
        public virtual int MemorySize { get; set; }
        public virtual double LocalTimeOffset { get; set; }
        public virtual double ElapsedTime { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            PvCPUOnceEntity other = (PvCPUOnceEntity)obj;

            if (other == null)
                return false;
            if (FromTimestamp == other.FromTimestamp &&
                ToTimestamp == other.ToTimestamp &&
                CpuNumber == other.CpuNumber)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            return (
                    FromTimestamp.ToString() + "|" +
                    ToTimestamp.ToString() + "|" +
                    CpuNumber.ToString()
                ).GetHashCode();
        }
    }
}
