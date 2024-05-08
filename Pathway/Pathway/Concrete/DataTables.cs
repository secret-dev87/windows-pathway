using System;
using System.Data;
using System.IO;
using System.Text;
using MySqlConnector;
using Pathway.Core.Abstract;

namespace Pathway.Core.Concrete {
    internal class DataTables : IDataTables {
        private readonly string _connectionString;

        public DataTables(string connectionString) {
            _connectionString = connectionString;
        }

        public void InsertEntityData(string tableName, DataTable dsData, string path) {
            try {
                string pathToCsv = path + @"\BulkInsert_" + DateTime.Now.Ticks + ".csv";
                var sb = new StringBuilder();
                if (dsData.Rows.Count > 0) {
                    foreach (DataRow dataRow in dsData.Rows) {
                        var row = new StringBuilder();
                        for (int x = 0; x < dsData.Columns.Count; x++) {
                            if (dsData.Columns[x].DataType.Name.Equals("DateTime")) {
                                DateTime tempDate = Convert.ToDateTime(dataRow[x]);
                                row.Append(tempDate.ToString("yyyy-MM-dd HH-mm-ss") + "|");
                            }
                            else {
                                if (!dataRow.IsNull(x))
                                    row.Append(dataRow[x] + "|");
                                else
                                    row.Append("|");
                            }
                        }
                        row = row.Remove(row.Length - 1, 1);
                        sb.Append(row + Environment.NewLine);
                    }
                }
                File.AppendAllText(pathToCsv, sb.ToString());

                int inserted = -1;
                try {
                    using (var mySqlConnection = new MySqlConnection(_connectionString)) {
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
                            FileName = pathToCsv,
                            NumberOfLinesToSkip = 0
                        };
                        inserted = bl.Load();
                    }
                }
                catch (Exception ex) {
                    throw new Exception();
                }

                if (inserted >= 0) {
                    File.Delete(pathToCsv);
                }
            }
            catch (Exception ex) {
                throw new Exception("Table " + tableName + " :" + ex.Message);
            }
        }
    }
}