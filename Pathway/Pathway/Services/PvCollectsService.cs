using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pathway.Core.Abstract.Repositories;
using Pathway.Core.Abstract.Services;
using Pathway.Core.Repositories;

namespace Pathway.Core.Services
{
    public class PvCollectsService : IPvCollectsService
    {
        static readonly IPvCollectsRepository _pvCollectsRepository = new PvCollectsRepository();

        public List<DateTime> GetDatesWithDataFor(DateTime fromTimeStamp, DateTime toTimeStamp)
        {
            return _pvCollectsRepository.GetDatesWithData(fromTimeStamp, toTimeStamp);
        }

        public long GetIntervalFor(DateTime fromTimeStamp, DateTime toTimeStamp)
        {
            var intervalInfo = _pvCollectsRepository.GetInterval(fromTimeStamp, toTimeStamp);
            long intervalInSec = 0;
            if (intervalInfo.IntervalType != null)
            {
                if (intervalInfo.IntervalType.Equals("H"))
                    intervalInSec = intervalInfo.IntervalNumber * 60 * 60;
                else
                    intervalInSec = intervalInfo.IntervalNumber * 60;
            }

            return intervalInSec;
        }

        public bool IsDuplictedFor(DateTime fromTimeStamp, DateTime toTimeStamp)
        {
            return _pvCollectsRepository.IsDuplicted(fromTimeStamp, toTimeStamp);
        }
    }
}
