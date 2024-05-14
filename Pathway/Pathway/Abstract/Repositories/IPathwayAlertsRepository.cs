using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Abstract.Repositories
{
    internal interface IPathwayAlertsRepository
    {
        List<string> GetAlerts();
        List<string> GetAllAlerts();
    }
}
