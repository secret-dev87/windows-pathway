using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.ReportGenerator.Infrastructure {
    public class WorksheetTitles {
        public readonly string CollectionAlertsTitle = "Alerts - ";
        public readonly string CollectionCPUSummaryTitle = "All Pathways All CPU - Collection";
        //public readonly string CollectionCPUDetailTitle = "All Pathways Per CPU - Collection";
        public readonly string CollectionCPUDetailTitle = "CPU Busy, Average ";
        public readonly string CollectionTransTitle = "Transactions, Total ";
        public readonly string CollectionPeakCPUBusyHourly = "Peak CPU Busy, Hourly ";
        public readonly string CollectionAvgCPUBusyHourly = "Avg CPU Busy, Hourly ";
        public readonly string CollectionLinkmonTransCountsHourly = "Linkmon Transaction Counts, Hourly ";
        public readonly string CollectionTcpTransCountsHourly = "TCP Transaction Counts, Hourly ";
        public readonly string CollectionTransactionsCountHourly = "Transactions, Hourly ";
        //public readonly string CollectionTransTitle = "All Pathways SERVER, TCP Transactions - Collection";
        //public readonly string CollectionTransServerTitle = "All Pathways SERVER, Linkmon Transactions - Collection";
        //public readonly string CollectionTransTCPTitle = "All Pathways TERM, TCP Transactions - Collection";

        public readonly string DailyAlertsTitle = "Alerts - ";
        public readonly string DailyCPUSummaryTitle = "All Pathways All CPU - ";
        //public readonly string DailyCPUDetailTitle = "All Pathways Per CPU - ";
        public readonly string DailyCPUDetailTitle = "All Pathways Per CPU - ";
        public readonly string DailyTransServerTitle = "All Pathways SERVER, TCP Transactions - ";
        //public readonly string DailyTransServerTitle = "All Pathways SERVER, Linkmon Transactions - ";
        //public readonly string DailyTransTCPTitle = "All Pathways TERM, TCP Transactions - ";

        /*public readonly string DailyServerCPUBusy = "-SER. CPU Busy";
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
        public readonly string DailyTCPUnusedTCP = "-TCP Unuse TCPs";*/

        public readonly string PathwayCollectionAlertsTitle = " Pathway Alerts - ";
        public readonly string PathwayCollectionCPUSummaryTitle = " Pathway All CPU - ";
        public readonly string PathwayCollectionCPUDetailTitle = " CPU Busy, Average ";
        public readonly string PathwayCollectionTransTitle = " Transactions, Total ";
        //public readonly string PathwayCollectionCPUDetailTitle = " Pathway Per CPU - ";
        //public readonly string PathwayCollectionTransTitle = " Pathway Transaction - ";
        //public readonly string PathwayCollectionTransServerTitle = " Pathway SERVER Transaction - ";
        //public readonly string PathwayCollectionTransTCPTitle = " Pathway TCP Transaction - ";

        public readonly string PathwayCollectionServerCPUBusyTitle = " Busiest SERVER Classes, CPU Busy, Average ";
        public readonly string PathwayCollectionServerTransactionTitle = " Busiest SERVER Classes, Transactions, Total ";
        //public readonly string PathwayCollectionServerCPUBusyTitle = " Pathway SERVER CPU Busy, Average. Busiest Classes - ";
        //public readonly string PathwayCollectionServerTransactionTitle = " Pathway SERVER Transactions, Total. Busiest Classes - ";

        public readonly string PathwayCollectionServerProCountTitle = " Pathway SERVER Process Count - ";
        //public readonly string PathwayCollectionServerQueTCPTitle = " Pathway SERVER Queue TCP - ";
        public readonly string PathwayCollectionServerQueTCPTitle = " SERVERS with Queued TCP transactions ";
        public readonly string PathwayCollectionServerQueTCPTitleSub = " SERVERS with Queued TCP transactions Instances - ";

        public readonly string PathwayCollectionServerQueLinkmonTitle = " SERVERS with Queued Linkmon transactions - ";
        public readonly string PathwayCollectionServerQueLinkmonTitleSub = " SERVERS with Queued Linkmon transactions Instances - ";

        public readonly string PathwayCollectionServerPendingClassTitle = " Pending SERVER Classes ";
        public readonly string PathwayCollectionServerPendingProTitle = " Pending SERVER Processes ";

        public readonly string PathwayCollectionServerUnuseClassTitle = " Unused SERVER Classes ";
        public readonly string PathwayCollectionServerUnuseProTitle = " Unused SERVER Processes ";

        public readonly string PathwayCollectionServerMaxLinksClassTitle = " High MaxLinks Classes ";
        public readonly string PathwayCollectionServerMaxLinksTitle = " High MaxLinks Processes ";
        public readonly string PathwayCollectionCheckDirectoryOnLinksTitle = " TCP with Check Directory On ";
        public readonly string PathwayCollectionHighDynamicServersTitle = "  High Dynamic Servers ";

        public readonly string PathwayCollectionTermTop20Title = " Busiest TERMs, Transactions ";
        public readonly string PathwayCollectionTermUnusedTitle = " Unused TERMs ";

        public readonly string PathwayCollectionServerErrorListTitle = " Pathway SERVER Error List - ";
        public readonly string PathwayCollectionServerErrorListTitleSub = " Pathway SERVER Error List Instances - ";

        public readonly string PathwayCollectionTCPTransaction = " Coll. - TCP Trans.";
        public readonly string PathwayCollectionTCPQueTransaction = " Coll. - TCP Que Trans.";
        public readonly string PathwayCollectionTCPUnusedTCP = " Coll. - TCP Unuse TCPs";

        public readonly string PathwayCollectionTCPTransactionTitle = " Busiest TCPs, Transactions ";
        public readonly string PathwayCollectionTCPQueTransactionTitle = " TCPs with Queued transactions ";
        public readonly string PathwayCollectionTCPUnusedTCPTitle = " Unused TCPs ";
    }
}
