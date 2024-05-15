using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Entity
{
    public class PvCPUManyEntity
    {
        public virtual DateTime FromTimestamp { get; set; }
        public virtual DateTime ToTimestamp { get; set; }
        public virtual string CpuNumber { get; set; }
        public virtual int SwapplePages { get; set; }
        public virtual int FreePages { get; set; }
        public virtual int CurrentLockedMemory { get; set; }
        public virtual int PageFaults { get; set; }
        public virtual double? MElapsedTime { get; set; }
        public virtual double? BusyTime { get; set; }
        public virtual int ProcessorQueueLength { get; set; }
        public virtual long Dispatches { get; set; }
        public virtual int LowPinMax { get; set; }
        public virtual int LowPinInUse { get; set; }
        public virtual int LowPinFree { get; set; }
        public virtual int LowPinFailure { get; set; }
        public virtual int HighPinMax { get; set; }
        public virtual int HighPinInUse { get; set; }
        public virtual int HighPinFree { get; set; }
        public virtual int HighPinFailure { get; set; }
        public virtual double SendBusy { get; set; }
        public virtual double DiskCacheHits { get; set; }
        public virtual double DiskIOs { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            PvCPUManyEntity other = (PvCPUManyEntity)obj;

            if (other == null)
                return false;
            if (FromTimestamp == other.FromTimestamp && ToTimestamp == other.ToTimestamp && CpuNumber == other.CpuNumber)
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
