using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Entity
{
    public class PvTcpStatEntity
    {
        public virtual DateTime FromTimestamp { get; set; }
        public virtual DateTime ToTimestamp { get; set; }
        public virtual string PathwayName { get; set; }
        public virtual string TcpName { get; set; }
        public virtual double PtSize { get; set; }
        public virtual double PtReqCnt { get; set; }
        public virtual double PtMaxAlloc { get; set; }
        public virtual double PtAggregateAlloc { get; set; }
        public virtual double PtCurAlloc { get; set; }
        public virtual double PtMaxReq { get; set; }
        public virtual double PtAggregateReqSize { get; set; }
        public virtual double PsSize { get; set; }
        public virtual double PsReqCnt { get; set; }
        public virtual double PsMaxAlloc { get; set; }
        public virtual double PsAggregateAlloc { get; set; }
        public virtual double PsCurAlloc { get; set; }
        public virtual double PsMaxReq { get; set; }
        public virtual double PsAggregateReqSize { get; set; }
        public virtual double AdSize { get; set; }
        public virtual double AdMaxAlloc { get; set; }
        public virtual double AdCurAlloc { get; set; }
        public virtual double AcSize { get; set; }
        public virtual double AcReqCnt { get; set; }
        public virtual double AcMaxAlloc { get; set; }
        public virtual double AcAggregateAlloc { get; set; }
        public virtual double AcCurAlloc { get; set; }
        public virtual double AcMaxReq { get; set; }
        public virtual double AcAggregateReqSize { get; set; }
        public virtual double AcAbsent { get; set; }
        public virtual double QTReqCnt { get; set; }
        public virtual double QTWaits { get; set; }
        public virtual double QTMaxWaits { get; set; }
        public virtual double QTAggregateWaits { get; set; }
        public virtual double QSReqCnt { get; set; }
        public virtual double QSWaits { get; set; }
        public virtual double QSMaxWaits { get; set; }
        public virtual double QSAggregateWaits { get; set; }
        public virtual double QMReqCnt { get; set; }
        public virtual double QMWaits { get; set; }
        public virtual double QMMaxWaits { get; set; }
        public virtual double QMAggregateWaits { get; set; }
        public virtual double QLReqCnt { get; set; }
        public virtual double QLWaits { get; set; }
        public virtual double QLMaxWaits { get; set; }
        public virtual double QLAggregateWaits { get; set; }
        public virtual double QDReqCnt { get; set; }
        public virtual double QDWaits { get; set; }
        public virtual double QDMaxWaits { get; set; }
        public virtual double QDAggregateWaits { get; set; }
        public virtual double QCReqCnt { get; set; }
        public virtual double QCWaits { get; set; }
        public virtual double QCMaxWaits { get; set; }
        public virtual double QCAggregateWaits { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            PvTcpStatEntity other = (PvTcpStatEntity)obj;

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
