using FluentNHibernate.Mapping;
using Pathway.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Mapping
{
    public class PvScAssignMap : ClassMap<PvScAssignEntity>
    {
        public PvScAssignMap()
        {
            Table("pvscassign");
            CompositeId()
                .KeyProperty(x => x.FromTimestamp, "FromTimestamp")
                .KeyProperty(x => x.ToTimestamp, "ToTimestamp")
                .KeyProperty(x => x.PathwayName, "PathwayName")
                .KeyProperty(x => x.ScName, "ScName")
                .KeyProperty(x => x.LogicalFile, "LogicalFile");
            Map(x => x.GuardianFile).Length(35);
        }
    }
}
