using System.Data;

namespace Pathway.Core.Abstract.Repositories {
    public interface IDataDictionaryRepository {
        DataTable GetPathwayColumns(string tableName);
    }
}