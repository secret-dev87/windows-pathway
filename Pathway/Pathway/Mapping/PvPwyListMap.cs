using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using Pathway.Core.Entity;

namespace Pathway.Core.Mapping
{
    public class PvPwyListMap : ClassMap<PvPwyListEntity>
    {
        public PvPwyListMap()
        {
            Table("pvpwylist");
            CompositeId()
                .KeyProperty(x => x.FromTimestamp, "FromTimestamp")
                .KeyProperty(x => x.ToTimestamp, "ToTimestamp")
                .KeyProperty(x => x.PathwayName, "PathwayName");
            Map(x => x.PFiller).Length(1);
            Map(x => x.StartTime);
            Map(x => x.LastTime);
        }
    }
}
