using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pathway.Core.Abstract.Repositories;
using Pathway.Core.Infrastructure;
using Pathway.Core.RemoteAnalyst.Concrete;
using Pathway.Core.Repositories;

namespace Pathway.Core.Services {
    public class PvPwyManyService {
        public PvPwyManyService() {
        }
        public List<PvPwyManyView> GetPvPwyMany(DateTime fromTimestamp, DateTime toTimestamp, string systemSerial) {
            IPvPwyManyRepository pwyMany = new PvPwyManyRepository();
            var process = new Process();

            var processTableName = systemSerial + "_PROCESS_" + fromTimestamp.Year + "_" + fromTimestamp.Month + "_" + fromTimestamp.Day;
            var pathwayNames = pwyMany.GetPathwayName(fromTimestamp, toTimestamp);
            var pwyInfo = process.GetPahtywayData(processTableName, fromTimestamp, toTimestamp, pathwayNames);

            return pwyInfo;
        }
    }
}
