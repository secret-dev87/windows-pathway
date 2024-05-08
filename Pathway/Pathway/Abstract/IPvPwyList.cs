using System;
using System.Collections.Generic;

namespace Pathway.Core.Abstract {
    public interface IPvPwyList {
        List<string> GetPathwayNames(DateTime fromTimestamp, DateTime toTimestamp);

        bool CheckPathwayName(string pathwayName, DateTime fromTimestamp, DateTime toTimestamp);

        
    }
}