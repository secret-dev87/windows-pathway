using FluentNHibernate.Mapping;
using Pathway.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Mapping
{
    public class PvTcpInfoMap : ClassMap<PvTcpInfoEntity>
    {
        public PvTcpInfoMap()
        {
            Table("pvtcpinfo");
            CompositeId()
                .KeyProperty(x => x.FromTimestamp, "FromTimestamp")
                .KeyProperty(x => x.ToTimestamp, "ToTimestamp")
                .KeyProperty(x => x.PathwayName, "PathwayName")
                .KeyProperty(x => x.TcpName, "TcpName");
            Map(x => x.AutoRestart);
            Map(x => x.CodeAreaLen);
            Map(x => x.PrimaryCpu);
            Map(x => x.BackupCpu);
            Map(x => x.MaxInputMsgLen);
            Map(x => x.MaxInputMsgs);
            Map(x => x.MaxPathways);
            Map(x => x.MaxReply);
            Map(x => x.MaxServerClasses);
            Map(x => x.MaxServerProcesses);
            Map(x => x.MaxTermData);
            Map(x => x.MaxTerms);
            Map(x => x.Priority);
            Map(x => x.Procname).Length(6);
            Map(x => x.Pid);
            Map(x => x.ServerPool);
            Map(x => x.TermBuf);
            Map(x => x.TermPool);
            Map(x => x.HighPin).Length(1);
            Map(x => x.PowerOnRecovery).Length(1);
            Map(x => x.CheckDirectory).Length(1);
            Map(x => x.Debug).Length(1);
            Map(x => x.Dump).Length(1);
            Map(x => x.DumpFile).Length(35);
            Map(x => x.GuardianLib).Length(35);
            Map(x => x.GuardianSwap).Length(35);
            Map(x => x.HomeTermTcp).Length(35);
            Map(x => x.InspectOnOff).Length(1);
            Map(x => x.InspectFile).Length(35);
            Map(x => x.NonStop).Length(1);
            Map(x => x.ProgramName).Length(35);
            Map(x => x.Stats).Length(1);
            Map(x => x.Swap).Length(35);
            Map(x => x.TclProg).Length(35);
        }
    }
}
