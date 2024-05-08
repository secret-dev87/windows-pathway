using System;
using Pathway.Core.Infrastructure;
using System.Collections.Generic;

namespace Pathway.Core.Abstract {
    internal interface IPvCollects {
        bool IsDuplicted(DateTime fromTimestamp, DateTime toTimestamp);

        CollectionInfo GetInterval(DateTime fromTimestamp, DateTime toTimestamp);

        List<DateTime> GetDatesWithData(DateTime fromTimestamp, DateTime toTimestamp);

        
    }
}