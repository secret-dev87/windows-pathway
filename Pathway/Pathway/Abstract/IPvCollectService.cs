using System;
using System.Collections.Generic;

namespace Pathway.Core.Abstract {
    public interface IPvCollectService {
        bool IsDuplictedFor(DateTime fromTimestamp, DateTime toTimestamp);

        long GetIntervalFor(DateTime fromTimestamp, DateTime toTimestamp);

        List<DateTime> GetDatesWithDataFor(DateTime fromTimestamp, DateTime toTimestamp);
    }
}