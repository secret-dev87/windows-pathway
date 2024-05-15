using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pathway.Core.Abstract.Repositories;
using Pathway.Core.Entity;
using Pathway.Core.Helper;
using Pathway.Core.Infrastructure.AllPathway;

namespace Pathway.Core.Repositories
{
    internal class PvCPUManyRepository : IPvCPUManyRepository
    {
        public List<string> GetCPUCount(DateTime fromTimestamp, DateTime toTimestamp)
        {
            var cpus = new List<string>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var cpuManys = session.Query<PvCPUManyEntity>()
                    .Where(x => x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp)
                    .Select(a => a.CpuNumber)
                    .Distinct()
                    .OrderBy(CpuNumber => CpuNumber)                    
                    .ToList();

                foreach (var cpu in cpuManys)
                {
                    cpus.Add(cpu.ToString());
                }
            }
            return cpus;
        }

        public double GetCPUElapse(DateTime fromTimestamp, DateTime toTimestamp)
        {
            double cpuElapse = 0;

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                double cpuElapseValue = 0;

                var CpuElapse = session.Query<PvCPUManyEntity>()
                    .Where(x => x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp)
                    .Sum(x => x.MElapsedTime);

                cpuElapseValue = Convert.ToDouble(CpuElapse);
                if (cpuElapseValue == 0)
                    cpuElapseValue = 0.01;

                cpuElapse = cpuElapseValue;
            }

            return cpuElapse;
        }

        public Dictionary<string, CPUDetailElapseAndBusyTimeView> GetCPUElapseAndBusyPercentPerCPU(DateTime fromTimestamp, DateTime toTimestamp, int ipus)
        {
            var views = new Dictionary<string, CPUDetailElapseAndBusyTimeView>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var cpuManys = session.Query<PvCPUManyEntity>()
                    .Where(x => x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp)
                    .GroupBy(x => x.CpuNumber)
                    .Select(g => new
                    {
                        CpuNumber = g.Key,
                        TotalElapsedTime = g.Sum(a => a.MElapsedTime.Value),
                        TotalBusyTime = g.Sum(a => a.BusyTime.Value) / ipus,
                    })                    
                    .OrderBy(CpuNumber => CpuNumber)
                    .ToList();

                foreach(var cpu in cpuManys)
                {
                    var view = new CPUDetailElapseAndBusyTimeView();
                    double cpuElapseValue = Convert.ToDouble(cpu.TotalElapsedTime);
                    if (cpuElapseValue == 0)
                        cpuElapseValue = 0.01;

                    view.ElapsedTime = cpuElapseValue;
                    view.BusyTime = Convert.ToInt64(cpu.TotalBusyTime);

                    views.Add(cpu.CpuNumber.ToString(), view);
                }
            }

            return views;
        }

        public CPUSummaryView GetCPUElapseAndBusyTime(DateTime fromTimestamp, DateTime toTimestamp)
        {
            var summaryView = new CPUSummaryView();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var TotalElapsedTime = session.Query<PvCPUManyEntity>()
                    .Where(x => x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp)
                    .Sum(x => x.MElapsedTime);

                var TotalBusyTime = session.Query<PvCPUManyEntity>()
                    .Where(x => x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp)
                    .Sum(x => x.BusyTime);

                double cpuElapseValue = Convert.ToDouble(TotalElapsedTime);

                if (cpuElapseValue == 0)
                    cpuElapseValue = 0.01;

                summaryView.ElapsedTime = cpuElapseValue;
                summaryView.BusyTime = Convert.ToInt64(TotalBusyTime);
                summaryView.AllPathways = new Dictionary<string, double>();
                summaryView.AllPathwaysWithoutFree = new Dictionary<string, double>();
                summaryView.OnlyPathways = new Dictionary<string, double>();
            }

            return summaryView;
        }

        public Dictionary<string, CPUDetailElapseAndBusyTimeView> GetCPUElapseAndBusyTimePerCPU(DateTime fromTimestamp, DateTime toTimestamp)
        {
            var views = new Dictionary<string, CPUDetailElapseAndBusyTimeView>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var cpuManys = session.Query<PvCPUManyEntity>()
                    .Where(x => x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp)                    
                    .GroupBy(x => x.CpuNumber)
                    .Select(g => new
                    {
                        CpuNumber = g.Key,
                        TotalElpasedTime = g.Sum(a => a.MElapsedTime.Value),
                        TotalBusyTime = g.Sum(a => a.BusyTime.Value)
                    })
                    .OrderBy(x => x.CpuNumber)
                    .ToList();

                foreach(var cpu in cpuManys)
                {
                    var view = new CPUDetailElapseAndBusyTimeView();
                    double cpuElapseValue = Convert.ToDouble(cpu.TotalElpasedTime);
                    if (cpuElapseValue == 0)
                        cpuElapseValue = 0.01;

                    view.ElapsedTime = cpuElapseValue;
                    view.BusyTime = Convert.ToInt64(cpu.TotalBusyTime);

                    views.Add(cpu.CpuNumber.ToString(), view);
                }
            }

            return views;
        }

        public Dictionary<string, double> GetCPUElapsePerCPU(DateTime fromTimestamp, DateTime toTimestamp)
        {
            var cpuElapses = new Dictionary<string, double>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var cpuManys = session.Query<PvCPUManyEntity>()
                    .Where(x => x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp)
                    .GroupBy(x => x.CpuNumber)
                    .Select(g => new
                        {
                            CpuNumber = g.Key,
                            TotalElapsedTime = g.Sum(a => a.MElapsedTime.Value)
                        })
                    .ToList();

                foreach(var cpu in cpuManys)
                {
                    double cpuElapse = Convert.ToDouble(cpu.TotalElapsedTime);
                    if (cpuElapse == 0)
                        cpuElapse = 0.01;

                    cpuElapses.Add(cpu.CpuNumber.ToString(), cpuElapse);
                }
            }

            return cpuElapses;
        }
    }
}
