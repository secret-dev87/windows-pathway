using System.Collections.Generic;
using MySqlConnector;
using Pathway.Core.Abstract;

namespace Pathway.Core.Concrete {
    internal class PathwayAlerts : IPathwayAlerts {
        private readonly string _connectionString = "";

        public PathwayAlerts(string connectionString) {
            _connectionString = connectionString;
        }

        public List<string> GetAlerts() {
            string cmdText = "SELECT AlertName FROM PathwayAlerts";
            var alertList = new List<string>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    alertList.Add(reader["AlertName"].ToString());
                }
            }

            return alertList;
        }

        public List<string> GetAllAlerts() {
            string cmdText = "SELECT AlertName FROM PathwayAllAlerts";
            var alertList = new List<string>();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.CommandTimeout = 0;connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    alertList.Add(reader["AlertName"].ToString());
                }
            }

            return alertList;
        }
    }
}