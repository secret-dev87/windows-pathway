using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Entity
{
    public class PvPwyManyEntity
    {
        public virtual DateTime FromTimestamp { get; set; }
        public virtual DateTime ToTimestamp { get; set; }
        public virtual string PathwayName { get; set; }
        public virtual string PFiller { get; set; }
        public virtual double PwyStatusTime { get; set; }
        public virtual double PwyProcessTime { get; set; }
        public virtual double DeltaTime { get; set; }
        public virtual double? DeltaProcTime { get; set; }
        public virtual string PwyCpu { get; set; }
        public virtual double CurPages { get; set; }
        public virtual double RecQueue { get; set; }
        public virtual double PageFaults { get; set; }
        public virtual int TermRunning { get; set; }
        public virtual int TermStopped { get; set; }
        public virtual int TermPending { get; set; }
        public virtual int TermSuspended { get; set; }
        public virtual int TcpRunning { get; set; }
        public virtual int TcpStopped { get; set; }
        public virtual int TcpPending { get; set; }
        public virtual int ScRunning { get; set; }
        public virtual int ScStopped { get; set; }
        public virtual int ScThawed { get; set; }
        public virtual int ScFrozen { get; set; }
        public virtual int ScFreezePending { get; set; }
        public virtual int SpRunning { get; set; }
        public virtual int SpStopped { get; set; }
        public virtual int SpPending { get; set; }
        public virtual int Pathcom { get; set; }
        public virtual int Linkmon { get; set; }
        public virtual int ExternalTcp { get; set; }
        public virtual int Spi { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            PvPwyManyEntity other = (PvPwyManyEntity)obj;

            if (other == null)
                return false;
            if (FromTimestamp == other.FromTimestamp && ToTimestamp == other.ToTimestamp && PathwayName == other.PathwayName)
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
