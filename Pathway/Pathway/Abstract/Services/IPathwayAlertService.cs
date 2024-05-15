using System.Collections.Generic;
using Pathway.Core.Infrastructure;

namespace Pathway.Core.Abstract.Services
{
    public interface IPathwayAlertService {
        List<string> GetAlertsFor();
        List<string> GetAllAlertsFor();
        List<ErrorInfo> GetErrorInfoFor(List<long> errorLists);
    }
}