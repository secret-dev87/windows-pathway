using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using Pathway.Core.Entity.Main;

namespace Pathway.Core.Mapping.Main
{
    public class SystemTblMap : ClassMap<SystemTblEntity>
    {
        public SystemTblMap()
        {
            Table("system_tbl");
            Id(x => x.SystemSerial);
            Map(x => x.SystemName).Length(10);
            Map(x => x.CompanyID);
            Map(x => x.PlanStartDate);
            Map(x => x.PlanEndDate).Length(100);
            Map(x => x.PlanRequest);
            Map(x => x.RetentionDay);
            Map(x => x.UWSRetentionDay);
            Map(x => x.TimeZone);
            Map(x => x.CollectorVersion).Length(50);
            Map(x => x.DateCollectorDownload);
            Map(x => x.CollectorDownloadUser);
            Map(x => x.ExpertReport);
            Map(x => x.ExpertReportRetentionDay);
            Map(x => x.Evaluation);
            Map(x => x.ArchiveRetention);
            Map(x => x.MEASFH).Length(50);
            Map(x => x.TrendMonths);
            Map(x => x.Storage);
            Map(x => x.ArchiveRetentionPathway);
            Map(x => x.TrendMonthsPathway);
            Map(x => x.TrendMonthsStorage);
            Map(x => x.IsNTS);
            Map(x => x.BusinessTolerance);
            Map(x => x.BatchTolerance);
            Map(x => x.OtherTolerance);
            Map(x => x.City).Length(45);
            Map(x => x.QNMRetentionDay);
            Map(x => x.CountryCode).Length(10);
            Map(x => x.LoadLimit);
            Map(x => x.AttachmentInEmail);
            Map(x => x.Notes);
        }
    }
}
