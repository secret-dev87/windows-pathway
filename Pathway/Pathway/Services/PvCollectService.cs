using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Pathway.Core.Abstract;
using Pathway.Core.Concrete;
using Pathway.Core.Infrastructure;

namespace Pathway.Core.Services {
    public class PvCollectService : IPvCollectService {
        private readonly string _connectionString = "";

        public PvCollectService(string connectionString) {
            _connectionString = connectionString;
        }

        public bool IsDuplictedFor(DateTime fromTimestamp, DateTime toTimestamp) {
            IPvCollects collects = new PvCollects(_connectionString);
            bool isDuplicated = collects.IsDuplicted(fromTimestamp, toTimestamp);

            return isDuplicated;
        }

        public long GetIntervalFor(DateTime fromTimestamp, DateTime toTimestamp) {
            IPvCollects collects = new PvCollects(_connectionString);
            var intervalInfo = collects.GetInterval(fromTimestamp, toTimestamp);
            long intervalInSec = 0;
            if (intervalInfo.IntervalType != null) {
                if (intervalInfo.IntervalType.Equals("H"))
                    intervalInSec = intervalInfo.IntervalNumber*60*60;
                else
                    intervalInSec = intervalInfo.IntervalNumber*60;
            }

            return intervalInSec;
        }

        public List<DateTime> GetDatesWithDataFor(DateTime fromTimestamp, DateTime toTimestamp) {
            IPvCollects collects = new PvCollects(_connectionString);
            var datesWithData = collects.GetDatesWithData(fromTimestamp, toTimestamp);

            return datesWithData;
        }
    }
}