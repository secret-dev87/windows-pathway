using MySqlConnector;
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

        public void InsertNewLog(int reportDownloadId, DateTime logDate, string message) {
            if (reportDownloadId > 0 && _connectionString.Length > 0) {
                try {
                    string cmdInsert = @"INSERT INTO ReportDownloadLogs (ReportDownloadId, LogDate, Message) 
                                VALUES (@ReportDownloadId, @LogDate, @Message)";

                    using (var connection = new MySqlConnection(_connectionString)) {
                        var insertCmd = new MySqlCommand(cmdInsert, connection);
                        insertCmd.CommandTimeout = 0;

                        insertCmd.Parameters.AddWithValue("@ReportDownloadId", reportDownloadId);
                        insertCmd.Parameters.AddWithValue("@LogDate", logDate);
                        insertCmd.Parameters.AddWithValue("@Message", message);

                        connection.Open();
                        insertCmd.ExecuteNonQuery();
                    }
                }
                catch { }
            }
        }
    }
}
