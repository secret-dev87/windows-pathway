using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pathway.Core.Abstract;
using Pathway.Core.Concrete;

namespace Pathway.Core.Services {
    public class CPUBusyService : ICPUBusyService {
        private readonly string _connectionString = "";

        public CPUBusyService(string connectionString) {
            _connectionString = connectionString;
        }
        public void InsertCPUBusyFor(string pathwayName, DateTime fromTimestamp, DateTime toTimestamp, double cpuBusy) {
            IPvCPUBusies cpuBusies = new PvCPUBusies(_connectionString);
            cpuBusies.InsertCPUBusy(pathwayName, fromTimestamp, toTimestamp, cpuBusy);
        }
    }
}
