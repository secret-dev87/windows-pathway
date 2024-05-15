using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Entity
{
    public class PvPwyOnceEntity
    {
        public virtual DateTime FromTimestamp { get; set; }
        public virtual DateTime ToTimestamp { get; set; }
        public virtual string PathwayName { get; set; }
        public virtual string Owner { get; set; }
        public virtual string SecurityCode { get; set; }
        public virtual string Dump { get; set; }
        public virtual string DumpFile { get; set; }
        public virtual string StatusCode1 { get; set; }
        public virtual string LogFile1 { get; set; }
        public virtual string State1 { get; set; }
        public virtual string Event1 { get; set; }
        public virtual string StatusCode2 { get; set; }
        public virtual string LogFile2 { get; set; }
        public virtual string State2 { get; set; }
        public virtual string Event2 { get; set; }
        public virtual int ProgMaxVal { get; set; }
        public virtual int TermMaxVal { get; set; }
        public virtual int TcpMaxVal { get; set; }
        public virtual int ServerClassMaxVal { get; set; }
        public virtual int ServerProcessMaxVal { get; set; }
        public virtual int PathwayAssignMaxVal { get; set; }
        public virtual int PathwayDefineMaxVal { get; set; }
        public virtual int PathwayParamMaxVal { get; set; }
        public virtual int ExternalTcpMaxVal { get; set; }
        public virtual int LinkmonMaxVal { get; set; }
        public virtual int PathcomMaxVal { get; set; }
        public virtual int StartupMaxVal { get; set; }
        public virtual int SpiMaxVal { get; set; }
        public virtual int TmfRestarts { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            PvPwyOnceEntity other = (PvPwyOnceEntity)obj;

            if (other == null)
                return false;
            if (FromTimestamp == other.FromTimestamp &&
                ToTimestamp == other.ToTimestamp &&
                PathwayName == other.PathwayName)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            return (
                    FromTimestamp.ToString() + "|" +
                    ToTimestamp.ToString() + "|" +
                    PathwayName.ToString()
                ).GetHashCode();
        }
    }
}
