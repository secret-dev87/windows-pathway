using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using Pathway.Core.Entity;


namespace Pathway.Core.Mapping
{
    public class PathwayAlertsMap : ClassMap<PathwayAlertsEntity>
    {
        public PathwayAlertsMap()
        {
            Table("pathwayalerts");
            Id(x => x.AlertId);
            Map(x => x.AlertName);
        }
    }
}
