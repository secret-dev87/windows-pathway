using System.Data;
using System.Linq;
using MySqlConnector;
using Newtonsoft.Json;
using Pathway.Core.Abstract;
using Pathway.Core.Entity.Main;
using Pathway.Core.Helper;

namespace Pathway.Core.Concrete {
    internal class DataDictionary : IDataDictionary {
        private readonly string _connectionString = "";

        public DataDictionary(string connectionString) {
            _connectionString = connectionString;
        }

        public DataTable GetPathwayColumns(string tableName) {

            var columnInfo = new DataTable();

            using(var session = NHibernateHelper.OpenMainSession())
            {
                var result = session.Query<PvTableTableNewEntity>()
                    .Where(x => x.TableName == tableName)
                    .OrderBy(x => x.FSeq)
                    .Select(x => new { x.FName, x.FType, x.SOff })
                    .ToList();

                var json = JsonConvert.SerializeObject(result);
                columnInfo = (DataTable)JsonConvert.DeserializeObject(json, (typeof(DataTable)));
            }

            return columnInfo;
        }
    }
}