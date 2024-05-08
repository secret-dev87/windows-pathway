using System;
using System.Collections.Generic;
using MySqlConnector;
using Pathway.Core.Abstract;
using Pathway.Core.Infrastructure;
using Pathway.Core.Infrastructure.PerPathway.Server;

namespace Pathway.Core.Concrete {
    public class PvScLStat : IPvScLStat {
        private readonly string _connectionString = "";

        public PvScLStat(string connectionString) {
            _connectionString = connectionString;
        }

        public long GetLinkmonToServer(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName) {
            string cmdText = @"SELECT SUM(IrReqCnt) AS TotalIrReqCnt
                            ,PathwayName 
                            FROM PvSclstat WHERE
                            (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp)
                            AND PathwayName = @PathwayName
                            GROUP BY PathwayName";

            long linkmonToServer = 0;

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);

                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                if (reader.Read()) {
                    linkmonToServer = Convert.ToInt64(reader["TotalIrReqCnt"]);
                }
            }
            return linkmonToServer;
        }

        public List<CPUView> GetServerCPUBusyLinkmonTrans(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName) {
            var cmdText = @"SELECT 
                            Sum(PvSclstat.IrReqCnt) AS SumOfIsReqcnt, 
                            PvSclstat.PathwayName, 
                            PvSclstat.ScName 
                            FROM PvSclstat
                            WHERE
                            PvSclstat.PathwayName = @PathwayName AND 
                            (PvSclstat.FromTimestamp >= @FromTimestamp AND  PvSclstat.ToTimestamp <= @ToTimestamp)
                            GROUP BY PvSclstat.PathwayName, PvSclstat.ScName";

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

        public Dictionary<string, LinkMonToServer> GetLinkmonToServer(DateTime fromTimestamp, DateTime toTimestamp) {
            string cmdText = @"SELECT 
                            SUM(`IrReqCnt`) AS TotalIrReqCnt, PathwayName, MAX(`IrReqCnt`) AS PeakReqCnt, AVG(`IrReqCnt`) AS AverageReqCnt
							FROM PvSclstat WHERE
                            (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp)
                            GROUP BY PathwayName";

            var linkmonToServer = new Dictionary<string, LinkMonToServer>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);

                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    linkmonToServer.Add(reader["PathwayName"].ToString(), new LinkMonToServer {
						TotalIrReqCnt = Convert.ToInt64(reader["TotalIrReqCnt"].ToString()),
						PeakReqCnt = Convert.ToInt64(reader["PeakReqCnt"].ToString()),
						AverageReqCnt = Convert.ToDouble(reader["AverageReqCnt"].ToString())
					});
                }
            }
            return linkmonToServer;
        }

        public List<ServerQueueLinkmonView> GetServerQueueLinkmon(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName) {
            var cmdText = @"SELECT 
                            Query.ScName,
                            Query.CountofCollectionTime,
                            Query.MaxofQIReqCnt,
                            PvSclstat.CollectionTime,
                            PvSclstat.ScLmName
                            FROM (
	                            SELECT
	                            PathwayName,
	                            ScName,
	                            Count(FromTimestamp) as CountofCollectionTime,
	                            Max(QIReqCnt) as MaxofQIReqCnt
	                            FROM PvSclstat 
	                            WHERE 
	                            (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) AND
                                QIWaits > 0 AND
	                            PathwayName = @PathwayName
	                            GROUP BY PathwayName, ScName
                            ) AS Query 
                            INNER JOIN (
	                            SELECT
	                            PathwayName,
	                            MIN(ScLmName) AS ScLmName,
	                            QIReqCnt,
	                            ScName,
	                            MIN(FromTimestamp) AS CollectionTime
	                            FROM PvSclstat  
	                            WHERE
	                            (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) AND
                                QIWaits > 0 AND
	                            PathwayName = @PathwayName
	                            GROUP BY PathwayName, ScName, QIReqCnt
                            ) AS PvSclstat
                            ON(Query.ScName=PvSclstat.ScName AND
                            Query.PathwayName=PvSclstat.PathwayName AND 
                            Query.MaxofQIReqCnt=PvSclstat.QIReqCnt)
                            ORDER BY Query.MaxofQIReqCnt DESC ";

            var views = new List<ServerQueueLinkmonView>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);

                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var view = new ServerQueueLinkmonView {
                        Server = reader["ScName"].ToString(),
                        PeakQueueLength = Convert.ToInt64(reader["MaxofQIReqCnt"]),
                        Linkmon = reader["ScLmName"].ToString(),
                        PeakTime = Convert.ToDateTime(reader["CollectionTime"]),
                        Instances = Convert.ToInt64(reader["CountofCollectionTime"])
                    };

                    views.Add(view);
                }
            }
            return views;
        }

        public List<ServerUnusedServerClassView> GetServerUnusedServerClass(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName) {
            var cmdText = @"SELECT 
                IFNULL(temp1.ScName, temp2.ScName) AS ScName, 
                IFNULL(temp1.SumofIsReqCnt, 0) AS tran1, 
                IFNULL(temp2.SumofIsReqCnt, 0) AS tran2 
                FROM (
	                SELECT 
	                ScName, 
	                PathwayName,
	                SUM(IsReqCnt) AS SumofIsReqCnt 
	                FROM PvSctstat 
	                WHERE PathwayName = @PathwayName AND
	                (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp)
	                GROUP BY ScName, PathwayName
                ) temp1 
                LEFT JOIN(
	                SELECT 
	                PvSclstat.ScName, 
	                PathwayName,
	                SUM(PvSclstat.IsReqCnt) AS SumofIsReqCnt 
	                FROM PvSclstat 
	                WHERE PvSclstat.PathwayName = @PathwayName AND 
	                (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp)
	                GROUP BY PvSclstat.SCName, PathwayName
                ) temp2
                 ON temp1.ScName = temp2.ScName AND 
                temp1.PathwayName = temp2.PathwayName
                WHERE 
                (IFNULL(temp1.SumofIsReqCnt, 0) = 0 AND 
                IFNULL(temp2.SumofIsReqCnt, 0) = 0)";

            var views = new List<ServerUnusedServerClassView>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);

                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var view = new ServerUnusedServerClassView {
                        ServerClass = reader["ScName"].ToString()
                    };

                    views.Add(view);
                }
            }
            return views;
        }

        public List<ServerQueueTcpSubView> GetQueueLinkmonSub(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName) {
            var cmdText = @"SELECT 
                            FromTimestamp,
                            ScName,
                            ScLmName,
                            QIReqCnt,
                            (QIWaits / QIReqCnt) AS QueueWait,
                            (QIDynamicLinks / QIReqCnt) AS PercentDynamic,
                            QIMaxWaits AS MaximumWaits,
                            (QIAggregateWaits/QIReqCnt) AS AverageWaits,
                            IsReqCnt AS SentRequestCount
                            FROM PvSclstat
                            WHERE 
                            (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) AND
                            QIWaits > 0 AND
                            PathwayName= @PathwayName
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
                        TcpName = reader["ScLmName"].ToString(),
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
