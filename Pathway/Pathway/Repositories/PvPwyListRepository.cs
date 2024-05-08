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
    internal class PvPwyListRepository : IPvPwyListRepository
    {
        public PvPwyListRepository() { }

        public bool CheckPathwayName(string pathwayName, DateTime fromTimestamp, DateTime toTimestamp)
        {
            var exits = false;

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var pwyList = session.Query<PvPwyListEntity>()
                    .Where(x => (x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp) && x.PathwayName == pathwayName)
                    .GroupBy(x => x.PathwayName)
                    .ToList();

                if (pwyList != null && pwyList.Count > 0)
                {
                    exits = true;
                }
            }

            return exits;            
        }

        public List<string> GetPathwayNames(DateTime fromTimestamp, DateTime toTimestamp)
        {
            var pathways = new List<string>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var pwyLists = session.Query<PvPwyListEntity>()
                    .Where(x => x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp)
                    .GroupBy(x => x.PathwayName)
                    .Select(g => new
                    {
                        PathwayName = g.Key
                    })
                    .ToList();

                foreach(var pwyList in pwyLists)
                {
                    pathways.Add(pwyList.PathwayName.ToString());
                }
            }

            return pathways;
        }
    }
}
