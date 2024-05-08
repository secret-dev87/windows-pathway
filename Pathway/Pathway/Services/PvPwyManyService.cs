using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pathway.Core.Concrete;
using Pathway.Core.Infrastructure;
using Pathway.Core.RemoteAnalyst.Concrete;

namespace Pathway.Core.Services {
    public class PvPwyManyService {
        private string _connectionStringSystem;

        public PvPwyManyService(string connectionStringSystem) {
            _connectionStringSystem = connectionStringSystem;
        }
        public List<PvPwyManyView> GetPvPwyMany(DateTime fromTimestamp, DateTime toTimestamp, string systemSerial) {
            var pwyMany = new PvPwyMany(_connectionStringSystem);
            var process = new Process(_connectionStringSystem);

            var processTableName = systemSerial + "_PROCESS_" + fromTimestamp.Year + "_" + fromTimestamp.Month + "_" + fromTimestamp.Day;
            var pathwayNames = pwyMany.GetPathwayName(fromTimestamp, toTimestamp);
            var pwyInfo = process.GetPahtywayData(processTableName, fromTimestamp, toTimestamp, pathwayNames);

            //
            //var pwyInfo = pwyName.GetPahtywayData(fromTimestamp, toTimestamp);

            return pwyInfo;
        }
    }
}
