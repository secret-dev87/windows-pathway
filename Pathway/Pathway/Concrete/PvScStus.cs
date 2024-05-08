using System;
using System.Collections.Generic;
using MySqlConnector;
using Pathway.Core.Abstract;
using Pathway.Core.Infrastructure.PerPathway.Server;

namespace Pathway.Core.Concrete {
    public class PvScStus : IPvScStus {
        private readonly string _connectionString = "";

        public PvScStus(string connectionString) {
            _connectionString = connectionString;
        }
        public List<ServerErrorListView> GetServerErrorLists(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName) {
            var cmdText = @"SELECT 
                            Query2.ScName, 
                            Query2.MaxofCountofError, 
                            Query2.SumofCountofError, 
                            Min(Query3.ToTimestamp) as MinofCollectionTime 
                            FROM (
	                            SELECT 
	                            Query.ScName AS ScName, 
	                            Max(Query.CountofError) AS MaxofCountofError, 
	                            SUM(Query.CountofError) AS SumofCountofError 
	                            FROM (
		                            SELECT
		                            PathwayName,
		                            ScName,
		                            Count(Distinct ErrorNumber) as CountofError,
		                            ToTimestamp
		                            FROM PvScstus
		                            WHERE (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) AND 
		                            PathwayName = @PathwayName AND 
		                            ErrorNumber <> 0
		                            GROUP BY PathwayName,ScName,ToTimestamp 
	                            )  AS Query
	                            GROUP BY Query.ScName
                            ) AS Query2
                            INNER JOIN (
	                            SELECT
	                            PathwayName,
	                            ScName,
	                            Count(Distinct ErrorNumber) as CountofError,
	                            ToTimestamp
	                            FROM PvScstus
	                            WHERE (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) AND 
	                            PathwayName = @PathwayName AND 
	                            ErrorNumber <> 0
	                            GROUP BY PathwayName,ScName,ToTimestamp
                            ) AS Query3
                            ON Query2.MaxofCountofError= Query3.CountofError AND Query2.ScName=Query3.ScName
                            GROUP BY  Query2.ScName,Query2.MaxofCountofError, Query2.SumofCountofError
                            ORDER BY  Query2.ScName	";

            var serverUnusedProcesses = new List<ServerErrorListView>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);

                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var view = new ServerErrorListView {
                        ServerClass = reader["ScName"].ToString(),
                        MostRecentTime = Convert.ToDateTime(reader["MinofCollectionTime"]),
                        Instances = Convert.ToInt32(reader["SumofCountofError"])
                    };
                    serverUnusedProcesses.Add(view);
                }
            }
            return serverUnusedProcesses;
        }
        public List<ServerErrorView> GetServerErrorListSub(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName) {
            var cmdText = @"SELECT 
                            ScName,
                            FromTimestamp, 
                            ErrorNumber, 
                            ErrorInfo
                            FROM PvScstus
                            WHERE 
                            (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) AND
                            PathwayName= @PathwayName AND
                            ErrorNumber <> 0
                            ORDER BY ScName, FromTimestamp";

            var serverUnusedProcesses = new List<ServerErrorView>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);

                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    var view = new ServerErrorView {
                        ServerClass = reader["ScName"].ToString(),
                        MostRecentTime = Convert.ToDateTime(reader["FromTimestamp"]),
                        ErrorNumber = Convert.ToInt32(reader["ErrorNumber"])
                    };
                    serverUnusedProcesses.Add(view);
                }
            }
            return serverUnusedProcesses;
        }
        public List<string> GetServerPendingClass(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName) {
            /*var cmdText = @"SELECT 
                            ScName
                            FROM PvScstus
                            WHERE 
                            (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) AND
                            PathwayName= @PathwayName AND
                            FreezeState = 1
                            ORDER BY ScName";
            */
            var cmdText = @"SELECT 
                            DISTINCT(SCName) AS SCName
                            FROM PvScprstus
                            WHERE 
                            (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp) AND
                            PathwayName= @PathwayName AND
                            ProcState = 1
                            ORDER BY ScName";
            var serverPendingClasses = new List<string>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);

                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    serverPendingClasses.Add(reader["ScName"].ToString());
                }
            }
            return serverPendingClasses;
        }

        public int GetFreezeState(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, string serverClassName) {
            var cmdText = @"SELECT
                            FREEZESTATE
                            FROM PvScstus WHERE
                            (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp)
                            AND PathwayName = @PathwayName AND SCName = @SCName
                            ORDER BY FromTimestamp DESC
                            LIMIT 1";

            var freezeState = 0;
            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);
                command.Parameters.AddWithValue("@SCName", serverClassName);

                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                if (reader.Read()) {
                    freezeState = Convert.ToInt32(reader["FREEZESTATE"]);
                }
            }

            return freezeState;
        }

        public Dictionary<string, int> GetFreezeState(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName) {
            var cmdText = @"SELECT
                            SCName,
                            FREEZESTATE
                            FROM PvScstus WHERE
                            (FromTimestamp >= @FromTimestamp AND  ToTimestamp <= @ToTimestamp)
                            AND PathwayName = @PathwayName
                            ORDER BY FromTimestamp DESC";

            var freezeStates = new Dictionary<string, int>();
            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);

                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    if (!freezeStates.ContainsKey(reader["SCName"].ToString()))
                        freezeStates.Add(reader["SCName"].ToString(), Convert.ToInt32(reader["FREEZESTATE"]));
                }
            }

            return freezeStates;
        }
    }
}
