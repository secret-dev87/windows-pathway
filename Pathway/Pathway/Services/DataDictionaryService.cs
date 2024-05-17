using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Pathway.Core.Abstract.Repositories;
using Pathway.Core.Abstract.Services;
using Pathway.Core.Infrastructure;
using Pathway.Core.Repositories;

namespace Pathway.Core.Services {
    public class DataDictionaryService : IDataDictionaryService {
        public DataDictionaryService() { }

        public IList<ColumnInfoView> GetPathwayColumnsFor(string tableName) {
            IDataDictionaryRepository dataDictionary = new DataDictionaryRepository();

            var columnInfo = dataDictionary.GetPathwayColumns(tableName);
            return (from DataRow dr in columnInfo.Rows
                select new ColumnInfoView {
                    ColumnName = dr["FName"].ToString(), 
                    TypeName = dr["FType"].ToString(), 
                    TypeValue = Convert.ToInt32(dr["FSize"]),
                    ColumnOffSet = Convert.ToInt32(dr["SOff"])
                }).ToList();
        }
    }
}