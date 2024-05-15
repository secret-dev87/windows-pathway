using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using FluentNHibernate.Mapping;
using Pathway.Core.Entity;

namespace Pathway.Core.Mapping
{
    public class PvErrorMap : ClassMap<PvErrorEntity>
    {
        public PvErrorMap()
        {
            Table("pverrors");
            CompositeId()
                .KeyProperty(x => x.FromTimestamp, "FromTimestamp")
                .KeyProperty(x => x.ErrorNumber);
            Map(x => x.Message).Length(150);
            Map(x => x.Cause).Length(500);
            Map(x => x.Effect).Length(500);
            Map(x => x.Recovery).Length(500);
        }
    }
}
