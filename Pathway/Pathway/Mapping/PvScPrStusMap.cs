using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using Pathway.Core.Entity;

namespace Pathway.Core.Mapping
{
    public class PvScPrStusMap : ClassMap<PvScPrStusEntity>
    {
        public PvScPrStusMap()
        {
            Table("pvscprstus");
            CompositeId()
                .KeyProperty(x => x.FromTimestamp, "FromTimestamp")
                .KeyProperty(x => x.ToTimestamp, "ToTimestamp")
                .KeyProperty(x => x.PathwayName, "PathwayName")
                .KeyProperty(x => x.ScName, "ScName")
                .KeyProperty(x => x.ScProcessName, "ScProcessName");
            Map(x => x.ErrorNumber);
            Map(x => x.ErrorInfo);
            Map(x => x.Pid);
            Map(x => x.ProcLinks);
            Map(x => x.ProcWeight);
            Map(x => x.ScStatusTime);
            Map(x => x.ScProcessTime);
            Map(x => x.DeltaTime);
            Map(x => x.DeltaProcTime);
            Map(x => x.ScprCpu).Length(2);
            Map(x => x.CurPages);
            Map(x => x.RecQueue);
            Map(x => x.PageFaults);
            Map(x => x.ProcState).Length(1);
        }
    }
}
