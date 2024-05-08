using System;
using System.Collections.Generic;
using MySqlConnector;
using Pathway.Core.Abstract;
using Pathway.Core.Infrastructure.AllPathway;

namespace Pathway.Core.Concrete {
    internal class PvCPUMany : IPvCPUMany {
        private readonly string _connectionString = "";

        public PvCPUMany(string connectionString) {
            _connectionString = connectionString;
        }

        public double GetCPUElapse(DateTime fromTimestamp, DateTime toTimestamp) {
            string cmdText = @"SELECT 
                            SUM(MElapsedTime) AS CpuElapse 
                            FROM PvCpumany WHERE
                            (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp)";

            double cpuElapse = 0;

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);

                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();
                double cpuElapseValue = 0;
                if (reader.Read()) {
                    if(!reader.IsDBNull(0))
                        cpuElapseValue = Convert.ToDouble(reader["CpuElapse"]);
                    if (cpuElapseValue == 0)
                        cpuElapseValue = 0.01;

                    cpuElapse = cpuElapseValue;
                }
            }
            return cpuElapse;
        }

        public Dictionary<string, double> GetCPUElapsePerCPU(DateTime fromTimestamp, DateTime toTimestamp) {
            string cmdText = @"SELECT 
                            CpuNumber, 
                            SUM(MElapsedTime) AS TotalElapsedTime
                            FROM PvCpumany WHERE
                            (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp)
                            GROUP BY CpuNumber 
                            ORDER BY CpuNumber";

            var cpuElapses = new Dictionary<string, double>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp.ToString("yyyy-MM-dd HH:mm:ss"));
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp.ToString("yyyy-MM-dd HH:mm:ss"));

                var test = fromTimestamp.ToString("yyyy-MM-dd HH:mm:ss");

                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    //double cpuElapse = Convert.ToDouble(reader["TotalElapsedTime"]) * 1000;
                    double cpuElapse = Convert.ToDouble(reader["TotalElapsedTime"]);
                    if (cpuElapse == 0)
                        cpuElapse = 0.01;

                    cpuElapses.Add(reader["CpuNumber"].ToString(), cpuElapse);
                }
            }
            return cpuElapses;
        }

        public CPUSummaryView GetCPUElapseAndBusyTime(DateTime fromTimestamp, DateTime toTimestamp) {
            string cmdText = @"SELECT 
                            SUM(MElapsedTime) AS TotalElapsedTime, 
                            SUM(BusyTime) AS TotalBusyTime
                            FROM PvCpumany WHERE
                            (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp)";

            var summaryView = new CPUSummaryView();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);

                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                if (reader.Read()) {
                    //double cpuElapseValue = Convert.ToDouble(reader["TotalElapsedTime"]) * 1000; ;
                    double cpuElapseValue = Convert.ToDouble(reader["TotalElapsedTime"]);
                    
                    if (cpuElapseValue == 0)
                        cpuElapseValue = 0.01;

                    summaryView.ElapsedTime = cpuElapseValue;
                    summaryView.BusyTime = Convert.ToInt64(reader["TotalBusyTime"]);
                    summaryView.AllPathways = new Dictionary<string, double>();
                    summaryView.AllPathwaysWithoutFree = new Dictionary<string, double>();
                    summaryView.OnlyPathways = new Dictionary<string, double>();
                }
            }
            return summaryView;
        }

        public Dictionary<string, CPUDetailElapseAndBusyTimeView> GetCPUElapseAndBusyTimePerCPU(DateTime fromTimestamp, DateTime toTimestamp) {
            string cmdText = @"SELECT 
                            CpuNumber,
                            SUM(MElapsedTime) AS TotalElapsedTime, 
                            SUM(BusyTime) AS TotalBusyTime
                            FROM PvCpumany WHERE
                            (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp)
                            GROUP BY CpuNumber 
                            ORDER BY CpuNumber";

            var views = new Dictionary<string, CPUDetailElapseAndBusyTimeView>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);

                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var view = new CPUDetailElapseAndBusyTimeView();
                    //double cpuElapseValue = Convert.ToDouble(reader["TotalElapsedTime"]) * 1000;
                    double cpuElapseValue = Convert.ToDouble(reader["TotalElapsedTime"]);
                    if (cpuElapseValue == 0)
                        cpuElapseValue = 0.01;

                    view.ElapsedTime = cpuElapseValue;
                    view.BusyTime = Convert.ToInt64(reader["TotalBusyTime"]);

                    views.Add(reader["CpuNumber"].ToString(), view);
                }
            }
            return views;
        }

        public Dictionary<string, CPUDetailElapseAndBusyTimeView> GetCPUElapseAndBusyPercentPerCPU(DateTime fromTimestamp, DateTime toTimestamp, int ipus) {
            string cmdText = @"SELECT 
                            CpuNumber,
                            SUM(MElapsedTime) AS TotalElapsedTime, 
                            SUM(BusyTime) / " + ipus + @" AS TotalBusyTime
                            FROM PvCpumany WHERE
                            (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp)
                            GROUP BY CpuNumber 
                            ORDER BY CpuNumber";

            var views = new Dictionary<string, CPUDetailElapseAndBusyTimeView>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);

                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var view = new CPUDetailElapseAndBusyTimeView();
                    //double cpuElapseValue = Convert.ToDouble(reader["TotalElapsedTime"]) * 1000;
                    double cpuElapseValue = Convert.ToDouble(reader["TotalElapsedTime"]);
                    if (cpuElapseValue == 0)
                        cpuElapseValue = 0.01;

                    view.ElapsedTime = cpuElapseValue; ;
                    view.BusyTime = Convert.ToInt64(reader["TotalBusyTime"]);

                    views.Add(reader["CpuNumber"].ToString(), view);
                }
            }
            return views;
        }

        public List<string> GetCPUCount(DateTime fromTimestamp, DateTime toTimestamp) {
            string cmdText = @"SELECT 
                                CpuNumber
                                FROM PvCpumany WHERE
                                (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp)
                                GROUP BY CpuNumber 
                                ORDER BY CpuNumber";

            var cpus = new List<string>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);

                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    cpus.Add(reader["CpuNumber"].ToString());
                }
            }
            return cpus;
        }

    }
}