using FluentNHibernate.Mapping;
using Pathway.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Mapping
{
    public class PvTcpStatMap : ClassMap<PvTcpStatEntity>
    {
        public PvTcpStatMap()
        {
            Table("pvtcpstat");
            CompositeId()
                .KeyProperty(x => x.FromTimestamp, "FromTimestamp")
                .KeyProperty(x => x.ToTimestamp, "ToTimestamp")
                .KeyProperty(x => x.PathwayName, "PathwayName")
                .KeyProperty(x => x.TcpName, "TcpName");
            Map(x => x.PtSize);
            Map(x => x.PtReqCnt);
            Map(x => x.PtMaxAlloc);
            Map(x => x.PtAggregateAlloc);
            Map(x => x.PtCurAlloc);
            Map(x => x.PtMaxReq);
            Map(x => x.PtAggregateReqSize);
            Map(x => x.PsSize);
            Map(x => x.PsReqCnt);
            Map(x => x.PsMaxAlloc);
            Map(x => x.PsAggregateAlloc);
            Map(x => x.PsCurAlloc);
            Map(x => x.PsMaxReq);
            Map(x => x.PsAggregateReqSize);
            Map(x => x.AdSize);
            Map(x => x.AdMaxAlloc);
            Map(x => x.AdCurAlloc);
            Map(x => x.AcSize);
            Map(x => x.AcReqCnt);
            Map(x => x.AcMaxAlloc);
            Map(x => x.AcAggregateAlloc);
            Map(x => x.AcCurAlloc);
            Map(x => x.AcMaxReq);
            Map(x => x.AcAggregateReqSize);
            Map(x => x.AcAbsent);
            Map(x => x.QTReqCnt);
            Map(x => x.QTWaits);
            Map(x => x.QTMaxWaits);
            Map(x => x.QTAggregateWaits);
            Map(x => x.QSReqCnt);
            Map(x => x.QSWaits);
            Map(x => x.QSMaxWaits);
            Map(x => x.QSAggregateWaits);
            Map(x => x.QMReqCnt);
            Map(x => x.QMWaits);
            Map(x => x.QMMaxWaits);
            Map(x => x.QMAggregateWaits);
            Map(x => x.QLReqCnt);
            Map(x => x.QLWaits);
            Map(x => x.QLMaxWaits);
            Map(x => x.QLAggregateWaits);
            Map(x => x.QDReqCnt);
            Map(x => x.QDWaits);
            Map(x => x.QDMaxWaits);
            Map(x => x.QDAggregateWaits);
            Map(x => x.QCReqCnt);
            Map(x => x.QCWaits);
            Map(x => x.QCMaxWaits);
            Map(x => x.QCAggregateWaits);
        }
    }
}
