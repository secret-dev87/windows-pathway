using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using Pathway.Core.Entity;

namespace Pathway.Core.Mapping
{
    public class PvScDefineMap : ClassMap<PvScDefineEntity>
    {
        public PvScDefineMap()
        {
            Table("pvscdefine");
            CompositeId()
                .KeyProperty(x => x.FromTimestamp, "FromTimestamp")
                .KeyProperty(x => x.ToTimestamp, "ToTimestamp")
                .KeyProperty(x => x.PathwayName, "PathwayName")
                .KeyProperty(x => x.ScName, "ScName")
                .KeyProperty(x => x.DefineName, "DefineName");
            Map(x => x.MapFileName).Length(35);
        }
    }
}
