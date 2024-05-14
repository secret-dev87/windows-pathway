using FluentNHibernate.Mapping;
using Pathway.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Mapping
{
    public class PvCPUBusyMap : ClassMap<PvCPUBusyEntity>
    {
        public PvCPUBusyMap()
        {
            Table("pvcpubusies");
            CompositeId()
                .KeyProperty(x => x.FromTimestamp, "FromTimestamp")
                .KeyProperty(x => x.ToTimestamp, "ToTimeStamp")
                .KeyProperty(x => x.Pathway, "Pathway");
            Map(x => x.CPUBusy);
        }
    }
}
