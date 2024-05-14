using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pathway.Core.Abstract.Repositories;
using Pathway.Core.Concrete;
using Pathway.Core.Entity;
using Pathway.Core.Helper;
using Pathway.Core.Infrastructure;
using Pathway.Core.Infrastructure.PerPathway.Server;

namespace Pathway.Core.Repositories
{
    internal class PvScTStatRepository : IPvScTStatRepository
    {
        public PvScTStatRepository() { }

        public List<ServerQueueTcpSubView> GetQueueTCPSub(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName)
        {
            var queueTCPSub = new List<ServerQueueTcpSubView>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var result = session.Query<PvScTStatEntity>()
                    .Where(x => (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp) && x.QIWaits > 0 && x.PathwayName == pathwayName)
                    .Select(x => new
                    {
                        x.FromTimestamp,
                        x.ScName,
                        x.ScTcpName,
                        x.PathwayName,
                        x.QIReqCnt,
                        QueueWait = (x.QIWaits / x.QIReqCnt),
                        PercentDynamic = (x.QIDynamicLinks / x.QIReqCnt),
                        MaximumWaits = x.QIMaxWaits,
                        AverageWaits = (x.QIAggregateWaits / x.QIReqCnt),
                        SentRequestCount = x.IsReqCnt
                    })
                    .ToList();

                foreach (var res in result)
                {
                    var tcpsub = new ServerQueueTcpSubView
                    {
                        Time = Convert.ToDateTime(res.FromTimestamp),
                        ServerClass = res.ScName.ToString(),
                        TcpName = res.ScTcpName.ToString(),
                        RequestCount = Convert.ToInt64(res.QIReqCnt),
                        PercentWait = Convert.ToDouble(res.QueueWait) * 100,
                        PercentDynamic = Convert.ToDouble(res.PercentDynamic) * 100,
                        MaxWaits = Convert.ToInt64(res.MaximumWaits),
                        AvgWaits = Convert.ToDouble(res.AverageWaits),
                        SentRequestCount = Convert.ToInt64(res.SentRequestCount)
                    };

                    queueTCPSub.Add(tcpsub);
                }
            }

            return queueTCPSub;
        }

        public List<CPUView> GetServerCPUBusyAvgRt(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName)
        {
            var serverCPUBusy = new List<CPUView>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var result = session.Query<PvScTStatEntity>()
                    .Where(x => x.PathwayName == pathwayName && (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp))
                    .GroupBy(x => new { x.PathwayName, x.ScName })
                    .Select(g => new
                    {
                        AvgResp = g.Average(x => x.AvgResp),
                        MaxResp = g.Max(a => a.MaxResp),
                        MinResp = g.Min(a => a.MinResp),
                        PathwayName = g.Key.PathwayName,
                        ScName = g.Key.ScName
                    })
                    .ToList();

                foreach (var res in result)
                {
                    var cpuBusy = new CPUView
                    {
                        CPUNumber = res.ScName.ToString(),
                        Value = Convert.ToInt64(res.AvgResp)
                    };

                    serverCPUBusy.Add(cpuBusy);
                }
            }

