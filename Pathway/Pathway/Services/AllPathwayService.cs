using System;
using System.Threading;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms.VisualStyles;
using Pathway.Core.Abstract;
using Pathway.Core.Concrete;
using Pathway.Core.Infrastructure;
using Pathway.Core.Infrastructure.AllPathway;
using Pathway.Core.RemoteAnalyst.Concrete;
using Pathway.Core.Abstract.Repositories;
using Pathway.Core.Abstract.Services;
using Pathway.Core.Repositories;

namespace Pathway.Core.Services {
    public class AllPathwayService : IAllPathwayService {
        private readonly string _connectionString = "";
        private readonly long _intervalInSec;

        public AllPathwayService(string connectionString, long intervalInSec) {
            _connectionString = connectionString;
            _intervalInSec = intervalInSec;
        }

        public Dictionary<string, PathwayHourlyView> GetPathwayHourly(DateTime fromTimestamp, DateTime toTimestamp) {
            ITrendPathwayHourlyRepository trendPathwayHourly = new TrendPathwayHourlyRepository();
            DataTable pathwayHourly = trendPathwayHourly.GetPathwayHourly(fromTimestamp, toTimestamp);
            Dictionary<string, PathwayHourlyView> pathwayNameHourDic = new Dictionary<string, PathwayHourlyView>();
            foreach (DataRow row in pathwayHourly.Rows) {
                string index = row[0].ToString() + " " + row[1].ToString();
                //wrap double limit under 15-17 digits to prevent excel populates data into scientific notation
                double peakCpuBusy = row[2] == DBNull.Value ? 0d : double.Parse(double.Parse(row[2].ToString()).ToString("0.000000000000"));
                double cpuBusy = row[3] == DBNull.Value ? 0d : double.Parse(double.Parse(row[3].ToString()).ToString("0.000000000000"));
                double peakLinkMon = row[4] == DBNull.Value ? 0d : double.Parse(double.Parse(row[4].ToString()).ToString("0.000000000000"));
                double avgLinkMon = row[5] == DBNull.Value ? 0d : double.Parse(double.Parse(row[5].ToString()).ToString("0.000000000000"));
                double peakTCPTrans = row[6] == DBNull.Value ? 0d : double.Parse(double.Parse(row[6].ToString()).ToString("0.000000000000"));
                double avgTCPTrans = row[7] == DBNull.Value ? 0d : double.Parse(double.Parse(row[7].ToString()).ToString("0.000000000000"));
                int transCount = row[8] == DBNull.Value ? 0 : int.Parse(row[8].ToString());
                pathwayNameHourDic.Add(index, new PathwayHourlyView(peakCpuBusy, cpuBusy, peakLinkMon, avgLinkMon, peakTCPTrans, avgTCPTrans, transCount));
            }
            return pathwayNameHourDic;
        }

        public Dictionary<string, List<CPUDetailView>> GetCPUBusyDetailFor(DateTime fromTimestamp, DateTime toTimestamp, string systemSerial, int ipu) {
            var cpuBusyDetail = new Dictionary<string, List<CPUDetailView>>();

            IPvCPUManyRepository cpuMany = new PvCPUManyRepository();
            IPvPwyManyRepository pwyMany = new PvPwyManyRepository();
            IPvTcpStusRepository tcpStus = new PvTcpStusRepository();
            IPvScPrStusRepository scPrStus = new PvScPrStusRepository();
            IPvPwyListRepository pwyList = new PvPwyListRepository();

            var cpuElapse = cpuMany.GetCPUElapseAndBusyTimePerCPU(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));
            var pwyLists = pwyList.GetPathwayNames(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));

            //Add others.
            foreach (var view in cpuElapse) {
                cpuBusyDetail.Add(view.Key, new List<CPUDetailView>());
                cpuBusyDetail[view.Key].Add(new CPUDetailView {
                            PathwayName = "Others",
                            Value = 100 * Convert.ToDouble(cpuElapse[view.Key].BusyTime) / (cpuElapse[view.Key].ElapsedTime * 1000 * ipu)
                            });
                //Add all the Pathway.
                foreach (var pathway in pwyLists) {
                    cpuBusyDetail[view.Key].Add(new CPUDetailView {
                        PathwayName = pathway,
                        Value = 0
                    });
                }
            }

