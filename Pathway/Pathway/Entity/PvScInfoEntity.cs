using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Entity
{
    public class PvScInfoEntity
    {
        public virtual DateTime FromTimestamp { get; set; }
        public virtual DateTime ToTimestamp { get; set; }
        public virtual string PathwayName { get; set; }
        public virtual string ScName { get; set; }
        public virtual int AutoRestart { get; set; }
        public virtual int CpuPairCount { get; set; }
        public virtual int PrimaryCpu1 { get; set; }
        public virtual int BackupCpu1 { get; set; }
        public virtual int PrimaryCpu2 { get; set; }
        public virtual int BackupCpu2 { get; set; }
        public virtual int PrimaryCpu3 { get; set; }
        public virtual int BackupCpu3 { get; set; }
        public virtual int PrimaryCpu4 { get; set; }
        public virtual int BackupCpu4 { get; set; }
        public virtual int PrimaryCpu5 { get; set; }
        public virtual int BackupCpu5 { get; set; }
        public virtual int PrimaryCpu6 { get; set; }
        public virtual int BackupCpu6 { get; set; }
        public virtual int PrimaryCpu7 { get; set; }
        public virtual int BackupCpu7 { get; set; }
        public virtual int PrimaryCpu8 { get; set; }
        public virtual int BackupCpu8 { get; set; }
        public virtual int PrimaryCpu9 { get; set; }
        public virtual int BackupCpu9 { get; set; }
        public virtual int PrimaryCpu10 { get; set; }
        public virtual int BackupCpu10 { get; set; }
        public virtual int PrimaryCpu11 { get; set; }
        public virtual int BackupCpu11 { get; set; }
        public virtual int PrimaryCpu12 { get; set; }
        public virtual int BackupCpu12 { get; set; }
        public virtual int PrimaryCpu13 { get; set; }
        public virtual int BackupCpu13 { get; set; }
        public virtual int PrimaryCpu14 { get; set; }
        public virtual int BackupCpu14 { get; set; }
        public virtual int PrimaryCpu15 { get; set; }
        public virtual int BackupCpu15 { get; set; }
        public virtual int PrimaryCpu16 { get; set; }
        public virtual int BackupCpu16 { get; set; }
        public virtual double CreateDelay { get; set; }
        public virtual double DeleteDelay { get; set;}
        public virtual int LinkDepth { get; set; }
        public virtual int MaxLinks { get; set; }
        public virtual int MaxServers { get; set; }
        public virtual int NumStatic { get; set; }
        public virtual int ScPriority { get; set; }
        public virtual double SendTimeout { get; set; }
        public virtual string ScOwner { get; set; }
        public virtual string HighPin { get; set; }
        public virtual string SecurityCode { get; set; }
        public virtual string ProcessType { get; set; }
        public virtual string Debug { get; set; }
        public virtual string GuardianLib { get; set; }
        public virtual string HomeTermSc { get; set; }
        public virtual string InFile { get; set; }
        public virtual string OutFile { get; set; }
        public virtual string ProgramName { get; set; }
        public virtual string TmfSc { get; set; }
        public virtual string DefaultVolume { get; set; }
    }
}
