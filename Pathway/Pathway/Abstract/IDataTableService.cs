using System.Data;

namespace Pathway.Core.Abstract {
    public interface IDataTableService {
        void InsertEntityDataFor(string tableName, DataTable dsData, string path);
    }
}