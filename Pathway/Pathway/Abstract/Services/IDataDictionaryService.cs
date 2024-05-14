using System.Collections.Generic;
using Pathway.Core.Infrastructure;

namespace Pathway.Core.Abstract {
    public interface IDataDictionaryService {
        IList<ColumnInfoView> GetPathwayColumnsFor(string tableName);
    }
}