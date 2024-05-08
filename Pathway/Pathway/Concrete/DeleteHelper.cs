using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Concrete
{
    internal class DeleteHelper
    {
        private string _connectionString = String.Empty;

        public DeleteHelper(string connectionString) {
            _connectionString = connectionString;
        }
        
        public void DeleteData(string tableName, DateTime fromTimestamp, DateTime toTimestamp)
        {
            var cmdText = @"DELETE FROM @TableName WHERE
                            FromTimestamp >= @FromTimestamp AND
                            ToTimestamp <= @ToTimestamp";
            using (var connection = new MySqlConnection(_connectionString))
            {
                var command = new MySqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@TableName", tableName);
                command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
                command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
                command.CommandTimeout = 0; connection.Open();
                command.ExecuteNonQuery();
            }


        }
    }
}
