using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using Pathway.Core.Entity;

namespace Pathway.Core.Mapping
{
    public class PvPwyManyMap : ClassMap<PvPwyManyEntity>
    {
        public PvPwyManyMap()
        {
            Table("pvpwymany");
            CompositeId()
                .KeyProperty(x => x.FromTimestamp, "FromTimestamp")
                .KeyProperty(x => x.ToTimestamp, "ToTimestamp")
                .KeyProperty(x => x.PathwayName, "PathwayName");
            Map(x => x.PFiller).Length(1);
            Map(x => x.PwyStatusTime);
            Map(x => x.PwyProcessTime);
            Map(x => x.DeltaTime);
            Map(x => x.DeltaProcTime);
            Map(x => x.PwyCpu).Length(2);
            Map(x => x.CurPages);
            Map(x => x.RecQueue);
            Map(x => x.PageFaults);
            Map(x => x.TermRunning);
            Map(x => x.TermStopped);
            Map(x => x.TermPending);
            Map(x => x.TermSuspended);
            Map(x => x.TcpRunning);
            Map(x => x.TcpStopped);
            Map(x => x.TcpPending);
            Map(x => x.ScRunning);
            Map(x => x.ScStopped);
            Map(x => x.ScThawed);
            Map(x => x.ScFrozen);
            Map(x => x.ScFreezePending);
            Map(x => x.SpRunning);
            Map(x => x.SpStopped);
            Map(x => x.SpPending);
            Map(x => x.Pathcom);
            Map(x => x.Linkmon);
            Map(x => x.ExternalTcp);
            Map(x => x.Spi);
        }
    }
}
