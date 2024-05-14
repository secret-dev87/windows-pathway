using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pathway.Core.Abstract.Repositories;
using Pathway.Core.Entity;
using Pathway.Core.Helper;
using Pathway.Core.Infrastructure.PerPathway.Server;

namespace Pathway.Core.Repositories
{
    internal class PvScStusRepository : IPvScStusRepository
    {
        public PvScStusRepository() { }

        public int GetFreezeState(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, string serverClassName)
        {
            var freezeState = 0;

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var result = session.Query<PvScStusEntity>()
                    .Where(x => x.FromTimestamp >= fromTimestamp &&
                                x.ToTimestamp <= toTimestamp &&
                                x.PathwayName == pathwayName &&
                                x.ScName == serverClassName)
                    .OrderByDescending(x => x.FromTimestamp)
                    .First();

                freezeState = Convert.ToInt32(result.FreezeState);
            }

            return freezeState;
        }

        public Dictionary<string, int> GetFreezeState(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName)
        {
            var freezeStates = new Dictionary<string, int>();

            using (var session = NHibernateHelper.OpenSystemSession())
            {
                var result = session.Query<PvScStusEntity>()
                    .Where(x => x.FromTimestamp >= fromTimestamp &&
                                x.ToTimestamp <= toTimestamp &&
                                x.PathwayName == pathwayName)
                    .OrderByDescending(x => x.FromTimestamp)
                    .ToList();

                foreach(var res in result)
                {
                    freezeStates.Add(res.ScName.ToString(), Convert.ToInt32(res.FreezeState));
                }
            }

            return freezeStates;
        }

        public List<ServerErrorListView> GetServerErrorLists(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName)
        {
            var serverUnusedProcesses = new List<ServerErrorListView>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var innerQuery = session.Query<PvScStusEntity>()
                    .Where(x => x.FromTimestamp >= fromTimestamp &&
                                x.ToTimestamp <= toTimestamp &&
                                x.PathwayName == pathwayName &&
                                x.ErrorNumber != 0)
                    .GroupBy(x => new { x.PathwayName, x.ScName, x.ToTimestamp })
                    .Select(g => new
                    {
                        PathwayName = g.Key.PathwayName,
                        ScName = g.Key.ScName,
                        ToTimestamp = g.Key.ToTimestamp,
                        CountofError = g.Select(a => a.ErrorNumber).Distinct().Count()
                    });

                var query2 = innerQuery
                    .GroupBy(x => x.ScName)
                    .Select(g => new
                    {
                        ScName = g.Key,
                        MaxofCountofError = g.Max(a => a.CountofError),
                        SumofCountofError = g.Sum(a => a.CountofError)
                    });

                var query3 = innerQuery;

                var resultQuery = from q2 in query2
                                  join q3 in query3
                                  on new { q2.ScName, q2.MaxofCountofError } equals new { q3.ScName, MaxofCountofError = q3.CountofError }
                                  group q3 by new { q2.ScName, q2.MaxofCountofError, q2.SumofCountofError } into g
                                  select new
                                  {
                                      g.Key.ScName,
                                      g.Key.MaxofCountofError,
                                      g.Key.SumofCountofError,
                                      MinofCollectionTime = g.Min(a => a.ToTimestamp)
                                  };

                var finalResults = resultQuery.OrderBy(x => x.ScName).ToList();

                foreach(var res in finalResults)
                {
                    var view = new ServerErrorListView
                    {
                        ServerClass = res.ScName.ToString(),
                        MostRecentTime = Convert.ToDateTime(res.MinofCollectionTime),
                        Instances = Convert.ToInt32(res.SumofCountofError)
                    };

                    serverUnusedProcesses.Add(view);
                }
            }

            return serverUnusedProcesses;
        }

        public List<ServerErrorView> GetServerErrorListSub(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName)
        {
            var serverUnusedProcesses = new List<ServerErrorView>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var result = session.Query<PvScStusEntity>()
                    .Where(x => x.FromTimestamp >= fromTimestamp &&
                                x.ToTimestamp <= toTimestamp &&
                                x.PathwayName == pathwayName &&
                                x.ErrorNumber != 0)
                    .OrderBy(x => new { x.ScName, x.FromTimestamp })
                    .ToList();

                foreach (var res in result)
                {
                    var view = new ServerErrorView
                    {
                        ServerClass = res.ScName.ToString(),
                        MostRecentTime = Convert.ToDateTime(res.FromTimestamp),
                        ErrorNumber = Convert.ToInt32(res.ErrorNumber)
                    };

                    serverUnusedProcesses.Add(view);
                }
            }

            return serverUnusedProcesses;
        }

        public List<string> GetServerPendingClass(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName)
        {
            var serverPendingClasses = new List<String>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var result = session.Query<PvScPrStusEntity>()
                    .Where(x => x.FromTimestamp >= fromTimestamp &&
                                x.ToTimestamp <= toTimestamp &&
                                x.PathwayName == pathwayName &&
                                x.ProcState == "1")
                    .Select(x => x.ScName)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();

                foreach (var res in result)
                {
                    serverPendingClasses.Add(res);
                }
            }

            return serverPendingClasses;
        }
    }
}
