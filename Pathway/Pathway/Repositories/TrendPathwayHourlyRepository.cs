using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Pathway.Core.Abstract.Repositories;
using Pathway.Core.Entity;
using Pathway.Core.Helper;

namespace Pathway.Core.Repositories
{
    internal class TrendPathwayHourlyRepository : ITrendPathwayHourlyRepository
    {
        public DataTable GetPathwayHourly(DateTime fromTimestamp, DateTime toTimestamp)
        {
            var table = new DataTable();

            try
            {
                using(var session = NHibernateHelper.OpenSystemSession())
                {
                    var res = session.Query<TrendPathwayHourlyEntity>()
                        .Where(x => x.Interval >= fromTimestamp && x.Interval < toTimestamp)
                        .OrderBy(x => new { x.Interval, x.PathwayName })
                        .ToList();

                    var json = JsonConvert.SerializeObject(res);
                    table = (DataTable)JsonConvert.DeserializeObject(json, (typeof(DataTable)));

                }
                return table;
            }
            catch( Exception e )
            {
                return table;
            }
            throw new NotImplementedException();
        }
    }
}
