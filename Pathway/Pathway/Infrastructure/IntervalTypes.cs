using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pathway.Core.Infrastructure {
    static class IntervalTypes {
        internal static DateTime AddInterval(Enums.IntervalTypes intervalTypes, DateTime currentDateTime, long intervalInSec) {
            DateTime newDateTime;

            if (intervalTypes == Enums.IntervalTypes.Hourly) {
                newDateTime = currentDateTime.AddSeconds(intervalInSec);
            }
            else
                newDateTime = currentDateTime.AddDays(1);

            return newDateTime;
        }
    }
}
