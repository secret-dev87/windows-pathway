using System;
using System.Collections.Generic;

namespace Pathway.Core.Infrastructure.AllPathway {
    public class CPUDetailView {
        public string PathwayName { get; set; }

        [System.ComponentModel.DefaultValue(0)]
        public double Value { get; set; }

        public DateTime FromTimestamp { get; set; }
    }

    public class PathwayHourlyView {
        public PathwayHourlyView(double peakCpuBusy, double cpuBusy, double peakLinkmonTransaction, double averageLinkmonTransaction, double peakTCPTransaction, double averageTCPTransaction, int transCount) {
            PeakCpuBusy = peakCpuBusy;
            CpuBusy = cpuBusy;
            PeakLinkmonTransaction = peakLinkmonTransaction;
            AverageLinkmonTransaction = averageLinkmonTransaction;
            PeakTCPTransaction = peakTCPTransaction;
            AverageTCPTransaction = averageTCPTransaction;
            TransactionCount = transCount;

        }
        public double CpuBusy { get; set; }
        public double PeakCpuBusy { get; set; }
        public double PeakLinkmonTransaction { get; set; }
        public double AverageLinkmonTransaction { get; set; }
        public double PeakTCPTransaction { set; get; }
        public double AverageTCPTransaction { set; get; }
        public int TransactionCount { get; set; }
    }

    public class CPUDetailElapseAndBusyTimeView {
        public string CPUNumber { get; set; }
        [System.ComponentModel.DefaultValue(0)]
        public double ElapsedTime { get; set; }
        [System.ComponentModel.DefaultValue(0)]
        public long BusyTime { get; set; }
    }
}
