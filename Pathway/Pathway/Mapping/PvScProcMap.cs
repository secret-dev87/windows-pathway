using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using Pathway.Core.Entity;

namespace Pathway.Core.Mapping
{
    public class PvScProcMap : ClassMap<PvScProcEntity>
    {
        public PvScProcMap()
        {
            Table("pvscproc");
            CompositeId()
                .KeyProperty(x => x.FromTimestamp, "FromTimestamp")
                .KeyProperty(x => x.ToTimestamp, "ToTimestamp")
                .KeyProperty(x => x.PathwayName, "PathwayName")
                .KeyProperty(x => x.ScName, "ScName")
                .KeyProperty(x => x.ScProcessName, "ScProcessName");
            Map(x => x.PrimaryCpu);
            Map(x => x.BackupCpu);
            Map(x => x.ScPriority);
            Map(x => x.ProcAssociative).Length(1);
            Map(x => x.Debug).Length(1);
            Map(x => x.GuardianSwap).Length(35);
            Map(x => x.HomeTermSc).Length(35);
        }
    }
}
