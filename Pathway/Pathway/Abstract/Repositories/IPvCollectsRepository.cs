using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pathway.Core.Infrastructure;

namespace Pathway.Core.Abstract.Repositories
{
    internal interface IPvCollectsRepository
    {
        bool IsDuplicted(DateTime fromTimestamp, DateTime toTimestamp);

        CollectionInfo GetInterval(DateTime fromTimestamp, DateTime toTimestamp);

        List<DateTime> GetDatesWithData(DateTime fromTimestamp, DateTime toTimestamp);
    }
}
