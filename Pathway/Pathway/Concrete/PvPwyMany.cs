using System;
using System.Collections.Generic;
using System.Data;
using MySqlConnector;
using Pathway.Core.Abstract;
using Pathway.Core.Infrastructure;

namespace Pathway.Core.Concrete {
    internal class PvPwyMany : IPvPwyMany {
        private readonly string _connectionString = "";

        public PvPwyMany(string connectionString) {
            _connectionString = connectionString;
        }

        public long GetPathmonProcessBusy(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName) {
            var cmdText = @"SELECT
                            SUM(DeltaProcTime) AS TotalBusyTime
                            FROM PvPwymany WHERE
                            (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp)
                            AND PathwayName = @PathwayName";

            long pathmonProcessBusy = 0;

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);

                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                if (reader.Read()) {
                    if (!reader.IsDBNull(0))
                        pathmonProcessBusy = Convert.ToInt64(reader["TotalBusyTime"]);
                }
            }
            return pathmonProcessBusy;
        }

        public List<CPUView> GetPathmonProcessBusyPerCPU(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName) {
            var cmdText = @"SELECT SUM(DeltaProcTime) AS TotalBusyTime,
                            PwyCpu 
                            FROM PvPwymany WHERE
                            (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp)
                            AND PathwayName = @PathwayName
                            GROUP BY PwyCpu
                            ORDER BY PwyCpu";

            var pathmonProcessBusy = new List<CPUView>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);

                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var cpuView = new CPUView {
                        CPUNumber = reader["PwyCpu"].ToString(),
                        Value = Convert.ToInt64(reader["TotalBusyTime"])
                    };
                    pathmonProcessBusy.Add(cpuView);
                }
            }
            return pathmonProcessBusy;
        }

        public List<CPUView> GetPathmonProcessBusyPerPathway(DateTime fromTimestamp, DateTime toTimestamp) {
            var cmdText = @"SELECT 
                            SUM(DeltaProcTime) AS TotalBusyTime,
                            PathwayName 
                            FROM PvPwymany WHERE
                            (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp)
                            GROUP BY PathwayName";

            var pathmonProcessBusy = new List<CPUView>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);

                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var cpuView = new CPUView {
                        CPUNumber = reader["PathwayName"].ToString(),
                        Value = Convert.ToInt64(reader["TotalBusyTime"])
                    };
                    pathmonProcessBusy.Add(cpuView);
                }
            }
            return pathmonProcessBusy;
        }

        public Dictionary<string, List<CPUView>> GetPathmonProcessBusyPerCPUPerPathway(DateTime fromTimestamp, DateTime toTimestamp) {
            var cmdText = @"SELECT 
                            PwyCpu,
                            PathwayName,
                            SUM(DeltaProcTime) AS TotalBusyTime
                            FROM PvPwymany WHERE
                            (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp)
                            AND DeltaProcTime < DeltaTime
                            GROUP BY PwyCpu, PathwayName
                            ORDER BY PwyCpu";

            var views = new Dictionary<string, List<CPUView>>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);

                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var view = new CPUView {
                        CPUNumber = reader["PathwayName"].ToString(),
                        Value = Convert.ToInt64(reader["TotalBusyTime"])
                    };
                    string cpuNumber = reader["PwyCpu"].ToString();
                    if (!views.ContainsKey(cpuNumber))
                        views.Add(reader["PwyCpu"].ToString(), new List<CPUView> {view});
                    else 
                        views[cpuNumber].Add(view);
                }
            }
            return views;
        }

        public string GetPathwayName(DateTime fromTimestamp, DateTime toTimestamp) {
            var cmdText = @"SELECT
                            PathwayName 
                            FROM PvPwymany WHERE
                            (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp)
                            GROUP BY PathwayName";

            var pathwayNameList = new List<string>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);

                command.CommandTimeout = 0;
                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    pathwayNameList.Add("'" + reader["PathwayName"].ToString() + "'");
                }
            }
            return string.Join("," , pathwayNameList);
        }
    }
}