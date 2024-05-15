using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Entity
{
    public class PvTcpStusEntity
    {
        public virtual DateTime FromTimestamp { get; set; }
        public virtual DateTime ToTimestamp { get; set; }
        public virtual string PathwayName { get; set; }
        public virtual string TcpName { get; set; }
        public virtual int ErrorNumber { get; set; }
        public virtual int ErrorInfo { get; set; }
        public virtual int Pid { get; set; }
        public virtual int PrimaryPin { get; set; }
        public virtual int BackupPin { get; set; }
        public virtual double TcpStatusTime { get; set; }
        public virtual double TcpProcessTime { get; set; }
        public virtual double DeltaTime { get; set; }
        public virtual double? DeltaProcTime { get; set; }
        public virtual string TcpCpu { get; set; }
        public virtual double CurPages { get; set; }
        public virtual double RecQueue { get; set; }
        public virtual double PageFaults { get; set; }
        public virtual string Procname { get; set; }
        public virtual string State { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            PvTcpStusEntity other = (PvTcpStusEntity)obj;

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
