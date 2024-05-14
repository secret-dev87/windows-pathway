using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pathway.Core.Abstract;
using Pathway.Core.Abstract.Repositories;
using Pathway.Core.Concrete;
using Pathway.Core.Entity;
using Pathway.Core.Helper;
using Pathway.Core.Infrastructure;
using Pathway.Core.Infrastructure.PerPathway.Server;

namespace Pathway.Core.Repositories
{
    internal class PvScPrStusRepository : IPvScPrStusRepository
    {
        public PvScPrStusRepository() { }

        public List<CheckDirectoryON> GetCheckDirectoryOnDetail(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName)
        {
            var checkDirectoryON = new List<CheckDirectoryON>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var subquery = session.Query<PvTcpInfoEntity>()
                    .Where(x => (x.FromTimestamp >= fromTimestamp && x.FromTimestamp < toTimestamp) &&
                                (x.ToTimestamp > fromTimestamp && x.ToTimestamp <= toTimestamp) &&
                                x.PathwayName == pathwayName &&
                                x.CheckDirectory != "0")
                    .GroupBy(x => new { x.PathwayName, x.TcpName })
                    .Select(g => new
                    {
                        PathwayName = g.Key.PathwayName,
                        TcpName = g.Key.TcpName
                    });

                var result = subquery
                    .GroupBy(x => x.TcpName)
                    .Select(g => new
                    {
                        TcpName = g.Key.Distinct()
                    })
                    .OrderBy(x => x.TcpName)
                    .ToList();

                foreach(var res in result)
                {
                    var view = new CheckDirectoryON
                    {
                        ServerClass = res.TcpName.ToString()
                    };

                    checkDirectoryON.Add(view);
                }
            }

            return checkDirectoryON;
        }

        public Dictionary<string, double> GetCPUBusy(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName)
        {
            var cpuBusies = new Dictionary<string, double>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var results = session.Query<PvScPrStusEntity>()
                    .Where(x => (x.FromTimestamp >= fromTimestamp && x.FromTimestamp < toTimestamp) &&
                                (x.ToTimestamp > fromTimestamp && x.ToTimestamp <= toTimestamp) &&
                                x.PathwayName == pathwayName)
                    .GroupBy(x => x.ScName)
                    .Select(g => new
                    {
                        ScName = g.Key,
                        TotalBusyTime = g.Sum(a => a.DeltaProcTime)
                    })
                    .ToList();

                foreach(var res in results)
                {
                    cpuBusies.Add(res.ScName.ToString(), Convert.ToDouble(res.TotalBusyTime));
                }
            }

