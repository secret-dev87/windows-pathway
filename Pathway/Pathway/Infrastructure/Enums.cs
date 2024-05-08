using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pathway.Core.Infrastructure
{
    public static class Enums
    {
        public enum IntervalTypes
        {
            Daily = 0,
            Hourly = 1
        }

        public enum AlertReportTypes
        {
            AlertCollection = 0,
            QueueTCP = 1,
            QueueLinkmon = 2,
            UnusedClass = 3,
            UnusedProcess = 4,
            ErrorList = 5,
            QueueTCPSub = 6,
            QueueLinkmomSub = 7,
            ErrorListSub = 8,
            HighMaxLinks = 9,
            CheckDirectoryOn = 10,
            HighDynamicServers = 11
        }

        public enum PathwayReportTypes
        {
            CpuBusy = 0,
            Transactions = 1,
            ServerCpuBusy = 2,
            ServerTransactions = 3,
            ServerQueuesTCP = 4,
            ServerQueuesLinkmon = 5,
            ServerUnusedClass = 6,
            ServerUnusedProcesses = 7,
            TCPTransactions = 8,
            TCPQueues = 9,
            TCPUnused = 10,
            TermTransactions = 11,
            TermUnused = 12
        }
    }
}
