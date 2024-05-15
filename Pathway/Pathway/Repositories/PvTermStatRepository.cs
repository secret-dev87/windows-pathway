using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.ModelBinding;
using Pathway.Core.Abstract.Repositories;
using Pathway.Core.Entity;
using Pathway.Core.Helper;
using Pathway.Core.Infrastructure.PerPathway.Server;
using Pathway.Core.Infrastructure.PerPathway.Term;

namespace Pathway.Core.Repositories
{
    internal class PvTermStatRepository : IPvTermStatRepository
    {
        public PvTermStatRepository() { }

        public List<ServerUnusedServerClassView> GetTcpUnused(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName)
        {
            var unusedView = new List<ServerUnusedServerClassView>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var result = session.Query<PvTermStatEntity>()
                    .Where(x => x.PathwayName == pathwayName &&
                                x.FromTimestamp >= fromTimestamp &&
                                x.ToTimestamp <= toTimestamp)
                    .GroupBy(x => new { x.PathwayName, x.TermTcpName })
                    .Select(g => new
                    {
                        TermTcpName = g.Key.TermTcpName,
                        Unused = g.Count(),
                        TotalIsReqCnt = g.Sum(a => a.IsReqCnt)
                    })
                    .Where(x => x.TotalIsReqCnt == 0)
                    .OrderBy(x => x.TermTcpName)
                    .ToList();

                foreach(var res in result)
                {
                    unusedView.Add(new ServerUnusedServerClassView
                    {
                        ServerClass = res.TermTcpName.ToString(),
                        Unused = Convert.ToInt64(res.Unused)
                    });
                }
            }

            return unusedView;
        }

        public List<TermView> GetTermAvgResponseTime(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName)
        {
            var top20Views = new List<TermView>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var result = session.Query<PvTermStatEntity>()
                    .Where(x => x.PathwayName == pathwayName &&
                                x.FromTimestamp >= fromTimestamp &&
                                x.ToTimestamp <= toTimestamp)
                    .GroupBy(x => new { x.TermName, x.TermTcpName })
                    .Select(g => new
                    {
                        TermName = g.Key.TermName,
                        MaxAvgResp = g.Max(a => a.AvgResp / 1000000),
                        TermTcpName = g.Key.TermTcpName
                    })
                    .OrderByDescending(x => x.MaxAvgResp)
                    .Take(20)
                    .ToList();

                foreach (var res in result)
                {
                    top20Views.Add(new TermView
                    {
                        Term = res.TermName.ToString(),
                        Tcp = res.TermTcpName.ToString(),
                        Value = Convert.ToDouble(res.MaxAvgResp)
                    });
                }
            }

            return top20Views;
        }

        public List<TermView> GetTermMaxResponseTime(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName)
        {
            var top20Views = new List<TermView>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var result = session.Query<PvTermStatEntity>()
                    .Where(x => x.PathwayName == pathwayName &&
                                x.FromTimestamp >= fromTimestamp &&
                                x.ToTimestamp <= toTimestamp)
                    .GroupBy(x => new { x.TermName, x.TermTcpName })
                    .Select(g => new
                    {
                        TermName = g.Key.TermName,
                        MaxMaxResp = g.Max(a => a.MaxResp / 1000000),
                        TermTcpName = g.Key.TermTcpName
                    })
                    .OrderByDescending(x => x.MaxMaxResp)
                    .Take(20)
                    .ToList();

                foreach (var res in result)
                {
                    top20Views.Add(new TermView
                    {
                        Term = res.TermName.ToString(),
                        Tcp = res.TermTcpName.ToString(),
                        Value = Convert.ToDouble(res.MaxMaxResp)
                    });
                }
            }

            return top20Views;
        }

        public long GetTermToTcp(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName)
        {
            long termToTcp = 0;

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var result = session.Query<PvTermStatEntity>()
                    .Where(x => x.PathwayName == pathwayName &&
                                x.FromTimestamp >= fromTimestamp &&
                                x.ToTimestamp <= toTimestamp)
                    .GroupBy(x => x.PathwayName)
                    .Select(g => new
                    {
                        g.Key,
                        TotalIsReqCnt = g.Sum(a => Convert.ToDouble(a.IsReqCnt))
                    })
                    .First();

                termToTcp = Convert.ToInt64(result.TotalIsReqCnt);
            }

            return termToTcp;
        }

        public Dictionary<string, long> GetTermToTcp(DateTime fromTimestamp, DateTime toTimestamp)
        {
            var termToTcp = new Dictionary<string, long>();

            try
            {
                using (var session = NHibernateHelper.OpenSystemSession())
                {
                    var result = session.Query<PvTermStatEntity>()
                        .Where(x => x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp)
                        .GroupBy(x => x.PathwayName)
                        .Select(g => new
                        {
                            PathwayName = g.Key,
                            TotalIsReqCnt = g.Sum(a => Convert.ToDouble(a.IsReqCnt))
                        })
                        .ToList();

                    foreach (var res in result)
                    {
                        termToTcp.Add(res.PathwayName.ToString(), Convert.ToInt64(res.TotalIsReqCnt));
                    }
                }
            }
            catch { }
            

            return termToTcp;
        }

        public List<TermView> GetTermTransactions(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName)
        {
            var top20Views = new List<TermView>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var result = session.Query<PvTermStatEntity>()
                    .Where(x => x.PathwayName == pathwayName &&
                                x.FromTimestamp >= fromTimestamp &&
                                x.ToTimestamp <= toTimestamp)
                    .GroupBy(x => new { x.TermName, x.TermTcpName })
                    .Select(g => new
                    {
                        TermName = g.Key.TermName,
                        TotalIsReqCnt = g.Sum(a => a.IsReqCnt),
                        TermTcpName = g.Key.TermTcpName
                    })
                    .OrderByDescending(x => x.TotalIsReqCnt)
                    .Take(20)
                    .ToList();

                foreach (var res in result)
                {
                    top20Views.Add(new TermView
                    {
                        Term = res.TermName.ToString(),
                        Tcp = res.TermTcpName.ToString(),
                        Value = Convert.ToDouble(res.TotalIsReqCnt)
                    });
                }
            }

            return top20Views;
        }

        public List<TermUnusedView> GetTermUnused(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName)
        {
            var unusedView = new List<TermUnusedView>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var subquery = session.Query<PvTermStatEntity>()
                    .Where(x => x.PathwayName == pathwayName &&
                                x.FromTimestamp >= fromTimestamp &&
                                x.ToTimestamp <= toTimestamp)
                    .GroupBy(x => new { x.TermName, x.TermTcpName })
                    .Select(g => new
                    {
                        TermName = g.Key.TermName,
                        TermTcpName = g.Key.TermTcpName,
                        TotalIsReqCnt = g.Sum(a => a.IsReqCnt)
                    });

                var result = subquery
                    .Where(x => x.TotalIsReqCnt == 0)
                    .OrderBy(x => x.TermTcpName)
                    .ToList();

                foreach(var res in result)
                {
                    unusedView.Add(new TermUnusedView
                    {
                        Term = res.TermName.ToString(),
                        Tcp = res.TermTcpName.ToString()
                    });
                }
            }

            return unusedView;
        }
    }
}