            return cpuBusies;
        }

        public List<CPUBusyPerInterval> GetCPUBusyPercentPerInterval(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName)
        {
            var cpuBusies = new List<CPUBusyPerInterval>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var results = session.Query<PvScPrStusEntity>()
                    .Where(x => (x.FromTimestamp >= fromTimestamp && x.FromTimestamp < toTimestamp) &&
                                (x.ToTimestamp > fromTimestamp && x.ToTimestamp <= toTimestamp) &&
                                x.PathwayName == pathwayName)
                    .GroupBy(x => new { x.ScName, x.FromTimestamp })
                    .Select(g => new
                    {
                        ScName = g.Key.ScName,
                        TotalBusyTime = g.Sum(a => a.DeltaProcTime) / g.Sum(a => a.DeltaTime) * 100,
                        FromTimestamp = g.Key.FromTimestamp
                    })
                    .ToList();

                foreach(var res in results)
                {
                    cpuBusies.Add(new CPUBusyPerInterval
                    {
                        ScName = res.ScName.ToString(),
                        TotalBusyTime = Convert.ToDouble(res.TotalBusyTime),
                        FromTimestamp = Convert.ToDateTime(res.FromTimestamp)
                    });
                }
            }

            return cpuBusies;
        }

        public List<CPUBusyPerInterval> GetCPUBusyPerInterval(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName)
        {
            var cpuBusies = new List<CPUBusyPerInterval>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var results = session.Query<PvScPrStusEntity>()
                    .Where(x => (x.FromTimestamp >= fromTimestamp && x.FromTimestamp < toTimestamp) &&
                                (x.ToTimestamp > fromTimestamp && x.ToTimestamp <= toTimestamp) &&
                                x.PathwayName == pathwayName)
                    .GroupBy(x => new { x.ScName, x.FromTimestamp })
                    .Select(g => new
                    {
                        ScName = g.Key,
                        TotalBusyTime = g.Sum(a => a.DeltaProcTime),
                        FromTimestamp = g.Key,
                    })
                    .ToList();

                foreach (var res in results)
                {
                    cpuBusies.Add(new CPUBusyPerInterval
                    {
                        ScName = res.ScName.ToString(),
                        TotalBusyTime = Convert.ToDouble(res.TotalBusyTime),
                        FromTimestamp = Convert.ToDateTime(res.FromTimestamp)
                    });
                }
            }

            return cpuBusies;
        }

        public List<HighDynamicServers> GetHighDynamicServersDetail(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName)
        {
            var highDynamicServers = new List<HighDynamicServers>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var subQuery = from prstus in session.Query<PvScPrStusEntity>()
                               where prstus.PathwayName == pathwayName &&
                                     prstus.FromTimestamp >= fromTimestamp && prstus.FromTimestamp < toTimestamp &&
                                     prstus.ToTimestamp > fromTimestamp && prstus.ToTimestamp <= toTimestamp
                               group prstus by new { prstus.PathwayName, prstus.ScName } into g
                               select new
                               {
                                   g.Key.PathwayName,
                                   g.Key.ScName,
                                   ProcessCount = g.Count()
                               };

                var query = from sq in subQuery
                            join info in session.Query<PvScInfoEntity>() on new { sq.PathwayName, sq.ScName } equals new { info.PathwayName, info.ScName } into infoGroup
                            from subInfo in infoGroup.DefaultIfEmpty() // Simulate LEFT OUTER JOIN
                            group new { sq, subInfo } by sq.ScName into grouped
                            let maxProcessCount = grouped.Max(x => x.sq.ProcessCount)
                            let maxNumStatic = grouped.Max(x => x.subInfo != null ? x.subInfo.NumStatic : 0)
                            let maxMaxServers = grouped.Max(x => x.subInfo != null ? x.subInfo.MaxServers : 0)
                            where maxProcessCount > maxNumStatic
                            orderby grouped.Key
                            select new
                            {
                                ScName = grouped.Key,
                                ProcessCount = maxProcessCount,
                                NumStatic = maxNumStatic,
                                MaxServers = maxMaxServers
                            };

                // Execute the query
                var results = query.ToList();

                foreach(var res in results)
                {
                    var view = new HighDynamicServers
                    {
                        ServerClass = res.ScName.ToString(),
                        ProcessCount = Convert.ToInt32(res.ProcessCount),
                        NumStatic = Convert.ToInt32(res.NumStatic),
                        MaxServers = Convert.ToInt32(res.MaxServers)
                    };
                    highDynamicServers.Add(view);
                }
            }

            return highDynamicServers;
        }

        public long GetServerBusy(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName)
        {
            long serverBusy = 0;

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var result = session.Query<PvScPrStusEntity>()
                    .Where(x => (x.FromTimestamp >= fromTimestamp && x.FromTimestamp < toTimestamp) &&
                                (x.ToTimestamp > fromTimestamp && x.ToTimestamp <= toTimestamp) &&
                                x.PathwayName == pathwayName)
                    .Sum(a => a.DeltaProcTime);

                serverBusy = Convert.ToInt64(result);
            }

            return serverBusy;
        }

        public List<CPUView> GetServerBusyPerCPU(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName)
        {
            var tcpProcessBusy = new List<CPUView>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var result = session.Query<PvScPrStusEntity>()
                    .Where(x => (x.FromTimestamp >= fromTimestamp && x.FromTimestamp < toTimestamp) &&
                                (x.ToTimestamp > fromTimestamp && x.ToTimestamp <= toTimestamp) &&
                                x.PathwayName == pathwayName &&
                                x.ScprCpu != "" &&
                                x.ScprCpu != "\0\0")
                    .GroupBy(x => x.ScprCpu)
                    .Select(g => new
                    {
                        ScPrCpu = g.Key,
                        TotalBusyTime = g.Sum(a => a.DeltaProcTime)
                    })
                    .OrderBy(x => x.ScPrCpu)
                    .ToList();

                foreach(var res in result)
                {
                    var cpuView = new CPUView
                    {
                        CPUNumber = res.ScPrCpu.ToString(),
                        Value = Convert.ToInt64(res.TotalBusyTime)
                    };

                    tcpProcessBusy.Add(cpuView);
                }
            }

            return tcpProcessBusy;
        }

        public Dictionary<string, List<CPUView>> GetServerBusyPerCPUPerPathway(DateTime fromTimestamp, DateTime toTimestamp)
        {
            var views = new Dictionary<string, List<CPUView>>();

            using (var session = NHibernateHelper.OpenSystemSession())
            {
                var result = session.Query<PvScPrStusEntity>()
                    .Where(x => (x.FromTimestamp >= fromTimestamp && x.FromTimestamp < toTimestamp) &&
                                (x.ToTimestamp > fromTimestamp && x.ToTimestamp <= toTimestamp) &&
                                x.ScprCpu != "" &&
                                x.ScprCpu != "\0\0")
                    .GroupBy(x => new { x.ScprCpu, x.PathwayName })
                    .Select(g => new
                    {
                        ScPrCpu = g.Key.ScprCpu,
                        TotalBusyTime = g.Sum(a => a.DeltaProcTime),
                        PathwayName = g.Key.PathwayName
                    })
                    .OrderBy(x => x.ScPrCpu)
                    .ToList();

                foreach (var res in result)
                {
                    var cpuview = new CPUView
                    {
                        CPUNumber = res.PathwayName.ToString(),
                        Value = Convert.ToInt64(res.TotalBusyTime)
                    };

                    string cpuNumber = res.ScPrCpu.ToString();

                    if (!views.ContainsKey(cpuNumber))
                        views.Add(res.ScPrCpu.ToString(), new List<CPUView> { cpuview });
                    else
                        views[cpuNumber].Add(cpuview);
                }
            }

            return views;
        }

        public List<CPUView> GetServerBusyPerPathway(DateTime fromTimestamp, DateTime toTimestamp)
        {
            var serverBusy = new List<CPUView>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var result = session.Query<PvScPrStusEntity>()
                    .Where(x => (x.FromTimestamp >= fromTimestamp && x.FromTimestamp < toTimestamp) &&
                                (x.ToTimestamp > fromTimestamp && x.ToTimestamp <= toTimestamp))
                    .GroupBy(x => x.PathwayName)
                    .Select(g => new
                    {
                        TotalBusyTime = g.Sum(a => a.DeltaProcTime),
                        PathwayName = g.Key
                    })
                    .ToList();

                foreach(var res in result)
                {
                    var cpuview = new CPUView
                    {
                        CPUNumber = res.PathwayName.ToString(),
                        Value = Convert.ToInt64(res.TotalBusyTime)
                    };
                    serverBusy.Add(cpuview);
                }
            }

            return serverBusy;
        }

        public List<CPUView> GetServerCPUBusyProcessCount(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName)
        {
            var serverCPUBusy = new List<CPUView>();

            using (var session = NHibernateHelper.OpenSystemSession())
            {
                var query1 = session.Query<PvScPrStusEntity>()
                    .Where(x => (x.FromTimestamp >= fromTimestamp && x.FromTimestamp < toTimestamp) &&
                                (x.ToTimestamp > fromTimestamp && x.ToTimestamp <= toTimestamp))
                    .GroupBy(x => new { x.PathwayName, x.ScName, x.FromTimestamp })
                    .Select(g => new
                    {
                        PathwayName = g.Key.PathwayName,
                        ScName = g.Key.ScName,
                        ScProcessCount = g.Select(a => a.ScProcessName).Distinct().Count()
                    });

                var result = query1
                    .Where(x => x.PathwayName == pathwayName)
                    .GroupBy(x => new { x.PathwayName, x.ScName })
                    .Select(g => new
                    {
                        AvgofScProcessCount = g.Average(a => a.ScProcessCount),
                        MaxofScProcessCount = g.Max(a => a.ScProcessCount),
                        PathwayName = g.Key.PathwayName,
                        ScName = g.Key.ScName
                    })
                    .ToList();

                foreach (var res in result)
                {
                    var cpuView = new CPUView
                    {
                        CPUNumber = res.ScName.ToString(),
                        Value = Convert.ToInt64(res.AvgofScProcessCount),
                        Value2 = Convert.ToInt64(res.MaxofScProcessCount)
                    };

                    serverCPUBusy.Add(cpuView);
                }
            }

            return serverCPUBusy;
        }

        public List<ServerMaxLinks> GetServerMaxLinks(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName)
        {
            var serverMaxLinksProcesses = new List<ServerMaxLinks>();

            using (var session = NHibernateHelper.OpenSystemSession())
            {
                var subQuery = from scl in session.Query<PvScLStatEntity>()
                               join sci in session.Query<PvScInfoEntity>() on new { scl.PathwayName, scl.ScName } equals new { sci.PathwayName, sci.ScName } into sciGroup
                               from sci in sciGroup.DefaultIfEmpty() // Simulate LEFT OUTER JOIN
                               where scl.PathwayName == pathwayName &&
                                     scl.FromTimestamp >= fromTimestamp && scl.FromTimestamp < toTimestamp &&
                                     scl.ToTimestamp > fromTimestamp && scl.ToTimestamp <= toTimestamp &&
                                     (sci == null || (sci.FromTimestamp >= fromTimestamp && sci.FromTimestamp < toTimestamp &&
                                                       sci.ToTimestamp > fromTimestamp && sci.ToTimestamp <= toTimestamp))
                               group new { scl, sci } by scl.ScName into g
                               select new
                               {
                                   ScName = g.Key,
                                   MaxLinks = g.Max(x => x.sci != null ? x.sci.MaxLinks : 0),
                                   LinksUsed = g.Sum(x => x.scl.ScLmName != null ? 1 : 0)
                               };

                var query = from sq in subQuery
                            group sq by sq.ScName into grouped
                            let maxMaxLinks = grouped.Max(x => x.MaxLinks)
                            where maxMaxLinks > 100
                            orderby grouped.Key
                            select new
                            {
                                ScName = grouped.Key,
                                LinkUsed = grouped.Max(x => x.LinksUsed),
                                MaxLinks = maxMaxLinks
                            };

                var results = query.ToList();

                foreach (var res in results)
                {
                    var view = new ServerMaxLinks
                    {
                        ServerClass = res.ScName.ToString(),
                        LinksUsed = Convert.ToInt32(res.LinkUsed),
                        MaxLinks = Convert.ToInt32(res.MaxLinks)
                    };
                    serverMaxLinksProcesses.Add(view);
                }
            }

            return serverMaxLinksProcesses;
        }

        public List<ServerUnusedServerProcessesView> GetServerPendingProcess(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName)
        {
            var serverUnusedProcesses = new List<ServerUnusedServerProcessesView>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var results = session.Query<PvScPrStusEntity>()
                    .Where(x => (x.FromTimestamp >= fromTimestamp && x.FromTimestamp < toTimestamp) &&
                                (x.ToTimestamp > fromTimestamp && x.ToTimestamp <= toTimestamp) &&
                                x.ProcState == "1" &&
                                x.PathwayName == pathwayName)
                    .OrderBy(x => x.ScName)
                    .ToList();

                foreach(var res in results)
                {
                    var view = new ServerUnusedServerProcessesView
                    {
                        ServerClass = res.ScName.ToString(),
                        Process = res.ScProcessName.ToString()
                    };

                    serverUnusedProcesses.Add(view);
                }
            }

            return serverUnusedProcesses;
        }

        public List<ServerUnusedServerProcessesView> GetServerUnusedServerProcessPerPathway(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName)
        {
            var serverUnusedProcesses = new List<ServerUnusedServerProcessesView>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var subquery = session.Query<PvScPrStusEntity>()
                    .Where(x => x.PathwayName == pathwayName &&
                                (x.FromTimestamp >= fromTimestamp && x.FromTimestamp < toTimestamp) &&
                                (x.ToTimestamp > fromTimestamp && x.ToTimestamp <= toTimestamp))
                    .GroupBy(x => new { x.ScName, x.ScProcessName })
                    .Select(g => new
                    {
                        ScName = g.Key.ScName,
                        ScProcessName = g.Key.ScProcessName,
                        SumofProcLinks = g.Sum(a => a.ProcLinks)
                    });

                var result = subquery
                    .Where(x => x.SumofProcLinks == 0)
                    .OrderBy(x => x.ScName)
                    .ToList();

                foreach(var res in result)
                {
                    var view = new ServerUnusedServerProcessesView
                    {
                        ServerClass = res.ScName.ToString(),
                        Process = res.ScProcessName.ToString()
                    };
                    serverUnusedProcesses.Add(view);
                }                    
            }

            return serverUnusedProcesses;
        }

        public List<UnusedServerProcesses> GetUnusedServerProcesseses(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName)
        {
            var serverUnusedProcesses = new List<UnusedServerProcesses>();

            using (var session = NHibernateHelper.OpenSystemSession())
            {
                var query = from prstus in session.Query<PvScPrStusEntity>()
                            join info in session.Query<PvScInfoEntity>() on new { prstus.PathwayName, prstus.ScName }
                            equals new { info.PathwayName, info.ScName } into infoGroup
                            from subInfo in infoGroup.DefaultIfEmpty() // Simulate LEFT OUTER JOIN
                            where prstus.PathwayName == pathwayName && prstus.ProcState == "2" &&
                                  prstus.FromTimestamp >= fromTimestamp && prstus.FromTimestamp < toTimestamp &&
                                  prstus.ToTimestamp > fromTimestamp && prstus.ToTimestamp <= toTimestamp &&
                                  subInfo.FromTimestamp >= fromTimestamp && subInfo.FromTimestamp < toTimestamp &&
                                  subInfo.ToTimestamp > fromTimestamp && subInfo.ToTimestamp <= toTimestamp
                            group new { prstus, subInfo } by new { prstus.PathwayName, prstus.ScName, prstus.ScProcessName, subInfo.MaxServers, subInfo.NumStatic } into g
                            where g.Sum(x => x.prstus.ProcLinks) == 0
                            orderby g.Key.ScName
                            select new
                            {
                                ScName = g.Key.ScName,
                                ScProcessName = g.Key.ScProcessName,
                                MaxServers = g.Key.MaxServers,
                                NumStatic = g.Key.NumStatic
                            };

                var result = query.ToList();

                foreach (var res in result)
                {
                    var view = new UnusedServerProcesses
                    {
                        ServerClass = res.ScName.ToString(),
                        Process = res.ScProcessName.ToString(),
                        MaxServers = Convert.ToInt32(res.MaxServers),
                        NumStatic = Convert.ToInt32(res.NumStatic)
                    };
                    serverUnusedProcesses.Add(view);
                }
            }

            return serverUnusedProcesses;
        }
    }
}
