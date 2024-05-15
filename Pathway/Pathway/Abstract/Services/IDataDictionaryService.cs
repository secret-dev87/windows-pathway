using System.Collections.Generic;
using Pathway.Core.Infrastructure;

namespace Pathway.Core.Abstract.Services
{
    public interface IDataDictionaryService {
        IList<ColumnInfoView> GetPathwayColumnsFor(string tableName);
    }
}