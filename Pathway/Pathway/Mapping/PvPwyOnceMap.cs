using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using Pathway.Core.Entity;

namespace Pathway.Core.Mapping
{
    public class PvPwyOnceMap : ClassMap<PvPwyOnceEntity>
    {
        public PvPwyOnceMap()
        {
            Table("pvpwyonce");
            CompositeId()
                .KeyProperty(x => x.FromTimestamp, "FromTimestamp")
                .KeyProperty(x => x.ToTimestamp, "ToTimestamp")
                .KeyProperty(x => x.PathwayName, "PathwayName");
            Map(x => x.Owner).Length(26);
            Map(x => x.SecurityCode).Length(1);
            Map(x => x.Dump).Length(3);
            Map(x => x.DumpFile).Length(35);
            Map(x => x.StatusCode1).Length(3);
            Map(x => x.LogFile1).Length(35);
            Map(x => x.State1).Length(6);
            Map(x => x.Event1).Length(3);
            Map(x => x.StatusCode2).Length(3);
            Map(x => x.LogFile2).Length(35);
            Map(x => x.State2).Length(6);
            Map(x => x.Event2).Length(3);
            Map(x => x.ProgMaxVal);
            Map(x => x.TermMaxVal);
            Map(x => x.TcpMaxVal);
            Map(x => x.ServerClassMaxVal);
            Map(x => x.ServerProcessMaxVal);
            Map(x => x.PathwayAssignMaxVal);
            Map(x => x.PathwayDefineMaxVal);
            Map(x => x.PathwayParamMaxVal);
            Map(x => x.ExternalTcpMaxVal);
            Map(x => x.LinkmonMaxVal);
            Map(x => x.PathcomMaxVal);
            Map(x => x.StartupMaxVal);
            Map(x => x.SpiMaxVal);
            Map(x => x.TmfRestarts);
        }
    }
}
