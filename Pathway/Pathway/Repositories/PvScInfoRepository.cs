using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pathway.Core.Abstract.Repositories;
using Pathway.Core.Entity;
using Pathway.Core.Helper;
using Pathway.Core.Infrastructure;
using Pathway.Core.Infrastructure.PerPathway.Server;

namespace Pathway.Core.Repositories
{
    internal class PvScInfoRepository : IPvScInfoRepository
    {
        public PvScInfoRepository() { }
        public List<CPUView> GetNumStaticMaxServers(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, int topCount = 0)
        {
            var serverCPUBusy = new List<CPUView>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var scInfos = session.Query<PvScInfoEntity>()
                    .Where(x => x.PathwayName == pathwayName && (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp))
                    .ToList();

                if (topCount > 0)
                    scInfos = scInfos.Take(10).ToList();

                foreach(var scInfo in scInfos)
                {
                    var cpuBusy = new CPUView
                    {
                        CPUNumber = scInfo.ScName.ToString(),
                        Value = Convert.ToInt64(scInfo.NumStatic),
                        Value2 = Convert.ToInt64(scInfo.MaxServers)
                    };

                    serverCPUBusy.Add(cpuBusy);
                }
            }

            return serverCPUBusy;
        }

        public List<CPUView> GetServerAverageResponseTime(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, int topCount = 0)
        {
            throw new NotImplementedException();
        }

        public List<CPUView> GetServerCPUBusy(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, int topCount = 0)
        {
            throw new NotImplementedException();
        }

        public List<CPUView> GetServerProcessCount(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, int topCount = 0)
        {
            throw new NotImplementedException();
        }

        public List<ServerCPUBusyView> GetServerTransactions(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName)
        {
            throw new NotImplementedException();
        }

        public List<ServerCPUBusyView> GetServerTransactionsPerInterval(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName)
        {
            throw new NotImplementedException();
        }
    }
}
