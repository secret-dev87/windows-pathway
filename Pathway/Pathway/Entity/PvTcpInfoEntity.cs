using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Entity
{
    public class PvTcpInfoEntity
    {
        public virtual DateTime FromTimestamp { get; set; }
        public virtual DateTime ToTimestamp { get; set; }
        public virtual string PathwayName { get; set; }
        public virtual string TcpName { get; set; }
        public virtual int AutoRestart { get; set; }
        public virtual int CodeAreaLen { get; set; }
        public virtual int PrimaryCpu { get; set; }
        public virtual int BackupCpu { get; set; }
        public virtual int MaxInputMsgLen { get; set; }
        public virtual int MaxInputMsgs { get; set; }
        public virtual int MaxPathways { get; set; }
        public virtual int MaxReply { get; set; }
        public virtual int MaxServerClasses { get; set; }
        public virtual int MaxServerProcesses { get; set; }
        public virtual int MaxTermData { get; set; }
        public virtual int MaxTerms { get; set; }
        public virtual int Priority { get; set; }
        public virtual string Procname { get; set; }
        public virtual int Pid { get; set; }
        public virtual int ServerPool { get; set; }
        public virtual int TermBuf { get; set; }
        public virtual int TermPool { get; set; }
        public virtual string HighPin { get; set; }
        public virtual string PowerOnRecovery { get; set; }
        public virtual string CheckDirectory { get; set; }
        public virtual string Debug { get; set; }
        public virtual string Dump { get; set; }
        public virtual string DumpFile { get; set; }
        public virtual string GuardianLib { get; set; }
        public virtual string GuardianSwap { get; set; }
        public virtual string HomeTermTcp { get; set; }
        public virtual string InspectOnOff { get; set; }
        public virtual string InspectFile { get; set; }
        public virtual string NonStop { get; set; }
        public virtual string ProgramName { get; set; }
        public virtual string Stats { get; set; }
        public virtual string Swap { get; set; }
        public virtual string TclProg { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            PvTcpInfoEntity other = (PvTcpInfoEntity)obj;

            if (other == null)
                return false;
            if (FromTimestamp == other.FromTimestamp &&
                ToTimestamp == other.ToTimestamp &&
                PathwayName == other.PathwayName &&
                TcpName == other.TcpName)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            return (
                    FromTimestamp.ToString() + "|" +
                    ToTimestamp.ToString() + "|" +
                    PathwayName.ToString() + "|" +
                    TcpName.ToString()
                ).GetHashCode();
        }
    }
}
