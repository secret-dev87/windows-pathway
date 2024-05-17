using MySqlConnector;
using Pathway.Core.Entity;
using Pathway.Core.Entity.Dynamic;
using Pathway.Core.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pathway.Core.RemoteAnalyst.Concrete {
    public class CPU {
        public CPU() {
        }

        public string GetLatestTableName() {            
            string tableName = "";

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var result = session.Query<CurrentTableEntity>()
                    .Where(x => x.TableName.Contains("_CPU_"))
                    .OrderByDescending(x => x.DataDate)
                    .Take(1)
                    .ToList();

                tableName = result[0].TableName.ToString();
            }
            return tableName;
        }

        public int GetIPU(string tableName)
        {
            int ipuNum = 1;

            using(var session = NHibernateHelper.OpenDynamicSession(tableName))
            {
                var result = session.Query<CpuEntity>()
                    .Select(x => x.Ipus)
                    .Take(1)
                    .ToList();

                ipuNum = Convert.ToInt32(result[0]);
            }

            return ipuNum;
        }
    }
}
