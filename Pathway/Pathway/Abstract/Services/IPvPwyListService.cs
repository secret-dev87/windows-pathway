using System;
using System.Collections.Generic;

namespace Pathway.Core.Abstract.Services
{
    public interface IPvPwyListService {
        List<string> GetPathwayNamesFor(DateTime fromTimestamp, DateTime toTimestamp);
        bool CheckPathwayNameFor(string pathwayName, DateTime fromTimestamp, DateTime toTimestamp);
    }
}