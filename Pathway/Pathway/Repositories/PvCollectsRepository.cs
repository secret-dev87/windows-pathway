using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pathway.Core.Abstract.Repositories;
using Pathway.Core.Entity;
using Pathway.Core.Helper;
using Pathway.Core.Infrastructure;

namespace Pathway.Core.Repositories
{
    internal class PvCollectsRepository : IPvCollectsRepository
    {
        public PvCollectsRepository() { }

        public List<DateTime> GetDatesWithData(DateTime fromTimestamp, DateTime toTimestamp)
        {
            var dates = new List<DateTime>();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var pvCollects = session.Query<PvCollectEntity>()
                    .Where(x => x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp)
                    .ToList();

                foreach(var collect in pvCollects)
                {
                    var date = Convert.ToDateTime(collect.FromTimestamp).Date;
                    if (!dates.Contains(date))
                        dates.Add(date);
                }
            }

            return dates;
        }

        public CollectionInfo GetInterval(DateTime fromTimestamp, DateTime toTimestamp)
        {
            var intervalInfo = new CollectionInfo();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var pvCollect = session.Query<PvCollectEntity>()
                    .Where(x => x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp)
                    .First();
                if (pvCollect != null)
                {
                    intervalInfo.IntervalNumber = Convert.ToInt32(pvCollect.IntervalNn);
                    intervalInfo.IntervalType = Convert.ToString(pvCollect.IntervalHOrM);
                }
            }

            return intervalInfo;
        }

        public bool IsDuplicted(DateTime fromTimestamp, DateTime toTimestamp)
        {
            bool isDuplicted = false;

            using (var session = NHibernateHelper.OpenSystemSession())
            {
                var pvCollect = session.Query<PvCollectEntity>()
                    .Where(x => x.FromTimestamp >= fromTimestamp && x.ToTimestamp <= toTimestamp)
                    .FirstOrDefault();
                if (pvCollect != null)
                {
                    isDuplicted = true;
                }
            }

            return isDuplicted;
        }
    }
}
