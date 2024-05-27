using System;
using System.IO;
using log4net;
using Pathway.Core.Helper;

namespace Pathway.Core.Services {
    public class DeletePathwayData {
        private readonly ILog _log;

        public DeletePathwayData(ILog log) {
            _log = log;
        }

        public void DelteAllPathwayDataFor(DateTime fromTimestamp, DateTime toTimestamp) {
            _log.InfoFormat("Removing PvAlerts at {0}", DateTime.Now);
            string[] tablesToDelete =
            {
                "PvAlerts", "PvCollects", "PvCPUBusies", "PvCPUMany",
                "PvCPUOnce", "PvErrInfo", "PvLmStus", "PvPwyList",
                "PvPwyMany", "PvPwyOnce", "PvScAssign", "PvScDefine",
                "PvScInfo", "PvScLStat", "PvScProc", "PvScPrStus",
                "PvScStus", "PvScTStat", "PvTcpInfo", "PvTcpStat",
                "PvTcpStus", "PvTermInfo", "PvTermStat", "PvTermStus",
            };

            DeleteHelper deleteHelper = new DeleteHelper();
            foreach (string tableToDelete in tablesToDelete) { 
                _log.InfoFormat("Removing {0} at {1}", tableToDelete, DateTime.Now);
                deleteHelper.DeleteData(tableToDelete, fromTimestamp, toTimestamp);
            }
        }
    }
}