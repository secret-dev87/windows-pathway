using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Abstract.Repositories
{
    internal interface ITrendPathwayHourlyRepository
    {
        DataTable GetPathwayHourly(DateTime fromTimestamp, DateTime toTimestamp);
    }
}
