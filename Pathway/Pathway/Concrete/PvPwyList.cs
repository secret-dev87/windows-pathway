using System;
using System.Collections.Generic;
using MySqlConnector;
using Pathway.Core.Abstract;

namespace Pathway.Core.Concrete {
    public class PvPwyList : IPvPwyList {
        private readonly string _connectionString = "";

        public PvPwyList(string connectionString) {
            _connectionString = connectionString;
        }

        public List<string> GetPathwayNames(DateTime fromTimestamp, DateTime toTimestamp) {
            string cmdText = "SELECT PathwayName FROM PvPwylist WHERE (FromTimestamp >= @FromTimestamp AND ToTimestamp <= @ToTimestamp) GROUP BY PathwayName";
            var pathways = new List<string>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.CommandTimeout = 0;
                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    pathways.Add(reader["PathwayName"].ToString());
                }
            }

            return pathways;
        }

        public bool CheckPathwayName(string pathwayName, DateTime fromTimestamp, DateTime toTimestamp) {
            string cmdText = @"SELECT PathwayName 
                                FROM PvPwylist 
                                WHERE (FromTimestamp >= @FromTimestamp AND ToTimestamp <= @ToTimestamp) 
                                AND PathwayName = @PathwayName
                                GROUP BY PathwayName";
            var exits = false;

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);
                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                if (reader.Read()) {
                    exits = true;
                }
            }

            return exits;
        }

    }
}