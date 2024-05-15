using FluentNHibernate.Mapping;
using Pathway.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Mapping
{
    public class PvCPUOnceMap : ClassMap<PvCPUOnceEntity>
    {
        public PvCPUOnceMap()
        {
            Table("pvcpuonce");
            CompositeId()
                .KeyProperty(x => x.FromTimestamp, "FromTimestamp")
                .KeyProperty(x => x.ToTimestamp, "ToTimestamp")
                .KeyProperty(x => x.CpuNumber, "CpuNumber");
            Map(x => x.ProcessorType);
            Map(x => x.SoftwareVersion).Length(6);
            Map(x => x.PageSize);
            Map(x => x.MemorySize);
            Map(x => x.LocalTimeOffset);
            Map(x => x.ElapsedTime);
        }
    }
}
