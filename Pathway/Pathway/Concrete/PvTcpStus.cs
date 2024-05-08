using System;
using System.Collections.Generic;
using MySqlConnector;
using Pathway.Core.Abstract;
using Pathway.Core.Infrastructure;
using Pathway.Core.Infrastructure.PerPathway.Server;
using Pathway.Core.Infrastructure.PerPathway.Tcp;

namespace Pathway.Core.Concrete {
    internal class PvTcpStus : IPvTcpStus {
        private readonly string _connectionString = "";

        public PvTcpStus(string connectionString) {
            _connectionString = connectionString;
        }

        public long GetTcpBusy(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName) {
            string cmdText = @"SELECT 
                            SUM(DeltaProcTime) AS TotalBusyTime
                            FROM PvTcpstus WHERE
                            (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp)
                            AND PathwayName = @PathwayName";

            long tcpBusy = 0;

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);

                connection.Open();
                var reader = command.ExecuteReader();

                if (reader.Read()) {
                    if (!reader.IsDBNull(0))
                        tcpBusy = Convert.ToInt64(reader["TotalBusyTime"]);
                }
            }
            return tcpBusy;
        }

        public List<CPUView> GetTcpBusyPerCPU(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName) {
            var cmdText = @"SELECT SUM(DeltaProcTime) AS TotalBusyTime,
                            TcpCpu  
                            FROM PvTcpstus WHERE
                            (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp)
                            AND PathwayName = @PathwayName
                            AND TcpCpu <> '' AND TcpCpu <> '\0\0'
                            GROUP BY TcpCpu
                            ORDER BY TcpCpu";

            var tcpProcessBusy = new List<CPUView>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);

                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var cpuView = new CPUView {
                        CPUNumber = reader["TcpCpu"].ToString(),
                        Value = Convert.ToInt64(reader["TotalBusyTime"])
                    };
                    tcpProcessBusy.Add(cpuView);
                }
            }
            return tcpProcessBusy;
        }

        public List<CPUView> GetTcpBusyPerPathway(DateTime fromTimestamp, DateTime toTimestamp) {
            var cmdText = @"SELECT 
                            SUM(DeltaProcTime) AS TotalBusyTime,
                            PathwayName 
                            FROM PvTcpstus WHERE
                            (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp)
                            GROUP BY PathwayName";

            var tcpBusy = new List<CPUView>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);

                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var cpuView = new CPUView {
                        CPUNumber = reader["PathwayName"].ToString(),
                        Value = Convert.ToInt64(reader["TotalBusyTime"])
                    };
                    tcpBusy.Add(cpuView);
                }
            }
            return tcpBusy;
        }

        public Dictionary<string, List<CPUView>> GetTcpBusyPerCPUPerPathway(DateTime fromTimestamp, DateTime toTimestamp) {
            var cmdText = @"SELECT
                            TcpCpu,
                            PathwayName,
                            SUM(DeltaProcTime) AS TotalBusyTime
                            FROM PvTcpstus WHERE
                            (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp)
                            AND TcpCpu <> '' AND TcpCpu <> '\0\0'
                            GROUP BY TcpCpu, PathwayName
                            ORDER BY TcpCpu";

            var views = new Dictionary<string, List<CPUView>>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);

                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var view = new CPUView {
                        CPUNumber = reader["PathwayName"].ToString(),
                        Value = Convert.ToInt64(reader["TotalBusyTime"])
                    };
                    string cpuNumber = reader["TcpCpu"].ToString();
                    if (!views.ContainsKey(cpuNumber))
                        views.Add(reader["TcpCpu"].ToString(), new List<CPUView> { view });
                    else
                        views[cpuNumber].Add(view);
                }
            }
            return views;
        }

        public List<TcpQueuedTransactionView> GetTcpQueuedTransactions(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName) {
            var cmdText = @"SELECT 
                            Query.TcpName,
                            Query.CountofCollectionTime,
                            Query.MaxofQLReqCnt,
                            PvTcpstat.CollectionTime
                            FROM (
	                            SELECT
	                            PathwayName,
	                            TcpName,
	                            Count(FromTimestamp) as CountofCollectionTime,
	                            Max(QLReqCnt) as MaxofQLReqCnt
	                            FROM PvTcpstat 
	                            WHERE 
	                            QLReqCnt > 0 AND 
	                            PathwayName = @PathwayName AND 
	                            (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp)
	                            GROUP BY PathwayName,TcpName
                            ) AS Query 
                            INNER JOIN (
	                            SELECT
	                            PathwayName,
	                            QLReqCnt,
	                            TcpName,
	                            MIN(FromTimestamp) AS CollectionTime
	                            FROM PvTcpstat  
	                            WHERE
	                            QLReqCnt > 0 AND 
	                            PathwayName = @PathwayName AND 
	                            (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp)
	                            GROUP BY PathwayName,TcpName,QLReqCnt
                            ) AS PvTcpstat
                            ON(Query.TcpName=PvTcpstat.TcpName AND
                            Query.PathwayName=PvTcpstat.PathwayName AND 
                            Query.MaxofQLReqCnt=PvTcpstat.QLReqCnt)
                            ORDER BY Query.TcpName ";

            var tcpqueued = new List<TcpQueuedTransactionView>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);

                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var view = new TcpQueuedTransactionView {
                        Tcp = reader["TcpName"].ToString(),
                        PeakQueueLength = Convert.ToInt64(reader["MaxofQLReqCnt"]),
                        PeakTime = Convert.ToDateTime(reader["CollectionTime"]),
                        Instances = Convert.ToInt64(reader["CountofCollectionTime"])
                    };
                    tcpqueued.Add(view);
                }
            }
            return tcpqueued;
        }

        public List<ServerQueueTcpSubView> GetQueueTCPSub(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName) {
            var cmdText = @"
                            SELECT 
                            FromTimestamp,
                            TcpName, 
                            QLReqCnt, 
                            QLWaits
                            FROM  PvTcpstat
                            WHERE QLReqCnt > 0  AND 
                            (FromTimestamp >= @FromTimestamp AND ToTimestamp <= @ToTimestamp) AND
                            pathwayName = @PathwayName
                            ORDER BY TcpName, FromTimestamp";

            var tcpqueued = new List<ServerQueueTcpSubView>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);

                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var view = new ServerQueueTcpSubView {
                        TcpName = reader["TcpName"].ToString(),
                        RequestCount = Convert.ToInt64(reader["QLReqCnt"]),
                        Time = Convert.ToDateTime(reader["FromTimestamp"]),
                        PercentWait = Convert.ToInt64(reader["QLWaits"])
                    };
                    tcpqueued.Add(view);
                }
            }
            return tcpqueued;
        }
    }
}