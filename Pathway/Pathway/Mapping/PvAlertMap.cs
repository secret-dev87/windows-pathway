using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using Pathway.Core.Entity;

namespace Pathway.Core.Mapping
{
    public class PvAlertMap : ClassMap<PvAlertEntity>
    {
        public PvAlertMap()
        {
            Table("pvalerts");
            CompositeId()
                .KeyProperty(x => x.FromTimestamp, "FromTimestamp")
                .KeyProperty(x => x.ToTimestamp, "ToTimestamp")
                .KeyProperty(x => x.Pathway, "Pathway");
            Map(x => x.TermHiMaxRT);
            Map(x => x.TermHiAvgRT);
            Map(x => x.TermUnused);
            Map(x => x.TermErrorList);
            Map(x => x.TCPQueuedTransactions);
            Map(x => x.TCPLowTermPool);
            Map(x => x.TCPLowServerPool);
            Map(x => x.TCPUnused);
            Map(x => x.TCPErrorList);
            Map(x => x.ServerHiMaxRT);
            Map(x => x.ServerHiAvgRT);
            Map(x => x.ServerQueueTCP);
            Map(x => x.ServerQueueLinkmon);
            Map(x => x.ServerUnusedClass);
            Map(x => x.ServerUnusedProcess);
            Map(x => x.ServerErrorList);
        }
    }
}
