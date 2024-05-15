using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Pathway.Core.Abstract;
using Pathway.Core.Abstract.Services;
using Pathway.Core.Concrete;
using Pathway.Core.Infrastructure;

namespace Pathway.Core.Services {
    public class DataDictionaryService : IDataDictionaryService {
        private readonly string _connectionString = "";

        public DataDictionaryService(string connectionString) {
            _connectionString = connectionString;
        }

        public IList<ColumnInfoView> GetPathwayColumnsFor(string tableName) {
            IDataDictionary dataDictionary = new DataDictionary(_connectionString);

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