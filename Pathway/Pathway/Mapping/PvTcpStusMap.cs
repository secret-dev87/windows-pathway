using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using Pathway.Core.Entity;

namespace Pathway.Core.Mapping
{
    public class PvTcpStusMap : ClassMap<PvTcpStusEntity>
    {
        public PvTcpStusMap()
        {
            Table("pvtcpstus");
            CompositeId()
                .KeyProperty(x => x.FromTimestamp, "FromTimestamp")
                .KeyProperty(x => x.ToTimestamp, "ToTimestamp")
                .KeyProperty(x => x.PathwayName, "PathwayName")
                .KeyProperty(x => x.TcpName, "TcpName");
            Map(x => x.ErrorNumber);
            Map(x => x.ErrorInfo);
            Map(x => x.Pid);
            Map(x => x.PrimaryPin);
            Map(x => x.BackupPin);
            Map(x => x.TcpStatusTime);
            Map(x => x.TcpProcessTime);
            Map(x => x.DeltaTime);
            Map(x => x.DeltaProcTime);
            Map(x => x.TcpCpu).Length(2);
            Map(x => x.CurPages);
            Map(x => x.RecQueue);
            Map(x => x.PageFaults);
            Map(x => x.Procname).Length(6);
            Map(x => x.State).Length(1);
        }
    }
}
