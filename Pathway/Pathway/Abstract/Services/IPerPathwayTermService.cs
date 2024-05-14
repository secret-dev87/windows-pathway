using System;
using System.Collections.Generic;
using Pathway.Core.Infrastructure;
using Pathway.Core.Infrastructure.PerPathway.Term;

namespace Pathway.Core.Abstract {
    public interface IPerPathwayTermService {
        Dictionary<string, List<TermTop20View>> GetTermTop20(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, Enums.IntervalTypes intervalTypes);

        Dictionary<string, List<TermUnusedView>> GetTermUnused(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, Enums.IntervalTypes intervalTypes);
    }
}