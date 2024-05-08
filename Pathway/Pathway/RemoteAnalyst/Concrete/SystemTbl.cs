using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pathway.Core.RemoteAnalyst.Concrete {
    public class SystemTbl {
        private string _connectionString;
        public SystemTbl(string connectionString) {
            _connectionString = connectionString;
        }
        public string GetSystemName(string systemSerial) {
            string cmdText = "SELECT SystemName FROM System_Tbl WHERE SystemSerial = @SystemSerial";
            string returnValue = string.Empty;

            var connection = new MySqlConnection(_connectionString);
            var command = new MySqlCommand(cmdText, connection);
            command.Parameters.AddWithValue("@SystemSerial", systemSerial);
            try {
                connection.Open();
                var reader = command.ExecuteReader();
                if (reader.Read()) {
                    returnValue = reader["SystemName"].ToString();
                }
                reader.Close();
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
            finally {
                connection.Close();
            }

            return returnValue;
        }
    }
}
