using Antlr.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Entity.Main
{
    public class ReportDownloadLogEntity
    {
        public virtual int ReportDownloadLogsID { get; set; }
        public virtual int ReportDownloadId { get; set; }
        public virtual DateTime LogDate { get; set; }
        public virtual string Message { get; set; }
    }
}
