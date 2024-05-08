using System.Data;

namespace Pathway.Core.Abstract {
    public interface IDataDictionary {
        DataTable GetPathwayColumns(string tableName);
    }
}