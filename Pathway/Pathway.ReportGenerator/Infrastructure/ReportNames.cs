using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.ReportGenerator.Infrastructure {
    public class ReportNames : WorksheetTitles {
        public readonly string CollectionAlerts = "Collection - Alerts";
        public readonly string CollectionCPUSummary = "Collection - CPU Summary";
        public readonly string CollectionCPUDetail = "Collection- CPU Busy, Average";
        public readonly string CollectionTrans = "Collection- Transactions, Total";
        public readonly string CollectionHourlyPeakCPUBusy = "Col- Peak CPU Busy, Hourly";
        public readonly string CollectionHourlyAvgCPUBusy = "Col- Avg CPU Busy, Hourly";
        public readonly string CollectionHourlyLinkmonCounts = "Col- Linkmon Trans, Hourly";
        public readonly string CollectionHourlyTcpCounts = "Col- TCP Trans, Hourly";
        public readonly string CollectionHourlyTransCounts = "Col- Trans Counts, Hourly";
        //public readonly string CollectionTrans = "Collection - Transactions";
        //public readonly string CollectionTransServer = "Collection - Trans. SERVER";
        //public readonly string CollectionTransTCP = "Collection - Trans. TCP";

        public readonly string DailyAlerts = " - Alerts";
        public readonly string DailyCPUSummary = " - CPU Summary";
        //public readonly string DailyCPUDetail = " - CPU Detail";
        public readonly string DailyCPUDetail = " - CPU Busy, Avg.";
        public readonly string DailyTrans = " - Trans, Total";
        //public readonly string DailyTrans = " - Transactions";
        //public readonly string DailyTransServer = " - Trans. Server";
        //public readonly string DailyTransTCP = " - Trans. TCP";


        public readonly string DailyServerCPUBusy = "-SER. CPU Busy";
        public readonly string DailyServerTransaction = "-SER. Trans.";
        public readonly string DailyServerProCount = "-SER. Pro. Count";
        public readonly string DailyServerQueTCP = "-SER. Que TCP";
        public readonly string DailyServerQueLinkmon = "-SER. Que Linkmon";
        public readonly string DailyServerUnuseClass = "-SER. Unuse Class";
        public readonly string DailyServerUnusePro = "-SER. Unuse Pro.";


        public readonly string DailyTermTop20 = "-TERM Top 20";
        public readonly string DailyTermUnused = "-TERM Unused";

        public readonly string DailyTCPTransaction = "-TCP Trans.";
        public readonly string DailyTCPQueTransaction = "-TCP Que Trans.";
        public readonly string DailyTCPUnusedTCP = "-TCP Unuse TCPs";

        public readonly string PathwayCollectionAlerts = " Coll. - Alerts";
        public readonly string PathwayCollectionCPUSummary = " Coll. - CPU Summary";
        public readonly string PathwayCollectionCPUDetail = " Col. CPU";
        public readonly string PathwayCollectionTransServer = " Col. - Trans.";
        //public readonly string PathwayCollectionCPUDetail = " Coll. - CPU Detail";
        //public readonly string PathwayCollectionTransServer = " Coll. - Trans.";
        public readonly string PathwayCollectionTransTCP = " Coll. - Trans. TCP";


        public readonly string PathwayCollectionServerCPUBusy = " Col. S. Class CPU";
        public readonly string PathwayCollectionServerTransaction = " Col. S. Class Trans.";
        public readonly string PathwayCollectionServerProCount = " Coll. - SER. Pro. Count";
        public readonly string PathwayCollectionServerPendingClass = " Col. Pending Classes";
        public readonly string PathwayCollectionServerPendingProcess = " Col. Pending Processes";
        public readonly string PathwayCollectionServerQueTCP = " Col. Queues - TCP";
        public readonly string PathwayCollectionServerQueTCPSub = " Que TCP Ins. ";
        public readonly string PathwayCollectionServerQueLinkmon = " Col. Queues Linkmon";
        public readonly string PathwayCollectionServerQueLinkmonSub = " Q Linkmon Ins. ";
        public readonly string PathwayCollectionServerUnuseClass = "  Col. Unused Classes";
        public readonly string PathwayCollectionServerUnusePro = "  Col. Unused Processes";
        public readonly string PathwayCollectionServerMaxLinks = "  High MaxLinks Processes";
        public readonly string PathwayCollectionCheckDirectoryOnLinks = "  Check Directory On";
        public readonly string PathwayCollectionHighDynamicServers = "  High Dynamic Servers";
        public readonly string PathwayCollectionServerErrorList = " Col. - SER. Error List";
        public readonly string PathwayCollectionServerErrorListSub = " Error List Ins. ";

        public readonly string PathwayCollectionTermTop20 = " Col. TERM Trans";
        public readonly string PathwayCollectionTermUnused = " Col. TERM Unused";

        public readonly string PathwayCollectionTCPTransaction = " Col. TCP Trans.";
        public readonly string PathwayCollectionTCPQueTransaction = " Col. TCP Queue";
        public readonly string PathwayCollectionTCPQueTransactionSub = " TCP Que Ins. ";
        public readonly string PathwayCollectionTCPUnusedTCP = " Col. TCP, Unused";

        public Dictionary<string, List<ExcelFileInfo>> PerPathwayFileInfo { get; set; }
    }
}
