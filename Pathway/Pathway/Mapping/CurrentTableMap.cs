using FluentNHibernate.Mapping;
using Pathway.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Mapping
{
    public class CurrentTableMap : ClassMap<CurrentTableEntity>
    {
        public CurrentTableMap()
        {
            Table("currenttables");
            Id(x => x.TableName);
            Map(x => x.EntityId);
            Map(x => x.SystemSerial).Length(10);
            Map(x => x.Interval);
            Map(x => x.DataDate);
            Map(x => x.MeasureVersion).Length(3);
        }
    }
}
