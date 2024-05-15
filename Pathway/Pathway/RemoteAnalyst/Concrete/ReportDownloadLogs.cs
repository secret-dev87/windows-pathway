using MySqlConnector;
using Pathway.Core.Entity.Main;
using Pathway.Core.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pathway.Core.RemoteAnalyst.Concrete {
    public class ReportDownloadLogs {
        private readonly string _connectionString;

        public ReportDownloadLogs(string connectionString) {
            _connectionString = connectionString;
        }

        public void InsertNewLog(int reportDownloadId, DateTime logDate, string message)
        {
            if (reportDownloadId > 0) {
                try {
                    using (var session = NHibernateHelper.OpenMainSession())
                    {
                        using(var transaction = session.BeginTransaction())
                        {
                            var newLog = new ReportDownloadLogEntity
                            {
                                ReportDownloadId = reportDownloadId,
                                LogDate = logDate,
                                Message = message
                            };

                            session.Save(newLog);
                            transaction.Commit();
                        }
                    }
                }
                catch { }
            }
        }
    }
}
