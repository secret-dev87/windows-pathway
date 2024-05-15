using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using Pathway.Core.Entity;

namespace Pathway.Core.Mapping
{
    public class PvLmStusMap : ClassMap<PvLmStusEntity>
    {
        public PvLmStusMap()
        {
            Table("pvlmstus");
            CompositeId()
                .KeyProperty(x => x.FromTimestamp, "FromTimestamp")
                .KeyProperty(x => x.ToTimestamp, "ToTimestamp")
                .KeyProperty(x => x.LinkmonName, "LinkmonName");
            Map(x => x.LFiller).Length(1);
            Map(x => x.LmStatusTime);
            Map(x => x.LmProcessTime);
            Map(x => x.DeltaTime);
            Map(x => x.DeltaProcTime);
            Map(x => x.LmCpu).Length(1);
            Map(x => x.CurPages);
            Map(x => x.RecQueue);
            Map(x => x.PageFaults);
        }
    }
}
