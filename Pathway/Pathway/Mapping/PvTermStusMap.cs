using FluentNHibernate.Mapping;
using Pathway.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Mapping
{
    public class PvTermStusMap : ClassMap<PvTermStusEntity>
    {
        public PvTermStusMap()
        {
            Table("pvtermstus");
            CompositeId()
                .KeyProperty(x => x.FromTimestamp, "FromTimestamp")
                .KeyProperty(x => x.ToTimestamp, "ToTimestamp")
                .KeyProperty(x => x.PathwayName, "PathwayName")
                .KeyProperty(x => x.TermName, "TermName");
            Map(x => x.ErrorNumber);
            Map(x => x.ErrorInfo);
            Map(x => x.TcpName);
            Map(x => x.FileName);
            Map(x => x.State);
        }
    }
}
