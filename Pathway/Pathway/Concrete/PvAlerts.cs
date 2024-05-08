using System;
using System.Collections.Generic;
using System.Configuration;
using MySqlConnector;
using Pathway.Core.Abstract;
using Pathway.Core.Infrastructure;

namespace Pathway.Core.Concrete {
    internal class PvAlerts : IPvAlerts {
        private readonly string _connectionString = "";
        private readonly int _maxTimes = 3; //Not sure what this 3 is, but it's on all the Alert SELECT Statment.

        public PvAlerts(string connectionString) {
            _connectionString = connectionString;
        }
        public void InsertEmptyData(string pathwayName, DateTime fromTimestamp, DateTime toTimestamp) {
            string cmdText = @"INSERT INTO PvAlerts (
                                FromTimestamp 
                                ,ToTimestamp 
                                ,Pathway
                                ,TermHiMaxRT
                                ,TermHiAvgRT
                                ,TermUnused
                                ,TermErrorList
                                ,TCPQueuedTransactions
                                ,TCPLowTermPool
                                ,TCPLowServerPool
                                ,TCPUnused
                                ,TCPErrorList
                                ,ServerHiMaxRT
                                ,ServerHiAvgRT
                                ,ServerQueueTCP
                                ,ServerQueueLinkmon
                                ,ServerUnusedClass
                                ,ServerUnusedProcess
                                ,ServerErrorList) 
                                VALUES (
                                @FromTimestamp 
                                ,@ToTimestamp 
                                ,@Pathway
                                ,@TermHiMaxRT
                                ,@TermHiAvgRT
                                ,@TermUnused
                                ,@TermErrorList
                                ,@TCPQueuedTransactions
                                ,@TCPLowTermPool
                                ,@TCPLowServerPool
                                ,@TCPUnused
                                ,@TCPErrorList
                                ,@ServerHiMaxRT
                                ,@ServerHiAvgRT
                                ,@ServerQueueTCP
                                ,@ServerQueueLinkmon
                                ,@ServerUnusedClass
                                ,@ServerUnusedProcess
                                ,@ServerErrorList) ";

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.Parameters.AddWithValue("@Pathway", pathwayName);
                command.Parameters.AddWithValue("@TermHiMaxRT", 0);
                command.Parameters.AddWithValue("@TermHiAvgRT", 0);
                command.Parameters.AddWithValue("@TermUnused", 0);
                command.Parameters.AddWithValue("@TermErrorList", 0);
                command.Parameters.AddWithValue("@TCPQueuedTransactions", 0);
                command.Parameters.AddWithValue("@TCPLowTermPool", 0);
                command.Parameters.AddWithValue("@TCPLowServerPool", 0);
                command.Parameters.AddWithValue("@TCPUnused", 0);
                command.Parameters.AddWithValue("@TCPErrorList", 0);
                command.Parameters.AddWithValue("@ServerHiMaxRT", 0);
                command.Parameters.AddWithValue("@ServerHiAvgRT", 0);
                command.Parameters.AddWithValue("@ServerQueueTCP", 0);
                command.Parameters.AddWithValue("@ServerQueueLinkmon", 0);
                command.Parameters.AddWithValue("@ServerUnusedClass", 0);
                command.Parameters.AddWithValue("@ServerUnusedProcess", 0);
                command.Parameters.AddWithValue("@ServerErrorList", 0);
                command.CommandTimeout = 0;connection.Open();
                command.ExecuteNonQuery();
            }
        }

        #region Hourly Data
        public List<Alert> GetTermHiMaxRtHourly(DateTime fromTimestamp, DateTime toTimestamp) {
            var cmdText = @"SELECT PathwayName, COUNT(DISTINCT(TermName)) AS Count, FromTimestamp, ToTimestamp
                                FROM PvTermstat 
                                WHERE (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) AND MaxResp > " +
                                ConfigurationManager.AppSettings["ResponseTime"] + @"
                                GROUP BY PathwayName, FromTimestamp, ToTimestamp
                                ORDER BY PathwayName, FromTimestamp";
            var alerts = new List<Alert>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var alert = new Alert {
                        PathwayName = reader["PathwayName"].ToString(),
                        Count = Convert.ToInt64(reader["Count"]),
                        FromTimestamp = Convert.ToDateTime(reader["FromTimestamp"]),
                        ToTimestamp = Convert.ToDateTime(reader["ToTimestamp"])
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        public List<Alert> GetTermHiAvgRtHourly(DateTime fromTimestamp, DateTime toTimestamp) {
            var cmdText = @"SELECT PathwayName, COUNT(DISTINCT(TermName)) AS Count, FromTimestamp, ToTimestamp
                            FROM PvTermstat 
                            WHERE (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) AND AvgResp > " +
                            ConfigurationManager.AppSettings["ResponseTime"] + @"
                            GROUP BY PathwayName, FromTimestamp, ToTimestamp
                            ORDER BY PathwayName, FromTimestamp";
            var alerts = new List<Alert>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var alert = new Alert {
                        PathwayName = reader["PathwayName"].ToString(),
                        Count = Convert.ToInt64(reader["Count"]),
                        FromTimestamp = Convert.ToDateTime(reader["FromTimestamp"]),
                        ToTimestamp = Convert.ToDateTime(reader["ToTimestamp"])
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        public List<Alert> GetTermUnusedHourly(DateTime fromTimestamp, DateTime toTimestamp) {
            var cmdText = @"SELECT PathwayName, COUNT(TotalIsReqCnt) AS Count, FromTimestamp, ToTimestamp
                            FROM  
                            (SELECT PathwayName, SUM(PvTermstat.IsReqCnt) AS TotalIsReqCnt, FromTimestamp, ToTimestamp
                             FROM PvTermstat WHERE (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp)
                            GROUP BY PathwayName,PvTermstat.TermName, PvTermstat.TermTcpName, FromTimestamp, ToTimestamp) DERIVEDTBL 
                            WHERE TotalIsReqCnt = 0 
                            GROUP BY PathwayName, FromTimestamp, ToTimestamp
                            ORDER BY PathwayName, FromTimestamp";
            var alerts = new List<Alert>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var alert = new Alert {
                        PathwayName = reader["PathwayName"].ToString(),
                        Count = Convert.ToInt64(reader["Count"]),
                        FromTimestamp = Convert.ToDateTime(reader["FromTimestamp"]),
                        ToTimestamp = Convert.ToDateTime(reader["ToTimestamp"])
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        public List<Alert> GetTermErrorListHourly(DateTime fromTimestamp, DateTime toTimestamp) {
            var cmdText = @"SELECT PathwayName, COUNT(DISTINCT( TermName)) AS Count, FromTimestamp, ToTimestamp
                            FROM PvTermstus
                            WHERE (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) AND 
                            PvTermstus.ErrorNumber <> 0
                            GROUP BY PathwayName, FromTimestamp, ToTimestamp
                            ORDER BY PathwayName, FromTimestamp";
            var alerts = new List<Alert>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var alert = new Alert {
                        PathwayName = reader["PathwayName"].ToString(),
                        Count = Convert.ToInt64(reader["Count"]),
                        FromTimestamp = Convert.ToDateTime(reader["FromTimestamp"]),
                        ToTimestamp = Convert.ToDateTime(reader["ToTimestamp"])
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        public List<Alert> GetTcpQueuedTransactionsHourly(DateTime fromTimestamp, DateTime toTimestamp) {
            var cmdText = @"SELECT PathwayName, COUNT(DISTINCT(TcpName)) AS Count, FromTimestamp, ToTimestamp
                            FROM  PvTcpstat
                            WHERE QLReqCnt > 0  AND (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp)
                            GROUP BY PathwayName, FromTimestamp, ToTimestamp
                            ORDER BY PathwayName, FromTimestamp";
            var alerts = new List<Alert>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var alert = new Alert {
                        PathwayName = reader["PathwayName"].ToString(),
                        Count = Convert.ToInt64(reader["Count"]),
                        FromTimestamp = Convert.ToDateTime(reader["FromTimestamp"]),
                        ToTimestamp = Convert.ToDateTime(reader["ToTimestamp"])
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        public List<Alert> GetTcpLowTermPoolHourly(DateTime fromTimestamp, DateTime toTimestamp) {
            var cmdText = @"SELECT PathwayName, COUNT(DISTINCT(TcpName)) AS Count, FromTimestamp, ToTimestamp
                            FROM
                            (
	                            SELECT PathwayName, TcpName, PtSize, PtMaxAlloc, PtMaxReq, PsSize, PsMaxAlloc, PsMaxReq,
	                            PtAggregateReqSize/IFNULL(IFNULL(PtReqCnt, 0), 1) AS AvgTReqSize,
	                            PsAggregateReqSize/IFNULL(IFNULL(PsReqCnt, 0), 1) AS AvgSReqSize,
	                            FromTimestamp, ToTimestamp
	                            FROM  PvTcpstat 
	                            WHERE (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp)
                            ) temp1
                            WHERE ((PtSize - PtMaxAlloc) < temp1.AvgTReqSize OR (PtSize - PtMaxAlloc) < temp1.PtMaxReq)
                            GROUP BY PathwayName, FromTimestamp, ToTimestamp
                            ORDER BY PathwayName, FromTimestamp";
            var alerts = new List<Alert>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var alert = new Alert {
                        PathwayName = reader["PathwayName"].ToString(),
                        Count = Convert.ToInt64(reader["Count"]),
                        FromTimestamp = Convert.ToDateTime(reader["FromTimestamp"]),
                        ToTimestamp = Convert.ToDateTime(reader["ToTimestamp"])
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        public List<Alert> GetTcpLowServerPoolHourly(DateTime fromTimestamp, DateTime toTimestamp) {
            var cmdText = @"SELECT PathwayName, COUNT(DISTINCT(TcpName)) AS Count, FromTimestamp, ToTimestamp
                            FROM
                            (	
	                            SELECT PathwayName,TcpName,PtSize,PtMaxAlloc,PtMaxReq, PsSize,PsMaxAlloc,PsMaxReq,
	                            PtAggregateReqSize/IFNULL(IFNULL(PtReqCnt, 0), 1) AS AvgTReqSize,
	                            PsAggregateReqSize/IFNULL(IFNULL(PsReqCnt, 0), 1) AS AvgSReqSize,
	                            FromTimestamp, ToTimestamp
	                            FROM  PvTcpstat 
	                            WHERE (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp)
                            ) temp1
                            WHERE ((PsSize - PsMaxAlloc) < temp1.AvgSReqSize OR (PsSize - PsMaxAlloc) < temp1.PsMaxReq)
                            GROUP BY PathwayName, FromTimestamp, ToTimestamp
                            ORDER BY PathwayName, FromTimestamp";
            var alerts = new List<Alert>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var alert = new Alert {
                        PathwayName = reader["PathwayName"].ToString(),
                        Count = Convert.ToInt64(reader["Count"]),
                        FromTimestamp = Convert.ToDateTime(reader["FromTimestamp"]),
                        ToTimestamp = Convert.ToDateTime(reader["ToTimestamp"])
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        public List<Alert> GetTcpUnusedHourly(DateTime fromTimestamp, DateTime toTimestamp) {
            var cmdText = @"SELECT PathwayName, COUNT(TermTcpName) AS Count, FromTimestamp, ToTimestamp
                            FROM  
                            (SELECT PathwayName, SUM(PvTermstat.IsReqCnt) AS TotalIsReqCnt ,
                            TermTcpName, FromTimestamp, ToTimestamp FROM PvTermstat 
                            WHERE (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp)
                            GROUP BY PathwayName, TermTcpName, FromTimestamp, ToTimestamp) DERIVEDTBL 
                            WHERE TotalIsReqCnt = 0 
                            GROUP BY PathwayName, FromTimestamp, ToTimestamp
                            ORDER BY PathwayName, FromTimestamp";
            var alerts = new List<Alert>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var alert = new Alert {
                        PathwayName = reader["PathwayName"].ToString(),
                        Count = Convert.ToInt64(reader["Count"]),
                        FromTimestamp = Convert.ToDateTime(reader["FromTimestamp"]),
                        ToTimestamp = Convert.ToDateTime(reader["ToTimestamp"])
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        public List<Alert> GetTcpErrorListHourly(DateTime fromTimestamp, DateTime toTimestamp) {
            var cmdText = @"SELECT PathwayName, COUNT(DISTINCT( TcpName)) AS Count, FromTimestamp, ToTimestamp
                            FROM PvTcpStus
                            WHERE
                            ErrorNumber <> 0 
                            AND (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) 
                            GROUP BY PathwayName, FromTimestamp, ToTimestamp
                            ORDER BY PathwayName, FromTimestamp";
            var alerts = new List<Alert>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var alert = new Alert {
                        PathwayName = reader["PathwayName"].ToString(),
                        Count = Convert.ToInt64(reader["Count"]),
                        FromTimestamp = Convert.ToDateTime(reader["FromTimestamp"]),
                        ToTimestamp = Convert.ToDateTime(reader["ToTimestamp"])
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        public List<Alert> GetServerHiMaxRtHourly(DateTime fromTimestamp, DateTime toTimestamp) {
            var cmdText = @"SELECT PathwayName, COUNT(DISTINCT( ScName)) AS Count, FromTimestamp, ToTimestamp
                            FROM PvSctstat
                            WHERE (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp)  
                            AND MaxResp > " + ConfigurationManager.AppSettings["ResponseTime"] + @"
                            GROUP BY PathwayName, FromTimestamp, ToTimestamp
                            ORDER BY PathwayName, FromTimestamp";
            var alerts = new List<Alert>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var alert = new Alert {
                        PathwayName = reader["PathwayName"].ToString(),
                        Count = Convert.ToInt64(reader["Count"]),
                        FromTimestamp = Convert.ToDateTime(reader["FromTimestamp"]),
                        ToTimestamp = Convert.ToDateTime(reader["ToTimestamp"])
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        public List<Alert> GetServerHiAvgRtHourly(DateTime fromTimestamp, DateTime toTimestamp) {
            var cmdText = @"SELECT PathwayName, COUNT(DISTINCT(ScName)) AS Count, FromTimestamp, ToTimestamp
                            FROM PvSctstat
                            WHERE (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) 
                            AND AvgResp > " + ConfigurationManager.AppSettings["ResponseTime"] + @"
                            GROUP BY PathwayName, FromTimestamp, ToTimestamp
                            ORDER BY PathwayName, FromTimestamp";
            var alerts = new List<Alert>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var alert = new Alert {
                        PathwayName = reader["PathwayName"].ToString(),
                        Count = Convert.ToInt64(reader["Count"]),
                        FromTimestamp = Convert.ToDateTime(reader["FromTimestamp"]),
                        ToTimestamp = Convert.ToDateTime(reader["ToTimestamp"])
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        public List<Alert> GetServerQueueTcpHourly(DateTime fromTimestamp, DateTime toTimestamp) {
            var cmdText = @"SELECT PathwayName, COUNT(DISTINCT(ScName)) AS Count, FromTimestamp, ToTimestamp
                            FROM PvSctstat
                            WHERE (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) 
                            AND QIReqCnt > 0
                            GROUP BY PathwayName, FromTimestamp, ToTimestamp";
            var alerts = new List<Alert>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var alert = new Alert {
                        PathwayName = reader["PathwayName"].ToString(),
                        Count = Convert.ToInt64(reader["Count"]),
                        FromTimestamp = Convert.ToDateTime(reader["FromTimestamp"]),
                        ToTimestamp = Convert.ToDateTime(reader["ToTimestamp"])
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        public List<Alert> GetServerQueueLinkmonHourly(DateTime fromTimestamp, DateTime toTimestamp) {
            var cmdText = @"SELECT PathwayName, COUNT(DISTINCT(ScName)) AS Count, FromTimestamp, ToTimestamp
                            FROM PvSclstat
                            WHERE (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp)
                            AND QIReqCnt > 0
                            GROUP BY PathwayName, FromTimestamp, ToTimestamp
                            ORDER BY PathwayName, FromTimestamp";
            var alerts = new List<Alert>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var alert = new Alert {
                        PathwayName = reader["PathwayName"].ToString(),
                        Count = Convert.ToInt64(reader["Count"]),
                        FromTimestamp = Convert.ToDateTime(reader["FromTimestamp"]),
                        ToTimestamp = Convert.ToDateTime(reader["ToTimestamp"])
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        public List<Alert> GetServerUnusedClassHourly(DateTime fromTimestamp, DateTime toTimestamp) {
            var cmdText = @"SELECT PathwayName, COUNT(ScName) AS Count, FromTimestamp, ToTimestamp
                            FROM
                            (
	                            SELECT IFNULL(temp1.ScName, temp2.ScName) AS ScName,
	                            IFNULL(temp1.PathwayName, temp2.PathwayName) AS PathwayName,
	                            IFNULL(temp1.SumofIsReqCnt, 0) AS tran1,
	                            IFNULL(temp2.SumofIsReqCnt, 0) AS tran2,
	                            IFNULL(temp1.FromTimestamp, temp2.FromTimestamp) AS FromTimestamp, 
	                            IFNULL(temp1.ToTimestamp, temp2.ToTimestamp) AS ToTimestamp
	                            FROM (
		                            SELECT ScName,PathwayName, SUM(IsReqCnt) AS SumofIsReqCnt, FromTimestamp, ToTimestamp FROM PvSctstat
		                            WHERE (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp)
		                            GROUP BY ScName, PathwayName, FromTimestamp, ToTimestamp
	                            ) temp1
	                            LEFT JOIN (
		                            SELECT PvSclstat.ScName,PathwayName, SUM(PvSclstat.IsReqCnt) AS SumofIsReqCnt , PvSclstat.FromTimestamp, PvSclstat.ToTimestamp
		                            FROM PvSclstat 
		                            WHERE (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp)
		                            GROUP BY PvSclstat.SCName,PathwayName, FromTimestamp, ToTimestamp
	                            ) temp2 
                            ON temp1.ScName = temp2.ScName AND temp1.PathwayName = temp2.PathwayName 
                            AND temp1.FromTimestamp = temp2.FromTimestamp AND temp1.ToTimestamp = temp2.ToTimestamp) temp3
                            WHERE tran1 = 0 AND tran2 = 0
                            GROUP BY PathwayName, FromTimestamp, ToTimestamp
                            ORDER BY PathwayName, FromTimestamp";
            var alerts = new List<Alert>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var alert = new Alert {
                        PathwayName = reader["PathwayName"].ToString(),
                        Count = Convert.ToInt64(reader["Count"]),
                        FromTimestamp = Convert.ToDateTime(reader["FromTimestamp"]),
                        ToTimestamp = Convert.ToDateTime(reader["ToTimestamp"])
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        public List<Alert> GetServerUnusedProcessHourly(DateTime fromTimestamp, DateTime toTimestamp) {
            var cmdText = @"SELECT PathwayName, COUNT(ScName) AS Count, FromTimestamp, ToTimestamp FROM (
	                        SELECT PvScprstus.PathwayName, PvScprstus.ScName,
	                        PvScprstus.FromTimestamp, PvScprstus.ToTimestamp
	                        FROM PvScprstus 
	                        LEFT OUTER JOIN PvScinfo  
	                        ON (
		                        PvScprstus.FromTimestamp = PvScinfo.FromTimestamp AND PvScprstus.ToTimestamp = PvScinfo.ToTimestamp AND
		                        PvScprstus.Pathwayname = PvScinfo.Pathwayname AND PvScprstus.Scname = PvScinfo.Scname
	                        )
	                        WHERE (PvScprstus.FromTimestamp >= @FromTimestamp AND  PvScprstus.ToTimestamp <= @ToTimestamp) AND
                            (PvScinfo.FromTimestamp >= @FromTimestamp AND  PvScinfo.ToTimestamp <= @ToTimestamp) AND
	                        PvScprstus.ProcState = 2
	                        GROUP BY PvScprstus.PathwayName, PvScprstus.ScName, PvScprstus.ScProcessName, PvScinfo.MaxServers, 
			                         PvScinfo.NumStatic, PvScprstus.FromTimestamp, PvScprstus.ToTimestamp
	                        HAVING (SUM(ProcLinks) = 0) 
                        ) AS Temp
                        GROUP BY PathwayName, FromTimestamp, ToTimestamp
                        ORDER BY PathwayName, FromTimestamp";
            var alerts = new List<Alert>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var alert = new Alert {
                        PathwayName = reader["PathwayName"].ToString(),
                        Count = Convert.ToInt64(reader["Count"]),
                        FromTimestamp = Convert.ToDateTime(reader["FromTimestamp"]),
                        ToTimestamp = Convert.ToDateTime(reader["ToTimestamp"])
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        public List<Alert> GetServerErrorListHourly(DateTime fromTimestamp, DateTime toTimestamp) {
            var cmdText = @"SELECT PathwayName, COUNT(DISTINCT(ScName)) AS Count, FromTimestamp, ToTimestamp
                            FROM PvScstus
                            WHERE (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) AND 
                            PvScstus.ErrorNumber <> 0
                            GROUP BY PathwayName, FromTimestamp, ToTimestamp
                            ORDER BY PathwayName, FromTimestamp";
            var alerts = new List<Alert>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var alert = new Alert {
                        PathwayName = reader["PathwayName"].ToString(),
                        Count = Convert.ToInt64(reader["Count"]),
                        FromTimestamp = Convert.ToDateTime(reader["FromTimestamp"]),
                        ToTimestamp = Convert.ToDateTime(reader["ToTimestamp"])
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        #endregion
        
        #region Collection
        public List<Alert> GetTermHiMaxRt(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "") {
            string cmdText;
            if (pathwayName.Length == 0) {
                cmdText = @"SELECT PathwayName, COUNT(DISTINCT(TermName)) AS Count
                                FROM PvTermstat 
                                WHERE (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) AND MaxResp > " +
                                (_maxTimes * Convert.ToInt64(ConfigurationManager.AppSettings["ResponseTime"])) + @"
                                GROUP BY PathwayName
                                ORDER BY PathwayName";
            }
            else {
                cmdText = @"SELECT PathwayName, COUNT(DISTINCT(TermName)) AS Count
                                FROM PvTermstat 
                                WHERE (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) AND MaxResp > " +
                                (_maxTimes * Convert.ToInt64(ConfigurationManager.AppSettings["ResponseTime"])) + @" AND
                                PathwayName = @PathwayName
                                GROUP BY PathwayName
                                ORDER BY PathwayName";
            }

            var alerts = new List<Alert>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                if (pathwayName.Length != 0)
                    command.Parameters.AddWithValue("@PathwayName", pathwayName);

                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var alert = new Alert {
                        PathwayName = reader["PathwayName"].ToString(),
                        Count = Convert.ToInt64(reader["Count"])
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        public List<Alert> GetTermHiAvgRt(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "") {
            string cmdText;
            if (pathwayName.Length == 0) {
                cmdText = @"SELECT PathwayName, COUNT(DISTINCT(TermName)) AS Count
                            FROM PvTermstat 
                            WHERE (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) AND AvgResp > " +
                            (_maxTimes * Convert.ToInt64(ConfigurationManager.AppSettings["ResponseTime"])) + @"                   
                            GROUP BY PathwayName
                            ORDER BY PathwayName";
            }
            else {

                cmdText = @"SELECT PathwayName, COUNT(DISTINCT(TermName)) AS Count
                            FROM PvTermstat 
                            WHERE (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) AND AvgResp > " +
                            (_maxTimes * Convert.ToInt64(ConfigurationManager.AppSettings["ResponseTime"])) + @" AND
                            PathwayName = @PathwayName
                            GROUP BY PathwayName
                            ORDER BY PathwayName";
            }
            var alerts = new List<Alert>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                if (pathwayName.Length != 0)
                    command.Parameters.AddWithValue("@PathwayName", pathwayName);
                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var alert = new Alert {
                        PathwayName = reader["PathwayName"].ToString(),
                        Count = Convert.ToInt64(reader["Count"])
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        public List<Alert> GetTermUnused(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "") {
            string cmdText;

            if (pathwayName.Length == 0) {
                cmdText = @"SELECT PathwayName, COUNT(TotalIsReqCnt) AS Count
                            FROM  
                            (SELECT PathwayName, SUM(PvTermstat.IsReqCnt) AS TotalIsReqCnt
                             FROM PvTermstat WHERE (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp)
                            GROUP BY PathwayName,PvTermstat.TermName, PvTermstat.TermTcpName) DERIVEDTBL 
                            WHERE TotalIsReqCnt = 0 
                            GROUP BY PathwayName
                            ORDER BY PathwayName";
            }
            else {
                cmdText = @"SELECT PathwayName, COUNT(TotalIsReqCnt) AS Count
                            FROM  
                            (SELECT PathwayName, SUM(PvTermstat.IsReqCnt) AS TotalIsReqCnt
                             FROM PvTermstat WHERE (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) AND
                             PathwayName = @PathwayName
                             GROUP BY PathwayName,PvTermstat.TermName, PvTermstat.TermTcpName) DERIVEDTBL 
                            WHERE TotalIsReqCnt = 0 
                            GROUP BY PathwayName
                            ORDER BY PathwayName";
            }
            var alerts = new List<Alert>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                if (pathwayName.Length != 0)
                    command.Parameters.AddWithValue("@PathwayName", pathwayName);
                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var alert = new Alert {
                        PathwayName = reader["PathwayName"].ToString(),
                        Count = Convert.ToInt64(reader["Count"])
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        public List<Alert> GetTermErrorList(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "") {
            string cmdText;
            if (pathwayName.Length == 0) {
                cmdText = @"SELECT PathwayName, COUNT(DISTINCT( TermName)) AS Count
                            FROM PvTermstus
                            WHERE (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) AND 
                            PvTermstus.ErrorNumber <> 0
                            GROUP BY PathwayName
                            ORDER BY PathwayName";
            }
            else {
                cmdText = @"SELECT PathwayName, COUNT(DISTINCT( TermName)) AS Count
                            FROM PvTermstus
                            WHERE (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) AND 
                            PvTermstus.ErrorNumber <> 0 AND
                            PathwayName = @PathwayName
                            GROUP BY PathwayName
                            ORDER BY PathwayName";
            }
            var alerts = new List<Alert>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                if (pathwayName.Length != 0)
                    command.Parameters.AddWithValue("@PathwayName", pathwayName);
                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var alert = new Alert {
                        PathwayName = reader["PathwayName"].ToString(),
                        Count = Convert.ToInt64(reader["Count"])
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        public List<Alert> GetTcpQueuedTransactions(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "") {
            string cmdText;
            if (pathwayName.Length == 0) {
                cmdText = @"SELECT PathwayName, COUNT(DISTINCT(TcpName)) AS Count
                            FROM  PvTcpstat
                            WHERE QLReqCnt > 0  AND 
                            (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp)
                            GROUP BY PathwayName
                            ORDER BY PathwayName";
            }
            else {
                cmdText = @"SELECT PathwayName, COUNT(DISTINCT(TcpName)) AS Count
                            FROM  PvTcpstat
                            WHERE QLReqCnt > 0  AND 
                            (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) AND
                            PathwayName = @PathwayName
                            GROUP BY PathwayName
                            ORDER BY PathwayName";
            }
            var alerts = new List<Alert>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                if (pathwayName.Length != 0)
                    command.Parameters.AddWithValue("@PathwayName", pathwayName);
                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var alert = new Alert {
                        PathwayName = reader["PathwayName"].ToString(),
                        Count = Convert.ToInt64(reader["Count"])
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        public List<Alert> GetTcpLowTermPool(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "") {
            string cmdText;
            if (pathwayName.Length == 0) {
                cmdText = @"SELECT PathwayName, COUNT(DISTINCT(TcpName)) AS Count
                            FROM
                            (
	                            SELECT PathwayName, TcpName, PtSize, PtMaxAlloc, PtMaxReq, PsSize, PsMaxAlloc, PsMaxReq,
	                            PtAggregateReqSize/IFNULL(IFNULL(PtReqCnt, 0), 1) AS AvgTReqSize,
	                            PsAggregateReqSize/IFNULL(IFNULL(PsReqCnt, 0), 1) AS AvgSReqSize
	                            FROM  PvTcpstat 
	                            WHERE (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp)
                            ) temp1
                            WHERE ((PtSize - PtMaxAlloc) < temp1.AvgTReqSize OR (PtSize - PtMaxAlloc) < temp1.PtMaxReq)
                            GROUP BY PathwayName
                            ORDER BY PathwayName";
            }
            else {
                cmdText = @"SELECT PathwayName, COUNT(DISTINCT(TcpName)) AS Count
                            FROM
                            (
	                            SELECT PathwayName, TcpName, PtSize, PtMaxAlloc, PtMaxReq, PsSize, PsMaxAlloc, PsMaxReq,
	                            PtAggregateReqSize/IFNULL(IFNULL(PtReqCnt, 0), 1) AS AvgTReqSize,
	                            PsAggregateReqSize/IFNULL(IFNULL(PsReqCnt, 0), 1) AS AvgSReqSize
	                            FROM  PvTcpstat 
	                            WHERE (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) AND
                                PathwayName = @PathwayName
                            ) temp1
                            WHERE ((PtSize - PtMaxAlloc) < temp1.AvgTReqSize OR (PtSize - PtMaxAlloc) < temp1.PtMaxReq)
                            GROUP BY PathwayName
                            ORDER BY PathwayName";
            }
            var alerts = new List<Alert>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                if (pathwayName.Length != 0)
                    command.Parameters.AddWithValue("@PathwayName", pathwayName);
                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var alert = new Alert {
                        PathwayName = reader["PathwayName"].ToString(),
                        Count = Convert.ToInt64(reader["Count"])
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        public List<Alert> GetTcpLowServerPool(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "") {
            string cmdText;
            if (pathwayName.Length == 0) {
                cmdText = @"SELECT PathwayName, COUNT(DISTINCT(TcpName)) AS Count
                            FROM
                            (	
	                            SELECT PathwayName,TcpName,PtSize,PtMaxAlloc,PtMaxReq, PsSize,PsMaxAlloc,PsMaxReq,
	                            PtAggregateReqSize/IFNULL(IFNULL(PtReqCnt, 0), 1) AS AvgTReqSize,
	                            PsAggregateReqSize/IFNULL(IFNULL(PsReqCnt, 0), 1) AS AvgSReqSize
	                            FROM  PvTcpstat 
	                            WHERE (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp)
                            ) temp1
                            WHERE ((PsSize - PsMaxAlloc) < temp1.AvgSReqSize OR (PsSize - PsMaxAlloc) < temp1.PsMaxReq)
                            GROUP BY PathwayName
                            ORDER BY PathwayName";
            }
            else {
                cmdText = @"SELECT PathwayName, COUNT(DISTINCT(TcpName)) AS Count
                            FROM
                            (	
	                            SELECT PathwayName,TcpName,PtSize,PtMaxAlloc,PtMaxReq, PsSize,PsMaxAlloc,PsMaxReq,
	                            PtAggregateReqSize/IFNULL(IFNULL(PtReqCnt, 0), 1) AS AvgTReqSize,
	                            PsAggregateReqSize/IFNULL(IFNULL(PsReqCnt, 0), 1) AS AvgSReqSize
	                            FROM  PvTcpstat 
	                            WHERE (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) AND
                                PathwayName = @PathwayName
                            ) temp1
                            WHERE ((PsSize - PsMaxAlloc) < temp1.AvgSReqSize OR (PsSize - PsMaxAlloc) < temp1.PsMaxReq)
                            GROUP BY PathwayName
                            ORDER BY PathwayName";
            }
            var alerts = new List<Alert>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                if (pathwayName.Length != 0)
                    command.Parameters.AddWithValue("@PathwayName", pathwayName);
                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var alert = new Alert {
                        PathwayName = reader["PathwayName"].ToString(),
                        Count = Convert.ToInt64(reader["Count"])
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        public List<Alert> GetTcpUnused(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "") {
            string cmdText;
            if (pathwayName.Length == 0) {
                cmdText = @"SELECT PathwayName, COUNT(TermTcpName) AS Count
                            FROM (
                                SELECT PathwayName, SUM(PvTermstat.IsReqCnt) AS TotalIsReqCnt, TermTcpName
                                FROM PvTermstat 
                                WHERE (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp)
                                GROUP BY PathwayName, TermTcpName
                            ) DERIVEDTBL 
                            WHERE TotalIsReqCnt = 0 
                            GROUP BY PathwayName
                            ORDER BY PathwayName";
            }
            else {
                cmdText = @"SELECT PathwayName, COUNT(TermTcpName) AS Count
                            FROM (
                                SELECT PathwayName, SUM(PvTermstat.IsReqCnt) AS TotalIsReqCnt, TermTcpName
                                FROM PvTermstat 
                                WHERE (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) AND
                                PathwayName = @PathwayName
                                GROUP BY PathwayName, TermTcpName
                            ) DERIVEDTBL 
                            WHERE TotalIsReqCnt = 0 
                            GROUP BY PathwayName
                            ORDER BY PathwayName";
            }
            var alerts = new List<Alert>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                if (pathwayName.Length != 0)
                    command.Parameters.AddWithValue("@PathwayName", pathwayName);
                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var alert = new Alert {
                        PathwayName = reader["PathwayName"].ToString(),
                        Count = Convert.ToInt64(reader["Count"])
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        public List<Alert> GetTcpErrorList(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "") {
            string cmdText;
            if (pathwayName.Length == 0) {
                cmdText = @"SELECT PathwayName, COUNT(DISTINCT( TcpName)) AS Count
                            FROM PvTcpStus
                            WHERE
                            ErrorNumber <> 0 AND 
                            (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) 
                            GROUP BY PathwayName
                            ORDER BY PathwayName";
            }
            else {
                cmdText = @"SELECT PathwayName, COUNT(DISTINCT( TcpName)) AS Count
                            FROM PvTcpStus
                            WHERE
                            ErrorNumber <> 0 AND 
                            (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) AND
                            PathwayName = @PathwayName
                            GROUP BY PathwayName
                            ORDER BY PathwayName";
            }
            var alerts = new List<Alert>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                if (pathwayName.Length != 0)
                    command.Parameters.AddWithValue("@PathwayName", pathwayName);
                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var alert = new Alert {
                        PathwayName = reader["PathwayName"].ToString(),
                        Count = Convert.ToInt64(reader["Count"])
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        public List<Alert> GetServerHiMaxRt(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "") {
            string cmdText;
            if (pathwayName.Length == 0) {
                cmdText = @"SELECT PathwayName, COUNT(DISTINCT( ScName)) AS Count
                            FROM PvSctstat
                            WHERE (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) AND 
                            MaxResp > " + (_maxTimes * Convert.ToInt64(ConfigurationManager.AppSettings["ResponseTime"])) + @"
                            GROUP BY PathwayName
                            ORDER BY PathwayName";
            }
            else {
                cmdText = @"SELECT PathwayName, COUNT(DISTINCT( ScName)) AS Count
                            FROM PvSctstat
                            WHERE (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) AND
                            PathwayName = @PathwayName AND 
                            MaxResp > " + (_maxTimes * Convert.ToInt64(ConfigurationManager.AppSettings["ResponseTime"])) + @"                   
                            GROUP BY PathwayName
                            ORDER BY PathwayName";
            }
            var alerts = new List<Alert>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                if (pathwayName.Length != 0)
                    command.Parameters.AddWithValue("@PathwayName", pathwayName);
                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var alert = new Alert {
                        PathwayName = reader["PathwayName"].ToString(),
                        Count = Convert.ToInt64(reader["Count"])
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        public List<Alert> GetServerHiAvgRt(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "") {
            string cmdText;
            if (pathwayName.Length == 0) {
                cmdText = @"SELECT PathwayName, COUNT(DISTINCT(ScName)) AS Count
                            FROM PvSctstat
                            WHERE (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) AND 
                            AvgResp > " + (_maxTimes * Convert.ToInt64(ConfigurationManager.AppSettings["ResponseTime"])) + @"                   
                            GROUP BY PathwayName
                            ORDER BY PathwayName";
            }
            else {
                cmdText = @"SELECT PathwayName, COUNT(DISTINCT(ScName)) AS Count
                            FROM PvSctstat
                            WHERE (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) AND
                            PathwayName = @PathwayName AND 
                            AvgResp > " + (_maxTimes * Convert.ToInt64(ConfigurationManager.AppSettings["ResponseTime"])) + @"  
                            GROUP BY PathwayName
                            ORDER BY PathwayName";
            }
            var alerts = new List<Alert>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                if (pathwayName.Length != 0)
                    command.Parameters.AddWithValue("@PathwayName", pathwayName);
                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var alert = new Alert {
                        PathwayName = reader["PathwayName"].ToString(),
                        Count = Convert.ToInt64(reader["Count"])
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        public List<Alert> GetServerPendingClass(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "") {
            /*string cmdText;
            if (pathwayName.Length == 0) {
                cmdText = @"SELECT PathwayName, COUNT(DISTINCT(ScName)) AS Count
                            FROM PvScstus
                            WHERE (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) AND
                            FreezeState = 1              
                            GROUP BY PathwayName
                            ORDER BY PathwayName";
            }
            else {
                cmdText = @"SELECT PathwayName, COUNT(DISTINCT(ScName)) AS Count
                            FROM PvScstus
                            WHERE (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) AND
                            PathwayName = @PathwayName  AND
                            FreezeState = 1              
                            GROUP BY PathwayName
                            ORDER BY PathwayName";
            }*/
            string cmdText;
            if (pathwayName.Length == 0) {
                cmdText = @"SELECT PathwayName, COUNT(DISTINCT(SCName)) AS Count
                            FROM PvScprstus
                            WHERE (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) AND
                            ProcState = 1               
                            GROUP BY PathwayName
                            ORDER BY PathwayName";
            }
            else {
                cmdText = @"SELECT PathwayName, COUNT(DISTINCT(SCName)) AS Count
                            FROM PvScprstus
                            WHERE (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) AND
                            PathwayName = @PathwayName  AND
                            ProcState = 1               
                            GROUP BY PathwayName
                            ORDER BY PathwayName";
            }
            var alerts = new List<Alert>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                if (pathwayName.Length != 0)
                    command.Parameters.AddWithValue("@PathwayName", pathwayName);
                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var alert = new Alert {
                        PathwayName = reader["PathwayName"].ToString(),
                        Count = Convert.ToInt64(reader["Count"])
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        public List<Alert> GetServerPendingProcess(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "") {
            string cmdText;
            if (pathwayName.Length == 0) {
                cmdText = @"SELECT PathwayName, COUNT(DISTINCT(SCProcessName)) AS Count
                            FROM PvScprstus
                            WHERE (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) AND
                            ProcState = 1               
                            GROUP BY PathwayName
                            ORDER BY PathwayName";
            }
            else {
                cmdText = @"SELECT PathwayName, COUNT(DISTINCT(SCProcessName)) AS Count
                            FROM PvScprstus
                            WHERE (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) AND
                            PathwayName = @PathwayName  AND
                            ProcState = 1               
                            GROUP BY PathwayName
                            ORDER BY PathwayName";
            }
            var alerts = new List<Alert>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                if (pathwayName.Length != 0)
                    command.Parameters.AddWithValue("@PathwayName", pathwayName);
                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var alert = new Alert {
                        PathwayName = reader["PathwayName"].ToString(),
                        Count = Convert.ToInt64(reader["Count"])
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        public List<Alert> GetServerQueueTcp(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "") {
            string cmdText;
            if (pathwayName.Length == 0) {
                cmdText = @"SELECT PathwayName, COUNT(DISTINCT(ScName)) AS Count
                            FROM PvSctstat
                            WHERE (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) AND
                            QIWaits > 0
                            GROUP BY PathwayName
                            ORDER BY PathwayName";
            }
            else {
                cmdText = @"SELECT PathwayName, COUNT(DISTINCT(ScName)) AS Count
                            FROM PvSctstat
                            WHERE (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) AND
                            PathwayName = @PathwayName AND 
                            QIWaits > 0
                            GROUP BY PathwayName
                            ORDER BY PathwayName";
            }
            var alerts = new List<Alert>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                if (pathwayName.Length != 0)
                    command.Parameters.AddWithValue("@PathwayName", pathwayName);
                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var alert = new Alert {
                        PathwayName = reader["PathwayName"].ToString(),
                        Count = Convert.ToInt64(reader["Count"])
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        public List<Alert> GetServerQueueLinkmon(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "") {
            string cmdText;
            if (pathwayName.Length == 0) {
                cmdText = @"SELECT PathwayName, COUNT(DISTINCT(ScName)) AS Count
                            FROM PvSclstat
                            WHERE (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) AND 
                            QIWaits > 0
                            GROUP BY PathwayName
                            ORDER BY PathwayName";
            }
            else {
                cmdText = @"SELECT PathwayName, COUNT(DISTINCT(ScName)) AS Count
                            FROM PvSclstat
                            WHERE (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) AND
                            PathwayName = @PathwayName AND 
                            QIWaits > 0
                            GROUP BY PathwayName
                            ORDER BY PathwayName";
            }
            var alerts = new List<Alert>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                if (pathwayName.Length != 0)
                    command.Parameters.AddWithValue("@PathwayName", pathwayName);
                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var alert = new Alert {
                        PathwayName = reader["PathwayName"].ToString(),
                        Count = Convert.ToInt64(reader["Count"])
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        public List<Alert> GetServerUnusedClass(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "") {
            string cmdText;
            if (pathwayName.Length == 0) {
                cmdText = @"SELECT PathwayName, COUNT(ScName) AS Count
                            FROM
                            (
	                            SELECT IFNULL(temp1.ScName, temp2.ScName) AS ScName,
	                            IFNULL(temp1.PathwayName, temp2.PathwayName) AS PathwayName,
	                            IFNULL(temp1.SumofIsReqCnt, 0) AS tran1,
	                            IFNULL(temp2.SumofIsReqCnt, 0) AS tran2
	                            FROM (
		                            SELECT ScName,PathwayName, SUM(IsReqCnt) AS SumofIsReqCnt FROM PvSctstat
		                            WHERE (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp)
		                            GROUP BY ScName, PathwayName
	                            ) temp1
	                            LEFT JOIN (
		                            SELECT PvSclstat.ScName,PathwayName, SUM(PvSclstat.IsReqCnt) AS SumofIsReqCnt
		                            FROM PvSclstat 
		                            WHERE (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp)
		                            GROUP BY PvSclstat.SCName,PathwayName
	                            ) temp2 
                            ON temp1.ScName = temp2.ScName AND temp1.PathwayName = temp2.PathwayName) temp3
                            WHERE tran1 = 0 AND tran2 = 0
                            GROUP BY PathwayName
                            ORDER BY PathwayName";
            }
            else {
                cmdText = @"SELECT PathwayName, COUNT(ScName) AS Count
                            FROM
                            (
	                            SELECT IFNULL(temp1.ScName, temp2.ScName) AS ScName,
	                            IFNULL(temp1.PathwayName, temp2.PathwayName) AS PathwayName,
	                            IFNULL(temp1.SumofIsReqCnt, 0) AS tran1,
	                            IFNULL(temp2.SumofIsReqCnt, 0) AS tran2
	                            FROM (
		                            SELECT ScName,PathwayName, SUM(IsReqCnt) AS SumofIsReqCnt FROM PvSctstat
		                            WHERE (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) AND
                                    PathwayName = @PathwayName
		                            GROUP BY ScName, PathwayName
	                            ) temp1
	                            LEFT JOIN (
		                            SELECT PvSclstat.ScName,PathwayName, SUM(PvSclstat.IsReqCnt) AS SumofIsReqCnt
		                            FROM PvSclstat 
		                            WHERE (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) AND
                                    PathwayName = @PathwayName
		                            GROUP BY PvSclstat.SCName, PathwayName
	                            ) temp2 
                            ON temp1.ScName = temp2.ScName AND temp1.PathwayName = temp2.PathwayName) temp3
                            WHERE tran1 = 0 AND tran2 = 0
                            GROUP BY PathwayName
                            ORDER BY PathwayName";
            }
            var alerts = new List<Alert>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                if (pathwayName.Length != 0)
                    command.Parameters.AddWithValue("@PathwayName", pathwayName);
                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var alert = new Alert {
                        PathwayName = reader["PathwayName"].ToString(),
                        Count = Convert.ToInt64(reader["Count"])
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        public List<Alert> GetServerUnusedProcess(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "") {
            string cmdText;
            if (pathwayName.Length == 0) {
                cmdText = @"SELECT PathwayName, COUNT(ScName) AS Count FROM (
	                        SELECT PvScprstus.PathwayName, PvScprstus.ScName
	                        FROM PvScprstus 
	                        LEFT OUTER JOIN PvScinfo  
	                        ON (
		                        PvScprstus.Pathwayname = PvScinfo.Pathwayname AND PvScprstus.Scname = PvScinfo.Scname
	                        )
	                        WHERE (PvScprstus.FromTimestamp >= @FromTimestamp AND  PvScprstus.ToTimestamp <= @ToTimestamp) AND
                            (PvScinfo.FromTimestamp >= @FromTimestamp AND  PvScinfo.ToTimestamp <= @ToTimestamp) AND
	                        PvScprstus.ProcState = 2
	                        GROUP BY PvScprstus.PathwayName, PvScprstus.ScName, PvScprstus.ScProcessName, PvScinfo.MaxServers, 
			                         PvScinfo.NumStatic
	                        HAVING (SUM(ProcLinks) = 0) 
                        ) AS Temp
                        GROUP BY PathwayName
                        ORDER BY PathwayName";
            }
            else {
                cmdText = @"SELECT PathwayName, COUNT(ScName) AS Count FROM (
	                        SELECT PvScprstus.PathwayName, PvScprstus.ScName
	                        FROM PvScprstus 
	                        LEFT OUTER JOIN PvScinfo  
	                        ON (
		                        PvScprstus.Pathwayname = PvScinfo.Pathwayname AND PvScprstus.Scname = PvScinfo.Scname
	                        )
	                        WHERE
                            (PvScprstus.FromTimestamp >= @FromTimestamp AND  PvScprstus.ToTimestamp <= @ToTimestamp) AND
                            (PvScinfo.FromTimestamp >= @FromTimestamp AND  PvScinfo.ToTimestamp <= @ToTimestamp) AND
                            PvScprstus.PathwayName = @PathwayName AND 
                            PvScprstus.ProcState = 2
	                        GROUP BY PvScprstus.PathwayName, PvScprstus.ScName, PvScprstus.ScProcessName, PvScinfo.MaxServers, 
			                         PvScinfo.NumStatic
	                        HAVING (SUM(ProcLinks) = 0) 
                        ) AS Temp
                        GROUP BY PathwayName
                        ORDER BY PathwayName";
            }
            var alerts = new List<Alert>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                if (pathwayName.Length != 0)
                    command.Parameters.AddWithValue("@PathwayName", pathwayName);
                command.CommandTimeout = 180;//Manually increase the timeout to 3 mins.
                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var alert = new Alert {
                        PathwayName = reader["PathwayName"].ToString(),
                        Count = Convert.ToInt64(reader["Count"])
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        public List<Alert> GetServerMaxLinks(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "")
        {
            var cmdText = @"SELECT T.PathwayName, COUNT(DISTINCT(T.ScName)) as ScNameCount
                            FROM(
                                 SELECT PvSclstat.PathwayName as PathwayName, PvSclstat.ScName as ScName, MAX(PvScinfo.MaxLinks) AS MaxLinks
                                 FROM PvSclstat 
                                 LEFT OUTER JOIN PvScinfo  ON (PvSclstat.Pathwayname = PvScinfo.Pathwayname AND PvSclstat.Scname = PvScinfo.Scname)
                                 WHERE (PvSclstat.FromTimestamp >= @FromTimestamp AND PvSclstat.ToTimestamp <= @ToTimestamp) AND
                                       (PvScinfo.FromTimestamp  >= @FromTimestamp AND PvScinfo.ToTimestamp  <= @ToTimestamp) 
                                 GROUP BY PvSclstat.PathwayName, PvSclstat.ScName, PvSclstat.FromTimestamp, PvSclstat.ToTimestamp
                            ) AS T
                            GROUP BY T.PathwayName
                            HAVING MAX(MaxLinks) > 100
                            ORDER BY T.PathwayName ";

            var alerts = new List<Alert>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var alert = new Alert
                    {
                        PathwayName = reader["PathwayName"].ToString(),
                        Count = Convert.ToInt64(reader["ScNameCount"]),
                        FromTimestamp = Convert.ToDateTime(fromTimestamp),
                        ToTimestamp = Convert.ToDateTime(toTimestamp)
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        public List<Alert> GetCheckDirectoryOn(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "")
        {
            var cmdText = @"SELECT T.PathwayName, COUNT(DISTINCT(T.tcpname)) as tcpnameCount
                            FROM(
                                SELECT PvTcpinfo.PathwayName, PvTcpinfo.tcpname as tcpname
                                FROM PvTcpinfo 
                                WHERE (PvTcpinfo.FromTimestamp >= @FromTimestamp AND  PvTcpinfo.ToTimestamp <= @ToTimestamp) AND CheckDirectory != 0
                                GROUP BY PvTcpinfo.PathwayName, PvTcpinfo.tcpname
                            ) as T
                            GROUP BY T.PathwayName
                            ORDER BY T.PathwayName  ";

            var alerts = new List<Alert>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var alert = new Alert
                    {
                        PathwayName = reader["PathwayName"].ToString(),
                        Count = Convert.ToInt64(reader["tcpnameCount"]),
                        FromTimestamp = Convert.ToDateTime(fromTimestamp),
                        ToTimestamp = Convert.ToDateTime(toTimestamp)
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        public List<Alert> GetHighDynamicServers(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "")
        {
            var cmdText = @"SELECT X.PathwayName, COUNT(*) AS SCNameCount
                            FROM (
                                SELECT T.PathwayName, T.SCName
                                FROM (
                                    SELECT PvScprstus.Pathwayname, PvScprstus.scname as SCName, COUNT(PvScprstus.ScProcessName) as SCPName,  PvScprstus.FromTimestamp, PvScprstus.ToTimestamp
                                    FROM PvScprstus  
                                    WHERE (PvScprstus.FromTimestamp >= @FromTimestamp AND  PvScprstus.ToTimestamp <= @ToTimestamp) 
                                    GROUP BY PvScprstus.Pathwayname, PvScprstus.scname, PvScprstus.FromTimestamp, PvScprstus.ToTimestamp
                                ) AS T
                                LEFT OUTER JOIN PvScinfo  ON (PvScinfo.Pathwayname = T.Pathwayname AND PvScinfo.Scname = T.Scname)
                                GROUP BY T.PathwayName, T.SCName
                                HAVING MAX(SCPNAme) > MAX(PvScinfo.NumStatic)
                            ) AS X
                            GROUP BY X.PathwayName
                            ORDER BY X.PathwayName  ";

            var alerts = new List<Alert>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.CommandTimeout = 0;
                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var alert = new Alert
                    {
                        PathwayName = reader["PathwayName"].ToString(),
                        Count = Convert.ToInt64(reader["SCNameCount"]),
                        FromTimestamp = Convert.ToDateTime(fromTimestamp),
                        ToTimestamp = Convert.ToDateTime(toTimestamp)
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }
        
        public List<Alert> GetServerErrorList(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "")
        {
            string cmdText;
            if (pathwayName.Length == 0) {
                cmdText = @"SELECT PathwayName, COUNT(DISTINCT(ScName)) AS Count
                            FROM PvScstus
                            WHERE (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) AND 
                            PvScstus.ErrorNumber <> 0
                            GROUP BY PathwayName
                            ORDER BY PathwayName";
            }
            else {
                cmdText = @"SELECT PathwayName, COUNT(DISTINCT(ScName)) AS Count
                            FROM PvScstus
                            WHERE (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) AND 
                            PvScstus.ErrorNumber <> 0 AND
                            PathwayName = @PathwayName
                            GROUP BY PathwayName
                            ORDER BY PathwayName";
            }
            var alerts = new List<Alert>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                if (pathwayName.Length != 0)
                    command.Parameters.AddWithValue("@PathwayName", pathwayName);
                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var alert = new Alert {
                        PathwayName = reader["PathwayName"].ToString(),
                        Count = Convert.ToInt64(reader["Count"])
                    };

                    alerts.Add(alert);
                }
            }

            return alerts;
        }

        #endregion

        public void UpdateAlert(string alertName, string pathwayName, long count, DateTime fromTimestamp, DateTime toTimestamp) {
            var cmdText = @"UPDATE PvAlerts  SET " + alertName +
                          @" = @Value WHERE
                            FromTimestamp >= @FromTimestamp AND
                            ToTimestamp <= @ToTimestamp AND
                            Pathway = @Pathway";

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.Parameters.AddWithValue("@Pathway", pathwayName);
                command.Parameters.AddWithValue("@Value", count);
                command.CommandTimeout = 0;connection.Open();
                command.ExecuteNonQuery();
            }
        }

    }
}