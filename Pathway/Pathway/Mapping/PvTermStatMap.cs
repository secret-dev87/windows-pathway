using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using Pathway.Core.Entity;

namespace Pathway.Core.Mapping
{
    public class PvTermStatMap : ClassMap<PvTermStatEntity>
    {
        public PvTermStatMap()
        {
            Table("pvtermstat");
            CompositeId()
                .KeyProperty(x => x.FromTimestamp, "FromTimestamp")
                .KeyProperty(x => x.ToTimestamp, "ToTimestamp")
                .KeyProperty(x => x.PathwayName, "PathwayName")
                .KeyProperty(x => x.TermName, "TermName");
            Map(x => x.IdReqCnt);
            Map(x => x.IdMaxSize);
            Map(x => x.IdAggregateSize);
            Map(x => x.IdIOs);
            Map(x => x.IaReqCnt);
            Map(x => x.IaMaxSize);
            Map(x => x.IaAggregateSize);
            Map(x => x.IaIOs);
            Map(x => x.IsReqCnt);
            Map(x => x.IsMaxSize);
            Map(x => x.IsAggregateSize);
            Map(x => x.IsIOs);
            Map(x => x.IrReqCnt);
            Map(x => x.IrMaxSize);
            Map(x => x.IrAggregateSize);
            Map(x => x.IrIOs);
            Map(x => x.IcReqCnt);
            Map(x => x.IcMaxSize);
            Map(x => x.IcAggregateSize);
            Map(x => x.IcIOs);
            Map(x => x.AdMaxSize);
            Map(x => x.AdCurSize);
            Map(x => x.AcMaxSize);
            Map(x => x.AcCurSize);
            Map(x => x.AcReqCnt);
            Map(x => x.AcAggregateReqSize);
            Map(x => x.TotalMeasCount);
            Map(x => x.AvgResp);
            Map(x => x.MaxResp);
            Map(x => x.MinResp);
            Map(x => x.SumOfSquares);
            Map(x => x.TermTcpName).Length(15);
        }
    }
}
