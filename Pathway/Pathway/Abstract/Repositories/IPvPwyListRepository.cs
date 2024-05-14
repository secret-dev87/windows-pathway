using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Abstract.Repositories
{
    internal interface IPvPwyListRepository
    {
        List<string> GetPathwayNames(DateTime fromTimestamp, DateTime toTimestamp);

        bool CheckPathwayName(string pathwayName, DateTime fromTimestamp, DateTime toTimestamp);
    }
}
