using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using Pathway.Core.Entity;

namespace Pathway.Core.Mapping
{
    public class PvErrInfoMap : ClassMap<PvErrInfoEntity>
    {
        public PvErrInfoMap()
        {
            Table("pverrinfo");
            CompositeId()
                .KeyProperty(x => x.FromTimestamp, "FromTimestamp")
                .KeyProperty(x => x.ToTimestamp, "ToTimestamp")
                .KeyProperty(x => x.PathwayName, "PathwayName")
                .KeyProperty(x => x.ErrCurrentEntity, "ErrCurrentEntity")
                .KeyProperty(x => x.ErrCommand, "ErrCommand");
            Map(x => x.ErrSpiStatus).Length(4);
        }
    }
}
