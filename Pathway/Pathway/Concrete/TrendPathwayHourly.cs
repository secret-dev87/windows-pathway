using System;
using System.Data;
using MySqlConnector;
using Pathway.Core.Abstract;

namespace Pathway.Core.Concrete {
	class TrendPathwayHourly : ITrendPathwayHourly {
		private readonly string _connectionString = "";

		public TrendPathwayHourly(string connectionString) {
			_connectionString = connectionString;
		}

		public DataTable GetPathwayHourly(DateTime fromTimestamp, DateTime toTimestamp) {
			var table = new DataTable();
			string cmdText = @"SELECT * FROM TrendPathwayHourly WHERE
                            `Interval` >= @FromTimestamp AND
                            `Interval` < @ToTimestamp ORDER BY `Interval`, `PathwayName`";
			try {
				using (var connection = new MySqlConnection(_connectionString)) {
					var command = new MySqlCommand(cmdText, connection);
					command.Parameters.AddWithValue("@FromTimestamp", fromTimestamp);
					command.Parameters.AddWithValue("@ToTimestamp", toTimestamp);
					command.CommandTimeout = 0; connection.Open();
					var adapter = new MySqlDataAdapter(command);
					adapter.Fill(table);
				}
				return table;
			}
			catch(Exception ex) {
				return table;
			}
		}

	}
}