            var process = new Process(_connectionString);
            var processTableName = systemSerial + "_PROCESS_" + fromTimestamp.Year + "_" + fromTimestamp.Month + "_" + fromTimestamp.Day;
            var pathwayNames = pwyMany.GetPathwayName(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));
            var pathmonData = process.GetPathmonProcessBusyPerCPUPerPathway(processTableName, fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayNames);

            //var pathmonData = pwyMany.GetPathmonProcessBusyPerCPUPerPathway(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));
            foreach (var view in pathmonData) {
                if (!cpuBusyDetail.ContainsKey(view.Key)) {
                    cpuBusyDetail.Add(view.Key, new List<CPUDetailView>());
                }
                foreach (var viewValue in view.Value) {
                    var temp = cpuBusyDetail[view.Key].Find(x => x.PathwayName == viewValue.CPUNumber);
                    if (temp == null)
                        cpuBusyDetail[view.Key].Add(new CPUDetailView {
                            PathwayName = viewValue.CPUNumber,
                            Value = 100 * Convert.ToDouble(viewValue.Value) / (cpuElapse[view.Key].ElapsedTime * 1000 * ipu)
                        });
                    else {
                        cpuBusyDetail[view.Key].Find(x => x.PathwayName == viewValue.CPUNumber).Value
                            += 100 * Convert.ToDouble(viewValue.Value) / (cpuElapse[view.Key].ElapsedTime * 1000 * ipu);
                    }

                    //Subtract Others.
                    cpuBusyDetail[view.Key].Find(x => x.PathwayName == "Others").Value -=
                        100 * Convert.ToDouble(viewValue.Value) / (cpuElapse[view.Key].ElapsedTime * 1000 * ipu);
                }
            }

            var tcpData = tcpStus.GetTcpBusyPerCPUPerPathway(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));
            foreach (var view in tcpData) {
                if (view.Key != "\0\0") {
                    if (!cpuBusyDetail.ContainsKey(view.Key)) {
                        cpuBusyDetail.Add(view.Key, new List<CPUDetailView>());
                    }
                    foreach (var viewValue in view.Value) {
                        var temp = cpuBusyDetail[view.Key].Find(x => x.PathwayName == viewValue.CPUNumber);
                        if (temp == null)
                            cpuBusyDetail[view.Key].Add(new CPUDetailView {
                                PathwayName = viewValue.CPUNumber,
                                Value = 100 * Convert.ToDouble(viewValue.Value) / (cpuElapse[view.Key].ElapsedTime * 1000 * ipu)
                            });
                        else {
                            cpuBusyDetail[view.Key].Find(x => x.PathwayName == viewValue.CPUNumber).Value
                                += 100 * Convert.ToDouble(viewValue.Value) / (cpuElapse[view.Key].ElapsedTime * 1000 * ipu);
                        }


                        //Subtract Others.
                        cpuBusyDetail[view.Key].Find(x => x.PathwayName == "Others").Value -=
                            100 * Convert.ToDouble(viewValue.Value) / (cpuElapse[view.Key].ElapsedTime * 1000 * ipu);
                    }
                }
            }

            var serverData = scPrStus.GetServerBusyPerCPUPerPathway(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));
            foreach (var view in serverData) {
                if (view.Key != "\0\0") {
                    if (!cpuBusyDetail.ContainsKey(view.Key)) {
                        cpuBusyDetail.Add(view.Key, new List<CPUDetailView>());
                    }
                    foreach (var viewValue in view.Value) {
                        var temp = cpuBusyDetail[view.Key].Find(x => x.PathwayName == viewValue.CPUNumber);
                        if (temp == null)
                            cpuBusyDetail[view.Key].Add(new CPUDetailView {
                                PathwayName = viewValue.CPUNumber,
                                Value = 100 * Convert.ToDouble(viewValue.Value) / (cpuElapse[view.Key].ElapsedTime * 1000 * ipu)
                            });
                        else {
                            cpuBusyDetail[view.Key].Find(x => x.PathwayName == viewValue.CPUNumber).Value
                                += 100 * Convert.ToDouble(viewValue.Value) / (cpuElapse[view.Key].ElapsedTime * 1000 * ipu);
                        }

                        //Subtract Others.
                        cpuBusyDetail[view.Key].Find(x => x.PathwayName == "Others").Value -=
                            100 * Convert.ToDouble(viewValue.Value) / (cpuElapse[view.Key].ElapsedTime * 1000 * ipu);
                    }
                }
            }

            try {
                foreach (var cpuBusy in cpuBusyDetail) {
                    foreach (var val in cpuBusy.Value) {
                        var totalBusyTime = process.GetTotalBusyTime(processTableName,
                            fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)),
                            toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])),
                            val.PathwayName, Convert.ToInt32(cpuBusy.Key));
                        val.Value += 100 * Convert.ToDouble(totalBusyTime) / (cpuElapse[cpuBusy.Key].ElapsedTime * 1000 * ipu);
                    }
                }
            }
            catch  { }
            return cpuBusyDetail;
        }

        public Dictionary<string, List<CPUDetailView>> GetCPUBusyDetailPerIntervalFor(DateTime fromTimestamp, DateTime toTimestamp, string systemSerial) {
            var cpuBusyDetail = new Dictionary<string, List<CPUDetailView>>();
            var process = new Process(_connectionString);

            IPvCPUManyRepository cpuMany = new PvCPUManyRepository();
            IPvPwyManyRepository pwyMany = new PvPwyManyRepository();
            IPvTcpStusRepository tcpStus = new PvTcpStusRepository();
            IPvScPrStusRepository scPrStus = new PvScPrStusRepository();
            IPvPwyListRepository pwyList = new PvPwyListRepository();
            var pwyLists = pwyList.GetPathwayNames(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));

            for (var newFromTime = fromTimestamp; newFromTime < toTimestamp; newFromTime = newFromTime.AddSeconds(_intervalInSec)) {

                var newToTime = newFromTime.AddSeconds(_intervalInSec);

                #region Data Call
                var cpuElapse = cpuMany.GetCPUElapseAndBusyTimePerCPU(newFromTime.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), newToTime.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));
                
                //Add others.
                foreach (var view in cpuElapse) {
                    if (!cpuBusyDetail.ContainsKey(view.Key)) {
                        cpuBusyDetail.Add(view.Key, new List<CPUDetailView>());
                    }
                    cpuBusyDetail[view.Key].Add(new CPUDetailView {
                        PathwayName = "Others",
                        Value = 100 * Convert.ToDouble(cpuElapse[view.Key].BusyTime) / cpuElapse[view.Key].ElapsedTime,
                        FromTimestamp = newToTime
                    });
                    //Add all the Pathway.
                    foreach (var pathway in pwyLists) {
                        cpuBusyDetail[view.Key].Add(new CPUDetailView {
                            PathwayName = pathway,
                            Value = 0,
                            FromTimestamp = newToTime
                        });
                    }
                }

                var processTableName = systemSerial + "_PROCESS_" + fromTimestamp.Year + "_" + fromTimestamp.Month + "_" + fromTimestamp.Day;
                var pathwayNames = pwyMany.GetPathwayName(newFromTime.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), newToTime.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));
                var pathmonData = process.GetPathmonProcessBusyPerCPUPerPathway(processTableName, newFromTime.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), newToTime.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayNames);

                //var pathmonData = pwyMany.GetPathmonProcessBusyPerCPUPerPathway(newFromTime.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), newToTime.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));
                foreach (var view in pathmonData) {
                    if (!cpuBusyDetail.ContainsKey(view.Key)) {
                        cpuBusyDetail.Add(view.Key, new List<CPUDetailView>());
                    }
                    foreach (var viewValue in view.Value) {
                        var temp = cpuBusyDetail[view.Key].Find(x => x.PathwayName == viewValue.CPUNumber);
                        if (temp == null)
                            cpuBusyDetail[view.Key].Add(new CPUDetailView {
                                PathwayName = viewValue.CPUNumber,
                                Value = 100 * Convert.ToDouble(viewValue.Value) / cpuElapse[view.Key].ElapsedTime,
                                FromTimestamp = newToTime
                            });
                        else {
                            cpuBusyDetail[view.Key].Find(x => x.PathwayName == viewValue.CPUNumber).Value
                                += 100 * Convert.ToDouble(viewValue.Value) / cpuElapse[view.Key].ElapsedTime;
                        }

                        //Subtract Others.
                        cpuBusyDetail[view.Key].Find(x => x.PathwayName == "Others").Value -=
                            100 * Convert.ToDouble(viewValue.Value) / cpuElapse[view.Key].ElapsedTime;
                    }
                }

                var tcpData = tcpStus.GetTcpBusyPerCPUPerPathway(newFromTime.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), newToTime.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));
                foreach (var view in tcpData) {
                    if (!cpuBusyDetail.ContainsKey(view.Key)) {
                        cpuBusyDetail.Add(view.Key, new List<CPUDetailView>());
                    }
                    foreach (var viewValue in view.Value) {
                        var temp = cpuBusyDetail[view.Key].Find(x => x.PathwayName == viewValue.CPUNumber);
                        if (temp == null)
                            cpuBusyDetail[view.Key].Add(new CPUDetailView {
                                PathwayName = viewValue.CPUNumber,
                                Value = 100 * Convert.ToDouble(viewValue.Value) / cpuElapse[view.Key].ElapsedTime,
                                FromTimestamp = newToTime
                            });
                        else {
                            cpuBusyDetail[view.Key].Find(x => x.PathwayName == viewValue.CPUNumber).Value
                                += 100 * Convert.ToDouble(viewValue.Value) / cpuElapse[view.Key].ElapsedTime;
                        }


                        //Subtract Others.
                        cpuBusyDetail[view.Key].Find(x => x.PathwayName == "Others").Value -=
                            100 * Convert.ToDouble(viewValue.Value) / cpuElapse[view.Key].ElapsedTime;
                    }
                }

                var serverData = scPrStus.GetServerBusyPerCPUPerPathway(newFromTime.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), newToTime.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));
                foreach (var view in serverData) {
                    if (view.Key != "\0\0") {
                        if (!cpuBusyDetail.ContainsKey(view.Key)) {
                            cpuBusyDetail.Add(view.Key, new List<CPUDetailView>());
                        }
                        foreach (var viewValue in view.Value) {
                            var temp = cpuBusyDetail[view.Key].Find(x => x.PathwayName == viewValue.CPUNumber);
                            if (temp == null)
                                cpuBusyDetail[view.Key].Add(new CPUDetailView {
                                    PathwayName = viewValue.CPUNumber,
                                    Value = 100*Convert.ToDouble(viewValue.Value)/cpuElapse[view.Key].ElapsedTime,
                                    FromTimestamp = newToTime
                                });
                            else {
                                cpuBusyDetail[view.Key].Find(x => x.PathwayName == viewValue.CPUNumber).Value
                                    += 100*Convert.ToDouble(viewValue.Value)/cpuElapse[view.Key].ElapsedTime;
                            }

                            //Subtract Others.
                            cpuBusyDetail[view.Key].Find(x => x.PathwayName == "Others").Value -=
                                100*Convert.ToDouble(viewValue.Value)/cpuElapse[view.Key].ElapsedTime;
                        }
                    }
                }
                #endregion
            }

            return cpuBusyDetail;
        }

        public Dictionary<string, List<CPUDetailView>> GetCPUBusyDetailPercentPerIntervalFor(DateTime fromTimestamp, DateTime toTimestamp, int ipus) {
            var cpuBusyDetail = new Dictionary<string, List<CPUDetailView>>();

            IPvCPUManyRepository cpuMany = new PvCPUManyRepository();
            IPvPwyManyRepository pwyMany = new PvPwyManyRepository();
            IPvTcpStusRepository tcpStus = new PvTcpStusRepository();
            IPvScPrStusRepository scPrStus = new PvScPrStusRepository();
            IPvPwyListRepository pwyList = new PvPwyListRepository();
            var pwyLists = pwyList.GetPathwayNames(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));

            for (var newFromTime = fromTimestamp; newFromTime < toTimestamp; newFromTime = newFromTime.AddSeconds(_intervalInSec)) {

                var newToTime = newFromTime.AddSeconds(_intervalInSec);

                #region Data Call
                var cpuElapse = cpuMany.GetCPUElapseAndBusyPercentPerCPU(newFromTime.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), newToTime.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), ipus);

                //Add others.
                foreach (var view in cpuElapse) {
                    if (!cpuBusyDetail.ContainsKey(view.Key)) {
                        cpuBusyDetail.Add(view.Key, new List<CPUDetailView>());
                    }
                    cpuBusyDetail[view.Key].Add(new CPUDetailView {
                        PathwayName = "Others",
                        Value = 100 * Convert.ToDouble(cpuElapse[view.Key].BusyTime) / cpuElapse[view.Key].ElapsedTime,
                        FromTimestamp = newToTime
                    });
                    //Add all the Pathway.
                    foreach (var pathway in pwyLists) {
                        cpuBusyDetail[view.Key].Add(new CPUDetailView {
                            PathwayName = pathway,
                            Value = 0,
                            FromTimestamp = newToTime
                        });
                    }
                }

                /*var pathmonData = pwyMany.GetPathmonProcessBusyPerCPUPerPathway(newFromTime.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), newToTime.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));
                foreach (var view in pathmonData) {
                    if (!cpuBusyDetail.ContainsKey(view.Key)) {
                        cpuBusyDetail.Add(view.Key, new List<CPUDetailView>());
                    }
                    foreach (var viewValue in view.Value) {
                        var temp = cpuBusyDetail[view.Key].Find(x => x.PathwayName == viewValue.CPUNumber);
                        if (temp == null)
                            cpuBusyDetail[view.Key].Add(new CPUDetailView {
                                PathwayName = viewValue.CPUNumber,
                                Value = 100 * Convert.ToDouble(viewValue.Value) / cpuElapse[view.Key].ElapsedTime,
                                FromTimestamp = newToTime
                            });
                        else {
                            cpuBusyDetail[view.Key].Find(x => x.PathwayName == viewValue.CPUNumber).Value
                                += 100 * Convert.ToDouble(viewValue.Value) / cpuElapse[view.Key].ElapsedTime;
                        }

                        //Subtract Others.
                        cpuBusyDetail[view.Key].Find(x => x.PathwayName == "Others").Value -=
                            100 * Convert.ToDouble(viewValue.Value) / cpuElapse[view.Key].ElapsedTime;
                    }
                }

                var tcpData = tcpStus.GetTcpBusyPerCPUPerPathway(newFromTime.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), newToTime.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));
                foreach (var view in tcpData) {
                    if (!cpuBusyDetail.ContainsKey(view.Key)) {
                        cpuBusyDetail.Add(view.Key, new List<CPUDetailView>());
                    }
                    foreach (var viewValue in view.Value) {
                        var temp = cpuBusyDetail[view.Key].Find(x => x.PathwayName == viewValue.CPUNumber);
                        if (temp == null)
                            cpuBusyDetail[view.Key].Add(new CPUDetailView {
                                PathwayName = viewValue.CPUNumber,
                                Value = 100 * Convert.ToDouble(viewValue.Value) / cpuElapse[view.Key].ElapsedTime,
                                FromTimestamp = newToTime
                            });
                        else {
                            cpuBusyDetail[view.Key].Find(x => x.PathwayName == viewValue.CPUNumber).Value
                                += 100 * Convert.ToDouble(viewValue.Value) / cpuElapse[view.Key].ElapsedTime;
                        }


                        //Subtract Others.
                        cpuBusyDetail[view.Key].Find(x => x.PathwayName == "Others").Value -=
                            100 * Convert.ToDouble(viewValue.Value) / cpuElapse[view.Key].ElapsedTime;
                    }
                }

                var serverData = scPrStus.GetServerBusyPerCPUPerPathway(newFromTime.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), newToTime.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));
                foreach (var view in serverData) {
                    if (!cpuBusyDetail.ContainsKey(view.Key)) {
                        cpuBusyDetail.Add(view.Key, new List<CPUDetailView>());
                    }
                    foreach (var viewValue in view.Value) {
                        var temp = cpuBusyDetail[view.Key].Find(x => x.PathwayName == viewValue.CPUNumber);
                        if (temp == null)
                            cpuBusyDetail[view.Key].Add(new CPUDetailView {
                                PathwayName = viewValue.CPUNumber,
                                Value = 100 * Convert.ToDouble(viewValue.Value) / cpuElapse[view.Key].ElapsedTime,
                                FromTimestamp = newToTime
                            });
                        else {
                            cpuBusyDetail[view.Key].Find(x => x.PathwayName == viewValue.CPUNumber).Value
                                += 100 * Convert.ToDouble(viewValue.Value) / cpuElapse[view.Key].ElapsedTime;
                        }

                        //Subtract Others.
                        cpuBusyDetail[view.Key].Find(x => x.PathwayName == "Others").Value -=
                            100 * Convert.ToDouble(viewValue.Value) / cpuElapse[view.Key].ElapsedTime;
                    }
                }*/
                #endregion
            }

            return cpuBusyDetail;
        }

        public CPUSummaryView GetCPUSummaryFor(DateTime fromTimestamp, DateTime toTimestamp, string systemSerial) {
            IPvCPUManyRepository cpuMany = new PvCPUManyRepository();
            IPvPwyManyRepository pwyMany = new PvPwyManyRepository();
            IPvTcpStusRepository tcpStus = new PvTcpStusRepository();
            IPvScPrStusRepository scPrStus = new PvScPrStusRepository();

            CPUSummaryView cpuSummary = cpuMany.GetCPUElapseAndBusyTime(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));

            double totalBusyTime = cpuSummary.BusyTime / cpuSummary.ElapsedTime;

            var process = new Process(_connectionString);
            var processTableName = systemSerial + "_PROCESS_" + fromTimestamp.Year + "_" + fromTimestamp.Month + "_" + fromTimestamp.Day;
            var pathwayNames = pwyMany.GetPathwayName(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));
            List<CPUView> pathmon = process.GetPathmonProcessBusyPerPathway(processTableName, fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])), pathwayNames);
            
            //List<CPUView> pathmon = pwyMany.GetPathmonProcessBusyPerPathway(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));
            List<CPUView> tcp = tcpStus.GetTcpBusyPerPathway(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));
            List<CPUView> server = scPrStus.GetServerBusyPerPathway(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));

            foreach (CPUView view in pathmon) {
                if (!cpuSummary.AllPathways.ContainsKey(view.CPUNumber))
                    cpuSummary.AllPathways.Add(view.CPUNumber, (view.Value / cpuSummary.ElapsedTime));
                else {
                    cpuSummary.AllPathways[view.CPUNumber] += (view.Value / cpuSummary.ElapsedTime);
                }

                if (!cpuSummary.AllPathwaysWithoutFree.ContainsKey(view.CPUNumber))
                    cpuSummary.AllPathwaysWithoutFree.Add(view.CPUNumber, (view.Value / cpuSummary.ElapsedTime));
                else {
                    cpuSummary.AllPathwaysWithoutFree[view.CPUNumber] += (view.Value / cpuSummary.ElapsedTime);
                }

                if (!cpuSummary.OnlyPathways.ContainsKey(view.CPUNumber))
                    cpuSummary.OnlyPathways.Add(view.CPUNumber, (view.Value / cpuSummary.ElapsedTime));
                else {
                    cpuSummary.OnlyPathways[view.CPUNumber] += (view.Value / cpuSummary.ElapsedTime);
                }
            }

            foreach (CPUView view in tcp) {
                if (!cpuSummary.AllPathways.ContainsKey(view.CPUNumber))
                    cpuSummary.AllPathways.Add(view.CPUNumber, (view.Value / cpuSummary.ElapsedTime));
                else {
                    cpuSummary.AllPathways[view.CPUNumber] += (view.Value / cpuSummary.ElapsedTime);
                }

                if (!cpuSummary.AllPathwaysWithoutFree.ContainsKey(view.CPUNumber))
                    cpuSummary.AllPathwaysWithoutFree.Add(view.CPUNumber, (view.Value / cpuSummary.ElapsedTime));
                else {
                    cpuSummary.AllPathwaysWithoutFree[view.CPUNumber] += (view.Value / cpuSummary.ElapsedTime);
                }

                if (!cpuSummary.OnlyPathways.ContainsKey(view.CPUNumber))
                    cpuSummary.OnlyPathways.Add(view.CPUNumber, (view.Value / cpuSummary.ElapsedTime));
                else {
                    cpuSummary.OnlyPathways[view.CPUNumber] += (view.Value / cpuSummary.ElapsedTime);
                }
            }

            foreach (CPUView view in server) {
                if (!cpuSummary.AllPathways.ContainsKey(view.CPUNumber))
                    cpuSummary.AllPathways.Add(view.CPUNumber, (view.Value / cpuSummary.ElapsedTime));
                else {
                    cpuSummary.AllPathways[view.CPUNumber] += (view.Value / cpuSummary.ElapsedTime);
                }

                if (!cpuSummary.AllPathwaysWithoutFree.ContainsKey(view.CPUNumber))
                    cpuSummary.AllPathwaysWithoutFree.Add(view.CPUNumber, (view.Value / cpuSummary.ElapsedTime));
                else {
                    cpuSummary.AllPathwaysWithoutFree[view.CPUNumber] += (view.Value / cpuSummary.ElapsedTime);
                }

                if (!cpuSummary.OnlyPathways.ContainsKey(view.CPUNumber))
                    cpuSummary.OnlyPathways.Add(view.CPUNumber, (view.Value / cpuSummary.ElapsedTime));
                else {
                    cpuSummary.OnlyPathways[view.CPUNumber] += (view.Value / cpuSummary.ElapsedTime);
                }
            }

            //subtract the busy times all the Pathways from Total to show Busy by "Others"
            cpuSummary.AllPathways.Add("Others", totalBusyTime - cpuSummary.AllPathways.Sum(x => x.Value));

            //set the value as 100 or 100%, then subtract all the value added till now to show the "Free" time for the system
            cpuSummary.AllPathways.Add("Free", 1 - cpuSummary.AllPathways.Sum(x => x.Value));

            //subtract the busy times all the Pathways from Total to show Busy by "Others"
            cpuSummary.AllPathwaysWithoutFree.Add("Others", totalBusyTime - cpuSummary.AllPathwaysWithoutFree.Sum(x => x.Value));

            return cpuSummary;
        }

        public TransactionServerView GetTransactionServer(DateTime fromTimestamp, DateTime toTimestamp) {
            IPvPwyListRepository pwyList = new PvPwyListRepository();
            IPvScLStatRepository scLStat = new PvScLStatRepository();
            IPvScTStatRepository scTStat = new PvScTStatRepository();

            var pwyLists = pwyList.GetPathwayNames(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));

            var linkmonToServer = scLStat.GetLinkmonToServer(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));
            var tcptoServer = scTStat.GetTcpToServer(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));

            foreach (string list in pwyLists.Where(list => !linkmonToServer.ContainsKey(list))) {
				LinkMonToServer linkMonToServerDefalut = new LinkMonToServer {
					AverageReqCnt = 0d,
					PeakReqCnt = 0,
					TotalIrReqCnt = 0
				};
				linkmonToServer.Add(list, linkMonToServerDefalut);
            }

            foreach (string list in pwyLists.Where(list => !tcptoServer.ContainsKey(list))) {
				TcpToServer tcpToServer = new TcpToServer {
					AverageReqCnt = 0d,
					PeakReqCnt = 0,
					TotalIsReqCnt = 0
				};
                tcptoServer.Add(list, tcpToServer);
            }

            var transactionServerView = new TransactionServerView {
                LinkmonToServer = linkmonToServer,
                TcptoServer = tcptoServer
            };

            return transactionServerView;
        }

        public TransactionTcpView GetTransactionTcp(DateTime fromTimestamp, DateTime toTimestamp) {
            IPvPwyListRepository pwyList = new PvPwyListRepository();
            IPvTermStatRepository termStat = new PvTermStatRepository();
            IPvScTStatRepository scTStat = new PvScTStatRepository();

            var pwyLists = pwyList.GetPathwayNames(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));

            var termToTcp = termStat.GetTermToTcp(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));
            var tcpToServer = scTStat.GetTcpToServer(fromTimestamp.AddSeconds(_intervalInSec * (Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"]) * -1)), toTimestamp.AddSeconds(_intervalInSec * Convert.ToDouble(ConfigurationManager.AppSettings["AllowTime"])));

            foreach (string list in pwyLists.Where(list => !termToTcp.ContainsKey(list))) {
                termToTcp.Add(list, 0);
            }

            foreach (string list in pwyLists.Where(list => !tcpToServer.ContainsKey(list))) {
				TcpToServer tcpToServerModel = new TcpToServer {
					AverageReqCnt = 0d,
					PeakReqCnt = 0,
					TotalIsReqCnt = 0
				};
				tcpToServer.Add(list, tcpToServerModel);
			}

            var transactionTcpView = new TransactionTcpView {
                TermToTcp = termToTcp,
                TcpToServer = tcpToServer
            };

            return transactionTcpView;
        }
    }
}
