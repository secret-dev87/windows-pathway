using System.Collections.Generic;
using System.Linq;
using Pathway.Core.Abstract;
using Pathway.Core.Concrete;
using Pathway.Core.Infrastructure;

namespace Pathway.Core.Services {
    public class PathwayAlertService : IPathwayAlertService {
        private readonly string _connectionString = "";

        public PathwayAlertService(string connectionString) {
            _connectionString = connectionString;
        }

        public List<string> GetAlertsFor() {
            IPathwayAlerts alerts = new PathwayAlerts(_connectionString);
            var alertList = alerts.GetAlerts();

            return alertList;
        }

        public List<string> GetAllAlertsFor() {
            IPathwayAlerts alerts = new PathwayAlerts(_connectionString);
            var alertList = alerts.GetAllAlerts();

            return alertList;
        }

        public List<ErrorInfo> GetErrorInfoFor(List<long> errorLists ) {
            IPvErrors pvErrors = new PvErrors(_connectionString);

            var errorInfos = errorLists.Select(pvErrors.GetErrorInfo).Where(errorInfo => errorInfo.ErrorNumber != 0).ToList();

            return errorInfos;
        }
    }
}