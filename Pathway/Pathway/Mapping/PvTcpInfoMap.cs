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
            Map(x => x.Procname);
            Map(x => x.Pid);
            Map(x => x.ServerPool);
            Map(x => x.TermBuf);
            Map(x => x.TermPool);
            Map(x => x.HighPin);
            Map(x => x.PowerOnRecovery);
            Map(x => x.CheckDirectory);
            Map(x => x.Debug);
            Map(x => x.Dump);
            Map(x => x.DumpFile);
            Map(x => x.GuardianLib);
            Map(x => x.GuardianSwap);
            Map(x => x.HomeTermTcp);
            Map(x => x.InspectOnOff);
            Map(x => x.InspectFile);
            Map(x => x.NonStop);
            Map(x => x.ProgramName);
            Map(x => x.Stats);
            Map(x => x.Swap);
            Map(x => x.TclProg);
        }
    }
}
