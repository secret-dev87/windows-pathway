using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Entity
{
    public class PvTermInfoEntity
    {
        public virtual DateTime FromTimestamp { get; set; }
        public virtual DateTime ToTimestamp { get; set; }
        public virtual string PathwayName { get; set; }
        public virtual string TcpName { get; set; }
        public virtual string TermName { get; set; }
        public virtual string TFiller { get; set; }
        public virtual int AutoRestart { get; set; }
        public virtual int DisplayPages { get; set; }
        public virtual string InitialProgram { get; set; }
        public virtual int MaxInputMsgs { get; set; }
        public virtual int TermSubType { get; set; }
        public virtual string TrailingBlanks { get; set; }
        public virtual string BreakTerm { get; set; }
        public virtual string Diagnostic { get; set; }
        public virtual string EchoTerm { get; set; }
        public virtual string ExclusiveOnOff { get; set; }
        public virtual string FileName { get; set; }
        public virtual string InspectOnOff { get; set; }
        public virtual string InspectFile { get; set; }
        public virtual string IOProtocol { get; set; }
        public virtual string PrinterIsAttached { get; set; }
        public virtual string PrinterFile { get; set; }
        public virtual string TclProg { get; set; }
        public virtual string TmfTerm { get; set; }
        public virtual string TermType { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            PvTermInfoEntity other = (PvTermInfoEntity)obj;

            if (other == null)
                return false;
            if (FromTimestamp == other.FromTimestamp &&
                ToTimestamp == other.ToTimestamp &&
                PathwayName == other.PathwayName &&
                TcpName == other.TcpName &&
                TermName == other.TermName)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            return (
                    FromTimestamp.ToString() + "|" +
                    ToTimestamp.ToString() + "|" +
                    PathwayName.ToString() + "|" +
                    TcpName.ToString() + "|" +
                    TermName.ToString()
                ).GetHashCode();
        }
    }
}
