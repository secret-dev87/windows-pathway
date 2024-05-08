using System;
using System.Collections.Generic;
using MySqlConnector;
using System.Linq;
using System.Text;
using Pathway.Core.Abstract;

namespace Pathway.Core.Concrete {
    internal class PvCPUBusies : IPvCPUBusies {
        private readonly string _connectionString = "";

        public PvCPUBusies(string connectionString) {
            _connectionString = connectionString;
        }
        public void InsertCPUBusy(string pathwayName, DateTime fromTimestamp, DateTime toTimestamp, double cpuBusy) {
            var cmdText = @"INSERT INTO PvCPUBusies
                               (FromTimestamp
                               ,ToTimestamp
                               ,Pathway
                               ,CPUBusy)
                                VALUES (
                                @FromTimestamp 
                                ,@ToTimestamp 
                                ,@Pathway
                                ,@CPUBusy) ";

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.Parameters.AddWithValue("@Pathway", pathwayName);
                command.Parameters.AddWithValue("@CPUBusy", cpuBusy);
                command.CommandTimeout = 0;connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}
