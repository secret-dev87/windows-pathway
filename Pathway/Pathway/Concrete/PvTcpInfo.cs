using System;
using System.Collections.Generic;
using MySqlConnector;
using System.Linq;
using System.Text;
using Pathway.Core.Abstract;
using Pathway.Core.Infrastructure;

namespace Pathway.Core.Concrete
{
    class PvTcpInfo : IPvTcpInfo
    {
        private readonly string _connectionString = "";
        public PvTcpInfo(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Dictionary<string, long> GetTermTransaction(string pathwayName, DateTime fromTimestamp, DateTime toTimestamp)
        {
            var cmdText = @"SELECT 
                            SUM(IsReqCnt) AS SumOfIsReqcnt, 
                            pathwayName, 
                            TermTcpName 
                            FROM PvTermstat 
                            WHERE pathwayName = @PathwayName AND 
                            (fromTimestamp >= @FromTimestamp AND toTimestamp <=  @ToTimestamp)
                            GROUP BY pathwayName, TermTcpName 
                            ORDER BY SUM(IsReqCnt) DESC LIMIT 10";

            var termTransaction = new Dictionary<string, long>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);

                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    termTransaction.Add(reader["TermTcpName"].ToString(), Convert.ToInt64(reader["SumOfIsReqcnt"]));
                }
            }
            return termTransaction;
        }

        public Dictionary<string, double> GetTermCPUBusy(string pathwayName, DateTime fromTimestamp, DateTime toTimestamp) {
            var cmdText = @"SELECT 
                            SUM(PvTcpstus.DeltaProcTime) AS SumOfDeltaProcTime, 
                            TcpName
                            FROM PvTcpstus 
                            WHERE pathwayName = @PathwayName AND 
                            (FromTimestamp >= @FromTimestamp AND ToTimestamp <= @ToTimestamp)
                            GROUP BY TcpName";

            var termCPUBusy = new Dictionary<string, double>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);

                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
					if (!termCPUBusy.ContainsKey(reader["TcpName"].ToString())) {
						termCPUBusy.Add(reader["TcpName"].ToString(), Convert.ToDouble(reader["SumOfDeltaProcTime"]));
					}
                }
            }
            return termCPUBusy;
        }

        public Dictionary<string, long> GetTermServerTransaction(string pathwayName, DateTime fromTimestamp, DateTime toTimestamp) {
            var cmdText = @"SELECT 
                            SUM(IsReqCnt) AS SumOfIsReqcnt, 
                            pathwayName, 
                            ScTcpName 
                            FROM PvSctstat
                            WHERE pathwayName = @PathwayName AND 
                            (FromTimestamp >= @FromTimestamp AND ToTimestamp <= @ToTimestamp)
                            GROUP BY pathwayName, ScTcpName";

            var termServerTransaction = new Dictionary<string, long>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);

                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    termServerTransaction.Add(reader["ScTcpName"].ToString(), Convert.ToInt64(reader["SumOfIsReqcnt"]));
                }
            }
            return termServerTransaction;
        }

        public Dictionary<string, long> GetTermCount(string pathwayName, DateTime fromTimestamp, DateTime toTimestamp) {

            var cmdText = @"SELECT 
                            AVG(TermCount) AS AvgofTermCount, 
                            pathwayName, 
                            TcpName 
                            FROM (
	                            SELECT 
	                            pathwayName, 
	                            TcpName, 
	                            COUNT(TermName) AS TermCount 
	                            FROM PvTermstus 
	                            WHERE pathwayName = @PathwayName AND 
	                            FromTimestamp >= @FromTimestamp AND ToTimestamp <= @ToTimestamp
	                            GROUP BY pathwayName, TcpName
                            ) DERIVEDTBL 
                            GROUP BY pathwayName, TcpName";

            var termCount = new Dictionary<string, long>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);

                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    termCount.Add(reader["TcpName"].ToString(), Convert.ToInt64(reader["AvgofTermCount"]));
                }
            }
            return termCount;
        }

        public Dictionary<string, double> GetTermAverageResponseTime(string pathwayName, DateTime fromTimestamp, DateTime toTimestamp) {
            var cmdText = @"SELECT 
                            AVG(AvgResp) AS AvgResp, 
                            pathwayName, 
                            TermTcpName 
                            FROM PvTermstat 
                            WHERE pathwayName = @PathwayName AND 
                            FromTimestamp >= @FromTimestamp AND ToTimestamp <= @ToTimestamp
                            GROUP BY pathwayName, TermTcpName";

            var termAverageResponseTime = new Dictionary<string, double>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@PathwayName", pathwayName);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);

                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    termAverageResponseTime.Add(reader["TermTcpName"].ToString(), Convert.ToInt64(reader["AvgResp"]));
                }
            }
            return termAverageResponseTime;
        }
    }
}
