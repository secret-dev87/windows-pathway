﻿using MySqlConnector;
using System;

namespace Pathway.Core.Concrete
{
    public class MySQLHelper {
        private readonly string _mySQLConnectionString;
        public MySQLHelper(string mySQLConnectionString) {
            _mySQLConnectionString = mySQLConnectionString;
        }
        public bool CheckMySqlTable(string databaseName, string tableName) {
            string cmdText = "SELECT COUNT(*) AS TableName FROM INFORMATION_SCHEMA.tables WHERE TABLE_SCHEMA = @DatabaseName AND table_name = @TableName";
            bool exists = true;
            try {
                using (var mySqlConnection = new MySqlConnection(_mySQLConnectionString)) {
                    mySqlConnection.Open();
                    var cmd = new MySqlCommand(cmdText, mySqlConnection);
                    cmd.Parameters.AddWithValue("@DatabaseName", databaseName);
                    cmd.Parameters.AddWithValue("@TableName", tableName);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read()) {
                        if (Convert.ToInt16(reader["TableName"]).Equals(0)) {
                            exists = false;
                        }
                    }
                }
            }
            catch (Exception ex) {
                exists = false;
            }
            return exists;
        }
        public int InsertData(string tableName, string fileLocation) {
            int inserted = 0;
            try {
                using (var mySqlConnection = new MySqlConnection(_mySQLConnectionString)) {
                    mySqlConnection.Open();
                    /*
                     * Explicitly added Local=true to allow Bulk loading
                     * https://dev.mysql.com/doc/connector-net/en/connector-net-programming-bulk-loader.html
                     */
                    var bl = new MySqlBulkLoader(mySqlConnection)
                    {
                        Local = true,
                        TableName = tableName,
                        FieldTerminator = "|",
                        LineTerminator = "\r\n",
                        FileName = fileLocation,
                        NumberOfLinesToSkip = 0,
                        Timeout = 0,
                        ConflictOption = MySqlBulkLoaderConflictOption.Replace
                    };
                    inserted = bl.Load();
                }
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
            return inserted;
        }
        public string FindDatabaseName(string mysqlConnectionString) {
            string databaseName = "";
            string[] tempNames = mysqlConnectionString.Split(';');
            foreach (string s in tempNames) {
                if (s.ToUpper().Contains("DATABASE")) {
                    databaseName = s.Split('=')[1];
                }
            }
            return databaseName;
        }

        public void CreateTrendPathwayHourly() {
            string sqlStr = @"CREATE TABLE `TrendPathwayHourly` (
                              `Interval` datetime NOT NULL,
                              `PathwayName` varchar(45) NOT NULL,
							  `PeakCPUBusy` double DEFAULT NULL,
                              `CpuBusy` double DEFAULT NULL,
							  `PeakLinkmonTransaction` double DEFAULT NULL,
							  `AverageLinkmonTransaction` double DEFAULT NULL,
							  `PeakTCPTransaction` double DEFAULT NULL,
							  `AverageTCPTransaction` double DEFAULT NULL,
                              `ServerTransaction` double DEFAULT NULL,
                              PRIMARY KEY (`Interval`,`PathwayName`)
                            ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";

            using (var connection = new MySqlConnection(_mySQLConnectionString)) {
                var command = new MySqlCommand(sqlStr, connection) { CommandTimeout = 0 };
                command.CommandTimeout = 0;connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void InsertStatement(string cmdText) {
            using (var connection = new MySqlConnection(_mySQLConnectionString)) {
                var command = new MySqlCommand(cmdText, connection) { CommandTimeout = 0 };
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

		public bool CheckMySqlColumn(string databaseName, string tableName, string columnName) {
			string cmdText = @"SELECT  COUNT(*) AS ColumnName FROM information_schema.COLUMNS 
                                WHERE TABLE_SCHEMA = @DatabaseName 
                                AND TABLE_NAME = @TableName 
                                AND COLUMN_NAME = @ColumnName";
			bool exists = true;
			try {
				using (var mySqlConnection = new MySqlConnection(_mySQLConnectionString)) {
					mySqlConnection.Open();
					var cmd = new MySqlCommand(cmdText, mySqlConnection);
					cmd.Parameters.AddWithValue("@DatabaseName", databaseName);
					cmd.Parameters.AddWithValue("@TableName", tableName);
					cmd.Parameters.AddWithValue("@ColumnName", columnName);
					cmd.CommandTimeout = 0;
					MySqlDataReader reader = cmd.ExecuteReader();
					if (reader.Read()) {
						if (Convert.ToInt16(reader["ColumnName"]).Equals(0)) {
							exists = false;
						}
					}
				}
			}
			catch (Exception ex) {
				exists = false;
			}
			return exists;
		}

		public void AlterTable(string trendTableName, string query) {
			string sqlStr = @"ALTER TABLE `" + trendTableName + "` " + query;

			using (var connection = new MySqlConnection(_mySQLConnectionString)) {
				var command = new MySqlCommand(sqlStr, connection) { CommandTimeout = 0 };
				connection.Open();
				command.ExecuteNonQuery();
			}
		}
	}
}
