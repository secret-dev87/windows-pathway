using System;
using System.Collections.Generic;
using MySqlConnector;
using Pathway.Core.Abstract;
using Pathway.Core.Infrastructure;
using Pathway.Core.Infrastructure.PerPathway.Server;

namespace Pathway.Core.Concrete {
    internal class PvScTStat : IPvScTStat {
        private readonly string _connectionString = "";

        public PvScTStat(string connectionString) {
            _connectionString = connectionString;
        }

        public long GetTcpToServer(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName) {
            string cmdText = @"SELECT SUM(IrReqCnt) AS TotalIrReqCnt
                            ,PathwayName 
                            FROM PvSctstat WHERE
                            (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp)
                            AND PathwayName = @PathwayName
                            GROUP BY PathwayName";

            long tcpToServer = 0;

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);

                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                if (reader.Read()) {
                    tcpToServer = Convert.ToInt64(reader["TotalIrReqCnt"]);
                }
            }
            return tcpToServer;
        }

        public long GetServerToTcp(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName) {
            string cmdText = @"SELECT SUM(IsReqCnt) AS TotalIsReqCnt
                            ,PathwayName 
                            FROM PvSctstat WHERE
                            (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp)
                            AND PathwayName = @PathwayName
                            GROUP BY PathwayName";

            long serverToTcp = 0;

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);

                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                if (reader.Read()) {
                    serverToTcp = Convert.ToInt64(reader["TotalIsReqCnt"]);
                }
            }
            return serverToTcp;
        }

        public List<CPUView> GetServerCPUBusyTcpTrans(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName) {
            var cmdText = @"SELECT 
                            Sum(PvSctstat.IrReqCnt) AS SumOfIsReqcnt,
                            PvSctstat.PathwayName, 
                            PvSctstat.ScName 
                            FROM PvSctstat
                            WHERE
                            PvSctstat.PathwayName = @PathwayName AND 
                            (PvSctstat.FromTimestamp >= @FromTimestamp AND  PvSctstat.ToTimestamp <= @ToTimestamp)
                            GROUP BY PvSctstat.PathwayName, PvSctstat.ScName";

            var serverCPUBusy = new List<CPUView>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);

                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var cpuBusy = new CPUView {
                        CPUNumber = reader["ScName"].ToString(),
                        Value = Convert.ToInt64(reader["SumOfIsReqcnt"])
                    };

                    serverCPUBusy.Add(cpuBusy);
                }
            }
            return serverCPUBusy;
        }

        public List<CPUView> GetServerCPUBusyAvgRt(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName) {
            var cmdText = @"SELECT 
                            AVG(PvSctstat.AvgResp) AS AvgResp, 
                            PvSctstat.PathwayName, 
                            PvSctstat.ScName, 
                            MAX(PvSctstat.MaxResp) AS MaxResp, 
                            MIN(PvSctstat.MinResp) AS MinResp 
                            FROM PvSctstat
                            WHERE
                            PvSctstat.PathwayName = @PathwayName AND 
                            (PvSctstat.FromTimestamp >= @FromTimestamp AND  PvSctstat.ToTimestamp <= @ToTimestamp)
                            GROUP BY PvSctstat.PathwayName, PvSctstat.ScName";

            var serverCPUBusy = new List<CPUView>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);

                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var cpuBusy = new CPUView {
                        CPUNumber = reader["ScName"].ToString(),
                        Value = Convert.ToInt64(reader["AvgResp"])
                    };

                    serverCPUBusy.Add(cpuBusy);
                }
            }
            return serverCPUBusy;
        }

        public Dictionary<string, TcpToServer> GetTcpToServer(DateTime fromTimestamp, DateTime toTimestamp) {
            string cmdText = @"SELECT 
                            SUM(`IsReqCnt`) AS TotalIsReqCnt, PathwayName, MAX(`IsReqCnt`) AS PeakReqCnt, AVG(`IsReqCnt`) AS AverageReqCnt
                            FROM PvSctstat WHERE
                            (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp)
                            GROUP BY PathwayName";

            var serverToTcp = new Dictionary<string, TcpToServer>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);

                command.CommandTimeout = 0;
                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    serverToTcp.Add(reader["PathwayName"].ToString(), new TcpToServer{
						TotalIsReqCnt = Convert.ToInt64(Convert.ToDouble(reader["TotalIsReqCnt"].ToString())),
						PeakReqCnt = Convert.ToInt64(Convert.ToDouble(reader["PeakReqCnt"].ToString())),
						AverageReqCnt = Convert.ToDouble(reader["AverageReqCnt"].ToString())
					});
                }
            }
            return serverToTcp;
        }

        public List<ServerQueueTcpView> GetServerQueueTCP(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName) {
            var cmdText = @"SELECT 
                            Query.ScName,
                            Query.CountofCollectionTime,
                            Query.MaxofQIReqCnt,
                            PvSctstat.CollectionTime,
                            PvSctstat.ScTcpName
                            FROM (
	                            SELECT 
	                            PathwayName,
	                            ScName,
	                            Count(FromTimestamp) as CountofCollectionTime,
	                            Max(QIReqCnt) as MaxofQIReqCnt
	                            FROM PvSctstat 
	                            WHERE 
	                            (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) AND 
                                QIWaits > 0 AND
	                            PathwayName = @PathwayName
	                            GROUP BY PathwayName,ScName
                            ) AS Query 
                            INNER JOIN 
	                            (SELECT 
	                            PathwayName,
	                            MIN(ScTcpName) AS ScTcpName,
	                            QIReqCnt,
	                            ScName,
	                            MIN(FromTimestamp) AS CollectionTime
	                            FROM PvSctstat  
	                            WHERE 
	                            (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) AND 
                                QIWaits > 0 AND
	                            PathwayName = @PathwayName
	                            GROUP BY PathwayName,ScName,QIReqCnt
                            ) AS PvSctstat
                            ON
                            (Query.ScName=PvSctstat.ScName AND 
                            Query.PathwayName=PvSctstat.PathwayName AND 
                            Query.MaxofQIReqCnt=PvSctstat.QIReqCnt)
                            ORDER BY Query.MaxofQIReqCnt DESC";

            var ququeTCP = new List<ServerQueueTcpView>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);

                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var tcp = new ServerQueueTcpView {
                        Server = reader["ScName"].ToString(),
                        PeakQueueLength = Convert.ToInt64(reader["MaxofQIReqCnt"]),
                        Tcp = reader["ScTcpName"].ToString(),
                        PeakTime = Convert.ToDateTime(reader["CollectionTime"]),
                        Instances = Convert.ToInt64(reader["CountofCollectionTime"]),
                    };

                    ququeTCP.Add(tcp);
                }
            }
            return ququeTCP;
        }

        public List<string> GetUnusedServerList(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName) {
            var cmdText = @"SELECT ScName 
                            FROM
                            (
	                            SELECT 
	                            IFNULL(temp1.ScName, temp2.ScName) AS ScName,
	                            IFNULL(temp1.PathwayName, temp2.PathwayName) AS PathwayName,
	                            IFNULL(temp1.SumofIsReqCnt, 0) AS tran1,
	                            IFNULL(temp2.SumofIsReqCnt, 0) AS tran2
	                            FROM (
		                            SELECT 
		                            ScName,
		                            PathwayName, 
		                            SUM(IsReqCnt) AS SumofIsReqCnt 
		                            FROM PvSctstat
		                            WHERE (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) AND
		                            PathwayName = @PathwayName
		                            GROUP BY ScName, PathwayName) temp1
	                            FULL OUTER JOIN (
		                            SELECT 
		                            ScName,
		                            PathwayName, 
		                            SUM(IsReqCnt) AS SumofIsReqCnt 
		                            FROM PvSclstat 
		                            WHERE(FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) AND
		                            PathwayName = @PathwayName
		                            GROUP BY SCName, PathwayName) temp2 
	                            ON temp1.ScName = temp2.ScName AND 
	                            temp1.PathwayName = temp2.PathwayName
                            ) temp3
                            WHERE tran1 = 0 AND tran2 = 0
                            ORDER BY ScName";

            var unusedClass = new List<string>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);

                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    unusedClass.Add(reader["ScName"].ToString());
                }
            }
            return unusedClass;
        }

        public List<ServerQueueTcpSubView> GetQueueTCPSub(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName) {
            var cmdText = @"SELECT 
                        FromTimestamp,
                        ScName,
                        ScTcpName,
                        PathwayName, 
                        QIReqCnt,
                        (QIWaits / QIReqCnt) AS QueueWait,
                        (QIDynamicLinks / QIReqCnt) AS PercentDynamic,
                        QIMaxWaits AS MaximumWaits,
                        (QIAggregateWaits/QIReqCnt) AS AverageWaits,
                        IsReqCnt AS SentRequestCount
                        FROM PvSctstat
                        WHERE 
                        (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) AND 
                        QIWaits > 0 AND
                        PathwayName = @PathwayName
                        ORDER BY ScName, FromTimestamp";

            var queueTCPSub = new List<ServerQueueTcpSubView>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);

                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var tcpSub = new ServerQueueTcpSubView {
                        Time = Convert.ToDateTime(reader["FromTimestamp"]),
                        ServerClass = reader["ScName"].ToString(),
                        TcpName = reader["ScTcpName"].ToString(),
                        RequestCount = Convert.ToInt64(reader["QIReqCnt"]),
                        PercentWait = Convert.ToDouble(reader["QueueWait"]) * 100,
                        PercentDynamic = Convert.ToDouble(reader["PercentDynamic"]) * 100,
                        MaxWaits = Convert.ToInt64(reader["MaximumWaits"]),
                        AvgWaits = Convert.ToDouble(reader["AverageWaits"]),
                        SentRequestCount = Convert.ToInt64(reader["SentRequestCount"])
                    };

                    queueTCPSub.Add(tcpSub);
                }
            }
            return queueTCPSub;
        }
    }
}
