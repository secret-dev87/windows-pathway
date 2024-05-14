using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pathway.Core.Abstract.Repositories;
using Pathway.Core.Entity;
using Pathway.Core.Helper;

namespace Pathway.Core.Repositories
{
    internal class PathwayAlertsRepository : IPathwayAlertsRepository
    {
        public PathwayAlertsRepository() { }

        public List<string> GetAlerts()
        {
            var alertList = new List<string>();

            using(var session = NHibernateHelper.OpenMainSession())
            {
                var results = session.Query<PathwayAlertsEntity>()
                    .ToList();

                foreach(var res in results)
                {
                    alertList.Add(res.AlertName.ToString());
                }
            }

            return alertList;
        }

        public List<string> GetAllAlerts()
        {
            var alertList = new List<string>();

            using(var session = NHibernateHelper.OpenMainSession())
            {
                var results = session.Query<PathwayAllAlertsEntity>()
                    .ToList();

                foreach(var res in results)
                {
                    alertList.Add(res.AlertName.ToString());
                }
            }

            return alertList;
        }
    }
}
