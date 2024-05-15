using System.Data;
using Pathway.Core.Abstract;
using Pathway.Core.Abstract.Services;
using Pathway.Core.Concrete;

namespace Pathway.Core.Services {
    public class DataTableService : IDataTableService {
        private readonly string _connectionString = "";

        public DataTableService(string connectionString) {
            _connectionString = connectionString;
        }

        public void InsertEntityDataFor(string tableName, DataTable dsData, string path) {
            IDataTables dataTables = new DataTables(_connectionString);
            dataTables.InsertEntityData(tableName, dsData, path);
        }
    }
}