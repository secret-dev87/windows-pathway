using System;
using System.Collections.Generic;
using MySqlConnector;
using Pathway.Core.Abstract;
using Pathway.Core.Infrastructure;

namespace Pathway.Core.Concrete {
    internal class PvErrors : IPvErrors {
        private readonly string _connectionString = "";

        public PvErrors(string connectionString) {
            _connectionString = connectionString;
        }

        public ErrorInfo GetErrorInfo(long errorNumber) {
            string cmdText = "SELECT `Message`, `Cause`, `Effect`, `Recovery` FROM PvErrorsNew WHERE ErrorNumber = @ErrorNumber";
            var pathways = new List<string>();

            var errorInfo = new ErrorInfo();
            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@ErrorNumber", errorNumber);
                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                if (reader.Read()) {
                    errorInfo.ErrorNumber = errorNumber;
                    errorInfo.Message = reader["Message"].ToString();
                    errorInfo.Cause = reader["Cause"].ToString();
                    errorInfo.Effect = reader["Effect"].ToString();
                    errorInfo.Recovery = reader["Recovery"].ToString();
                }
            }

            return errorInfo;
        }
    }
}