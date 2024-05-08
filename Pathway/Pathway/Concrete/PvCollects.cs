using System;
using System.Collections.Generic;
using MySqlConnector;
using Pathway.Core.Abstract;
using Pathway.Core.Infrastructure;

namespace Pathway.Core.Concrete {
    internal class PvCollects : IPvCollects {
        private readonly string _connectionString = "";

        public PvCollects(string connectionString) {
            _connectionString = connectionString;
        }

        public bool IsDuplicted(DateTime fromTimestamp, DateTime toTimestamp) {
            string cmdText = "SELECT IntervalNn FROM PvCollects WHERE FromTimestamp >= @FromTimestamp AND ToTimestamp <= @ToTimestamp";
            bool isDuplicted = false;

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                if (reader.Read()) {
                    isDuplicted = true;
                }
            }

            return isDuplicted;
        }

        public CollectionInfo GetInterval(DateTime fromTimestamp, DateTime toTimestamp) {
            string cmdText = "SELECT IntervalNn, IntervalHOrM FROM PvCollects WHERE FromTimestamp >= @FromTimestamp AND ToTimestamp <= @ToTimestamp LIMIT 1";
            var intervalInfo = new CollectionInfo();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                if (reader.Read()) {
                    intervalInfo.IntervalNumber = Convert.ToInt32(reader["IntervalNn"]);
                    intervalInfo.IntervalType = Convert.ToString(reader["IntervalHOrM"]);
                }
            }

            return intervalInfo;
        }


        public List<DateTime> GetDatesWithData(DateTime fromTimestamp, DateTime toTimestamp) {
            string cmdText = "SELECT FromTimestamp FROM PvCollects WHERE FromTimestamp >= @FromTimestamp AND ToTimestamp <= @ToTimestamp ORDER BY FromTimestamp";
            var dates = new List<DateTime>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var date = Convert.ToDateTime(reader["FromTimestamp"]).Date;
                    if(!dates.Contains(date))
                        dates.Add(date);
                }
            }

            return dates;
        }
    }
}