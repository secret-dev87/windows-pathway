using System;
using System.Collections.Generic;
using MySqlConnector;
using Pathway.Core.Abstract;
using Pathway.Core.Infrastructure.PerPathway.Server;
using Pathway.Core.Infrastructure.PerPathway.Term;

namespace Pathway.Core.Concrete {
    internal class PvTermStat : IPvTermStat {
        private readonly string _connectionString = "";

        public PvTermStat(string connectionString) {
            _connectionString = connectionString;
        }

        public long GetTermToTcp(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName) {
            string cmdText = @"SELECT SUM(IsReqCnt) AS TotalIsReqCnt
                            ,PathwayName 
                            FROM PvTermstat WHERE
                            (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp)
                            AND PathwayName = @PathwayName
                            GROUP BY PathwayName";

            long termToTcp = 0;

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);

                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                if (reader.Read()) {
                    termToTcp = Convert.ToInt64(reader["TotalIsReqCnt"]);
                }
            }
            return termToTcp;
        }

        public Dictionary<string, long> GetTermToTcp(DateTime fromTimestamp, DateTime toTimestamp) {
            string cmdText = @"SELECT 
                            PathwayName,
                            SUM(IsReqCnt) AS TotalIsReqCnt
                            FROM PvTermstat WHERE
                            (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp)
                            GROUP BY PathwayName";

            var termToTcp = new Dictionary<string, long>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);

                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    termToTcp.Add(reader["PathwayName"].ToString(), Convert.ToInt64(reader["TotalIsReqCnt"]));
                }
            }
            return termToTcp;
        }

        public List<TermView> GetTermTransactions(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName) {
            var cmdText = @"SELECT 
                            TermName, 
                            SUM(IsReqCnt) AS TotalIsReqCnt, 
                            TermTcpName 
                            FROM PvTermstat 
                            WHERE 
                            PathwayName = @PathwayName AND 
                            (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp)
                            GROUP BY TermName, TermTcpName
                            ORDER BY TotalIsReqCnt DESC LIMIT 20";

            var top20Views = new List<TermView>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);

                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var view = new TermView {
                        Term = reader["TermName"].ToString(),
                        Tcp = reader["TermTcpName"].ToString(),
                        Value = Convert.ToDouble(reader["TotalIsReqCnt"])
                    };
                    top20Views.Add(view);
                }
            }
            return top20Views;
        }

        public List<TermView> GetTermAvgResponseTime(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName) {
            var cmdText = @"SELECT
                            TermName, 
                            MAX(AvgResp/1000000) AS MaxAvgResp, 
                            TermTcpName 
                            FROM PvTermstat 
                            WHERE
                            PathwayName = @PathwayName AND 
                            (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp)
                            GROUP BY TermName,TermTcpName
                            ORDER BY MaxAvgResp DESC LIMIT 20";

            var top20Views = new List<TermView>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);

                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var view = new TermView {
                        Term = reader["TermName"].ToString(),
                        Tcp = reader["TermTcpName"].ToString(),
                        Value = Convert.ToDouble(reader["MaxAvgResp"])
                    };
                    top20Views.Add(view);
                }
            }
            return top20Views;
        }

        public List<TermView> GetTermMaxResponseTime(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName) {
            var cmdText = @"SELECT 
                            TermName,
                            MAX(MaxResp/1000000) MaxMaxResp, 
                            TermTcpName 
                            FROM PvTermstat 
                            WHERE
                            PathwayName = @PathwayName AND  
                            (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp)
                            GROUP BY TermName, TermTcpName
                            ORDER BY MaxMaxResp DESC LIMIT 20";

            var top20Views = new List<TermView>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);

                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var view = new TermView {
                        Term = reader["TermName"].ToString(),
                        Tcp = reader["TermTcpName"].ToString(),
                        Value = Convert.ToDouble(reader["MaxMaxResp"])
                    };
                    top20Views.Add(view);
                }
            }
            return top20Views;
        }

        public List<TermUnusedView> GetTermUnused(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName) {
            var cmdText = @"SELECT 
                            TermName,
                            TermTcpName 
                            FROM (
	                            SELECT 
	                            TermName, 
	                            SUM(IsReqCnt) AS TotalIsReqCnt, 
	                            TermTcpName 
	                            FROM PvTermstat 
	                            WHERE
	                            PathwayName = @PathwayName AND
	                            (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp)
	                            GROUP BY TermName, TermTcpName
                            ) DERIVEDTBL
                            WHERE TotalIsReqCnt = 0 
                            ORDER BY TermTcpName";

            var unusedView = new List<TermUnusedView>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);

                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var view = new TermUnusedView {
                        Term = reader["TermName"].ToString(),
                        Tcp = reader["TermTcpName"].ToString(),
                    };
                    unusedView.Add(view);
                }
            }
            return unusedView;
        }

        public List<ServerUnusedServerClassView> GetTcpUnused(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName) {
            var cmdText = @"SELECT
                            TermTcpName ,
                            COUNT(TermTcpName) AS Unused
                            FROM PvTermstat
                            WHERE 
                            PathwayName = @PathwayName AND 
                            (PvTermstat.FromTimestamp >= @FromTimestamp AND  PvTermstat.ToTimestamp <= @ToTimestamp)
                            GROUP BY PathwayName, TermTcpName
                            HAVING Sum(IsReqCnt) = 0
                            ORDER BY TermTcpName";

            var unusedView = new List<ServerUnusedServerClassView>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);

                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var view = new ServerUnusedServerClassView { 
                        ServerClass = reader["TermTcpName"].ToString(),
                        Unused = Convert.ToInt64(reader["Unused"])
                    };
                    unusedView.Add(view);
                }
            }
            return unusedView;
        }

    }
}
