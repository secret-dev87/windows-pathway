using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pathway.Core.Abstract.Repositories;
using Pathway.Core.Entity;
using Pathway.Core.Helper;
using Pathway.Core.Infrastructure;

namespace Pathway.Core.Repositories
{
    internal class PvPwyManyRepository : IPvPwyManyRepository
    {
        public long GetPathmonProcessBusy(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName)
        {
            long pathmonProcessBusy = 0;

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var TotalBusyTime = session.Query<PvPwyManyEntity>()
                    .Where(x => (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp) && x.PathwayName == pathwayName)
                    .Sum(x => x.DeltaProcTime);

                pathmonProcessBusy = Convert.ToInt64(TotalBusyTime);
            }

            return pathmonProcessBusy;
        }

        public List<CPUView> GetPathmonProcessBusyPerCPU(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName)
        {
            var pathmonProcessBusy = new List<CPUView>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var pwymanys = session.Query<PvPwyManyEntity>()
                    .Where(x => (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp) && x.PathwayName == pathwayName)
                    .GroupBy(x => x.PwyCpu)
                    .Select(g => new
                    {
                        PwyCpu = g.Key,
                        TotalBusyTime = g.Sum(a => a.DeltaProcTime.Value)
                    })
                    .OrderBy(PwyCpu => PwyCpu)
                    .ToList();

                foreach(var pwymany in pwymanys)
                {
                    var cpuView = new CPUView
                    {
                        CPUNumber = pwymany.PwyCpu.ToString(),
                        Value = Convert.ToInt64(pwymany.TotalBusyTime)
                    };
                    pathmonProcessBusy.Add(cpuView);
                }
            }

            return pathmonProcessBusy;
        }

        public Dictionary<string, List<CPUView>> GetPathmonProcessBusyPerCPUPerPathway(DateTime fromTimestamp, DateTime toTimestamp)
        {
            var views = new Dictionary<string, List<CPUView>>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var pwymanys = session.Query<PvPwyManyEntity>()
                    .Where(x => (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp) && x.DeltaProcTime < x.DeltaTime)
                    .GroupBy(x => new { x.PwyCpu, x.PathwayName })
                    .Select(g => new
                    {
                        PwyCpu = g.Key.PwyCpu,
                        PathwayName = g.Key.PathwayName,
                        TotalBusyTime = g.Sum(a => a.DeltaProcTime.Value)
                    })
                    .OrderBy(PwyCpu => PwyCpu)
                    .ToList();

                foreach(var pwymany in pwymanys)
                {
                    var view = new CPUView
                    {
                        CPUNumber = pwymany.PathwayName.ToString(),
                        Value = Convert.ToInt64(pwymany.TotalBusyTime)
                    };
                    string cpuNumber = pwymany.PwyCpu.ToString();
                    if (!views.ContainsKey(cpuNumber))
                    {
                        views.Add(pwymany.PwyCpu.ToString(), new List<CPUView> { view });
                    }
                    else
                    {
                        views[cpuNumber].Add(view);
                    }
                }
            }

            return views;
        }

        public List<CPUView> GetPathmonProcessBusyPerPathway(DateTime fromTimestamp, DateTime toTimestamp)
        {
            var pathmonProcessBusy = new List<CPUView>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var pwymanys = session.Query<PvPwyManyEntity>()
                    .Where(x => x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp)
                    .GroupBy(x => x.PathwayName)
                    .Select(g => new
                    {
                        PathwayName = g.Key,
                        TotalBusyTime = g.Sum(a => a.DeltaProcTime.Value)
                    })
                    .ToList();

                foreach(var pwymany in pwymanys)
                {
                    var cpuView = new CPUView
                    {
                        CPUNumber = pwymany.PathwayName.ToString(),
                        Value = Convert.ToInt64(pwymany.TotalBusyTime)
                    };
                    pathmonProcessBusy.Add(cpuView);
                }
            }

            return pathmonProcessBusy;
        }

        public string GetPathwayName(DateTime fromTimestamp, DateTime toTimestamp)
        {
            var pathwayNameList = new List<string>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var pwymanys = session.Query<PvPwyManyEntity>()
                    .Where(x => x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp)
                    .GroupBy(x => x.PathwayName)
                    .Select(g => new
                    {
                        PathwayName = g.Key
                    })
                    .ToList();

                foreach(var pwymany in pwymanys)
                {
                    pathwayNameList.Add("'" + pwymany.PathwayName.ToString() + "'");
                }
            }

            return string.Join(",", pathwayNameList);
        }
    }
}
