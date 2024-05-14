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
using Pathway.Core.Infrastructure.PerPathway.Tcp;

namespace Pathway.Core.Repositories
{
    internal class PvTcpStusRepository : IPvTcpStusRepository
    {
        public PvTcpStusRepository() { }

        public List<ServerQueueTcpSubView> GetQueueTCPSub(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName)
        {
            var tcpqueued = new List<ServerQueueTcpSubView>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var result = session.Query<PvTcpStatEntity>()
                    .Where(x => x.QLReqCnt > 0 &&
                                x.FromTimestamp >= fromTimestamp &&
                                x.ToTimestamp <= toTimestamp &&
                                x.PathwayName == pathwayName)
                    .Select(x => new { x.FromTimestamp, x.TcpName, x.QLReqCnt, x.QLWaits })
                    .OrderBy(x => new { x.TcpName, x.FromTimestamp })
                    .ToList();

                foreach (var res in result)
                {
                    var view = new ServerQueueTcpSubView
                    {
                        TcpName = res.TcpName.ToString(),
                        RequestCount = Convert.ToInt64(res.QLReqCnt),
                        Time = Convert.ToDateTime(res.FromTimestamp),
                        PercentWait = Convert.ToInt64(res.QLWaits)
                    };
                    tcpqueued.Add(view);
                }
            }

            return tcpqueued;
        }

        public long GetTcpBusy(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName)
        {
            long tcpBusy = 0;

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var result = session.Query<PvTcpStusEntity>()
                    .Where(x => x.PathwayName == pathwayName && (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp))
                    .Sum(x => x.DeltaProcTime);

                tcpBusy = Convert.ToInt64(result);
            }

            return tcpBusy;
        }

        public List<CPUView> GetTcpBusyPerCPU(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName)
        {
            var tcpProcessBusy = new List<CPUView>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var result = session.Query<PvTcpStusEntity>()
                    .Where(x => x.PathwayName == pathwayName && (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp))
                    .GroupBy(x => x.TcpCpu)
                    .Select(g => new
                    {
                        TotalBusyTime = g.Sum(a => a.DeltaProcTime),
                        TcpCpu = g.Key
                    })
                    .OrderBy(x => x.TcpCpu)
                    .ToList();

                foreach(var res in result)
                {
                    tcpProcessBusy.Add(new CPUView
                    {
                        CPUNumber = res.TcpCpu.ToString(),
                        Value = Convert.ToInt64(res.TotalBusyTime)
                    });
                }
            }

            return tcpProcessBusy;
        }

        public Dictionary<string, List<CPUView>> GetTcpBusyPerCPUPerPathway(DateTime fromTimestamp, DateTime toTimestamp)
        {
            var views = new Dictionary<string, List<CPUView>>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var result = session.Query<PvTcpStusEntity>()
                    .Where(x => (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp) && x.TcpCpu != "" && x.TcpCpu != "\0\0")
                    .GroupBy(x => new { x.TcpCpu, x.PathwayName })
                    .Select(g => new
                    {
                        TcpCpu = g.Key.TcpCpu,
                        PathwayName = g.Key.PathwayName,
                        TotalBusyTime = g.Sum(a => a.DeltaProcTime)
                    })
                    .OrderBy(x => x.TcpCpu)
                    .ToList();
                
                foreach(var res in result)
                {
                    var view = new CPUView
                    {
                        CPUNumber = res.PathwayName.ToString(),
                        Value = Convert.ToInt64(res.TotalBusyTime)
                    };

                    string cpuNumber = res.TcpCpu.ToString();

                    if (!views.ContainsKey(cpuNumber))
                        views.Add(cpuNumber, new List<CPUView> { view });
                    else
                        views[cpuNumber].Add(view);
                }
            }

            return views;
        }

        public List<CPUView> GetTcpBusyPerPathway(DateTime fromTimestamp, DateTime toTimestamp)
        {
            var tcpBusy = new List<CPUView>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var result = session.Query<PvTcpStusEntity>()
                    .Where(x => x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp)
                    .GroupBy(x => x.PathwayName)
                    .Select(g => new
                    {
                        TotalBusyTime = g.Sum(a => a.DeltaProcTime),
                        PathwayName = g.Key
                    })
                    .ToList();

                foreach(var res in result)
                {
                    var cpuView = new CPUView
                    {
                        CPUNumber = res.PathwayName.ToString(),
                        Value = Convert.ToInt64(res.TotalBusyTime)
                    };

                    tcpBusy.Add(cpuView);
                }
            }

            return tcpBusy;
        }

        public List<TcpQueuedTransactionView> GetTcpQueuedTransactions(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName)
        {
            var tcpqueued = new List<TcpQueuedTransactionView>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var subQuery1 = session.Query<PvTcpStatEntity>()
                    .Where(x => x.QLReqCnt > 0 &&
                                x.PathwayName == pathwayName &&
                                x.FromTimestamp >= fromTimestamp &&
                                x.ToTimestamp <= toTimestamp)
                    .GroupBy(x => new { x.PathwayName, x.TcpName })
                    .Select(g => new
                    {
                        g.Key.PathwayName,
                        g.Key.TcpName,
                        CountofCollectionTime = g.Count(),
                        MaxofQLReqCnt = g.Max(x => x.QLReqCnt)
                    });

                var subQuery2 = session.Query<PvTcpStatEntity>()
                    .Where(x => x.QLReqCnt > 0 &&
                                x.PathwayName == pathwayName &&
                                x.FromTimestamp >= fromTimestamp &&
                                x.ToTimestamp <= toTimestamp)
                    .GroupBy(x => new { x.PathwayName, x.TcpName, x.QLReqCnt })
                    .Select(g => new
                    {
                        g.Key.PathwayName,
                        g.Key.TcpName,
                        g.Key.QLReqCnt,
                        CollectionTime = g.Min(x => x.FromTimestamp)
                    });

                var result = from sq1 in subQuery1
                             join sq2 in subQuery2
                             on new { sq1.PathwayName, sq1.TcpName, QLReqCnt = sq1.MaxofQLReqCnt } equals new { sq2.PathwayName, sq2.TcpName, sq2.QLReqCnt }
                             orderby sq1.TcpName
                             select new
                             {
                                 sq1.TcpName,
                                 sq1.CountofCollectionTime,
                                 sq1.MaxofQLReqCnt,
                                 sq2.CollectionTime
                             };

                var finalResults = result.ToList();

                foreach(var res in finalResults)
                {
                    var view = new TcpQueuedTransactionView
                    {
                        Tcp = res.TcpName.ToString(),
                        PeakQueueLength = Convert.ToInt64(res.MaxofQLReqCnt),
                        PeakTime = Convert.ToDateTime(res.CollectionTime),
                        Instances = Convert.ToInt64(res.CountofCollectionTime)
                    };

                    tcpqueued.Add(view);
                }
            }

            return tcpqueued;
        }
    }
}
