using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using FluentNHibernate.Mapping;
using Pathway.Core.Entity;

namespace Pathway.Core.Mapping
{
    public class PvScInfoMap : ClassMap<PvScInfoEntity>
    {
        public PvScInfoMap()
        {
            Table("pvscinfo");
            CompositeId()
                .KeyProperty(x => x.FromTimestamp, "FromTimestamp")
                .KeyProperty(x => x.ToTimestamp, "ToTimestamp")
                .KeyProperty(x => x.PathwayName)
                .KeyProperty(x => x.ScName);
            Map(x => x.AutoRestart);
            Map(x => x.CpuPairCount);
            Map(x => x.PrimaryCpu1);
            Map(x => x.BackupCpu1);
            Map(x => x.PrimaryCpu2);
            Map(x => x.BackupCpu2);
            Map(x => x.PrimaryCpu3);
            Map(x => x.BackupCpu3);
            Map(x => x.PrimaryCpu4);
            Map(x => x.BackupCpu4);
            Map(x => x.PrimaryCpu5);
            Map(x => x.BackupCpu5);
            Map(x => x.PrimaryCpu6);
            Map(x => x.BackupCpu6);
            Map(x => x.PrimaryCpu7);
            Map(x => x.BackupCpu7);
            Map(x => x.PrimaryCpu8);
            Map(x => x.BackupCpu8);
            Map(x => x.PrimaryCpu9);
            Map(x => x.BackupCpu9);
            Map(x => x.PrimaryCpu10);
            Map(x => x.BackupCpu10);
            Map(x => x.PrimaryCpu11);
            Map(x => x.BackupCpu11);
            Map(x => x.PrimaryCpu12);
            Map(x => x.BackupCpu12);
            Map(x => x.PrimaryCpu13);
            Map(x => x.BackupCpu13);
            Map(x => x.PrimaryCpu14);
            Map(x => x.BackupCpu14);
            Map(x => x.PrimaryCpu15);
            Map(x => x.BackupCpu15);
            Map(x => x.PrimaryCpu16);
            Map(x => x.BackupCpu16);
            Map(x => x.CreateDelay);
            Map(x => x.DeleteDelay);
            Map(x => x.LinkDepth);
            Map(x => x.MaxLinks);
            Map(x => x.MaxServers);
            Map(x => x.NumStatic);
            Map(x => x.ScPriority);
            Map(x => x.SendTimeout);
            Map(x => x.ScOwner);
            Map(x => x.HighPin);
            Map(x => x.SecurityCode);
            Map(x => x.ProcessType);
            Map(x => x.Debug);
            Map(x => x.GuardianLib);
            Map(x => x.HomeTermSc);
            Map(x => x.InFile);
            Map(x => x.OutFile);
            Map(x => x.ProgramName);
            Map(x => x.TmfSc);
            Map(x => x.DefaultVolume);
        }
    }
}
