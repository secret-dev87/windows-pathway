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
    internal class PvScLStatRepository : IPvScLStatRepository
    {
        public PvScLStatRepository() { }

        public long GetLinkmonToServer(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName)
        {
            long linkmonToServer = 0;

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var result = session.Query<PvScLStatEntity>()
                    .Where(x => x.PathwayName == pathwayName && (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp))
                    .GroupBy(x => x.PathwayName)
                    .Select(g => new
                    {
                        PathwayName = g.Key,
                        TotalIrReqCnt = g.Sum(a => a.IrReqCnt)
                    })
                    .First();

                linkmonToServer = Convert.ToInt64(result.TotalIrReqCnt);
            }
            
            return linkmonToServer;
        }

        public Dictionary<string, LinkMonToServer> GetLinkmonToServer(DateTime fromTimestamp, DateTime toTimestamp)
        {
            var linkmonToServer = new Dictionary<string, LinkMonToServer>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var result = session.Query<PvScLStatEntity>()
                    .Where(x => x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp)
                    .GroupBy(x => x.PathwayName)
                    .Select(g => new
                    {
                        TotalIrReqCnt = g.Sum(a => a.IrReqCnt),
                        PathwayName = g.Key,
                        PeakReqCnt = g.Max(a => a.IrReqCnt),
                        AverageReqCnt = g.Average(a => a.IrReqCnt)
                    })
                    .ToList();

                foreach(var res in result)
                {
                    linkmonToServer.Add(res.PathwayName.ToString(),
                        new LinkMonToServer
                        {
                            TotalIrReqCnt = Convert.ToInt64(res.TotalIrReqCnt),
                            PeakReqCnt = Convert.ToInt64(res.PeakReqCnt),
                            AverageReqCnt = Convert.ToInt64(res.AverageReqCnt)
                        });
                }
            }

            return linkmonToServer;
        }

        public List<ServerQueueTcpSubView> GetQueueLinkmonSub(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName)
        {
            var queueTCPSub = new List<ServerQueueTcpSubView>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var result = session.Query<PvScLStatEntity>()
                    .Where(x => (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp) && x.QIWaits > 0 && x.PathwayName == pathwayName)
                    .Select(x => new
                    {
                        x.FromTimestamp,
                        x.ScName,
                        x.ScLmName,
                        x.QIReqCnt,
                        QueueWait = x.QIWaits / x.QIReqCnt,
                        PercentDynamic = x.QIDynamicLinks / x.QIReqCnt,
                        MaximumWaits = x.QIMaxWaits,
                        AverageWaits = x.QIAggregateWaits / x.QIReqCnt,
                        SentRequestCount = x.IsReqCnt
                    })
                    .OrderBy(x => new { x.ScName, x.FromTimestamp })
                    .ToList();

                foreach(var res in result)
                {
                    var tcpSub = new ServerQueueTcpSubView
                    {
                        Time = Convert.ToDateTime(res.FromTimestamp),
                        ServerClass = res.ScName.ToString(),
                        TcpName = res.ScLmName.ToString(),
                        RequestCount = Convert.ToInt64(res.QIReqCnt),
                        PercentWait = Convert.ToDouble(res.QueueWait) * 100,
                        PercentDynamic = Convert.ToDouble(res.PercentDynamic) * 100,
                        MaxWaits = Convert.ToInt64(res.MaximumWaits),
                        AvgWaits = Convert.ToDouble(res.AverageWaits),
                        SentRequestCount = Convert.ToInt64(res.SentRequestCount)
                    };

                    queueTCPSub.Add(tcpSub);
                }
            }

            return queueTCPSub;
        }

        public List<CPUView> GetServerCPUBusyLinkmonTrans(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName)
        {
            var serverCPUBusy = new List<CPUView>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var result = session.Query<PvScLStatEntity>()
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

        public List<ServerQueueLinkmonView> GetServerQueueLinkmon(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName)
        {
            var views = new List<ServerQueueLinkmonView>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var subQuery1 = session.Query<PvScLStatEntity>()
                    .Where(x => x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp && x.QIWaits > 0 && x.PathwayName == pathwayName)
                    .GroupBy(x => new { x.PathwayName, x.ScName })
                    .Select(g => new {
                        g.Key.PathwayName,
                        g.Key.ScName,
                        CountofCollectionTime = g.Count(),
                        MaxofQIReqCnt = g.Max(x => x.QIReqCnt)
                    });

                var subQuery2 = session.Query<PvScLStatEntity>()
                    .Where(x => x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp && x.QIWaits > 0 && x.PathwayName == pathwayName)
                    .GroupBy(x => new { x.PathwayName, x.ScName, x.QIReqCnt })
                    .Select(g => new {
                        g.Key.PathwayName,
                        g.Key.ScName,
                        g.Key.QIReqCnt,
                        ScLmName = g.Min(x => x.ScLmName),
                        CollectionTime = g.Min(x => x.FromTimestamp)
                    });

                var result = from q1 in subQuery1
                             join q2 in subQuery2
                             on new { q1.PathwayName, q1.ScName, QIReqCnt = q1.MaxofQIReqCnt } equals new { q2.PathwayName, q2.ScName, q2.QIReqCnt }
                             orderby q1.MaxofQIReqCnt descending
                             select new
                             {
                                 q1.ScName,
                                 q1.CountofCollectionTime,
                                 q1.MaxofQIReqCnt,
                                 q2.CollectionTime,
                                 q2.ScLmName
                             };

                var resultList = result.ToList();

                foreach(var res in resultList)
                {
                    var view = new ServerQueueLinkmonView
                    {
                        Server = res.ScName.ToString(),
                        PeakQueueLength = Convert.ToInt64(res.MaxofQIReqCnt),
                        Linkmon = res.ScLmName.ToString(),
                        PeakTime = Convert.ToDateTime(res.CollectionTime),
                        Instances = Convert.ToInt64(res.CountofCollectionTime)
                    };

                    views.Add(view);
                }
            }

            return views;
        }

        public List<ServerUnusedServerClassView> GetServerUnusedServerClass(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName)
        {
            var views = new List<ServerUnusedServerClassView>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var query1 = session.Query<PvScTStatEntity>()
                    .Where(x => x.PathwayName == pathwayName && (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp))
                    .GroupBy(x => new { x.ScName, x.PathwayName })
                    .Select(g => new
                    {
                        ScName = g.Key.ScName,
                        PathwayName = g.Key.PathwayName,
                        SumofIsReqCnt = g.Sum(a => a.IsReqCnt)
                    });

                var query2 = session.Query<PvScLStatEntity>()
                    .Where(x => (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp) && x.PathwayName == pathwayName)
                    .GroupBy(x => new { x.ScName, x.PathwayName })
                    .Select(g => new
                    {
                        ScName = g.Key.ScName,
                        PathwayName = g.Key.PathwayName,
                        SumofIsReqCnt = g.Sum(a => a.IsReqCnt)
                    });

                var result = from q1 in query1.ToList()
                             from q2 in query2.ToList()
                             .Where(q2 => q1.ScName == q2.ScName && q1.PathwayName == q2.PathwayName)
                             .DefaultIfEmpty()
                             select new
                             {
                                 ScName = q1.ScName ?? q2?.ScName,
                                 Tran1 = q1.SumofIsReqCnt,
                                 Tran2 = q2 != null ? q2.SumofIsReqCnt : 0
                             };

                var resultList = result.ToList();

                foreach (var res in resultList)
                {
                    var view = new ServerUnusedServerClassView
                    {
                        ServerClass = res.ScName.ToString()
                    };

                    views.Add(view);
                }
            }

            return views;
        }
    }
}
