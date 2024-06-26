﻿using FluentNHibernate.Mapping;
using Pathway.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Mapping
{
    public class PvScLStatMap : ClassMap<PvScLStatEntity>
    {
        public PvScLStatMap()
        {
            Table("pvsclstat");
            CompositeId()
                .KeyProperty(x => x.FromTimestamp, "FromTimestamp")
                .KeyProperty(x => x.ToTimestamp, "ToTimestamp")
                .KeyProperty(x => x.PathwayName, "PathwayName")
                .KeyProperty(x => x.ScName, "ScName")
                .KeyProperty(x => x.ScLmName, "ScLmName");
            Map(x => x.SFiller).Length(1);
            Map(x => x.QIReqCnt);
            Map(x => x.QIWaits);
            Map(x => x.QIMaxWaits);
            Map(x => x.QIAggregateWaits);
            Map(x => x.QIDynamicLinks);
            Map(x => x.IsReqCnt);
            Map(x => x.IsMaxSize);
            Map(x => x.IsAggregateSize);
            Map(x => x.IsIOs);
            Map(x => x.IrReqCnt);
            Map(x => x.IrMaxSize);
            Map(x => x.IrAggregateSize);
            Map(x => x.IrIOs);
            Map(x => x.TotalMeasCount);
            Map(x => x.AvgResp);
            Map(x => x.MaxResp);
            Map(x => x.MinResp);
            Map(x => x.SumOfSquares);
        }
    }
}
