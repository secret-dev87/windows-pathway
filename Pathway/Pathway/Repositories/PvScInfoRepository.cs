using System;
using System.Collections.Generic;
using System.Configuration;
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
            var serverProcessCount = new List<CPUView>();
            int responseTime = Convert.ToInt32(ConfigurationManager.AppSettings["ResponseTime"]);

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var sctstats = session.Query<PvScTStatEntity>()
                    .Where(x => x.PathwayName == pathwayName && (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp))
                    .GroupBy(x => new { x.PathwayName, x.ScName })
                    .Select(g => new
                    {
                        ScName = g.Key.ScName,
                        AvgResp = g.Average(a => a.AvgResp) / responseTime
                    })
                    .ToList();

                foreach(var sctstat in sctstats)
                {
                    var cpuBusy = new CPUView
                    {
                        CPUNumber = sctstat.ScName.ToString(),
                        DoubleValue = Convert.ToDouble(sctstat.AvgResp)
                    };

                    serverProcessCount.Add(cpuBusy);
                }
            }

            return serverProcessCount;
        }

        public List<CPUView> GetServerCPUBusy(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, int topCount = 0)
        {
            var serverCPUBusy = new List<CPUView>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var scprStuses = session.Query<PvScPrStusEntity>()
                    .Where(x => x.PathwayName == pathwayName && (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp))
                    .GroupBy(x => new { x.PathwayName, x.ScName })
                    .Select(g => new
                    {
                        PathwayName = g.Key.PathwayName,
                        ScName = g.Key.ScName,
                        SumOfDeltaProcTime = g.Sum(a => a.DeltaProcTime)
                    })
                    .OrderByDescending(a => a.SumOfDeltaProcTime)
                    .ToList();

                if (topCount > 0)
                {
                    scprStuses = scprStuses.Take(topCount).ToList();
                }

                foreach(var scprStus in scprStuses)
                {
                    var cpuBusy = new CPUView
                    {
                        CPUNumber = scprStus.ScName.ToString(),
                        Value = Convert.ToInt64(scprStus.SumOfDeltaProcTime)
                    };
                    serverCPUBusy.Add(cpuBusy);
                }
            }

            return serverCPUBusy;
        }

        public List<CPUView> GetServerProcessCount(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, int topCount = 0)
        {
            var serverCPUBusy = new List<CPUView>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var derivedTbl = session.Query<PvScPrStusEntity>()
                    .Where(x => x.PathwayName == pathwayName && (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp))
                    .GroupBy(x => new { x.PathwayName, x.ScName, x.FromTimestamp })
                    .Select(g => new
                    {
                        PathwayName = g.Key.PathwayName,
                        ScName = g.Key.ScName,
                        ScProcessCount = g.Count(a => a.ScProcessName != null)
                    })
                    .ToList();

                var scprStuses = derivedTbl
                    .GroupBy(x => new { x.PathwayName, x.ScName })
                    .Select(g => new
                    {
                        AvgofScProcessCount = g.Average(a => a.ScProcessCount),
                        MaxofScProcessCount = g.Max(a => a.ScProcessCount),
                        PathwayName = g.Key.PathwayName,
                        ScName = g.Key.ScName
                    })
                    .OrderByDescending(x => x.AvgofScProcessCount)
                    .ToList();

                if (topCount > 0)
                {
                    scprStuses = scprStuses.Take(topCount).ToList();
                }

                foreach (var scprStus in scprStuses)
                {
                    var cpuBusy = new CPUView
                    {
                        CPUNumber = scprStus.ScName.ToString(),
                        Value = Convert.ToInt64(scprStus.AvgofScProcessCount),
                        Value2 = Convert.ToInt64(scprStus.MaxofScProcessCount)
                    };
                    serverCPUBusy.Add(cpuBusy);
                }
            }

            return serverCPUBusy;
        }

        public List<ServerCPUBusyView> GetServerTransactions(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName)
        {
            var transactions = new List<ServerCPUBusyView>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var query1 = session.Query<PvScTStatEntity>()
                    .Where(x => x.PathwayName == pathwayName && (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp))
                    .GroupBy(x => new { x.PathwayName, x.ScName })
                    .Select(g => new
                    {
                        SumOfIsReqcnt = g.Sum(a => a.IrReqCnt),
                        PathwayName = g.Key.PathwayName,
                        ScName = g.Key.ScName
                    });

                var query2 = session.Query<PvScLStatEntity>()
                    .Where(x => x.PathwayName == pathwayName && (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp))
                    .GroupBy(x => new { x.PathwayName, x.ScName })
                    .Select(g => new
                    {
                        SumOfIsReqcnt2 = g.Sum(a => a.IrReqCnt),
                        PathwayName = g.Key.PathwayName,
                        ScName = g.Key.ScName
                    });

                var result = from q1 in query1.ToList()
                             join q2 in query2.ToList() on new { q1.PathwayName, q1.ScName } equals new { q2.PathwayName, q2.ScName } into q2s
                             from q2 in q2s.DefaultIfEmpty()
                             select new
                             {
                                 TotalSum = (q1.SumOfIsReqcnt + (q2?.SumOfIsReqcnt2 ?? 0)),
                                 SumOfIsReqcntT = q1.SumOfIsReqcnt,
                                 SumOfIsReqcntL = q2 != null ? q2.SumOfIsReqcnt2 : 0,
                                 ScName = q1.ScName ?? q2?.ScName
                             };
                
                var resultList = result.OrderByDescending(x => x.TotalSum).ToList();

                foreach(var res in resultList)
                {
                    var transaction = new ServerCPUBusyView
                    {
                        ServerClass = res.ScName.ToString(),
                        TotalTransaction = Convert.ToInt64(res.TotalSum),
                        TcpTransaction = Convert.ToInt64(res.SumOfIsReqcntT),
                        LinkmonTransaction = Convert.ToInt64(res.SumOfIsReqcntL)
                    };

                    transactions.Add(transaction);
                }
            }
            return transactions;
        }

        public List<ServerCPUBusyView> GetServerTransactionsPerInterval(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName)
        {
            var transactions = new List<ServerCPUBusyView>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var query1 = session.Query<PvScTStatEntity>()
                    .Where(x => x.PathwayName == pathwayName && (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp))
                    .GroupBy(x => new { x.PathwayName, x.ScName, x.FromTimestamp })
                    .Select(g => new
                    {
                        SumOfIsReqcnt = g.Sum(a => a.IrReqCnt),
                        PathwayName = g.Key.PathwayName,
                        ScName = g.Key.ScName,
                        FromTimestamp = g.Key.FromTimestamp
                    });

                var query2 = session.Query<PvScLStatEntity>()
                    .Where(x => x.PathwayName == pathwayName && (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp))
                    .GroupBy(x => new { x.PathwayName, x.ScName })
                    .Select(g => new
                    {
                        SumOfIsReqcnt2 = g.Sum(a => a.IrReqCnt),
                        PathwayName = g.Key.PathwayName,
                        ScName = g.Key.ScName
                    });

                var result = from q1 in query1.ToList()
                             join q2 in query2.ToList() on new { q1.PathwayName, q1.ScName } equals new { q2.PathwayName, q2.ScName } into q2s
                             from q2 in q2s.DefaultIfEmpty()
                             select new
                             {
                                 TotalSum = (q1.SumOfIsReqcnt + (q2?.SumOfIsReqcnt2 ?? 0)),
                                 SumOfIsReqcntT = q1.SumOfIsReqcnt,
                                 SumOfIsReqcntL = q2 != null ? q2.SumOfIsReqcnt2 : 0,
                                 ScName = q1.ScName ?? q2?.ScName,
                                 FromTimestamp = q1.FromTimestamp
                             };

                var resultList = result.OrderByDescending(x => x.TotalSum).ToList();

                foreach (var res in resultList)
                {
                    var transaction = new ServerCPUBusyView
                    {
                        ServerClass = res.ScName.ToString(),
                        TotalTransaction = Convert.ToInt64(res.TotalSum),
                        TcpTransaction = Convert.ToInt64(res.SumOfIsReqcntT),
                        LinkmonTransaction = Convert.ToInt64(res.SumOfIsReqcntL),
                        FromTimestamp = Convert.ToDateTime(res.FromTimestamp)
                    };

                    transactions.Add(transaction);
                }
            }
            return transactions;
        }
    }
}
