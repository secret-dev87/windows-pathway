using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Abstract.Services
{
    public interface IPvCollectsService
    {
        bool IsDuplictedFor(DateTime fromTimeStamp, DateTime toTimeStamp);

        long GetIntervalFor(DateTime fromTimeStamp, DateTime toTimeStamp);

        List<DateTime> GetDatesWithDataFor(DateTime fromTimeStamp, DateTime toTimeStamp);
    }
}
