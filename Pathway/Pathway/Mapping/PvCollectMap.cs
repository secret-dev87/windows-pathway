using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using Pathway.Core.Entity;

namespace Pathway.Core.Mapping
{
    public class PvCollectMap : ClassMap<PvCollectEntity>
    {
        public PvCollectMap()
        {
            Table("pvcollects");
            CompositeId()
                .KeyProperty(x => x.FromTimestamp, "FromTimestamp")
                .KeyProperty(x => x.ToTimestamp, "ToTimestamp");
            Map(x => x.IntervalNn);
            Map(x => x.PathwayCountDefined);
            Map(x => x.PathmonCountDefined);
            Map(x => x.ActualStartTime);
            Map(x => x.ActualStopTime);
            Map(x => x.LastTimePcHour);
            Map(x => x.LastTimePcMinute);
            Map(x => x.CollectionStatus);
            Map(x => x.Pathmon01);
            Map(x => x.Pathmon02);
            Map(x => x.Pathmon03);
            Map(x => x.Pathmon04);
            Map(x => x.Pathmon05);
            Map(x => x.Pathmon06);
            Map(x => x.Pathmon07);
            Map(x => x.Pathmon08);
            Map(x => x.Pathmon09);
            Map(x => x.Pathmon10);
            Map(x => x.IntervalHOrM);
        }
    }
}
