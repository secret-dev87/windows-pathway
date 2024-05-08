using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pathway.Core.Abstract
{
    internal interface IPvTcpInfo
    {
        Dictionary<string, long> GetTermTransaction(string pathwayName, DateTime fromTimestamp, DateTime toTimestamp);

        Dictionary<string, double> GetTermCPUBusy(string pathwayName, DateTime fromTimestamp, DateTime toTimestamp);

        Dictionary<string, long> GetTermServerTransaction(string pathwayName, DateTime fromTimestamp, DateTime toTimestamp);

        Dictionary<string, long> GetTermCount(string pathwayName, DateTime fromTimestamp, DateTime toTimestamp);

        Dictionary<string, double> GetTermAverageResponseTime(string pathwayName, DateTime fromTimestamp, DateTime toTimestamp);
        
    }
}
