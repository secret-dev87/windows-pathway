using FluentNHibernate.Mapping;
using Pathway.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Mapping
{
    public class PathwayAllAlertsMap : ClassMap<PathwayAllAlertsEntity>
    {
        public PathwayAllAlertsMap()
        {
            Table("pathwayallalerts");
            Id(x => x.AlertId);
            Map(x => x.AlertType);
            Map(x => x.AlertName);
        }
    }
}
