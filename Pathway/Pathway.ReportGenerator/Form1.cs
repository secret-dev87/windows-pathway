using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using log4net;
using Pathway.Core.Abstract.Services;
using Pathway.Core.Services;

namespace Pathway.ReportGenerator
{
    public partial class Form1 : Form
    {
        private static readonly ILog Log = LogManager.GetLogger("PathwayReport");
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            string saveLocation = @"";
            var fromTimestamp = Convert.ToDateTime("2024-03-29 00:00:00");
            var toTimestamp = Convert.ToDateTime("2024-03-31 00:00:00");
            var interval = 3600;

            string connectionStringMain = "server=localhost;uid=root;pwd=Dev1121!;database=pmc";
            string connectionStringSystem = "server=localhost;uid=root;pwd=Dev1121!;database=pmc080627";
            var systemSerial = "080627";


            //IPerPathwayServerService serverService = new PerPathwayServerService(connectionStringSystem, interval);
            //var serverTransactionCount = serverService.GetServerTransactionsFor(fromTimestamp, toTimestamp, "$FAIA");

            IPvPwyListService pwyList = new PvPwyListService();
            var pwyLists = pwyList.GetPathwayNamesFor(fromTimestamp, toTimestamp);
            //var pwyLists = new List<string> { "$ISMS" };
            var report = new ReportGenerate(Log, saveLocation, connectionStringMain, connectionStringSystem, fromTimestamp, toTimestamp, pwyLists, interval, systemSerial, 0, true);

            try {
                //James Test.
                ///1> CPU Busy All pathway
                //IAllPathwayService service = new AllPathwayService(connectionStringSystem, interval);
                //IPerPathwayServerService perServerService = new PerPathwayServerService(connectionStringSystem, interval);
                //IPvScPrStus scPrStus = new PvScPrStus(connectionStringSystem);
                //IPvAlertService alertService = new PvAlertService(connectionStringSystem, interval);
                //IPvScStus scStus = new PvScStus(connectionStringSystem);

                //var detail = service.GetCPUBusyDetailPercentPerIntervalFor(fromTimestamp, toTimestamp, 2);

                //2> CPU Busy Single pathway
                //var cpuBusy = scPrStus.GetCPUBusyPercentPerInterval(fromTimestamp, toTimestamp, "$OMS");

                //3> Transaction count for all pathway ?? TODO: I don' think it's the right call.
                //var serverTransaction = service.GetTransactionServer(fromTimestamp, toTimestamp);

                //4> Transaction count for single pathway
                //var serverTransactionCount = perServerService.GetServerTransactionsPerIntervalFor(fromTimestamp, toTimestamp, "$PM");


                //8> Unused server classes [HOURLY] Same as 10.
                //var unusedClass = perServerService.GetServerUnusedClassesIntervalFor(fromTimestamp, toTimestamp, "$PM", Enums.IntervalTypes.Hourly);


                //11> Server Status
                //IPvScStus test = new PvScStus(connectionStringSystem);

                //12> Server Error
                //TODO: Instances is the Error Number.

                //2016-03-15 FIX.
                //1. Fixed
                //2. Done
                //var queueLinkmon = perServerService.GetServerQueueLinkmonIntervalFor(fromTimestamp, toTimestamp, "$ZVPT", Enums.IntervalTypes.Hourly, null); 

                //3. ?? it works....
                //var serverUnusedClass = perServerService.GetServerUnusedClassesIntervalFor(fromTimestamp, toTimestamp, "$RM1", Enums.IntervalTypes.Hourly, null);

                //4. ??
                //var freezeState = scStus.GetFreezeState(fromTimestamp, toTimestamp, "$ZVPT");


                //5. Is this out put correct?
                //var errorLists = alertService.GetServerErrorListSubFor(fromTimestamp, toTimestamp, "$RM1", Enums.IntervalTypes.Hourly, null);


                /*IPvScPrStus pvScPrStus = new PvScPrStus(connectionStringSystem);
                var returnValue = pvScPrStus.GetServerBusyPerCPUPerPathway(fromTimestamp, toTimestamp);

                IPvScInfo pvScInfo = new PvScInfo(connectionStringSystem);
                var returnValue2 = pvScInfo.GetServerCPUBusy(fromTimestamp, toTimestamp, "$FAIA");

                var returnValue3 = pvScPrStus.GetServerUnusedServerProcessPerPathway(fromTimestamp, toTimestamp, "$FAIA");
                IPerPathwayServerService serverService = new PerPathwayServerService(connectionStringSystem, interval);
                var unusedProcess = serverService.GetServerUnusedProcessesIntervalFor(fromTimestamp, toTimestamp, "$FAIA", Pathway.Core.Infrastructure.Enums.IntervalTypes.Daily);
                */
                report.CreateExcelReport();
                //report.CreateExcelAlertReport();

                //IPathwayAlertService alertListService = new PathwayAlertService(connectionStringMain);
                //var alertList = alertListService.GetAlertsFor();

                //IPvAlertService alertService = new PvAlertService(connectionStringSystem, interval);
                //var alerts = alertService.GetCollectionAlertForAllIntervals(alertList, fromTimestamp, toTimestamp);

                this.Focus();
                this.Activate();
            }
            catch (Exception ex)
            {
                 throw new Exception(ex.Message);
            }
        }
    }
}
