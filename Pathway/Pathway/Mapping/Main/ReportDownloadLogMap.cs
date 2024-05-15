using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using Pathway.Core.Entity.Main;

namespace Pathway.Core.Mapping.Main
{
    public class ReportDownloadLogMap : ClassMap<ReportDownloadLogEntity>
    {
        public ReportDownloadLogMap()
        {
            Table("reportdownloads");
            Id(x => x.ReportDownloadLogsID);
            Map(x => x.ReportDownloadId);
            Map(x => x.LogDate);
            Map(x => x.Message).Length(1024);
        }
    }
}
