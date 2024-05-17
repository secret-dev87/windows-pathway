using System;
using System.Collections.Generic;
using MySqlConnector;
using System.Linq;
using System.Text;
using Pathway.Core.Infrastructure;
using Pathway.Core.Helper;
using Pathway.Core.Entity.Dynamic;

namespace Pathway.Core.RemoteAnalyst.Concrete {
    class Process {
        public Process() {
        }

        public long GetTotalBusyTime(string processTableName, DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, int cpuNum)
        {
            long cpuBusy = 0;

            using(var session = NHibernateHelper.OpenDynamicSession(processTableName))
            {
                var result = session.Query<ProcessEntity>()
                    .Where(x => x.AncestorProcessName == pathwayName &&
                                (x.FromTimestamp >= fromTimestamp && x.FromTimestamp < toTimestamp) &&
                                (x.ToTimestamp > fromTimestamp && x.ToTimestamp <= toTimestamp) &&
                                x.CpuNum == cpuNum)
                    .Sum(x => x.CpuBusyTime.Value);

                cpuBusy = Convert.ToInt64(result);
            }

            return cpuBusy;
        }

        public long GetPathmonProcessBusy(string processTableName, DateTime fromTimestamp, DateTime toTimestamp, string pathwayName)
        {
            long cpuBusy = 0;

            using(var session = NHibernateHelper.OpenDynamicSession(processTableName))
            {
                var result = session.Query<ProcessEntity>()
                    .Where(x => x.ProcessName == pathwayName &&
                                (x.FromTimestamp >= fromTimestamp && x.FromTimestamp < toTimestamp) &&
                                (x.ToTimestamp > fromTimestamp && x.ToTimestamp <= toTimestamp))
                    .Sum(x => x.CpuBusyTime.Value);

                cpuBusy = Convert.ToInt64(result);
            }

            return cpuBusy;
        }

        public List<CPUView> GetPathmonProcessBusyPerCPU(string processTableName, DateTime fromTimestamp, DateTime toTimestamp, string pathwayName)
        {
            var pathmonProcessBusy = new List<CPUView>();

            using(var session = NHibernateHelper.OpenDynamicSession(processTableName))
            {
                var result = session.Query<ProcessEntity>()
                    .Where(x => (x.FromTimestamp >= fromTimestamp && x.FromTimestamp < toTimestamp) &&
                                (x.ToTimestamp > fromTimestamp && x.ToTimestamp <= toTimestamp) &&
                                x.ProcessName == pathwayName)
                    .GroupBy(x => x.CpuNum)
                    .Select(g => new
                    {
                        TotalBusyTime = g.Sum(a => a.CpuBusyTime.Value),
                        CpuNum = g.Key
                    })
                    .OrderBy(x => x.CpuNum)
                    .ToList();

                foreach(var res in result)
                {
                    pathmonProcessBusy.Add(new CPUView
                    {
                        CPUNumber = string.Format("{0:D2}", res.CpuNum),
                        Value = Convert.ToInt64(Convert.ToInt64(res.TotalBusyTime) / 1000)
                    });
                }
            }

            return pathmonProcessBusy;
        }

        public List<CPUView> GetPathmonProcessBusyPerPathway(string processTableName, DateTime fromTimestamp, DateTime toTimestamp, string pathwayNames)
        {
            var pathmonProcessBusy = new List<CPUView>();

            using(var session = NHibernateHelper.OpenDynamicSession(processTableName))
            {
                var result = session.Query<ProcessEntity>()
                    .Where(x => (x.FromTimestamp >= fromTimestamp && x.FromTimestamp < toTimestamp) &&
                                (x.ToTimestamp > fromTimestamp && x.ToTimestamp <= toTimestamp) &&
                                pathwayNames.Contains(x.ProcessName))
                    .GroupBy(x => x.ProcessName)
                    .Select(g => new
                    {
                        TotalBusyTime = g.Sum(a => a.CpuBusyTime.Value),
                        ProcessName = g.Key
                    })
                    .ToList();

                foreach(var res in result)
                {
                    pathmonProcessBusy.Add(new CPUView
                    {
                        CPUNumber = res.ProcessName.ToString(),
                        Value = Convert.ToInt64(Convert.ToInt64(res.TotalBusyTime) / 1000)
                    });
                }                
            }

            return pathmonProcessBusy;
        }

        public Dictionary<string, List<CPUView>> GetPathmonProcessBusyPerCPUPerPathway(string processTableName, DateTime fromTimestamp, DateTime toTimestamp, string pathwayNames)
        {
            var views = new Dictionary<string, List<CPUView>>();

            using(var session = NHibernateHelper.OpenDynamicSession(processTableName))
            {
                var result = session.Query<ProcessEntity>()
                    .Where(x => (x.FromTimestamp >= fromTimestamp && x.FromTimestamp < toTimestamp) &&
                                (x.ToTimestamp > fromTimestamp && x.ToTimestamp <= toTimestamp) &&
                                pathwayNames.Contains(x.ProcessName) &&
                                x.CpuBusyTime.Value < x.DeltaTime)
                    .GroupBy(x => new { x.CpuNum, x.ProcessName })
                    .Select(g => new
                    {
                        CpuNum = g.Key.CpuNum,
                        ProcessName = g.Key.ProcessName,
                        TotalBusyTime = g.Sum(a => a.CpuBusyTime.Value)
                    })
                    .ToList();

                foreach(var res in result)
                {
                    var view = new CPUView
                    {
                        CPUNumber = res.ProcessName.ToString(),
                        Value = Convert.ToInt64(Convert.ToInt64(res.TotalBusyTime) / 1000)
                    };

                    string cpuNumber = string.Format("{0:D2}", res.CpuNum);

                    if (!views.ContainsKey(cpuNumber))
                        views.Add(cpuNumber, new List<CPUView> { view });
                    else
                        views[cpuNumber].Add(view);
                }
            }

            return views;
        }

        internal List<PvPwyManyView> GetPahtywayData(string processTableName, DateTime fromTimestamp, DateTime toTimestamp, string pathwayNames)
        {
            var pvPwyManyList = new List<PvPwyManyView>();

            using(var session = NHibernateHelper.OpenDynamicSession(processTableName))
            {
                var result = session.Query<ProcessEntity>()
                    .Where(x => (x.FromTimestamp >= fromTimestamp && x.FromTimestamp < toTimestamp) &&
                                (x.ToTimestamp > fromTimestamp && x.ToTimestamp <= toTimestamp) &&
                                pathwayNames.Contains(x.ProcessName))
                    .Select(x => new { x.FromTimestamp, x.ToTimestamp, x.ProcessName, x.CpuBusyTime, x.CpuNum })
                    .OrderBy(x => new { x.ProcessName, x.FromTimestamp, x.ToTimestamp })
                    .ToList();

                foreach(var res in result)
                {
                    pvPwyManyList.Add(new PvPwyManyView
                    {
                        FromTimestamp = Convert.ToDateTime(res.FromTimestamp),
                        ToTimestamp = Convert.ToDateTime(res.ToTimestamp),
                        PathwayName = res.ProcessName.ToString(),
                        PwyCpu = string.Format("{0:D2}", res.CpuNum),
                        DeltaProcTime = Convert.ToInt64(Convert.ToInt64(res.CpuBusyTime.Value) / 1000)
                    });
                }
            }

            return pvPwyManyList;
        }
    }
}
