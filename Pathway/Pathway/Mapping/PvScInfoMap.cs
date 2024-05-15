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
            Map(x => x.ScOwner).Length(26);
            Map(x => x.HighPin).Length(1);
            Map(x => x.SecurityCode).Length(1);
            Map(x => x.ProcessType).Length(1);
            Map(x => x.Debug).Length(1);
            Map(x => x.GuardianLib).Length(35);
            Map(x => x.HomeTermSc).Length(35);
            Map(x => x.InFile).Length(35);
            Map(x => x.OutFile).Length(35);
            Map(x => x.ProgramName).Length(35);
            Map(x => x.TmfSc).Length(1);
            Map(x => x.DefaultVolume).Length(35);
        }
    }
}
