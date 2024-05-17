using System.Collections.Generic;
using System.Linq;
using Pathway.Core.Abstract.Services;
using Pathway.Core.Abstract.Repositories;
using Pathway.Core.Infrastructure;
using Pathway.Core.Repositories;

namespace Pathway.Core.Services {
    public class PathwayAlertService : IPathwayAlertService {
        public PathwayAlertService() { }

        public List<string> GetAlertsFor() {
            IPathwayAlertsRepository alerts = new PathwayAlertsRepository();
            var alertList = alerts.GetAlerts();

            return alertList;
        }

        public List<string> GetAllAlertsFor() {
            IPathwayAlertsRepository alerts = new PathwayAlertsRepository();
            var alertList = alerts.GetAllAlerts();

            return alertList;
        }

        public List<ErrorInfo> GetErrorInfoFor(List<long> errorLists ) {
            IPvErrorsRepository pvErrors = new PvErrorsRepository();

            var errorInfos = errorLists.Select(pvErrors.GetErrorInfo).Where(errorInfo => errorInfo.ErrorNumber != 0).ToList();

            return errorInfos;
        }
    }
}