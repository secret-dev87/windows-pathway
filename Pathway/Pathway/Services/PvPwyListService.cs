using System;
using System.Collections.Generic;
using Pathway.Core.Abstract;
using Pathway.Core.Concrete;

namespace Pathway.Core.Services {
    public class PvPwyListService : IPvPwyListService {
        private readonly string _connectionString = "";

        public PvPwyListService(string connectionString) {
            _connectionString = connectionString;
        }

        public List<string> GetPathwayNamesFor(DateTime fromTimestamp, DateTime toTimestamp) {
            IPvPwyList pathwayList = new PvPwyList(_connectionString);
            List<string> pathways = pathwayList.GetPathwayNames(fromTimestamp, toTimestamp);

            return pathways;
        }

        public bool CheckPathwayNameFor(string pathwayName, DateTime fromTimestamp, DateTime toTimestamp) {
            IPvPwyList pathwayList = new PvPwyList(_connectionString);
            var pathway = pathwayList.CheckPathwayName(pathwayName, fromTimestamp, toTimestamp);

            return pathway;
        }
    }
}