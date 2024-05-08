using System.Data;
using MySqlConnector;
using Pathway.Core.Abstract;

namespace Pathway.Core.Concrete {
    internal class DataDictionary : IDataDictionary {
        private readonly string _connectionString = "";

        public DataDictionary(string connectionString) {
            _connectionString = connectionString;
        }

        public DataTable GetPathwayColumns(string tableName) {
            string cmdText = "SELECT FName, FType, FSize, SOff FROM _PvTableTableNew" +
                             " WHERE TableName = @TableName " +
                             "ORDER BY FSeq";
            var columnInfo = new DataTable();

            using (var connection = new MySqlConnection(_connectionString)) {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@TableName", tableName);
                command.CommandTimeout = 0;connection.Open();

                var adapter = new MySqlDataAdapter(command);
                adapter.Fill(columnInfo);
            }

            return columnInfo;
        }
    }
}