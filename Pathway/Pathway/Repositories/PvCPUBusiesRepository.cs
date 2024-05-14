using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pathway.Core.Abstract.Repositories;
using Pathway.Core.Entity;
using Pathway.Core.Helper;

namespace Pathway.Core.Repositories
{
    internal class PvCPUBusiesRepository : IPvCPUBusiesRepository
    {
        public void InsertCPUBusy(string pathwayName, DateTime fromTimestamp, DateTime toTimestamp, double cpuBusy)
        {
            using(var session = NHibernateHelper.OpenSystemSession())
            {
                using(var transaction = session.BeginTransaction())
                {
                    var pvCPUBusy = new PvCPUBusyEntity
                    {
                        FromTimestamp = fromTimestamp,
                        ToTimestamp = toTimestamp,
                        Pathway = pathwayName,
                        CPUBusy = cpuBusy
                    };

                    session.SaveOrUpdate(pvCPUBusy);
                    transaction.Commit();
                }
            }
            throw new NotImplementedException();
        }
    }
}
