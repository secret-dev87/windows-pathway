using System;
using System.Collections.Generic;
using MySqlConnector;
using System.Linq;
using System.Text;
using Pathway.Core.Infrastructure;

namespace Pathway.Core.RemoteAnalyst.Concrete {
    class Process {
        private string _connectionString;
        public Process(string connectionString) {
            _connectionString = connectionString;
        }

        public long GetTotalBusyTime(string processTableName, DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, int cpuNum) {
            var cmdText = @"SELECT SUM(CpuBusyTime) AS CpuBusyTime
                            FROM " + processTableName + @"
                            WHERE (AncestorProcessname = @PathwayName) AND (
                            FromTimestamp >= @FromTimestamp AND FromTimestamp < @ToTimestamp AND
                            ToTimestamp > @FromTimestamp AND ToTimestamp <= @ToTimestamp) AND CpuNum = @CpuNum";

            long cpuBusy = 0;

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);
                command.Parameters.AddWithValue("@CpuNum", cpuNum);

                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                if (reader.Read()) {
                    if (!reader.IsDBNull(0))
                        cpuBusy = Convert.ToInt64(reader["CpuBusyTime"]);
                }
            }
            return cpuBusy;
        }

        public long GetPathmonProcessBusy(string processTableName, DateTime fromTimestamp, DateTime toTimestamp, string pathwayName) {
            var cmdText = @"SELECT SUM(CpuBusyTime) AS TotalBusyTime
                            FROM " + processTableName + @"
                            WHERE (Processname = @PathwayName) AND (
                            FromTimestamp >= @FromTimestamp AND FromTimestamp < @ToTimestamp AND
                            ToTimestamp > @FromTimestamp AND ToTimestamp <= @ToTimestamp)";

            long cpuBusy = 0;

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);

                command.CommandTimeout = 0;
                connection.Open();
                var reader = command.ExecuteReader();

                if (reader.Read()) {
                    if (!reader.IsDBNull(0)) {
                        cpuBusy = Convert.ToInt64(Convert.ToInt64(reader["TotalBusyTime"]) / 1000);
                    }
                }
            }
            return cpuBusy;
        }

        public List<CPUView> GetPathmonProcessBusyPerCPU(string processTableName, DateTime fromTimestamp, DateTime toTimestamp, string pathwayName) {
            var cmdText = @"SELECT SUM(CpuBusyTime) AS TotalBusyTime,
                            CpuNum 
                            FROM " + processTableName + @" WHERE
                            (FromTimestamp >= @FromTimestamp AND FromTimestamp < @ToTimestamp AND
                            ToTimestamp > @FromTimestamp AND ToTimestamp <= @ToTimestamp)
                            AND Processname = @PathwayName
                            GROUP BY CpuNum
                            ORDER BY CpuNum";

            var pathmonProcessBusy = new List<CPUView>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);

                command.CommandTimeout = 0;
                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var cpuView = new CPUView {
                        CPUNumber = string.Format("{0:D2}", reader["CpuNum"]),
                        Value = Convert.ToInt64(Convert.ToInt64(reader["TotalBusyTime"]) / 1000)
                    };
                    pathmonProcessBusy.Add(cpuView);
                }
            }
            return pathmonProcessBusy;
        }

        public List<CPUView> GetPathmonProcessBusyPerPathway(string processTableName, DateTime fromTimestamp, DateTime toTimestamp, string pathwayNames) {
            var cmdText = @"SELECT 
                            SUM(CpuBusyTime) AS TotalBusyTime,
                            Processname 
                            FROM " + processTableName + @" WHERE
                            (FromTimestamp >= @FromTimestamp AND FromTimestamp < @ToTimestamp AND
                            ToTimestamp > @FromTimestamp AND ToTimestamp <= @ToTimestamp)
                            AND Processname IN (" + pathwayNames + @")
                            GROUP BY Processname";

            var pathmonProcessBusy = new List<CPUView>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);

                command.CommandTimeout = 0;
                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var cpuView = new CPUView {
                        CPUNumber = reader["Processname"].ToString(),
                        Value = Convert.ToInt64(Convert.ToInt64(reader["TotalBusyTime"]) / 1000)
                    };
                    pathmonProcessBusy.Add(cpuView);
                }
            }
            return pathmonProcessBusy;
        }

        public Dictionary<string, List<CPUView>> GetPathmonProcessBusyPerCPUPerPathway(string processTableName, DateTime fromTimestamp, DateTime toTimestamp, string pathwayNames) {
            var cmdText = @"SELECT 
                            CpuNum,
                            Processname,
                            SUM(CpuBusyTime) AS TotalBusyTime
                            FROM " + processTableName + @" WHERE
                            (FromTimestamp >= @FromTimestamp AND FromTimestamp < @ToTimestamp AND
                            ToTimestamp > @FromTimestamp AND ToTimestamp <= @ToTimestamp)
                            AND CpuBusyTime < DeltaTime
                            AND Processname IN (" + pathwayNames + @")
                            GROUP BY CpuNum, Processname
                            ORDER BY CpuNum";
            
            var views = new Dictionary<string, List<CPUView>>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);

                command.CommandTimeout = 0;
                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var view = new CPUView {
                        CPUNumber = reader["Processname"].ToString(),
                        Value = Convert.ToInt64(Convert.ToInt64(reader["TotalBusyTime"]) / 1000)
                    };
                    string cpuNumber = string.Format("{0:D2}", reader["CpuNum"]);
                    if (!views.ContainsKey(cpuNumber))
                        views.Add(string.Format("{0:D2}", reader["CpuNum"]), new List<CPUView> { view });
                    else
                        views[cpuNumber].Add(view);
                }
            }
            return views;
        }

        internal List<PvPwyManyView> GetPahtywayData(string processTableName, DateTime fromTimestamp, DateTime toTimestamp, string pathwayNames) {
            var cmdText = @"SELECT FromTimestamp
                                ,ToTimestamp
                                ,Processname
                                ,CpuBusyTime
                                ,CpuNum
                            FROM " + processTableName + @"
                            WHERE (FromTimestamp >= @FromTimestamp AND FromTimestamp < @ToTimestamp AND
                            ToTimestamp > @FromTimestamp AND ToTimestamp <= @ToTimestamp)
                            AND Processname IN (" + pathwayNames + @")
                            ORDER BY Processname, FromTimestamp, ToTimestamp";

            var pvPwyManyList = new List<PvPwyManyView>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);

                command.CommandTimeout = 0;
                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var cpuView = new PvPwyManyView {
                        FromTimestamp = Convert.ToDateTime(reader["FromTimestamp"]),
                        ToTimestamp = Convert.ToDateTime(reader["ToTimestamp"]),
                        PathwayName = reader["Processname"].ToString(),
                        PwyCpu = string.Format("{0:D2}", reader["CpuNum"]),
                        DeltaProcTime = Convert.ToInt64(Convert.ToInt64(reader["CpuBusyTime"]) / 1000)
                    };
                    pvPwyManyList.Add(cpuView);
                }
            }
            return pvPwyManyList;
        }
    }
}
