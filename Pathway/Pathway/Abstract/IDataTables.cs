using System.Data;

namespace Pathway.Core.Abstract {
    public interface IDataTables {
        void InsertEntityData(string tableName, DataTable dsData, string path);
    }
}