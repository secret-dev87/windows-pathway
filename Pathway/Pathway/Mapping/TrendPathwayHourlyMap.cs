using FluentNHibernate.Mapping;
using Pathway.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Mapping
{
    public class TrendPathwayHourlyMap : ClassMap<TrendPathwayHourlyEntity>
    {
        public TrendPathwayHourlyMap()
        {
            Table("trendpathwayhourly");
            CompositeId()
                .KeyProperty(x => x.Interval, "Interval")
                .KeyProperty(x => x.PathwayName, "PathwayName");
            Map(x => x.PeakCPUBusy);
            Map(x => x.CpuBusy);
            Map(x => x.PeakLinkmonTransaction);
            Map(x => x.AverageLinkmonTransaction);
            Map(x => x.PeakTCPTransaction);
            Map(x => x.AverageTCPTransaction);
            Map(x => x.ServerTransaction);
        }
    }
}