            return serverCPUBusy;
        }

        public List<CPUView> GetServerCPUBusyTcpTrans(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName)
        {
            var serverCPUBusy = new List<CPUView>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var result = session.Query<PvScTStatEntity>()
                    .Where(x => x.PathwayName == pathwayName && (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp))
                    .GroupBy(x => new { x.PathwayName, x.ScName })
                    .Select(g => new
                    {
                        SumOfIsReqcnt = g.Sum(a => a.IrReqCnt),
                        PathwayName = g.Key.PathwayName,
                        ScName = g.Key.ScName
                    })
                    .ToList();

                foreach(var res in result)
                {
                    var cpuBusy = new CPUView
                    {
                        CPUNumber = res.ScName.ToString(),
                        Value = Convert.ToInt64(res.SumOfIsReqcnt)
                    };

                    serverCPUBusy.Add(cpuBusy);
                }
            }

            return serverCPUBusy;
        }

        public List<ServerQueueTcpView> GetServerQueueTCP(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName)
        {
            var ququeTCP = new List<ServerQueueTcpView>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var subQuery1 = session.Query<PvScTStatEntity>()
                    .Where(x => x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp &&
                                x.QIWaits > 0 && x.PathwayName == pathwayName)
                    .GroupBy(x => new { x.PathwayName, x.ScName })
                    .Select(g => new
                    {
                        PathwayName = g.Key.PathwayName,
                        ScName = g.Key.ScName,
                        CountofCollectionTime = g.Count(),
                        MaxofQIReqCnt = g.Max(x => x.QIReqCnt)
                    });

                var subQuery2 = session.Query<PvScTStatEntity>()
                    .Where(x => x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp &&
                                x.QIWaits > 0 && x.PathwayName == pathwayName)
                    .GroupBy(x => new { x.PathwayName, x.ScName, x.QIReqCnt })
                    .Select(g => new
                    {
                        PathwayName = g.Key.PathwayName,
                        ScName = g.Key.ScName,
                        QIReqCnt = g.Key.QIReqCnt,
                        ScTcpName = g.Min(x => x.ScTcpName),
                        CollectionTime = g.Min(x => x.FromTimestamp)
                    });

                var finalQuery = from q1 in subQuery1
                                 join q2 in subQuery2
                                 on new { q1.PathwayName, q1.ScName, QIReqCnt = q1.MaxofQIReqCnt } equals new { q2.PathwayName, q2.ScName, q2.QIReqCnt }
                                 orderby q1.MaxofQIReqCnt descending
                                 select new
                                 {
                                     q1.ScName,
                                     q1.CountofCollectionTime,
                                     q1.MaxofQIReqCnt,
                                     q2.CollectionTime,
                                     q2.ScTcpName
                                 };

                var results = finalQuery.ToList();

                foreach (var res in results)
                {
                    var tcp = new ServerQueueTcpView
                    {
                        Server = res.ScName.ToString(),
                        PeakQueueLength = Convert.ToInt64(res.MaxofQIReqCnt),
                        Tcp = res.ScTcpName.ToString(),
                        PeakTime = Convert.ToDateTime(res.CollectionTime),
                        Instances = Convert.ToInt64(res.CountofCollectionTime),
                    };

                    ququeTCP.Add(tcp);
                }
            }

            return ququeTCP;
        }

        public long GetServerToTcp(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName)
        {
            long serverToTcp = 0;

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var result = session.Query<PvScTStatEntity>()
                    .Where(x => x.PathwayName == pathwayName && (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp))
                    .GroupBy(x => x.PathwayName)
                    .Select(g => new
                    {
                        TotalIsReqCnt = g.Sum(a => a.IsReqCnt),
                        PathwayName = g.Key
                    })
                    .First();

                serverToTcp = Convert.ToInt64(result.TotalIsReqCnt);
            }

            return serverToTcp;
        }

        public long GetTcpToServer(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName)
        {
            long tcpToServer = 0;

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var result = session.Query<PvScTStatEntity>()
                    .Where(x => x.PathwayName == pathwayName && (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp))
                    .GroupBy(x => x.PathwayName)
                    .Select(g => new
                    {
                        TotalIrReqCnt = g.Sum(a => a.IrReqCnt),
                        PathwayName = g.Key
                    })
                    .First();

                tcpToServer = Convert.ToInt64(result.TotalIrReqCnt);
            }

            return tcpToServer;
        }

        public Dictionary<string, TcpToServer> GetTcpToServer(DateTime fromTimestamp, DateTime toTimestamp)
        {
            var serverToTcp = new Dictionary<string, TcpToServer>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var result = session.Query<PvScTStatEntity>()
                    .Where(x => x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp)
                    .GroupBy(x => x.PathwayName)
                    .Select(g => new
                    {
                        TotalIsReqCnt = g.Sum(a => a.IsReqCnt),
                        PathwayName = g.Key,
                        PeakReqCnt = g.Max(a => a.IsReqCnt),
                        AverageReqCnt = g.Average(a => a.IsReqCnt)
                    })
                    .ToList();

                foreach(var res in result)
                {
                    serverToTcp.Add(res.PathwayName.ToString(),
                        new TcpToServer
                        {
                            TotalIsReqCnt = Convert.ToInt64(Convert.ToDouble(res.TotalIsReqCnt.ToString())),
                            PeakReqCnt = Convert.ToInt64(Convert.ToDouble(res.PeakReqCnt.ToString())),
                            AverageReqCnt = Convert.ToInt64(Convert.ToDouble(res.AverageReqCnt.ToString()))
                        });
                }
            }    

            return serverToTcp;
        }

        public List<string> GetUnusedServerList(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName)
        {
            var unusedClass = new List<string>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var query1 = session.Query<PvScTStatEntity>()
                    .Where(x => x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp && x.PathwayName == pathwayName)
                    .GroupBy(x => new { x.ScName, x.PathwayName })
                    .Select(g => new
                    {
                        ScName = g.Key.ScName,
                        PathwayName = g.Key.PathwayName,
                        SumOfIsReqCnt = g.Sum(x => x.IsReqCnt)
                    });

                var query2 = session.Query<PvScLStatEntity>()
                    .Where(x => x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp && x.PathwayName == pathwayName)
                    .GroupBy(x => new { x.ScName, x.PathwayName })
                    .Select(g => new
                    {
                        ScName = g.Key.ScName,
                        PathwayName = g.Key.PathwayName,
                        SumOfIsReqCnt = g.Sum(x => x.IsReqCnt)
                    });

                var results = query1.ToList().Union(query2.ToList())
                    .GroupBy(x => new { x.ScName, x.PathwayName })
                    .Select(g => new
                    {
                        ScName = g.Key.ScName,
                        PathwayName = g.Key.PathwayName,
                        Tran1 = g.Max(x => x.SumOfIsReqCnt)
                    })
                    .Where(x => x.Tran1 == 0)
                    .OrderBy(x => x.ScName)
                    .Select(x => x.ScName)
                    .ToList();

                foreach(var res in results)
                {
                    unusedClass.Add(res.ToString());
                }
            }

            return unusedClass;
        }
    }
}
