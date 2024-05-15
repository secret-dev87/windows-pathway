using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pathway.Core.Abstract.Repositories;
using Pathway.Core.Entity;
using Pathway.Core.Helper;
using Pathway.Core.Infrastructure;

namespace Pathway.Core.Repositories
{
    internal class PvAlertsRepository : IPvAlertsRepository
    {
        private readonly int _maxTimes = 3; //Not sure what this 3 is, but it's on all the Alert SELECT Statment.
        public PvAlertsRepository() { }

        public void InsertEmptyData(string pathwayName, DateTime fromTimestamp, DateTime toTimestamp)
        {
            using(var session = NHibernateHelper.OpenSystemSession())
            {
                using(var transaction = session.BeginTransaction())
                {
                    var newAlerts = new PvAlertEntity
                    {
                        FromTimestamp = fromTimestamp,
                        ToTimestamp = toTimestamp,
                        Pathway = pathwayName,
                        TermHiMaxRT = 0,
                        TermHiAvgRT = 0,
                        TermUnused = 0,
                        TermErrorList = 0,
                        TCPQueuedTransactions = 0,
                        TCPLowTermPool = 0,
                        TCPLowServerPool = 0,
                        TCPUnused = 0,
                        TCPErrorList = 0,
                        ServerHiMaxRT = 0,
                        ServerHiAvgRT = 0,
                        ServerQueueTCP = 0,
                        ServerQueueLinkmon = 0,
                        ServerUnusedClass = 0,
                        ServerUnusedProcess = 0,
                        ServerErrorList = 0
                    };

                    session.Save(newAlerts);
                    transaction.Commit();
                }
            }
        }

        public void UpdateAlert(string alertName, string pathwayName, long count, DateTime fromTimestamp, DateTime toTimestamp)
        {
            using (var session = NHibernateHelper.OpenSystemSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var sql = $"UPDATE PvAlerts SET {alertName} = :value WHERE FromTimestamp >= :fromTimestamp AND ToTimestamp <= :toTimestamp AND Pathway = :pathway";
                    int updatedEntities = session.CreateSQLQuery(sql)
                        .SetParameter("value", count)
                        .SetParameter("fromTimestamp", fromTimestamp)
                        .SetParameter("toTimestamp", toTimestamp)
                        .SetParameter("pathwya", pathwayName)
                        .ExecuteUpdate();

                    transaction.Commit();
                }
            }
        }

        #region Hourly Data

