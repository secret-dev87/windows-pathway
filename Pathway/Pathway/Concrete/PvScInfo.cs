using System;
using System.Collections.Generic;
using System.Configuration;
using MySqlConnector;
using System.Text;
using Pathway.Core.Abstract;
using Pathway.Core.Infrastructure;
using Pathway.Core.Infrastructure.PerPathway.Server;

namespace Pathway.Core.Concrete {
    public class PvScInfo : IPvScInfo {
        private readonly string _connectionString;

        public PvScInfo(string connectionString) {
            _connectionString = connectionString;
        }
        
        public List<CPUView> GetServerCPUBusy(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, int topCount = 0) {
            var cmdText = new StringBuilder();
            cmdText.Append(@"SELECT Sum(PvScprstus.DeltaProcTime) AS SumOfDeltaProcTime, 
                            PvScprstus.PathwayName, 
                            PvScprstus.ScName 
                            FROM PvScprstus
                            WHERE
                            PvScprstus.PathwayName = @PathwayName AND 
                            (PvScprstus.FromTimestamp >= @FromTimestamp AND  PvScprstus.ToTimestamp <= @ToTimestamp)
                            GROUP BY PvScprstus.PathwayName, PvScprstus.ScName
                            ORDER BY Sum(PvScprstus.DeltaProcTime) DESC ");

            if (topCount != 0)
                cmdText.Append("LIMIT " + topCount);

            var serverCPUBusy = new List<CPUView>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText.ToString(), connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);

                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var cpuBusy = new CPUView {
                        CPUNumber = reader["ScName"].ToString(),
                        Value = Convert.ToInt64(reader["SumOfDeltaProcTime"])
                    };

                    serverCPUBusy.Add(cpuBusy);
                }
            }
            return serverCPUBusy;
        }

        public List<CPUView> GetNumStaticMaxServers(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, int topCount = 0) {
            var cmdText = new StringBuilder();

            cmdText.Append(@"SELECT ScName, NumStatic, MaxServers
                            FROM PvScinfo
                            WHERE
                            PathwayName = @PathwayName AND 
                            (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) ");

            if (topCount != 0)
                cmdText.Append("LIMIT " + topCount);

            var serverCPUBusy = new List<CPUView>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText.ToString(), connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);

                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var cpuBusy = new CPUView {
                        CPUNumber = reader["ScName"].ToString(),
                        Value = Convert.ToInt64(reader["NumStatic"]),
                        Value2 = Convert.ToInt64(reader["MaxServers"])
                    };

                    serverCPUBusy.Add(cpuBusy);
                }
            }
            return serverCPUBusy;
        }
        public List<CPUView> GetServerProcessCount(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, int topCount = 0) {
            var cmdText = new StringBuilder();

            cmdText.Append(@"SELECT AVG(ScProcessCount) AS AvgofScProcessCount, 
                            Max(ScProcessCount) AS MaxofScProcessCount,
                            PathwayName, 
                            ScName
                            FROM 
                            (
	                            SELECT 
	                            PvScprstus.PathwayName, 
	                            PvScprstus.ScName, 
	                            COUNT(PvScprstus.ScProcessName) AS ScProcessCount 
	                            FROM PvScprstus
	                            WHERE
	                            (PvScprstus.FromTimestamp >= @FromTimestamp AND  PvScprstus.ToTimestamp <= @ToTimestamp)
	                            AND PvScprstus.PathwayName = @PathwayName
	                            GROUP BY PvScprstus.PathwayName, PvScprstus.ScName, PvScprstus.FromTimestamp
                            ) DERIVEDTBL 
                            GROUP BY PathwayName, ScName 
                            ORDER BY AVG(ScProcessCount) DESC ");

            if (topCount != 0)
                cmdText.Append("LIMIT " + topCount);

            var serverProcessCount = new List<CPUView>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText.ToString(), connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);

                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var cpuBusy = new CPUView {
                        CPUNumber = reader["ScName"].ToString(),
                        Value = Convert.ToInt64(reader["AvgofScProcessCount"]),
                        Value2 = Convert.ToInt64(reader["MaxofScProcessCount"])
                    };

                    serverProcessCount.Add(cpuBusy);
                }
            }
            return serverProcessCount;
        }

        public List<CPUView> GetServerAverageResponseTime(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, int topCount = 0) {
            var cmdText = new StringBuilder();

            cmdText.Append(@"SELECT AVG(PvSctstat.AvgResp) / " + ConfigurationManager.AppSettings["ResponseTime"] 
                            + @" AS AvgResp, 
                            PvSctstat.ScName 
                            FROM PvSctstat
                            WHERE 
                            PvSctstat.PathwayName = @PathwayName AND 
                            (PvSctstat.FromTimestamp >= @FromTimestamp AND  PvSctstat.ToTimestamp <= @ToTimestamp)
                            GROUP BY PvSctstat.PathwayName, PvSctstat.ScName ");

            if (topCount != 0)
                cmdText.Append("LIMIT " + topCount);

            var serverProcessCount = new List<CPUView>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText.ToString(), connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);

                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var cpuBusy = new CPUView {
                        CPUNumber = reader["ScName"].ToString(),
                        DoubleValue = Convert.ToDouble(reader["AvgResp"])
                    };

                    serverProcessCount.Add(cpuBusy);
                }
            }
            return serverProcessCount;
        }

        public List<ServerCPUBusyView> GetServerTransactions(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName) {
            string cmdText = @"SELECT 
                                IFNULL(temp1.SumOfIsReqcnt, 0) + IFNULL(temp2.SumOfIsReqcnt2, 0) AS totalSum, 
                                IFNULL(temp1.SumOfIsReqcnt, 0) AS SumOfIsReqcntT, 
                                IFNULL(temp2.SumOfIsReqcnt2, 0) AS SumOfIsReqcntL, 
                                IFNULL(temp1.ScName, temp2.ScName) AS ScName 
                                FROM (
	                                SELECT 
	                                SUM(PvSctstat.IrReqCnt) AS SumOfIsReqcnt, 
	                                PvSctstat.PathwayName, 
	                                PvSctstat.ScName 
	                                FROM PvSctstat 
	                                WHERE (PvSctstat.PathwayName = @PathwayName) AND 
	                                (PvSctstat.FromTimestamp >= @FromTimestamp AND PvSctstat.ToTimestamp <= @ToTimestamp)
	                                GROUP BY PvSctstat.PathwayName, PvSctstat.ScName
                                ) temp1 LEFT JOIN 
                                (
	                                SELECT 
	                                SUM(PvSclstat.IrReqCnt) AS SumOfIsReqcnt2, 
	                                PvSclstat.PathwayName, 
	                                PvSclstat.ScName
	                                FROM PvSclstat 
	                                WHERE (PvSclstat.PathwayName = @PathwayName) AND 
	                                (PvSclstat.FromTimestamp >= @FromTimestamp AND PvSclstat.ToTimestamp <= @ToTimestamp)
	                                GROUP BY PvSclstat.PathwayName, PvSclstat.ScName
                                ) temp2 ON 
                                temp1.PathwayName = temp2.PathwayName AND 
                                temp1.ScName = temp2.ScName 
                                ORDER BY totalSum DESC";

            var transactions = new List<ServerCPUBusyView>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);

                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var transaction = new ServerCPUBusyView {
                        ServerClass = reader["ScName"].ToString(),
                        TotalTransaction = Convert.ToInt64(reader["totalSum"]),
                        TcpTransaction = Convert.ToInt64(reader["SumOfIsReqcntT"]),
                        LinkmonTransaction = Convert.ToInt64(reader["SumOfIsReqcntL"])
                    };

                    transactions.Add(transaction);
                }
            }
            return transactions;
        }

        public List<ServerCPUBusyView> GetServerTransactionsPerInterval(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName) {
            string cmdText = @"SELECT 
                                IFNULL(temp1.SumOfIsReqcnt, 0) + IFNULL(temp2.SumOfIsReqcnt2, 0) AS totalSum, 
                                IFNULL(temp1.SumOfIsReqcnt, 0) AS SumOfIsReqcntT, 
                                IFNULL(temp2.SumOfIsReqcnt2, 0) AS SumOfIsReqcntL, 
                                IFNULL(temp1.ScName, temp2.ScName) AS ScName,
                                FromTimestamp 
                                FROM (
	                                SELECT 
	                                SUM(PvSctstat.IrReqCnt) AS SumOfIsReqcnt, 
	                                PvSctstat.PathwayName, 
	                                PvSctstat.ScName ,
                                    DATE_FORMAT(FromTimestamp, '%Y-%m-%d %H:%i:00') AS FromTimestamp
	                                FROM PvSctstat 
	                                WHERE (PvSctstat.PathwayName = @PathwayName) AND 
	                                (PvSctstat.FromTimestamp >= @FromTimestamp AND PvSctstat.ToTimestamp <= @ToTimestamp)
	                                GROUP BY PvSctstat.PathwayName, PvSctstat.ScName, FromTimestamp
                                ) temp1 LEFT JOIN 
                                (
	                                SELECT 
	                                SUM(PvSclstat.IrReqCnt) AS SumOfIsReqcnt2, 
	                                PvSclstat.PathwayName, 
	                                PvSclstat.ScName
	                                FROM PvSclstat 
	                                WHERE (PvSclstat.PathwayName = @PathwayName) AND 
	                                (PvSclstat.FromTimestamp >= @FromTimestamp AND PvSclstat.ToTimestamp <= @ToTimestamp)
	                                GROUP BY PvSclstat.PathwayName, PvSclstat.ScName
                                ) temp2 ON 
                                temp1.PathwayName = temp2.PathwayName AND 
                                temp1.ScName = temp2.ScName 
                                ORDER BY FromTimestamp, totalSum DESC";

            var transactions = new List<ServerCPUBusyView>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);

                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var transaction = new ServerCPUBusyView {
                        ServerClass = reader["ScName"].ToString(),
                        TotalTransaction = Convert.ToInt64(reader["totalSum"]),
                        TcpTransaction = Convert.ToInt64(reader["SumOfIsReqcntT"]),
                        LinkmonTransaction = Convert.ToInt64(reader["SumOfIsReqcntL"]),
                        FromTimestamp = Convert.ToDateTime(reader["FromTimestamp"])
                    };

                    transactions.Add(transaction);
                }
            }
            return transactions;
        } 

    }
}