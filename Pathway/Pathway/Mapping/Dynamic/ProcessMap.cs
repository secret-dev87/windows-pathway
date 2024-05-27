using FluentNHibernate.Mapping;
using Pathway.Core.Entity.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Mapping.Dynamic
{
    public class ProcessMap : ClassMap<ProcessEntity>
    {
        public ProcessMap()
        {
            Id(x => x.EntityCounterID);
            Map(x => x.SystemName).Length(8);
            Map(x => x.CpuNum);
            Map(x => x.OSLetter).Length(1);
            Map(x => x.OSNumber);
            Map(x => x.FromTimestamp);
            Map(x => x.ToTimestamp);
            Map(x => x.DeltaTime);
            Map(x => x.CpuBusyTime);
            Map(x => x.ReadyTime);
            Map(x => x.MemQtime);
            Map(x => x.Dispatches);
            Map(x => x.PageFaults);
            Map(x => x.PresPagesQTime);
            Map(x => x.RecvQTime);
            Map(x => x.MessagesSent);
            Map(x => x.SentBytesF);
            Map(x => x.ReturnedBytesF);
            Map(x => x.MessagesReceived);
            Map(x => x.ReceivedBytesF);
            Map(x => x.ReplyBytesF);
            Map(x => x.LcbsInUseQTime);
            Map(x => x.CheckPoints);
            Map(x => x.CompTraps);
            Map(x => x.TnsrBusyTime);
            Map(x => x.AccelBusyTime);
            Map(x => x.TnsBusyTime);
            Map(x => x.FileOpenCalls);
            Map(x => x.BeginTrans);
            Map(x => x.AbortTrans);
            Map(x => x.PresPagesStart);
            Map(x => x.PresPagesEnd);
            Map(x => x.OssnsRequests);
            Map(x => x.OssnsWaitTime);
            Map(x => x.OssnsRedirects);
            Map(x => x.Launches);
            Map(x => x.LaunchWaitTime);
            Map(x => x.OpenCloseWaitTime);
            Map(x => x.IpuSwitches);
            Map(x => x.IpuNum);
            Map(x => x.IpuNumPrev);
            Map(x => x.LockedPagesQtime);
            Map(x => x.LockedPagesStart);
            Map(x => x.LockedPagesEnd);
            Map(x => x.Pin);
            Map(x => x.Priority);
            Map(x => x.Group);
            Map(x => x.User);
            Map(x => x.ProcessName).Length(8);
            Map(x => x.Volume).Length(8);
            Map(x => x.SubVol).Length(8);
            Map(x => x.FileName).Length(8);
            Map(x => x.OssPid);
            Map(x => x.AncestorCpu);
            Map(x => x.AncestorPin);
            Map(x => x.AncestorSysName).Length(8);
            Map(x => x.AncestorProcessName).Length(8);
            Map(x => x.DeviceName).Length(8);
            Map(x => x.PathID).Length(24);
            Map(x => x.Crvsn).Length(6);
            Map(x => x.ProgramAccelerated);
            Map(x => x.Ipus);
            Map(x => x.HomeTermSysName).Length(8);
            Map(x => x.Device).Length(8);
            Map(x => x.SubDevice).Length(8);
            Map(x => x.Qualifier).Length(8);
            Map(x => x.UniqueID);
        }
    }
}
