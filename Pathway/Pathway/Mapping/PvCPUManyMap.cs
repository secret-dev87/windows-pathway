using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using Pathway.Core.Entity;

namespace Pathway.Core.Mapping
{
    public class PvCPUManyMap : ClassMap<PvCPUManyEntity>
    {
        public PvCPUManyMap()
        {
            Table("pvcpumany");
            CompositeId()
                .KeyProperty(x => x.FromTimestamp, "FromTimestamp")
                .KeyProperty(x => x.ToTimestamp, "ToTimestamp")
                .KeyProperty(x => x.CpuNumber, "CpuNumber");
            Map(x => x.SwapplePages);
            Map(x => x.FreePages);
            Map(x => x.CurrentLockedMemory);
            Map(x => x.PageFaults);
            Map(x => x.MElapsedTime);
            Map(x => x.BusyTime);
            Map(x => x.ProcessorQueueLength);
            Map(x => x.Dispatches);
            Map(x => x.LowPinMax);
            Map(x => x.LowPinInUse);
            Map(x => x.LowPinFree);
            Map(x => x.LowPinFailure);
            Map(x => x.HighPinMax);
            Map(x => x.HighPinInUse);
            Map(x => x.HighPinFree);
            Map(x => x.HighPinFailure);
            Map(x => x.SendBusy);
            Map(x => x.DiskCacheHits);
            Map(x => x.DiskIOs);
        }
    }
}
