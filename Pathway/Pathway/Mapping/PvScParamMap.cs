using FluentNHibernate.Mapping;
using Pathway.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Mapping
{
    public class PvScParamMap : ClassMap<PvScParamEntity>
    {
        public PvScParamMap()
        {
            Table("pvscparam");
            CompositeId()
                .KeyProperty(x => x.FromTimestamp, "FromTimestamp")
                .KeyProperty(x => x.ToTimestamp, "ToTimestamp")
                .KeyProperty(x => x.PathwayName, "PathwayName")
                .KeyProperty(x => x.ScName, "ScName")
                .KeyProperty(x => x.ParamName, "ParamName");
            Map(x => x.SFiller).Length(1);
            Map(x => x.ParamArea).Length(132);
        }
    }
}
