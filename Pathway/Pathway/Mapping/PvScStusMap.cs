using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using FluentNHibernate.Mapping;
using Pathway.Core.Entity;

namespace Pathway.Core.Mapping
{
    public class PvScStusMap : ClassMap<PvScStusEntity>
    {
        public PvScStusMap()
        {
            Table("pvscstus");
            CompositeId()
                .KeyProperty(x => x.FromTimestamp, "FromTimestamp")
                .KeyProperty(x => x.ToTimestamp, "ToTimestamp")
                .KeyProperty(x => x.PathwayName, "PathwayName")
                .KeyProperty(x => x.ScName, "ScName");
            Map(x => x.ErrorNumber);
            Map(x => x.ErrorInfo);
            Map(x => x.ScRunning);
            Map(x => x.FreezeState);
        }
    }
}
