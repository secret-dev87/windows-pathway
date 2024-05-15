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
            Map(x => x.IntervalNn).Length(2);
            Map(x => x.PathwayCountDefined).Length(2);
            Map(x => x.PathmonCountDefined).Length(2);
            Map(x => x.ActualStartTime);
            Map(x => x.ActualStopTime);
            Map(x => x.LastTimePcHour).Length(2);
            Map(x => x.LastTimePcMinute).Length(2);
            Map(x => x.CollectionStatus).Length(8);
            Map(x => x.Pathmon01).Length(35);
            Map(x => x.Pathmon02).Length(35);
            Map(x => x.Pathmon03).Length(35);
            Map(x => x.Pathmon04).Length(35);
            Map(x => x.Pathmon05).Length(35);
            Map(x => x.Pathmon06).Length(35);
            Map(x => x.Pathmon07).Length(35);
            Map(x => x.Pathmon08).Length(35);
            Map(x => x.Pathmon09).Length(35);
            Map(x => x.Pathmon10).Length(35);
            Map(x => x.IntervalHOrM).Length(1);
        }
    }
}
