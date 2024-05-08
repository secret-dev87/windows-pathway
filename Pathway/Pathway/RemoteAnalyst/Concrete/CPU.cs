using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pathway.Core.RemoteAnalyst.Concrete {
    public class CPU {
        private string _connectionString;
        public CPU(string connectionString) {
            _connectionString = connectionString;
        }

        public string GetLatestTableName() {
            //string cmdText = @"SELECT TableName FROM CurrentTables WHERE DataDate <= @DataDate ORDER BY DataDate DESC LIMIT 1";
            var cmdText = @"SELECT TableName FROM CurrentTables WHERE TableName LIKE '%\_CPU\_%'
                            ORDER BY DataDate DESC LIMIT 1";
            string tableName = "";
            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);

                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();

                if (reader.Read()) {
                    tableName = Convert.ToString(reader["TableName"]);
                }
            }
            return tableName;
        }

        public int GetIPU(string tableName) {
            var cmdText = "SELECT Ipus FROM " + tableName + " LIMIT 1;";

            int ipuNum = 1;
            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);

                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();

                if (reader.Read()) {
                    ipuNum = Convert.ToInt32(reader["Ipus"]);
                }
            }
            return ipuNum;
        }
    }
}
