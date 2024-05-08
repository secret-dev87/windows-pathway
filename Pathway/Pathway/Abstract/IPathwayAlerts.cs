using System.Collections.Generic;

namespace Pathway.Core.Abstract {
    internal interface IPathwayAlerts {
        List<string> GetAlerts();
        List<string> GetAllAlerts();
    }
}