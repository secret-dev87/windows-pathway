using System;
using System.Collections.Generic;
using System.Reflection;
using MySqlConnector;
using System.IO;
using Pathway.Core.Abstract;
using Pathway.Core.Infrastructure;
using Pathway.Core.Infrastructure.PerPathway.Server;
using log4net;

namespace Pathway.Core.Concrete {
    public class PvScPrStus : IPvScPrStus {
        private static readonly ILog Log = LogManager.GetLogger("PathwayReport");
        private readonly string _connectionString = "";

        public PvScPrStus(string connectionString) {
            _connectionString = connectionString;
        }

        public long GetServerBusy(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName) {
            string cmdText = @"SELECT 
                            SUM(DeltaProcTime) AS TotalBusyTime
                            FROM PvScprstus WHERE
                            (FromTimestamp >= @FromTimestamp AND FromTimestamp < @ToTimestamp AND
                            ToTimestamp > @FromTimestamp AND ToTimestamp <= @ToTimestamp)
                            AND PathwayName = @PathwayName";

            long serverBusy = 0;

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);

                connection.Open();
                var reader = command.ExecuteReader();

                if (reader.Read()) {
                    if (!reader.IsDBNull(0))
                        serverBusy = Convert.ToInt64(reader["TotalBusyTime"]);
                }
            }
            return serverBusy;
        }

        public List<CPUView> GetServerBusyPerCPU(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName) {
            var cmdText = @"SELECT SUM(DeltaProcTime) AS TotalBusyTime,
                            ScPrCpu 
                            FROM PvScprstus WHERE
                            (FromTimestamp >= @FromTimestamp AND FromTimestamp < @ToTimestamp AND
                            ToTimestamp > @FromTimestamp AND ToTimestamp <= @ToTimestamp)
                            AND PathwayName = @PathwayName
                            AND ScPrCpu <> '' AND ScPrCpu <> '\0\0'
                            GROUP BY ScPrCpu
                            ORDER BY ScPrCpu";

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
                        CPUNumber = reader["ScPrCpu"].ToString(),
                        Value = Convert.ToInt64(reader["TotalBusyTime"])
                    };
                    tcpProcessBusy.Add(cpuView);
                }
            }
            return tcpProcessBusy;
        }

        public List<CPUView> GetServerCPUBusyProcessCount(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName) {
            var cmdText = @"SELECT 
                            AVG(ScProcessCount) AS AvgofScProcessCount, 
                            MAX(ScProcessCount) AS MaxofScProcessCount, 
                            PathwayName, 
                            ScName 
                            FROM (
	                            SELECT 
	                            PvScprstus.PathwayName, 
	                            PvScprstus.ScName, 
	                            COUNT(PvScprstus.ScProcessName) AS ScProcessCount 
	                            FROM PvScprstus
	                            WHERE
	                            (PvScprstus.FromTimestamp >= @FromTimestamp AND PvScprstus.FromTimestamp < @ToTimestamp AND
									PvScprstus.ToTimestamp > @FromTimestamp AND PvScprstus.ToTimestamp <= @ToTimestamp)
	                            GROUP BY PvScprstus.PathwayName, PvScprstus.ScName, PvScprstus.FromTimestamp
	                            ) DERIVEDTBL 
                            WHERE PathwayName = @PathwayName 
                            GROUP BY PathwayName, ScName";

            var serverCPUBusy = new List<CPUView>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);

                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var cpuBusy = new CPUView {
                        CPUNumber = reader["ScName"].ToString(),
                        Value = Convert.ToInt64(reader["AvgofScProcessCount"]),
                        Value2 = Convert.ToInt64(reader["MaxofScProcessCount"])
                    };

                    serverCPUBusy.Add(cpuBusy);
                }
            }
            return serverCPUBusy;
        }

        public List<CPUView> GetServerBusyPerPathway(DateTime fromTimestamp, DateTime toTimestamp) {
            var cmdText = @"SELECT 
                            SUM(DeltaProcTime) AS TotalBusyTime,
                            PathwayName 
                            FROM PvScprstus WHERE
                            (FromTimestamp >= @FromTimestamp AND FromTimestamp < @ToTimestamp AND
                            ToTimestamp > @FromTimestamp AND ToTimestamp <= @ToTimestamp)
                            GROUP BY PathwayName";

            var serverBusy = new List<CPUView>();

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
                    serverBusy.Add(cpuView);
                }
            }
            return serverBusy;
        }

        public Dictionary<string, List<CPUView>> GetServerBusyPerCPUPerPathway(DateTime fromTimestamp, DateTime toTimestamp) {
            var cmdText = @"SELECT 
                            ScPrCpu,
                            PathwayName,
                            SUM(DeltaProcTime) AS TotalBusyTime
                            FROM PvScprstus WHERE
                            (FromTimestamp >= @FromTimestamp AND FromTimestamp < @ToTimestamp AND
                            ToTimestamp > @FromTimestamp AND ToTimestamp <= @ToTimestamp)
                            AND ScPrCpu <> '' AND ScPrCpu <> '\0\0'
                            GROUP BY ScPrCpu, PathwayName
                            ORDER BY ScPrCpu";

            var views = new Dictionary<string, List<CPUView>>();

            try {
                using (var connection = new MySqlConnection(_connectionString)) {
                    var command = new MySqlCommand(cmdText, connection);
                    command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                    command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);

                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read()) {
                        if (reader["PathwayName"] != DBNull.Value &&
                            reader["TotalBusyTime"] != DBNull.Value &&
                            reader["ScPrCpu"] != DBNull.Value) {
                            var view = new CPUView {
                                CPUNumber = reader["PathwayName"].ToString(),
                                Value = Convert.ToInt64(reader["TotalBusyTime"])
                            };
                            string cpuNumber = reader["ScPrCpu"].ToString();
                            if (!views.ContainsKey(cpuNumber))
                                views.Add(reader["ScPrCpu"].ToString(), new List<CPUView> {view});
                            else
                                views[cpuNumber].Add(view);
                        }
                    }
                }
            }
            catch (Exception ex) {
                Log.ErrorFormat("cmd: {0}, FromTimestamp: {1}, ToTimestamp: {2}",
                    cmdText, fromTimestamp, toTimestamp);
                throw new Exception(ex.Message);
            }

            return views;
        }

        public List<ServerUnusedServerProcessesView> GetServerUnusedServerProcessPerPathway(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName) {
            var cmdText = @"SELECT 
                            ScName, 
                            ScProcessName 
                            FROM (
	                            SELECT 
	                            PvScprstus.ScName, 
	                            SUM(PvScprstus.ProcLinks) AS SumofProcLinks, 
	                            PvScprstus.ScProcessName 
	                            FROM PvScprstus 
	                            WHERE PvScprstus.ProcState=2 AND 
	                            PvScprstus.PathwayName = @PathwayName AND 
	                            (FromTimestamp >= @FromTimestamp AND FromTimestamp < @ToTimestamp AND
									ToTimestamp > @FromTimestamp AND ToTimestamp <= @ToTimestamp)
	                            GROUP BY PvScprstus.ScName, PvScprstus.ScProcessName
                            ) DERIVEDTBL 
                            WHERE (SumofProcLinks = 0) 
                            ORDER BY ScName";

            var serverUnusedProcesses = new List<ServerUnusedServerProcessesView>();

            try {
                using (var connection = new MySqlConnection(_connectionString)) {
                    var command = new MySqlCommand(cmdText, connection);
                    command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                    command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                    command.Parameters.AddWithValue("@PathwayName", pathwayName);

                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read()) {
                        if (reader["ScName"] != DBNull.Value &&
                            reader["ScProcessName"] != DBNull.Value) {
                            var view = new ServerUnusedServerProcessesView {
                                ServerClass = reader["ScName"].ToString(),
                                Process = reader["ScProcessName"].ToString()
                            };
                            serverUnusedProcesses.Add(view);
                        }
                    }
                }
            }
            catch (Exception ex) {
                Log.ErrorFormat("cmd: {0}, FromTimestamp: {1}, ToTimestamp: {2}",
                    cmdText, fromTimestamp, toTimestamp);
                throw new Exception(ex.Message);
            }

            return serverUnusedProcesses;
        }

        public List<UnusedServerProcesses> GetUnusedServerProcesseses(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName) {
            var cmdText = @"SELECT
                            PvScprstus.ScName, 
                            PvScprstus.ScProcessName, 
                            IFNULL(PvScinfo.MaxServers, 0) AS MaxServers,
                            IFNULL(PvScinfo.NumStatic, 0) AS NumStatic
                            FROM
                            PvScprstus 
                            LEFT OUTER JOIN PvScinfo  ON ( 
                                PvScprstus.Pathwayname = PvScinfo.Pathwayname AND 
                                PvScprstus.Scname = PvScinfo.Scname
                            )
                            WHERE 
                            (PvScprstus.FromTimestamp >= @FromTimestamp AND PvScprstus.FromTimestamp < @ToTimestamp AND
								PvScprstus.ToTimestamp > @FromTimestamp AND PvScprstus.ToTimestamp <= @ToTimestamp) AND
                            (PvScinfo.FromTimestamp >= @FromTimestamp AND PvScinfo.FromTimestamp < @ToTimestamp AND
								PvScinfo.ToTimestamp > @FromTimestamp AND PvScinfo.ToTimestamp <= @ToTimestamp) AND
                            PvScprstus.ProcState = 2 AND
                            PvScprstus.PathwayName = @PathwayName
                            GROUP BY PvScprstus.PathwayName, PvScprstus.ScName, PvScprstus.ScProcessName, PvScinfo.MaxServers, PvScinfo.NumStatic
                            HAVING (SUM(ProcLinks) = 0) 
                            ORDER BY PvScprstus.ScName";

            var serverUnusedProcesses = new List<UnusedServerProcesses>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);
                command.CommandTimeout = 180;//Manually increase the timeout to 3 mins.
                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var view = new UnusedServerProcesses {
                        ServerClass = reader["ScName"].ToString(),
                        Process = reader["ScProcessName"].ToString(),
                        MaxServers = Convert.ToInt32(reader["MaxServers"]),
                        NumStatic = Convert.ToInt32(reader["NumStatic"])
                    };
                    serverUnusedProcesses.Add(view);
                }
            }
            return serverUnusedProcesses;
        }

        public List<ServerMaxLinks> GetServerMaxLinks(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName)
        {
            var cmdText = @"SELECT T.SCName, MAX(LinksUsed) as LinkUsed, MAX(MaxLinks) as MaxLinks
                            FROM(
                                 SELECT PvSclstat.ScName as ScName, MAX(PvScinfo.MaxLinks) AS MaxLinks, COUNT(PvSclstat.ScLmName) AS LinksUsed
                                 FROM PvSclstat 
                                 LEFT OUTER JOIN PvScinfo  ON (PvSclstat.Pathwayname = PvScinfo.Pathwayname AND PvSclstat.Scname = PvScinfo.Scname)
                                 WHERE (PvSclstat.FromTimestamp >= @FromTimestamp AND PvSclstat.FromTimestamp < @ToTimestamp AND
											PvSclstat.ToTimestamp > @FromTimestamp AND PvSclstat.ToTimestamp <= @ToTimestamp) AND
									   (PvScinfo.FromTimestamp >= @FromTimestamp AND PvScinfo.FromTimestamp < @ToTimestamp AND
											PvScinfo.ToTimestamp > @FromTimestamp AND PvScinfo.ToTimestamp <= @ToTimestamp) AND
                                        PvSclstat.PathwayName    = @PathwayName
                                 GROUP BY PvSclstat.PathwayName, PvSclstat.ScName, PvSclstat.FromTimestamp, PvSclstat.ToTimestamp
                            ) AS T
                            GROUP BY T.ScName
                            HAVING MAX(MaxLinks) > 100
                            ORDER BY T.ScName ";

            var serverMaxLinksProcesses = new List<ServerMaxLinks>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);
                command.CommandTimeout = 180; //Manually increase the timeout to 3 mins.
                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var view = new ServerMaxLinks
                    {
                        ServerClass = reader["ScName"].ToString(),
                        LinksUsed = Convert.ToInt32(reader["LinkUsed"]),
                        MaxLinks = Convert.ToInt32(reader["MaxLinks"])
                    };
                    serverMaxLinksProcesses.Add(view);
                }
            }
            return serverMaxLinksProcesses;
        }

        public List<CheckDirectoryON> GetCheckDirectoryOnDetail(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName)
        {
            var cmdText = @"SELECT DISTINCT(T.tcpname) as tcpname
                            FROM(
                                SELECT PvTcpinfo.PathwayName, PvTcpinfo.tcpname as tcpname
                                FROM PvTcpinfo 
                                WHERE (PvTcpinfo.FromTimestamp >= @FromTimestamp AND PvTcpinfo.FromTimestamp < @ToTimestamp AND
									   PvTcpinfo.ToTimestamp > @FromTimestamp AND PvTcpinfo.ToTimestamp <= @ToTimestamp)
                                       AND PvTcpinfo.PathwayName    = @PathwayName   AND CheckDirectory != 0
                                GROUP BY PvTcpinfo.PathwayName, PvTcpinfo.tcpname
                            ) AS T
                            GROUP BY T.tcpname
                            ORDER BY T.tcpname ";

            var checkDirectoryON = new List<CheckDirectoryON>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);
                command.CommandTimeout = 180; //Manually increase the timeout to 3 mins.
                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var view = new CheckDirectoryON
                    {
                        ServerClass = reader["tcpname"].ToString(),
                    };
                    checkDirectoryON.Add(view);
                }
            }
            return checkDirectoryON;
        }

        public List<HighDynamicServers> GetHighDynamicServersDetail(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName)
        {
            var cmdText = @"SELECT T.SCName, MAX(SCPName) ProcessCount, MAX(PvScinfo.NumStatic) AS 'NumStatic', MAX(PvScinfo.MaxServers) AS 'MaxServers' 
                            FROM (
                                SELECT PvScprstus.Pathwayname, PvScprstus.scname as SCName, COUNT(PvScprstus.ScProcessName) as SCPName,  PvScprstus.FromTimestamp, PvScprstus.ToTimestamp
                                FROM PvScprstus  
                                WHERE (PvScprstus.FromTimestamp >= @FromTimestamp AND PvScprstus.FromTimestamp < @ToTimestamp AND
										PvScprstus.ToTimestamp > @FromTimestamp AND PvScprstus.ToTimestamp <= @ToTimestamp)
                                       AND PvScprstus.PathwayName    = @PathwayName
                                GROUP BY PvScprstus.Pathwayname, PvScprstus.scname, PvScprstus.FromTimestamp, PvScprstus.ToTimestamp
                            ) AS T
                            LEFT OUTER JOIN PvScinfo  ON (PvScinfo.Pathwayname = T.Pathwayname AND PvScinfo.Scname = T.Scname)
                            GROUP BY T.scname
                            HAVING MAX(SCPNAme) > MAX(PvScinfo.NumStatic)
                            ORDER BY T.scname ";

            var highDynamicServers = new List<HighDynamicServers>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);
                command.CommandTimeout = 0; //It's keep timing out. Need to change this to no timeout.
                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var view = new HighDynamicServers
                    {
                        ServerClass = reader["SCName"].ToString(),
                        ProcessCount = Convert.ToInt32(reader["ProcessCount"]),
                        NumStatic = Convert.ToInt32(reader["NumStatic"]),
                        MaxServers = Convert.ToInt32(reader["MaxServers"])
                    };
                    highDynamicServers.Add(view);
                }
            }
            return highDynamicServers;
        }
        
        public List<ServerUnusedServerProcessesView> GetServerPendingProcess(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName) {
            var cmdText = @"SELECT
                            ScName, 
                            SCProcessName
                            FROM PvScprstus 
                            WHERE 
                            (FromTimestamp >= @FromTimestamp AND FromTimestamp < @ToTimestamp AND
                            ToTimestamp > @FromTimestamp AND ToTimestamp <= @ToTimestamp) AND
                            ProcState = 1 AND
                            PathwayName = @PathwayName
                            ORDER BY ScName";

            var serverUnusedProcesses = new List<ServerUnusedServerProcessesView>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);
                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var view = new ServerUnusedServerProcessesView {
                        ServerClass = reader["ScName"].ToString(),
                        Process = reader["ScProcessName"].ToString(),
                    };
                    serverUnusedProcesses.Add(view);
                }
            }
            return serverUnusedProcesses;
        }

        public Dictionary<string, double> GetCPUBusy(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName) {
            var cmdText = @"SELECT 
                            SCName, SUM(DeltaProcTime) AS TotalBusyTime
                            FROM PvScprstus WHERE
                            (FromTimestamp >= @FromTimestamp AND FromTimestamp < @ToTimestamp AND
                            ToTimestamp > @FromTimestamp AND ToTimestamp <= @ToTimestamp)
                            AND PathwayName = @PathwayName
                            GROUP BY SCName";

            var cpuBusies = new Dictionary<string, double>();
            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);

                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    cpuBusies.Add(reader["SCName"].ToString(), Convert.ToDouble(reader["TotalBusyTime"]));
                }
            }

            return cpuBusies;
        }

        public List<CPUBusyPerInterval> GetCPUBusyPerInterval(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName) {
            var cmdText = @"SELECT 
                            SCName, 
                            SUM(DeltaProcTime) AS TotalBusyTime,
                            DATE_FORMAT(FromTimestamp, '%Y-%m-%d %H:%i:00') AS FromTimestamp
                            FROM PvScprstus WHERE
                            (FromTimestamp >= @FromTimestamp AND FromTimestamp < @ToTimestamp AND
                            ToTimestamp > @FromTimestamp AND ToTimestamp <= @ToTimestamp)
                            AND PathwayName = @PathwayName
                            GROUP BY SCName, FromTimestamp
                            ORDER BY SCName, FromTimestamp";

            var cpuBusies = new List<CPUBusyPerInterval>();
            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);

                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    //cpuBusies.Add(reader["SCName"].ToString(), Convert.ToDouble(reader["TotalBusyTime"]));
                    cpuBusies.Add(new CPUBusyPerInterval {
                        ScName = reader["SCName"].ToString(),
                        TotalBusyTime = Convert.ToDouble(reader["TotalBusyTime"]),
                        FromTimestamp = Convert.ToDateTime(reader["FromTimestamp"])
                    });
                }
            }

            return cpuBusies;
        }

        public List<CPUBusyPerInterval> GetCPUBusyPercentPerInterval(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName) {
            var cmdText = @"SELECT 
                            SCName, 
                            (SUM(DeltaProcTime) / SUM(DeltaTime)) * 100 AS TotalBusyTime,
                            DATE_FORMAT(FromTimestamp, '%Y-%m-%d %H:%i:00') AS FromTimestamp
                            FROM PvScprstus WHERE
                            (FromTimestamp >= @FromTimestamp AND FromTimestamp < @ToTimestamp AND
                            ToTimestamp > @FromTimestamp AND ToTimestamp <= @ToTimestamp)
                            AND PathwayName = @PathwayName
                            GROUP BY SCName, FromTimestamp
                            ORDER BY SCName, FromTimestamp";

            var cpuBusies = new List<CPUBusyPerInterval>();
            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);

                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    //cpuBusies.Add(reader["SCName"].ToString(), Convert.ToDouble(reader["TotalBusyTime"]));
                    cpuBusies.Add(new CPUBusyPerInterval {
                        ScName = reader["SCName"].ToString(),
                        TotalBusyTime = Convert.ToDouble(reader["TotalBusyTime"]),
                        FromTimestamp = Convert.ToDateTime(reader["FromTimestamp"])
                    });
                }
            }

            return cpuBusies;
        }
    }
}