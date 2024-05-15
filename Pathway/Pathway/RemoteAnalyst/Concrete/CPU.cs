using MySqlConnector;
using Pathway.Core.Entity;
using Pathway.Core.Helper;
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
            string tableName = "";

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var result = session.Query<CurrentTableEntity>()
                    .Where(x => x.TableName.Contains("_CPU_"))
                    .OrderByDescending(x => x.DataDate)
                    .Take(1)
                    .ToList();

                tableName = result[0].TableName.ToString();
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