        public List<Alert> GetTermHiMaxRtHourly(DateTime fromTimestamp, DateTime toTimestamp)
        {
            var alerts = new List<Alert>();

            using (var session = NHibernateHelper.OpenSystemSession())
            {
                var results = session.Query<PvTermStatEntity>()
                    .Where(x => (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp) &&
                                x.MaxResp > Convert.ToDouble(ConfigurationManager.AppSettings["ResponseTime"]))
                    .GroupBy(x => new { x.PathwayName, x.FromTimestamp, x.ToTimestamp })
                    .Select(g => new
                    {
                        PathwayName = g.Key.PathwayName,
                        FromTimestamp = g.Key.FromTimestamp,
                        ToTimestamp = g.Key.ToTimestamp,
                        Count = g.Select(a => a.TermName).Distinct().Count(),
                    })
                    .OrderBy(x => new { x.PathwayName, x.FromTimestamp })
                    .ToList();

                foreach (var res in results)
                {
                    var alert = new Alert
                    {
                        PathwayName = res.PathwayName.ToString(),
                        Count = Convert.ToInt64(res.Count),
                        FromTimestamp = Convert.ToDateTime(res.FromTimestamp),
                        ToTimestamp = Convert.ToDateTime(res.ToTimestamp)
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        public List<Alert> GetTermHiAvgRtHourly(DateTime fromTimestamp, DateTime toTimestamp)
        {
            var alerts = new List<Alert>();

            using (var session = NHibernateHelper.OpenSystemSession())
            {
                var results = session.Query<PvTermStatEntity>()
                    .Where(x => (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp) &&
                                x.AvgResp > Convert.ToDouble(ConfigurationManager.AppSettings["ResponseTime"]))
                    .GroupBy(x => new { x.PathwayName, x.FromTimestamp, x.ToTimestamp })
                    .Select(g => new
                    {
                        PathwayName = g.Key.PathwayName,
                        FromTimestamp = g.Key.FromTimestamp,
                        ToTimestamp = g.Key.ToTimestamp,
                        Count = g.Select(a => a.TermName).Distinct().Count(),
                    })
                    .OrderBy(x => new { x.PathwayName, x.FromTimestamp })
                    .ToList();

                foreach (var res in results)
                {
                    var alert = new Alert
                    {
                        PathwayName = res.PathwayName.ToString(),
                        Count = Convert.ToInt64(res.Count),
                        FromTimestamp = Convert.ToDateTime(res.FromTimestamp),
                        ToTimestamp = Convert.ToDateTime(res.ToTimestamp)
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        public List<Alert> GetTermUnusedHourly(DateTime fromTimestamp, DateTime toTimestamp)
        {
            var alerts = new List<Alert>();

            using (var session = NHibernateHelper.OpenSystemSession())
            {
                var subquery = session.Query<PvTermStatEntity>()
                    .Where(x => x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp)
                    .GroupBy(x => new { x.PathwayName, x.TermName, x.TermTcpName, x.FromTimestamp, x.ToTimestamp })
                    .Select(g => new
                    {
                        PathwayName = g.Key.PathwayName,
                        TotalIsReqCnt = g.Sum(a => a.IsReqCnt),
                        FromTimestamp = g.Key.FromTimestamp,
                        ToTimestamp = g.Key.ToTimestamp
                    });

                var results = subquery
                    .Where(x => x.TotalIsReqCnt == 0)
                    .GroupBy(x => new { x.PathwayName, x.FromTimestamp, x.ToTimestamp })
                    .Select(g => new
                    {
                        PathwayName = g.Key.PathwayName,
                        Count = g.Count(),
                        FromTimestamp = g.Key.FromTimestamp,
                        ToTimestamp = g.Key.ToTimestamp
                    })
                    .OrderBy(x => new { x.PathwayName, x.FromTimestamp })
                    .ToList();

                foreach (var res in results)
                {
                    var alert = new Alert
                    {
                        PathwayName = res.PathwayName.ToString(),
                        Count = Convert.ToInt64(res.Count),
                        FromTimestamp = Convert.ToDateTime(res.FromTimestamp),
                        ToTimestamp = Convert.ToDateTime(res.ToTimestamp)
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        public List<Alert> GetTermErrorListHourly(DateTime fromTimestamp, DateTime toTimestamp)
        {
            var alerts = new List<Alert>();

            using (var session = NHibernateHelper.OpenSystemSession())
            {
                var results = session.Query<PvTermStusEntity>()
                    .Where(x => x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp && x.ErrorNumber != 0)
                    .GroupBy(x => new { x.PathwayName, x.FromTimestamp, x.ToTimestamp })
                    .Select(g => new
                    {
                        PathwayName = g.Key.PathwayName,
                        Count = g.Select(a => a.TermName).Distinct().Count(),
                        FromTimestamp = g.Key.FromTimestamp,
                        ToTimestamp = g.Key.ToTimestamp
                    })
                    .OrderBy(x => new { x.PathwayName, x.FromTimestamp })
                    .ToList();

                foreach (var res in results)
                {
                    var alert = new Alert
                    {
                        PathwayName = res.PathwayName.ToString(),
                        Count = Convert.ToInt64(res.Count),
                        FromTimestamp = Convert.ToDateTime(res.FromTimestamp),
                        ToTimestamp = Convert.ToDateTime(res.ToTimestamp)
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        public List<Alert> GetTcpQueuedTransactionsHourly(DateTime fromTimestamp, DateTime toTimestamp)
        {
            var alerts = new List<Alert>();

            using (var session = NHibernateHelper.OpenSystemSession())
            {
                var results = session.Query<PvTcpStatEntity>()
                    .Where(x => x.QLReqCnt > 0 &&
                                (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp))
                    .GroupBy(x => new { x.PathwayName, x.FromTimestamp, x.ToTimestamp })
                    .Select(g => new
                    {
                        PathwayName = g.Key.PathwayName,
                        Count = g.Select(a => a.TcpName).Distinct().Count(),
                        FromTimestamp = g.Key.FromTimestamp,
                        ToTimestamp = g.Key.ToTimestamp
                    })
                    .OrderBy(x => new { x.PathwayName, x.FromTimestamp })
                    .ToList();

                foreach (var res in results)
                {
                    var alert = new Alert
                    {
                        PathwayName = res.PathwayName.ToString(),
                        Count = Convert.ToInt64(res.Count),
                        FromTimestamp = Convert.ToDateTime(res.FromTimestamp),
                        ToTimestamp = Convert.ToDateTime(res.ToTimestamp)
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        public List<Alert> GetTcpLowTermPoolHourly(DateTime fromTimestamp, DateTime toTimestamp)
        {
            var alerts = new List<Alert>();

            using (var session = NHibernateHelper.OpenSystemSession())
            {
                var subquery = session.Query<PvTcpStatEntity>()
                    .Where(x => x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp)
                    .Select(g => new
                    {
                        PathwayName = g.PathwayName,
                        TcpName = g.TcpName,
                        PtSize = g.PtSize,
                        PtMaxAlloc = g.PtMaxAlloc,
                        PtMaxReq = g.PtMaxReq,
                        PsSize = g.PsSize,
                        PsMaxAlloc = g.PsMaxAlloc,
                        PsMaxReq = g.PsMaxReq,
                        AvgTReqSize = g.PtAggregateReqSize / (g.PtReqCnt != 0 ? g.PtReqCnt : 1),
                        AvgSReqSize = g.PsAggregateReqSize / (g.PsReqCnt != 0 ? g.PsReqCnt : 1),
                        FromTimestamp = g.FromTimestamp,
                        ToTimestamp = g.ToTimestamp
                    });

                var results = subquery
                    .Where(x => (x.PtSize - x.PtMaxAlloc) < x.AvgTReqSize ||
                                (x.PtSize - x.PtMaxAlloc) < x.PtMaxReq)
                    .GroupBy(x => new { x.PathwayName, x.FromTimestamp, x.ToTimestamp })
                    .Select(g => new
                    {
                        PathwayName = g.Key.PathwayName,
                        FromTimestamp = g.Key.FromTimestamp,
                        ToTimestamp = g.Key.ToTimestamp,
                        Count = g.Select(a => a.TcpName).Distinct().Count()
                    })
                    .OrderBy(x => new { x.PathwayName, x.FromTimestamp })
                    .ToList();

                foreach (var res in results)
                {
                    var alert = new Alert
                    {
                        PathwayName = res.PathwayName.ToString(),
                        Count = Convert.ToInt64(res.Count),
                        FromTimestamp = Convert.ToDateTime(res.FromTimestamp),
                        ToTimestamp = Convert.ToDateTime(res.ToTimestamp)
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        public List<Alert> GetTcpLowServerPoolHourly(DateTime fromTimestamp, DateTime toTimestamp)
        {
            var alerts = new List<Alert>();

            using (var session = NHibernateHelper.OpenSystemSession())
            {
                var subquery = session.Query<PvTcpStatEntity>()
                    .Where(x => x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp)
                    .Select(g => new
                    {
                        PathwayName = g.PathwayName,
                        TcpName = g.TcpName,
                        PtSize = g.PtSize,
                        PtMaxAlloc = g.PtMaxAlloc,
                        PtMaxReq = g.PtMaxReq,
                        PsSize = g.PsSize,
                        PsMaxAlloc = g.PsMaxAlloc,
                        PsMaxReq = g.PsMaxReq,
                        AvgTReqSize = g.PtAggregateReqSize / (g.PtReqCnt != 0 ? g.PtReqCnt : 1),
                        AvgSReqSize = g.PsAggregateReqSize / (g.PsReqCnt != 0 ? g.PsReqCnt : 1),
                        FromTimestamp = g.FromTimestamp,
                        ToTimestamp = g.ToTimestamp
                    });

                var results = subquery
                    .Where(x => (x.PsSize - x.PsMaxAlloc) < x.AvgSReqSize ||
                                (x.PsSize - x.PsMaxAlloc) < x.PsMaxReq)
                    .GroupBy(x => new { x.PathwayName, x.FromTimestamp, x.ToTimestamp })
                    .Select(g => new
                    {
                        PathwayName = g.Key.PathwayName,
                        FromTimestamp = g.Key.FromTimestamp,
                        ToTimestamp = g.Key.ToTimestamp,
                        Count = g.Select(a => a.TcpName).Distinct().Count()
                    })
                    .OrderBy(x => new { x.PathwayName, x.FromTimestamp })
                    .ToList();

                foreach (var res in results)
                {
                    var alert = new Alert
                    {
                        PathwayName = res.PathwayName.ToString(),
                        Count = Convert.ToInt64(res.Count),
                        FromTimestamp = Convert.ToDateTime(res.FromTimestamp),
                        ToTimestamp = Convert.ToDateTime(res.ToTimestamp)
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        public List<Alert> GetTcpUnusedHourly(DateTime fromTimestamp, DateTime toTimestamp)
        {
            var alerts = new List<Alert>();

            using (var session = NHibernateHelper.OpenSystemSession())
            {
                var subquery = session.Query<PvTermStatEntity>()
                    .Where(x => x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp)
                    .GroupBy(x => new { x.PathwayName, x.TermTcpName, x.FromTimestamp, x.ToTimestamp })
                    .Select(g => new
                    {
                        PathwayName = g.Key.PathwayName,
                        FromTimestamp = g.Key.FromTimestamp,
                        ToTimestamp = g.Key.ToTimestamp,
                        TotalIsReqCnt = g.Sum(a => a.IsReqCnt),
                        TermTcpName = g.Key.TermTcpName
                    });

                var results = subquery
                    .Where(x => x.TotalIsReqCnt == 0)
                    .GroupBy(x => new { x.PathwayName, x.FromTimestamp, x.ToTimestamp })
                    .Select(g => new
                    {
                        PathwayName = g.Key.PathwayName,
                        FromTimestamp = g.Key.FromTimestamp,
                        ToTimestamp = g.Key.ToTimestamp,
                        Count = g.Select(a => a.TermTcpName).Distinct().Count()
                    })
                    .OrderBy(x => new { x.PathwayName, x.FromTimestamp })
                    .ToList();

                foreach (var res in results)
                {
                    var alert = new Alert
                    {
                        PathwayName = res.PathwayName.ToString(),
                        Count = Convert.ToInt64(res.Count),
                        FromTimestamp = Convert.ToDateTime(res.FromTimestamp),
                        ToTimestamp = Convert.ToDateTime(res.ToTimestamp)
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        public List<Alert> GetTcpErrorListHourly(DateTime fromTimestamp, DateTime toTimestamp)
        {
            var alerts = new List<Alert>();

            using (var session = NHibernateHelper.OpenSystemSession())
            {
                var results = session.Query<PvTcpStusEntity>()
                    .Where(x => x.ErrorNumber != 0 &&
                                x.FromTimestamp >= fromTimestamp &&
                                x.ToTimestamp <= toTimestamp)
                    .GroupBy(x => new { x.PathwayName, x.FromTimestamp, x.ToTimestamp })
                    .Select(g => new
                    {
                        PathwayName = g.Key.PathwayName,
                        FromTimestamp = g.Key.FromTimestamp,
                        ToTimestamp = g.Key.ToTimestamp,
                        Count = g.Select(a => a.TcpName).Distinct().Count()
                    })
                    .OrderBy(x => new { x.PathwayName, x.FromTimestamp })
                    .ToList();

                foreach (var res in results)
                {
                    var alert = new Alert
                    {
                        PathwayName = res.PathwayName.ToString(),
                        Count = Convert.ToInt64(res.Count),
                        FromTimestamp = Convert.ToDateTime(res.FromTimestamp),
                        ToTimestamp = Convert.ToDateTime(res.ToTimestamp)
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        public List<Alert> GetServerHiMaxRtHourly(DateTime fromTimestamp, DateTime toTimestamp)
        {
            var alerts = new List<Alert>();

            using (var session = NHibernateHelper.OpenSystemSession())
            {
                var results = session.Query<PvScTStatEntity>()
                    .Where(x => x.MaxResp > Convert.ToDouble(ConfigurationManager.AppSettings["ResponseTime"]) &&
                                x.FromTimestamp >= fromTimestamp &&
                                x.ToTimestamp <= toTimestamp)
                    .GroupBy(x => new { x.PathwayName, x.FromTimestamp, x.ToTimestamp })
                    .Select(g => new
                    {
                        PathwayName = g.Key.PathwayName,
                        FromTimestamp = g.Key.FromTimestamp,
                        ToTimestamp = g.Key.ToTimestamp,
                        Count = g.Select(a => a.ScName).Distinct().Count()
                    })
                    .OrderBy(x => new { x.PathwayName, x.FromTimestamp })
                    .ToList();

                foreach (var res in results)
                {
                    var alert = new Alert
                    {
                        PathwayName = res.PathwayName.ToString(),
                        Count = Convert.ToInt64(res.Count),
                        FromTimestamp = Convert.ToDateTime(res.FromTimestamp),
                        ToTimestamp = Convert.ToDateTime(res.ToTimestamp)
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        public List<Alert> GetServerHiAvgRtHourly(DateTime fromTimestamp, DateTime toTimestamp)
        {
            var alerts = new List<Alert>();

            using (var session = NHibernateHelper.OpenSystemSession())
            {
                var results = session.Query<PvScTStatEntity>()
                    .Where(x => x.AvgResp > Convert.ToDouble(ConfigurationManager.AppSettings["ResponseTime"]) &&
                                x.FromTimestamp >= fromTimestamp &&
                                x.ToTimestamp <= toTimestamp)
                    .GroupBy(x => new { x.PathwayName, x.FromTimestamp, x.ToTimestamp })
                    .Select(g => new
                    {
                        PathwayName = g.Key.PathwayName,
                        FromTimestamp = g.Key.FromTimestamp,
                        ToTimestamp = g.Key.ToTimestamp,
                        Count = g.Select(a => a.ScName).Distinct().Count()
                    })
                    .OrderBy(x => new { x.PathwayName, x.FromTimestamp })
                    .ToList();

                foreach (var res in results)
                {
                    var alert = new Alert
                    {
                        PathwayName = res.PathwayName.ToString(),
                        Count = Convert.ToInt64(res.Count),
                        FromTimestamp = Convert.ToDateTime(res.FromTimestamp),
                        ToTimestamp = Convert.ToDateTime(res.ToTimestamp)
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        public List<Alert> GetServerQueueTcpHourly(DateTime fromTimestamp, DateTime toTimestamp)
        {
            var alerts = new List<Alert>();

            using (var session = NHibernateHelper.OpenSystemSession())
            {
                var results = session.Query<PvScTStatEntity>()
                    .Where(x => (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp) &&
                                x.QIReqCnt > 0)
                    .GroupBy(x => new { x.PathwayName, x.FromTimestamp, x.ToTimestamp })
                    .Select(g => new
                    {
                        PathwayName = g.Key.PathwayName,
                        FromTimestamp = g.Key.FromTimestamp,
                        ToTimestamp = g.Key.ToTimestamp,
                        Count = g.Select(a => a.ScName).Distinct().Count()
                    })
                    .ToList();

                foreach (var res in results)
                {
                    var alert = new Alert
                    {
                        PathwayName = res.PathwayName.ToString(),
                        Count = Convert.ToInt64(res.Count),
                        FromTimestamp = Convert.ToDateTime(res.FromTimestamp),
                        ToTimestamp = Convert.ToDateTime(res.ToTimestamp)
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        public List<Alert> GetServerQueueLinkmonHourly(DateTime fromTimestamp, DateTime toTimestamp)
        {
            var alerts = new List<Alert>();

            using (var session = NHibernateHelper.OpenSystemSession())
            {
                var results = session.Query<PvScLStatEntity>()
                    .Where(x => (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp) &&
                                x.QIReqCnt > 0)
                    .GroupBy(x => new { x.PathwayName, x.FromTimestamp, x.ToTimestamp })
                    .Select(g => new
                    {
                        PathwayName = g.Key.PathwayName,
                        FromTimestamp = g.Key.FromTimestamp,
                        ToTimestamp = g.Key.ToTimestamp,
                        Count = g.Select(a => a.ScName).Distinct().Count()
                    })
                    .OrderBy(x => new { x.PathwayName, x.FromTimestamp })
                    .ToList();

                foreach (var res in results)
                {
                    var alert = new Alert
                    {
                        PathwayName = res.PathwayName.ToString(),
                        Count = Convert.ToInt64(res.Count),
                        FromTimestamp = Convert.ToDateTime(res.FromTimestamp),
                        ToTimestamp = Convert.ToDateTime(res.ToTimestamp)
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        public List<Alert> GetServerUnusedClassHourly(DateTime fromTimestamp, DateTime toTimestamp)
        {
            var alerts = new List<Alert>();

            using (var session = NHibernateHelper.OpenSystemSession())
            {
                var temp1 = session.Query<PvScTStatEntity>()
                    .Where(x => x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp)
                    .GroupBy(x => new { x.ScName, x.PathwayName, x.FromTimestamp, x.ToTimestamp })
                    .Select(g => new
                    {
                        ScName = g.Key.ScName,
                        PathwayName = g.Key.PathwayName,
                        SumofIsReqCnt = g.Sum(x => x.IsReqCnt),
                        FromTimestamp = g.Key.FromTimestamp,
                        ToTimestamp = g.Key.ToTimestamp
                    });

                var temp2 = session.Query<PvScLStatEntity>() // Assuming PvSclstat can be represented by PvStat for this example
                    .Where(x => x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp)
                    .GroupBy(x => new { x.ScName, x.PathwayName, x.FromTimestamp, x.ToTimestamp })
                    .Select(g => new
                    {
                        ScName = g.Key.ScName,
                        PathwayName = g.Key.PathwayName,
                        SumofIsReqCnt = g.Sum(x => x.IsReqCnt),
                        FromTimestamp = g.Key.FromTimestamp,
                        ToTimestamp = g.Key.ToTimestamp
                    });

                var combinedResults = from t1 in temp1
                                      join t2 in temp2
                                      on new { t1.ScName, t1.PathwayName, t1.FromTimestamp, t1.ToTimestamp } equals
                                         new { t2.ScName, t2.PathwayName, t2.FromTimestamp, t2.ToTimestamp } into t2s
                                      from t2 in t2s.DefaultIfEmpty()
                                      select new
                                      {
                                          ScName = t1.ScName ?? t2.ScName,
                                          PathwayName = t1.PathwayName ?? t2.PathwayName,
                                          Tran1 = t1.SumofIsReqCnt,
                                          Tran2 = t2 != null ? t2.SumofIsReqCnt : 0,
                                          FromTimestamp = t1.FromTimestamp,
                                          ToTimestamp = t1.ToTimestamp
                                      };

                var finalResults = combinedResults
                    .Where(x => x.Tran1 == 0 && x.Tran2 == 0)
                    .GroupBy(x => new { x.PathwayName, x.FromTimestamp, x.ToTimestamp })
                    .Select(g => new
                    {
                        PathwayName = g.Key.PathwayName,
                        Count = g.Count(),
                        FromTimestamp = g.Key.FromTimestamp,
                        ToTimestamp = g.Key.ToTimestamp
                    })
                    .OrderBy(x => x.PathwayName).ThenBy(x => x.FromTimestamp)
                    .ToList();

                foreach (var res in finalResults)
                {
                    var alert = new Alert
                    {
                        PathwayName = res.PathwayName.ToString(),
                        Count = Convert.ToInt64(res.Count),
                        FromTimestamp = Convert.ToDateTime(res.FromTimestamp),
                        ToTimestamp = Convert.ToDateTime(res.ToTimestamp)
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        public List<Alert> GetServerUnusedProcessHourly(DateTime fromTimestamp, DateTime toTimestamp)
        {
            var alerts = new List<Alert>();

            using (var session = NHibernateHelper.OpenSystemSession())
            {
                var query = from prstus in session.Query<PvScPrStusEntity>()
                            join info in session.Query<PvScInfoEntity>() on new
                            {
                                prstus.PathwayName,
                                prstus.ScName,
                                prstus.FromTimestamp,
                                prstus.ToTimestamp
                            } equals new
                            {
                                info.PathwayName,
                                info.ScName,
                                info.FromTimestamp,
                                info.ToTimestamp
                            } into infoGroup
                            from subInfo in infoGroup.DefaultIfEmpty() // Simulate LEFT OUTER JOIN
                            where prstus.FromTimestamp >= fromTimestamp && prstus.ToTimestamp <= toTimestamp &&
                                  subInfo.FromTimestamp >= fromTimestamp && subInfo.ToTimestamp <= toTimestamp &&
                                  prstus.ProcState == "2"
                            group prstus by new { prstus.PathwayName, prstus.FromTimestamp, prstus.ToTimestamp } into g
                            where g.Sum(x => x.ProcLinks) == 0
                            select new
                            {
                                PathwayName = g.Key.PathwayName,
                                Count = g.Count(),
                                FromTimestamp = g.Key.FromTimestamp,
                                ToTimestamp = g.Key.ToTimestamp
                            };

                var results = query.OrderBy(x => x.PathwayName).ThenBy(x => x.FromTimestamp).ToList();

                foreach (var res in results)
                {
                    var alert = new Alert
                    {
                        PathwayName = res.PathwayName.ToString(),
                        Count = Convert.ToInt64(res.Count),
                        FromTimestamp = Convert.ToDateTime(res.FromTimestamp),
                        ToTimestamp = Convert.ToDateTime(res.ToTimestamp)
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        public List<Alert> GetServerErrorListHourly(DateTime fromTimestamp, DateTime toTimestamp)
        {
            var alerts = new List<Alert>();

            using (var session = NHibernateHelper.OpenSystemSession())
            {
                var results = session.Query<PvScStusEntity>()
                    .Where(x => x.ErrorNumber != 0 &&
                                (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp))
                    .GroupBy(x => new { x.PathwayName, x.FromTimestamp, x.ToTimestamp })
                    .Select(g => new
                    {
                        PathwayName = g.Key.PathwayName,
                        FromTimestamp = g.Key.FromTimestamp,
                        ToTimestamp = g.Key.ToTimestamp,
                        Count = g.Select(a => a.ScName).Distinct().Count()
                    })
                    .OrderBy(x => new { x.PathwayName, x.FromTimestamp })
                    .ToList();

                foreach (var res in results)
                {
                    var alert = new Alert
                    {
                        PathwayName = res.PathwayName.ToString(),
                        Count = Convert.ToInt64(res.Count),
                        FromTimestamp = Convert.ToDateTime(res.FromTimestamp),
                        ToTimestamp = Convert.ToDateTime(res.ToTimestamp)
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        #endregion

        #region Collection

        public List<Alert> GetTermHiMaxRt(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "")
        {
            var alerts = new List<Alert>();

            using (var session = NHibernateHelper.OpenSystemSession())
            {
                var query = pathwayName.Length == 0 ?
                    session.Query<PvTermStatEntity>()
                        .Where(x => (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp) &&
                                    x.MaxResp > _maxTimes * Convert.ToInt64(ConfigurationManager.AppSettings["ResponseTime"])) :
                    session.Query<PvTermStatEntity>()
                        .Where(x => (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp) &&
                                    x.MaxResp > _maxTimes * Convert.ToInt64(ConfigurationManager.AppSettings["ResponseTime"]) &&
                                    x.PathwayName == pathwayName);

                var results = query
                    .GroupBy(x => x.PathwayName)
                    .Select(g => new
                    {
                        PathwayName = g.Key,
                        Count = g.Select(a => a.TermName).Distinct().Count()
                    })
                    .OrderBy(x => x.PathwayName)
                    .ToList();

                foreach (var res in results)
                {
                    alerts.Add(new Alert
                    {
                        PathwayName = res.PathwayName.ToString(),
                        Count = Convert.ToInt64(res.Count)
                    });
                }

            }

            return alerts;
        }

        public List<Alert> GetTermHiAvgRt(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "")
        {
            var alerts = new List<Alert>();

            using (var session = NHibernateHelper.OpenSystemSession())
            {
                var query = pathwayName.Length == 0 ?
                    session.Query<PvTermStatEntity>()
                        .Where(x => (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp) &&
                                    x.AvgResp > _maxTimes * Convert.ToInt64(ConfigurationManager.AppSettings["ResponseTime"])) :
                    session.Query<PvTermStatEntity>()
                        .Where(x => (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp) &&
                                    x.AvgResp > _maxTimes * Convert.ToInt64(ConfigurationManager.AppSettings["ResponseTime"]) &&
                                    x.PathwayName == pathwayName);

                var results = query
                    .GroupBy(x => x.PathwayName)
                    .Select(g => new
                    {
                        PathwayName = g.Key,
                        Count = g.Select(a => a.TermName).Distinct().Count()
                    })
                    .OrderBy(x => x.PathwayName)
                    .ToList();

                foreach (var res in results)
                {
                    alerts.Add(new Alert
                    {
                        PathwayName = res.PathwayName.ToString(),
                        Count = Convert.ToInt64(res.Count)
                    });
                }

            }

            return alerts;
        }

        public List<Alert> GetTermUnused(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "")
        {
            var alerts = new List<Alert>();

            using (var session = NHibernateHelper.OpenSystemSession())
            {
                var subquery = pathwayName.Length == 0 ?
                    session.Query<PvTermStatEntity>()
                        .Where(x => (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp)) :
                    session.Query<PvTermStatEntity>()
                        .Where(x => (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp) &&
                                    x.PathwayName == pathwayName);

                var query = subquery
                    .GroupBy(x => new { x.PathwayName, x.TermName, x.TermTcpName })
                    .Select(g => new
                    {
                        PathwayName = g.Key,
                        TotalIsReqCnt = g.Sum(a => a.IsReqCnt)
                    });

                var results = query
                    .Where(x => x.TotalIsReqCnt == 0)
                    .GroupBy(x => x.PathwayName)
                    .Select(g => new
                    {
                        PathwayName = g.Key,
                        Count = g.Select(a => a.TotalIsReqCnt).Count()
                    })
                    .OrderBy(x => x.PathwayName)
                    .ToList();

                foreach (var res in results)
                {
                    alerts.Add(new Alert
                    {
                        PathwayName = res.PathwayName.ToString(),
                        Count = Convert.ToInt64(res.Count)
                    });
                }

            }

            return alerts;
        }

        public List<Alert> GetTermErrorList(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "")
        {
            var alerts = new List<Alert>();

            using (var session = NHibernateHelper.OpenSystemSession())
            {
                var query = pathwayName.Length == 0 ?
                    session.Query<PvTermStusEntity>()
                        .Where(x => (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp) &&
                                    x.ErrorNumber != 0) :
                    session.Query<PvTermStusEntity>()
                        .Where(x => (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp) &&
                                    x.ErrorNumber != 0 &&
                                    x.PathwayName == pathwayName);

                var results = query
                    .GroupBy(x => x.PathwayName)
                    .Select(g => new
                    {
                        PathwayName = g.Key,
                        Count = g.Select(a => a.TermName).Distinct().Count()
                    })
                    .OrderBy(x => x.PathwayName)
                    .ToList();

                foreach (var res in results)
                {
                    alerts.Add(new Alert
                    {
                        PathwayName = res.PathwayName.ToString(),
                        Count = Convert.ToInt64(res.Count)
                    });
                }

            }

            return alerts;
        }

        public List<Alert> GetTcpQueuedTransactions(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "")
        {
            var alerts = new List<Alert>();

            using (var session = NHibernateHelper.OpenSystemSession())
            {
                var query = pathwayName.Length == 0 ?
                    session.Query<PvTcpStatEntity>()
                        .Where(x => (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp) &&
                                    x.QLReqCnt != 0) :
                    session.Query<PvTcpStatEntity>()
                        .Where(x => (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp) &&
                                    x.QLReqCnt != 0 &&
                                    x.PathwayName == pathwayName);

                var results = query
                    .GroupBy(x => x.PathwayName)
                    .Select(g => new
                    {
                        PathwayName = g.Key,
                        Count = g.Select(a => a.TcpName).Distinct().Count()
                    })
                    .OrderBy(x => x.PathwayName)
                    .ToList();

                foreach (var res in results)
                {
                    alerts.Add(new Alert
                    {
                        PathwayName = res.PathwayName.ToString(),
                        Count = Convert.ToInt64(res.Count)
                    });
                }

            }

            return alerts;
        }

        public List<Alert> GetTcpLowTermPool(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "")
        {
            var alerts = new List<Alert>();

            using (var session = NHibernateHelper.OpenSystemSession())
            {
                var subquery = pathwayName.Length == 0 ?
                    session.Query<PvTcpStatEntity>()
                        .Where(x => (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp)) :
                    session.Query<PvTcpStatEntity>()
                        .Where(x => (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp) &&
                                    x.PathwayName == pathwayName);

                var query = subquery
                    .Select(g => new
                    {
                        PathwayName = g.PathwayName,
                        TcpName = g.TcpName,
                        PtSize = g.PtSize,
                        PtMaxAlloc = g.PtMaxAlloc,
                        PtMaxReq = g.PtMaxReq,
                        PsSize = g.PsSize,
                        PsMaxAlloc = g.PsMaxAlloc,
                        PsMaxReq = g.PsMaxReq,
                        AvgTReqSize = g.PtAggregateReqSize / (g.PtReqCnt != 0 ? g.PtReqCnt : 1),
                        AvgSReqSize = g.PsAggregateReqSize / (g.PsReqCnt != 0 ? g.PsReqCnt : 1)
                    });

                var results = query
                    .Where(x => ((x.PtSize - x.PtMaxAlloc) < x.AvgTReqSize) ||
                                ((x.PtSize - x.PtMaxAlloc) < x.PtMaxReq))
                    .GroupBy(x => x.PathwayName)
                    .Select(g => new
                    {
                        PathwayName = g.Key,
                        Count = g.Select(a => a.TcpName).Distinct().Count()
                    })
                    .OrderBy(x => x.PathwayName)
                    .ToList();

                foreach (var res in results)
                {
                    alerts.Add(new Alert
                    {
                        PathwayName = res.PathwayName.ToString(),
                        Count = Convert.ToInt64(res.Count)
                    });
                }

            }

            return alerts;
        }

        public List<Alert> GetTcpLowServerPool(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "")
        {
            var alerts = new List<Alert>();

            using (var session = NHibernateHelper.OpenSystemSession())
            {
                var subquery = pathwayName.Length == 0 ?
                    session.Query<PvTcpStatEntity>()
                        .Where(x => (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp)) :
                    session.Query<PvTcpStatEntity>()
                        .Where(x => (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp) &&
                                    x.PathwayName == pathwayName);

                var query = subquery
                    .Select(g => new
                    {
                        PathwayName = g.PathwayName,
                        TcpName = g.TcpName,
                        PtSize = g.PtSize,
                        PtMaxAlloc = g.PtMaxAlloc,
                        PtMaxReq = g.PtMaxReq,
                        PsSize = g.PsSize,
                        PsMaxAlloc = g.PsMaxAlloc,
                        PsMaxReq = g.PsMaxReq,
                        AvgTReqSize = g.PtAggregateReqSize / (g.PtReqCnt != 0 ? g.PtReqCnt : 1),
                        AvgSReqSize = g.PsAggregateReqSize / (g.PsReqCnt != 0 ? g.PsReqCnt : 1)
                    });

                var results = query
                    .Where(x => ((x.PsSize - x.PsMaxAlloc) < x.AvgSReqSize) ||
                                ((x.PsSize - x.PsMaxAlloc) < x.PsMaxReq))
                    .GroupBy(x => x.PathwayName)
                    .Select(g => new
                    {
                        PathwayName = g.Key,
                        Count = g.Select(a => a.TcpName).Distinct().Count()
                    })
                    .OrderBy(x => x.PathwayName)
                    .ToList();

                foreach (var res in results)
                {
                    alerts.Add(new Alert
                    {
                        PathwayName = res.PathwayName.ToString(),
                        Count = Convert.ToInt64(res.Count)
                    });
                }

            }

            return alerts;
        }

        public List<Alert> GetTcpUnused(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "")
        {
            var alerts = new List<Alert>();

            using (var session = NHibernateHelper.OpenSystemSession())
            {
                var subquery = pathwayName.Length == 0 ?
                    session.Query<PvTermStatEntity>()
                        .Where(x => (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp)) :
                    session.Query<PvTermStatEntity>()
                        .Where(x => (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp) &&
                                    x.PathwayName == pathwayName);

                var query = subquery
                    .GroupBy(x => new { x.PathwayName, x.TermTcpName })
                    .Select(g => new
                    {
                        PathwayName = g.Key.PathwayName,
                        TermTcpName = g.Key.TermTcpName,
                        TotalIsReqCnt = g.Sum(a => a.IsReqCnt)
                    });

                var results = query
                    .Where(x => x.TotalIsReqCnt == 0)
                    .GroupBy(x => x.PathwayName)
                    .Select(g => new
                    {
                        PathwayName = g.Key,
                        Count = g.Select(a => a.TermTcpName).Distinct().Count()
                    })
                    .OrderBy(x => x.PathwayName)
                    .ToList();

                foreach (var res in results)
                {
                    alerts.Add(new Alert
                    {
                        PathwayName = res.PathwayName.ToString(),
                        Count = Convert.ToInt64(res.Count)
                    });
                }

            }

            return alerts;
        }

        public List<Alert> GetTcpErrorList(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "")
        {
            var alerts = new List<Alert>();

            using (var session = NHibernateHelper.OpenSystemSession())
            {
                var query = pathwayName.Length == 0 ?
                    session.Query<PvTcpStusEntity>()
                        .Where(x => (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp) &&
                                    x.ErrorNumber != 0) :
                    session.Query<PvTcpStusEntity>()
                        .Where(x => (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp) &&
                                    x.ErrorNumber != 0 &&
                                    x.PathwayName == pathwayName);

                var results = query
                    .GroupBy(x => x.PathwayName)
                    .Select(g => new
                    {
                        PathwayName = g.Key,
                        Count = g.Select(a => a.TcpName).Distinct().Count()
                    })
                    .OrderBy(x => x.PathwayName)
                    .ToList();

                foreach (var res in results)
                {
                    alerts.Add(new Alert
                    {
                        PathwayName = res.PathwayName.ToString(),
                        Count = Convert.ToInt64(res.Count)
                    });
                }

            }

            return alerts;
        }

        public List<Alert> GetServerHiMaxRt(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "")
        {
            var alerts = new List<Alert>();

            using (var session = NHibernateHelper.OpenSystemSession())
            {
                var query = pathwayName.Length == 0 ?
                    session.Query<PvScTStatEntity>()
                        .Where(x => (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp) &&
                                    x.MaxResp > (_maxTimes * Convert.ToInt64(ConfigurationManager.AppSettings["ResponseTime"]))) :
                    session.Query<PvScTStatEntity>()
                        .Where(x => (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp) &&
                                    x.MaxResp > (_maxTimes * Convert.ToInt64(ConfigurationManager.AppSettings["ResponseTime"])) &&
                                    x.PathwayName == pathwayName);

                var results = query
                    .GroupBy(x => x.PathwayName)
                    .Select(g => new
                    {
                        PathwayName = g.Key,
                        Count = g.Select(a => a.ScName).Distinct().Count()
                    })
                    .OrderBy(x => x.PathwayName)
                    .ToList();

                foreach (var res in results)
                {
                    alerts.Add(new Alert
                    {
                        PathwayName = res.PathwayName.ToString(),
                        Count = Convert.ToInt64(res.Count)
                    });
                }

            }

            return alerts;
        }

        public List<Alert> GetServerHiAvgRt(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "")
        {
            var alerts = new List<Alert>();

            using (var session = NHibernateHelper.OpenSystemSession())
            {
                var query = pathwayName.Length == 0 ?
                    session.Query<PvScTStatEntity>()
                        .Where(x => (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp) &&
                                    x.AvgResp > (_maxTimes * Convert.ToInt64(ConfigurationManager.AppSettings["ResponseTime"]))) :
                    session.Query<PvScTStatEntity>()
                        .Where(x => (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp) &&
                                    x.AvgResp > (_maxTimes * Convert.ToInt64(ConfigurationManager.AppSettings["ResponseTime"])) &&
                                    x.PathwayName == pathwayName);

                var results = query
                    .GroupBy(x => x.PathwayName)
                    .Select(g => new
                    {
                        PathwayName = g.Key,
                        Count = g.Select(a => a.ScName).Distinct().Count()
                    })
                    .OrderBy(x => x.PathwayName)
                    .ToList();

                foreach (var res in results)
                {
                    alerts.Add(new Alert
                    {
                        PathwayName = res.PathwayName.ToString(),
                        Count = Convert.ToInt64(res.Count)
                    });
                }

            }

            return alerts;
        }

        public List<Alert> GetServerPendingClass(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "")
        {
            var alerts = new List<Alert>();

            using (var session = NHibernateHelper.OpenSystemSession())
            {
                var query = pathwayName.Length == 0 ?
                    session.Query<PvScPrStusEntity>()
                        .Where(x => (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp) &&
                                    x.ProcState == "1") :
                    session.Query<PvScPrStusEntity>()
                        .Where(x => (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp) &&
                                    x.ProcState == "1" &&
                                    x.PathwayName == pathwayName);

                var results = query
                    .GroupBy(x => x.PathwayName)
                    .Select(g => new
                    {
                        PathwayName = g.Key,
                        Count = g.Select(a => a.ScName).Distinct().Count()
                    })
                    .OrderBy(x => x.PathwayName)
                    .ToList();

                foreach (var res in results)
                {
                    alerts.Add(new Alert
                    {
                        PathwayName = res.PathwayName.ToString(),
                        Count = Convert.ToInt64(res.Count)
                    });
                }

            }

            return alerts;
        }

        public List<Alert> GetServerPendingProcess(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "")
        {
            var alerts = new List<Alert>();

            using (var session = NHibernateHelper.OpenSystemSession())
            {
                var query = pathwayName.Length == 0 ?
                    session.Query<PvScPrStusEntity>()
                        .Where(x => (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp) &&
                                    x.ProcState == "1") :
                    session.Query<PvScPrStusEntity>()
                        .Where(x => (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp) &&
                                    x.ProcState == "1" &&
                                    x.PathwayName == pathwayName);

                var results = query
                    .GroupBy(x => x.PathwayName)
                    .Select(g => new
                    {
                        PathwayName = g.Key,
                        Count = g.Select(a => a.ScProcessName).Distinct().Count()
                    })
                    .OrderBy(x => x.PathwayName)
                    .ToList();

                foreach (var res in results)
                {
                    alerts.Add(new Alert
                    {
                        PathwayName = res.PathwayName.ToString(),
                        Count = Convert.ToInt64(res.Count)
                    });
                }

            }

            return alerts;
        }

        public List<Alert> GetServerQueueTcp(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "")
        {
            var alerts = new List<Alert>();

            using (var session = NHibernateHelper.OpenSystemSession())
            {
                var query = pathwayName.Length == 0 ?
                    session.Query<PvScTStatEntity>()
                        .Where(x => (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp) &&
                                    x.QIWaits > 0) :
                    session.Query<PvScTStatEntity>()
                        .Where(x => (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp) &&
                                    x.QIWaits > 0 &&
                                    x.PathwayName == pathwayName);

                var results = query
                    .GroupBy(x => x.PathwayName)
                    .Select(g => new
                    {
                        PathwayName = g.Key,
                        Count = g.Select(a => a.ScName).Distinct().Count()
                    })
                    .OrderBy(x => x.PathwayName)
                    .ToList();

                foreach (var res in results)
                {
                    alerts.Add(new Alert
                    {
                        PathwayName = res.PathwayName.ToString(),
                        Count = Convert.ToInt64(res.Count)
                    });
                }

            }

            return alerts;
        }

        public List<Alert> GetServerQueueLinkmon(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "")
        {
            var alerts = new List<Alert>();

            using (var session = NHibernateHelper.OpenSystemSession())
            {
                var query = pathwayName.Length == 0 ?
                    session.Query<PvScLStatEntity>()
                        .Where(x => (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp) &&
                                    x.QIWaits > 0) :
                    session.Query<PvScLStatEntity>()
                        .Where(x => (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp) &&
                                    x.QIWaits > 0 &&
                                    x.PathwayName == pathwayName);

                var results = query
                    .GroupBy(x => x.PathwayName)
                    .Select(g => new
                    {
                        PathwayName = g.Key,
                        Count = g.Select(a => a.ScName).Distinct().Count()
                    })
                    .OrderBy(x => x.PathwayName)
                    .ToList();

                foreach (var res in results)
                {
                    alerts.Add(new Alert
                    {
                        PathwayName = res.PathwayName.ToString(),
                        Count = Convert.ToInt64(res.Count)
                    });
                }

            }

            return alerts;
        }

        public List<Alert> GetServerUnusedClass(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "")
        {
            var alerts = new List<Alert>();

            using (var session = NHibernateHelper.OpenSystemSession())
            {
                var temp1 = pathwayName.Length == 0 ?
                    session.Query<PvScTStatEntity>()
                        .Where(x => x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp)
                        .GroupBy(x => new { x.ScName, x.PathwayName })
                        .Select(g => new
                        {
                            g.Key.ScName,
                            g.Key.PathwayName,
                            SumofIsReqCnt = g.Sum(a => a.IsReqCnt)
                        }) :
                    session.Query<PvScTStatEntity>()
                        .Where(x => x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp && x.PathwayName == pathwayName)
                        .GroupBy(x => new { x.ScName, x.PathwayName })
                        .Select(g => new
                        {
                            g.Key.ScName,
                            g.Key.PathwayName,
                            SumofIsReqCnt = g.Sum(a => a.IsReqCnt)
                        });

                var temp2 = pathwayName.Length == 0 ?
                    session.Query<PvScLStatEntity>()
                        .Where(x => x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp)
                        .GroupBy(x => new { x.ScName, x.PathwayName })
                        .Select(g => new
                        {
                            g.Key.ScName,
                            g.Key.PathwayName,
                            SumofIsReqCnt = g.Sum(a => a.IsReqCnt)
                        }) :
                    session.Query<PvScLStatEntity>()
                        .Where(x => x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp && x.PathwayName == pathwayName)
                        .GroupBy(x => new { x.ScName, x.PathwayName })
                        .Select(g => new
                        {
                            g.Key.ScName,
                            g.Key.PathwayName,
                            SumofIsReqCnt = g.Sum(a => a.IsReqCnt)
                        });

                var combinedResults = from t1 in temp1
                                      from t2 in temp2.Where(t2 => t1.ScName == t2.ScName && t1.PathwayName == t2.PathwayName).DefaultIfEmpty()
                                      select new
                                      {
                                          ScName = t1.ScName ?? t2.ScName,
                                          PathwayName = t1.PathwayName ?? t2.PathwayName,
                                          Tran1 = t1.SumofIsReqCnt != 0 ? t1.SumofIsReqCnt : 0,
                                          Tran2 = t2.SumofIsReqCnt != 0 ? t2.SumofIsReqCnt : 0
                                      };

                var results = combinedResults
                    .Where(x => x.Tran1 == 0 && x.Tran2 == 0)
                    .GroupBy(x => x.PathwayName)
                    .Select(g => new
                    {
                        PathwayName = g.Key,
                        Count = g.Count()
                    })
                    .OrderBy(x => x.PathwayName)
                    .ToList();

                foreach (var res in results)
                {
                    alerts.Add(new Alert
                    {
                        PathwayName = res.PathwayName.ToString(),
                        Count = Convert.ToInt64(res.Count)
                    });
                }
            }

            return alerts;
        }

        public List<Alert> GetServerUnusedProcess(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "")
        {
            var alerts = new List<Alert>();

            using (var session = NHibernateHelper.OpenSystemSession())
            {
                var subquery = pathwayName.Length == 0 ?
                    from prstus in session.Query<PvScPrStusEntity>()
                    join info in session.Query<PvScInfoEntity>()
                    on new { prstus.PathwayName, prstus.ScName }
                    equals new { info.PathwayName, info.ScName } into infoGroup
                    from subInfo in infoGroup.DefaultIfEmpty() // Simulate LEFT OUTER JOIN
                    where prstus.FromTimestamp >= fromTimestamp && prstus.ToTimestamp <= toTimestamp &&
                          subInfo.FromTimestamp >= fromTimestamp && subInfo.ToTimestamp <= toTimestamp &&
                          prstus.ProcState == "2"
                    group prstus by new { prstus.PathwayName, prstus.ScName, prstus.ScProcessName, subInfo.MaxServers, subInfo.NumStatic } into g
                    where g.Sum(x => x.ProcLinks) == 0 // HAVING clause equivalent
                    select new
                    {
                        PathwayName = g.Key.PathwayName,
                        ScName = g.Key.ScName
                    } :
                    from prstus in session.Query<PvScPrStusEntity>()
                    join info in session.Query<PvScInfoEntity>()
                    on new { prstus.PathwayName, prstus.ScName }
                    equals new { info.PathwayName, info.ScName } into infoGroup
                    from subInfo in infoGroup.DefaultIfEmpty() // Simulate LEFT OUTER JOIN
                    where prstus.FromTimestamp >= fromTimestamp && prstus.ToTimestamp <= toTimestamp &&
                          subInfo.FromTimestamp >= fromTimestamp && subInfo.ToTimestamp <= toTimestamp &&
                          prstus.ProcState == "2" &&
                          prstus.PathwayName == pathwayName
                    group prstus by new { prstus.PathwayName, prstus.ScName, prstus.ScProcessName, subInfo.MaxServers, subInfo.NumStatic } into g
                    where g.Sum(x => x.ProcLinks) == 0 // HAVING clause equivalent
                    select new
                    {
                        PathwayName = g.Key.PathwayName,
                        ScName = g.Key.ScName
                    };

                var resultQuery = from sq in subquery
                                  group sq by sq.PathwayName into grouped
                                  select new
                                  {
                                      PathwayName = grouped.Key,
                                      Count = grouped.Count()
                                  };

                var results = resultQuery.ToList();

                foreach (var res in results)
                {
                    alerts.Add(new Alert
                    {
                        PathwayName = res.PathwayName.ToString(),
                        Count = Convert.ToInt64(res.Count)
                    });
                }
            }

            return alerts;
        }

        public List<Alert> GetServerMaxLinks(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "")
        {
            var alerts = new List<Alert>();

            using (var session = NHibernateHelper.OpenSystemSession())
            {
                var subQuery = from scl in session.Query<PvScLStatEntity>()
                               join sci in session.Query<PvScInfoEntity>() on new { scl.PathwayName, scl.ScName } equals new { sci.PathwayName, sci.ScName } into sciJoin
                               from sci in sciJoin.DefaultIfEmpty()
                               where scl.FromTimestamp >= fromTimestamp && scl.ToTimestamp <= toTimestamp &&
                                     sci.FromTimestamp >= fromTimestamp && sci.ToTimestamp <= toTimestamp
                               group sci by new { scl.PathwayName, scl.ScName, scl.FromTimestamp, scl.ToTimestamp } into g
                               select new
                               {
                                   PathwayName = g.Key.PathwayName,
                                   ScName = g.Key.ScName,
                                   MaxLinks = g.Max(x => x.MaxLinks)
                               };

                var finalQuery = from sq in subQuery
                                 group sq by sq.PathwayName into grouped
                                 let maxMaxLinks = grouped.Max(x => x.MaxLinks)
                                 where maxMaxLinks > 100
                                 select new
                                 {
                                     PathwayName = grouped.Key,
                                     ScNameCount = grouped.Select(x => x.ScName).Distinct().Count()
                                 };

                var results = finalQuery.OrderBy(x => x.PathwayName).ToList();

                foreach (var res in results)
                {
                    alerts.Add(new Alert
                    {
                        PathwayName = res.PathwayName.ToString(),
                        Count = Convert.ToInt64(res.ScNameCount)
                    });
                }
            }

            return alerts;
        }

        public List<Alert> GetCheckDirectoryOn(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "")
        {
            var alerts = new List<Alert>();

            using (var session = NHibernateHelper.OpenSystemSession())
            {
                var subquery = session.Query<PvTcpInfoEntity>()
                    .Where(x => (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp) &&
                                x.CheckDirectory != "0")
                    .GroupBy(x => new { x.PathwayName, x.TcpName })
                    .Select(g => new
                    {
                        PathwayName = g.Key.PathwayName,
                        TcpName = g.Key.TcpName
                    });

                var results = subquery
                    .GroupBy(x => x.PathwayName)
                    .Select(g => new
                    {
                        PathwayName = g.Key,
                        TcpNameCount = g.Select(a => a.TcpName).Count()
                    })
                    .OrderBy(x => x.PathwayName)
                    .ToList();

                foreach (var res in results)
                {
                    alerts.Add(new Alert
                    {
                        PathwayName = res.PathwayName.ToString(),
                        Count = Convert.ToInt64(res.TcpNameCount),
                        FromTimestamp = Convert.ToDateTime(fromTimestamp),
                        ToTimestamp = Convert.ToDateTime(toTimestamp)
                    });
                }

            }

            return alerts;
        }

        public List<Alert> GetHighDynamicServers(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "")
        {
            var alerts = new List<Alert>();

            using (var session = NHibernateHelper.OpenSystemSession())
            {
                var subQuery = from prstus in session.Query<PvScPrStusEntity>()
                               where prstus.FromTimestamp >= fromTimestamp && prstus.ToTimestamp <= toTimestamp
                               group prstus by new { prstus.PathwayName, prstus.ScName, prstus.FromTimestamp, prstus.ToTimestamp } into g
                               select new
                               {
                                   PathwayName = g.Key.PathwayName,
                                   ScName = g.Key.ScName,
                                   ScProcessCount = g.Select(a => a.ScProcessName).Count()
                               };

                var joinQuery = from t in subQuery
                                join info in session.Query<PvScInfoEntity>()
                                on new { t.PathwayName, t.ScName } equals new { info.PathwayName, info.ScName } into infoGroup
                                from subInfo in infoGroup.DefaultIfEmpty()
                                group new { t, subInfo } by new { t.PathwayName, t.ScName } into grouped
                                where grouped.Max(x => x.t.ScProcessCount) > grouped.Max(x => x.subInfo != null ? x.subInfo.NumStatic : 0)
                                select new
                                {
                                    PathwayName = grouped.Key.PathwayName,
                                    ScName = grouped.Key.ScName
                                };

                var finalQuery = from x in joinQuery
                                 group x by x.PathwayName into g
                                 select new
                                 {
                                     PathwayName = g.Key,
                                     SCNameCount = g.Count()
                                 };

                var results = finalQuery.OrderBy(x => x.PathwayName).ToList();

                foreach (var res in results)
                {
                    alerts.Add(new Alert
                    {
                        PathwayName = res.PathwayName,
                        Count = Convert.ToInt64(res.SCNameCount),
                        FromTimestamp = Convert.ToDateTime(fromTimestamp),
                        ToTimestamp = Convert.ToDateTime(toTimestamp)
                    });
                }
            }

            return alerts;
        }

        public List<Alert> GetServerErrorList(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "")
        {
            var alerts = new List<Alert>();

            using (var session = NHibernateHelper.OpenSystemSession())
            {
                var query = pathwayName.Length == 0 ?
                    session.Query<PvScStusEntity>()
                        .Where(x => (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp) &&
                                    x.ErrorNumber != 0) :
                    session.Query<PvScStusEntity>()
                        .Where(x => (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp) &&
                                    x.ErrorNumber != 0 &&
                                    x.PathwayName == pathwayName);

                var results = query
                    .GroupBy(x => x.PathwayName)
                    .Select(g => new
                    {
                        PathwayName = g.Key,
                        Count = g.Select(a => a.ScName).Distinct().Count()
                    })
                    .OrderBy(x => x.PathwayName)
                    .ToList();

                foreach (var res in results)
                {
                    alerts.Add(new Alert
                    {
                        PathwayName = res.PathwayName.ToString(),
                        Count = Convert.ToInt64(res.Count)
                    });
                }
            }

            return alerts;
        }

        #endregion
    }
}
