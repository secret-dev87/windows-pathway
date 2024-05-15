using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Pathway.Core.Abstract.Services;
using Pathway.Core.Abstract.Repositories;
using Pathway.Core.Concrete;
using Pathway.Core.Infrastructure;
using Pathway.Core.Infrastructure.PerPathway.Term;
using Pathway.Core.Repositories;

namespace Pathway.Core.Services {
    public class PerPathwayTermService : IPerPathwayTermService {
        private readonly string _connectionString = "";
        private readonly long _intervalInSec;

        public PerPathwayTermService(string connectionString, long intervalInSec) {
            _connectionString = connectionString;
            _intervalInSec = intervalInSec;
        }

        public Dictionary<string, List<TermTop20View>> GetTermTop20(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, Enums.IntervalTypes intervalTypes) {
            IPvTermStatRepository termStat = new PvTermStatRepository();

            var termTop20 = new Dictionary<string, List<TermTop20View>>();

            for (var dtStart = fromTimestamp; dtStart.Date < toTimestamp.Date; dtStart = IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec)) {
                var tranactions = termStat.GetTermTransactions(dtStart.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec).AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);
                var avgResponseTime = termStat.GetTermAvgResponseTime(dtStart.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec).AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);
                var maxResponseTime = termStat.GetTermMaxResponseTime(dtStart.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec).AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);

                //Since all three has same number of rows, just use transaction to build the data.
                var views = tranactions.Select((t, x) => new TermTop20View {
                    TransactionTerm = t.Term, 
                    TransactionTcp = t.Tcp, 
                    TransactionValue = Convert.ToInt64(t.Value), 
                    AvgResponseTerm = avgResponseTime[x].Term, 
                    AvgResponseTcp = avgResponseTime[x].Tcp, 
                    AvgResponseValue = avgResponseTime[x].Value, 
                    MaxResponseTerm = maxResponseTime[x].Term, 
                    MaxResponseTcp = maxResponseTime[x].Tcp, 
                    MaxResponseValue = maxResponseTime[x].Value
                }).ToList();

                if (views.Count > 0) {
                    if (Enums.IntervalTypes.Hourly == intervalTypes)
                        termTop20.Add(dtStart.ToString("HH:mm"), views);
                    else
                        termTop20.Add(dtStart.ToString("yyyy-MM-dd"), views);
                }
            }
            return termTop20;
        }

        public Dictionary<string, List<TermUnusedView>> GetTermUnused(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, Enums.IntervalTypes intervalTypes) {
            IPvTermStatRepository termStat = new PvTermStatRepository();

            var termTop20 = new Dictionary<string, List<TermUnusedView>>();

            for (var dtStart = fromTimestamp; dtStart.Date < toTimestamp.Date; dtStart = IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec)) {
                var unused = termStat.GetTermUnused(dtStart.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), IntervalTypes.AddInterval(intervalTypes, dtStart, _intervalInSec).AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayName);

                if (unused.Count <= 0) continue;
                if (Enums.IntervalTypes.Hourly == intervalTypes)
                    termTop20.Add(dtStart.ToString("HH:mm"), unused);
                else
                    termTop20.Add(dtStart.ToString("yyyy-MM-dd"), unused);
            }
            return termTop20;
        }
    }
}
