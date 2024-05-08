using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.ReportGenerator.Infrastructure {
    public class ExcelFileInfo {
        public DateTime ExcelReportDate { get; set; }
        public string FileName { get; set; }

        public string AlertWorksheetName { get; set; }
        public string CPUSummaryWorksheetName { get; set; }
        public string CPUDetailWorksheetName { get; set; }
        public string ServerTransactionWorksheetName { get; set; }
        public string TCPTransactionWorksheetName { get; set; }
    }
}
