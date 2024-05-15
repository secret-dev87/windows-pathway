using System;
using System.Collections.Generic;
using Pathway.Core.Abstract.Services;
using Pathway.Core.Abstract.Repositories;
using Pathway.Core.Repositories;

namespace Pathway.Core.Services {
    public class PvPwyListService : IPvPwyListService {
        public PvPwyListService() {
        }

        public List<string> GetPathwayNamesFor(DateTime fromTimestamp, DateTime toTimestamp) {
            IPvPwyListRepository pathwayList = new PvPwyListRepository();
            List<string> pathways = pathwayList.GetPathwayNames(fromTimestamp, toTimestamp);

            return pathways;
        }

        public bool CheckPathwayNameFor(string pathwayName, DateTime fromTimestamp, DateTime toTimestamp) {
            IPvPwyListRepository pathwayList = new PvPwyListRepository();
            var pathway = pathwayList.CheckPathwayName(pathwayName, fromTimestamp, toTimestamp);

            return pathway;
        }
    }
}