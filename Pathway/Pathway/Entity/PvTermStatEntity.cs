using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Entity
{
    public class PvTermStatEntity
    {
        public virtual DateTime FromTimestamp { get; set; }
        public virtual DateTime ToTimestamp { get; set; }
        public virtual string PathwayName { get; set; }
        public virtual string TermName { get; set; }
        public virtual double IdReqCnt { get; set; }
        public virtual double IdMaxSize { get; set; }
        public virtual double IdAggregateSize { get; set; }
        public virtual double IdIOs { get; set; }
        public virtual double IaReqCnt { get; set; }
        public virtual double IaMaxSize { get; set; }
        public virtual double IaAggregateSize { get; set; }
        public virtual double IaIOs { get;set; }
        public virtual double IsReqCnt { get; set; }
        public virtual double IsMaxSize { get; set; }
        public virtual double IsAggregateSize { get; set; }
        public virtual double IsIOs { get; set; }
        public virtual double IrReqCnt { get; set; }
        public virtual double IrMaxSize { get; set; }
        public virtual double IrAggregateSize { get; set; }
        public virtual double IrIOs { get; set; }
        public virtual double IcReqCnt { get; set; }
        public virtual double IcMaxSize { get; set; }
        public virtual double IcAggregateSize { get; set; }
        public virtual double IcIOs { get; set; }
        public virtual double AdMaxSize { get; set; }
        public virtual double AdCurSize { get; set; }
        public virtual double AcMaxSize { get; set; }
        public virtual double AcCurSize { get; set; }
        public virtual double AcReqCnt { get; set; }
        public virtual double AcAggregateReqSize { get; set; }
        public virtual int TotalMeasCount { get; set; }
        public virtual double AvgResp { get; set; }
        public virtual double MaxResp { get; set; }
        public virtual double MinResp { get; set; }
        public virtual double SumOfSquares { get; set; }
        public virtual string TermTcpName { get; set; }
    }
}
