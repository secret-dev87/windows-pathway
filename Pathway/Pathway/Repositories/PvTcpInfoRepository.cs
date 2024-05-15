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
    internal class PvTcpInfoRepository : IPvTcpInfoRepository
    {
        public PvTcpInfoRepository() { }

        public Dictionary<string, double> GetTermAverageResponseTime(string pathwayName, DateTime fromTimestamp, DateTime toTimestamp)
        {
            var termAverageResponseTime = new Dictionary<string, double>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var result = session.Query<PvTermStatEntity>()
                    .Where(x => x.PathwayName == pathwayName && (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp))
                    .GroupBy(x => new { x.PathwayName, x.TermTcpName })
                    .Select(g => new
                    {
                        AvgResp = g.Average(a => a.AvgResp.Value),
                        PathwayName = g.Key.PathwayName,
                        TermTcpName = g.Key.TermTcpName
                    })
                    .ToList();

                foreach(var res in result)
                {
                    termAverageResponseTime.Add(res.TermTcpName.ToString(), Convert.ToInt64(res.AvgResp));
                }
            }

            return termAverageResponseTime;
        }

        public Dictionary<string, long> GetTermCount(string pathwayName, DateTime fromTimestamp, DateTime toTimestamp)
        {
            var termCount = new Dictionary<string, long>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var subQuery = session.Query<PvTermStusEntity>()
                    .Where(x => x.PathwayName == pathwayName && (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp))
                    .GroupBy(x => new {x.PathwayName, x.TcpName })
                    .Select(g => new
                    {
                        PathwayName = g.Key.PathwayName,
                        TcpName = g.Key.TcpName,
                        TermCount = g.Count(a => a.TermName != null)
                    });

                var resultQuery = subQuery
                    .GroupBy(x => new { x.PathwayName, x.TcpName })
                    .Select(g => new
                    {
                        PathwayName = g.Key.PathwayName,
                        TcpName = g.Key.TcpName,
                        AveofTermCount = g.Average(x => x.TermCount)
                    })
                    .ToList();

                foreach(var item in resultQuery)
                {
                    termCount.Add(item.TcpName, Convert.ToInt64(item.AveofTermCount));
                }
            }

            return termCount;
        }

        public Dictionary<string, double> GetTermCPUBusy(string pathwayName, DateTime fromTimestamp, DateTime toTimestamp)
        {
            var termCPUBusy = new Dictionary<string, double>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var pvTcpstus = session.Query<PvTcpStusEntity>()
                    .Where(x => x.PathwayName == pathwayName && (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp))
                    .GroupBy(x => x.TcpName)
                    .Select(g => new
                    {
                        SumOfDeltaProcTime = g.Sum(a => a.DeltaProcTime.Value),
                        TcpName = g.Key
                    })
                    .ToList();

                foreach (var pvTcpS in pvTcpstus)
                {
                    termCPUBusy.Add(pvTcpS.TcpName.ToString(), Convert.ToDouble(pvTcpS.SumOfDeltaProcTime));
                }
            }

            return termCPUBusy;
        }

        public Dictionary<string, long> GetTermServerTransaction(string pathwayName, DateTime fromTimestamp, DateTime toTimestamp)
        {
            var termServerTransaction = new Dictionary<string, long>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var pvSctstats = session.Query<PvScTStatEntity>()
                    .Where(x => x.PathwayName == pathwayName && (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp))
                    .GroupBy(x => new { x.PathwayName, x.ScTcpName })
                    .Select(g => new
                    {
                        SumOfIsReqcnt = g.Sum(a => a.IsReqCnt.Value),
                        PathwayName = g.Key.PathwayName,
                        ScTcpName = g.Key.ScTcpName
                    })
                    .ToList();

                foreach(var pvSctstat in pvSctstats)
                {
                    termServerTransaction.Add(pvSctstat.ScTcpName.ToString(), Convert.ToInt64(pvSctstat.SumOfIsReqcnt));
                }
            }

            return termServerTransaction;
        }

        public Dictionary<string, long> GetTermTransaction(string pathwayName, DateTime fromTimestamp, DateTime toTimestamp)
        {
            var termTransaction = new Dictionary<string, long>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var pvTermstats = session.Query<PvTermStatEntity>()
                    .Where(x => (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp) && x.PathwayName == pathwayName)
                    .GroupBy(x => new { x.PathwayName, x.TermTcpName })
                    .Select(g => new
                    {
                        SumOfIsReqcnt = g.Sum(a => a.IsReqCnt.Value),
                        PathwayName = g.Key.PathwayName,
                        TermTcpName = g.Key.TermTcpName,
                    })
                    .OrderBy(x => x.SumOfIsReqcnt)
                    .Take(10);

                foreach(var pvTermstat in pvTermstats)
                {
                    termTransaction.Add(pvTermstat.TermTcpName.ToString(), Convert.ToInt64(pvTermstat.SumOfIsReqcnt));
                }
            }

            return termTransaction;
        }
    }
}
