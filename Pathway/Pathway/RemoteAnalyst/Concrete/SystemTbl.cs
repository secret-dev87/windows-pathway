using MySqlConnector;
using Pathway.Core.Entity.Main;
using Pathway.Core.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pathway.Core.RemoteAnalyst.Concrete {
    public class SystemTbl {
        public SystemTbl() {
        }
        public string GetSystemName(string systemSerial) {
            string returnValue = string.Empty;

            using(var session = NHibernateHelper.OpenMainSession())
            {
                var result = session.Query<SystemTblEntity>()
                    .Where(x => x.SystemSerial == systemSerial)
                    .First();

                returnValue = result.SystemName.ToString();
            }

            return returnValue;
        }
    }
}
