using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using Pathway.Core.Infrastructure;
using Pathway.Core.Infrastructure.AllPathway;
using Pathway.Core.Infrastructure.PerPathway;
using Pathway.Core.Infrastructure.PerPathway.Server;
using Pathway.ReportGenerator.Infrastructure;
using Pathway.Core.RemoteAnalyst.Concrete;
using OfficeOpenXml;
using log4net;

namespace Pathway.ReportGenerator.Excel {
    internal class ExcelChart : ExcelChartBase {
        private readonly ILog _log;
        private readonly int _reportDownloadId;
        private readonly ReportDownloadLogs _reportDownloadLogs;
        
        public ExcelChart(int reportDownloadId, ReportDownloadLogs reportDownloadLogs, ILog log) {
            _reportDownloadId = reportDownloadId;
            _reportDownloadLogs = reportDownloadLogs;
            _log = log;
        }

        internal void InsertWorksheetCPUDetailAllPathway(ExcelWorkbook xlWorkBook, int worksheetCount, Dictionary<string, List<Core.Infrastructure.AllPathway.CPUDetailView>> detailView, string worksheetName, string saveLocation, string previousSection, string previousWorksheet, string nextWorksheet, string nextSection, string worksheetToc, string worksheetTitle, bool isLocalAnalyst) {
            try {
                ExcelWorksheet xlWorkSheet = null;
                if (xlWorkBook.Worksheets.Count < worksheetCount)
                    xlWorkSheet = xlWorkBook.Worksheets.Add(worksheetName);

                int rowCount = 3;
                int chartStartRow = 3;
                int chartLastRow = 19;

                // Insert Sheet Title.
                xlWorkSheet.Cells[2, 1].Value = worksheetTitle;
                xlWorkSheet.Cells["A2:Z2"].Merge = true;
                var title = xlWorkSheet.Cells["A2"];
                AddTitle(title);


                int dataStartRow = rowCount;
                int pathwayCount = 0;
                //Insert Columns.
                for (var x = 0; x < detailView.Count; x++) {
                    var cpuNumber = detailView.Keys.ElementAt(x);
                    xlWorkSheet.Cells[rowCount, (x + 2)].Value = cpuNumber;

                    for (var i = 0; i < detailView[cpuNumber].Count; i++) {
                        if (x == 0) {
                            xlWorkSheet.Cells[rowCount + (i + 1), (x + 1)].Value = detailView[cpuNumber][i].PathwayName;
                            pathwayCount = detailView[cpuNumber].Count;
                            xlWorkSheet.Cells[ConvertNumberToChar((x + 1)) + rowCount + (i + 1)].Style.Font.Size = 10;
                        }
                        xlWorkSheet.Cells[rowCount + (i + 1), (x + 2)].Value = Math.Round(detailView[cpuNumber][i].Value, 2);
                    }

                    if (x % 2 == 0) {
                        xlWorkSheet.Cells[ConvertNumberToChar((x + 2)) + rowCount + ":" + ConvertNumberToChar((x + 2)) + (rowCount + detailView[cpuNumber].Count)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        xlWorkSheet.Cells[ConvertNumberToChar((x + 2)) + rowCount + ":" + ConvertNumberToChar((x + 2)) + (rowCount + detailView[cpuNumber].Count)].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                    }
                }
                //Format the Header.
                var header = xlWorkSheet.Cells["B" + rowCount + ":" + ConvertNumberToChar(detailView.Count + 1) + rowCount];
                AddHeaderNoFormat(header);

                //Border
                var border = xlWorkSheet.Cells["A" + dataStartRow + ":" + ConvertNumberToChar(detailView.Count + 1) + (rowCount + pathwayCount)];
                AddBorder(border);
                border.Style.Numberformat.Format = "#0.00";

                //Add Format Condition. (Cell chart).          
                xlWorkSheet.Cells["A" + (dataStartRow + 1) + ":" + ConvertNumberToChar(detailView.Count + 1) + (rowCount + pathwayCount + 1)].ConditionalFormatting.AddDatabar(System.Drawing.Color.RoyalBlue);
                // xlWorkSheet.Range["A" + dataStartRow, ConvertNumberToChar(detailView.Count + 1) + dataStartRow].NumberFormat = "00";
                //CreateStackedBarChart(xlWorkSheet, chartStartRow, chartLastRow, dataStartRow, (rowCount + pathwayCount), detailView.Count + 1, true, "", "% Busy");

                InsertFreezeRow(xlWorkSheet);

                //Change Font.
                var sheetFontRange = xlWorkSheet.Cells["B" + (rowCount + 1) + ":" + "Z" + (rowCount + pathwayCount + 1)];
                ChangeFontAndWidth(sheetFontRange);

                InsertNavigate(xlWorkSheet, saveLocation, previousSection, previousWorksheet, nextWorksheet, nextSection, worksheetToc);

                InsertPathwayHelpNavigate(xlWorkSheet, saveLocation, Enums.PathwayReportTypes.CpuBusy, isLocalAnalyst);

                //xlWorkSheet.Name = "Collection - CPU Detail";
                xlWorkSheet.Name = worksheetName.Length > 31 ? worksheetName.Substring(0, 31) : worksheetName;
            } catch (Exception ex) {
                _log.ErrorFormat("Error: InsertWorksheetCPUDetailAllPathway: {0}: {1}", worksheetName, ex);
                _reportDownloadLogs.InsertNewLog(_reportDownloadId, DateTime.Now, "There was an error generating the following worksheet: " + worksheetName);
            }

        }

        internal void InsertWorksheetCPUDetailPerPathway(ExcelWorkbook xlWorkBook, int worksheetCount, string pathwayName, Dictionary<string, Core.Infrastructure.PerPathway.CPUDetailView> detailView, string worksheetName, string saveLocation, string previousSection, string previousWorksheet, string nextWorksheet, string nextSection, string worksheetToc, string worksheetTitle, bool isLocalAnalyst) {
            try {
                if (xlWorkBook.Worksheets.Count < worksheetCount)
                    xlWorkBook.Worksheets.Add(worksheetName);
                //var xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.Item["Sheet" + Convert.ToString(worksheetCount)];
                var xlWorkSheet = (ExcelWorksheet)xlWorkBook.Worksheets[xlWorkBook.Worksheets.Count];

                int rowCount = 3;
                int chartStartRow = 3;
                int chartLastRow = 19;

                //Insert Sheet Title.
                xlWorkSheet.Cells[2, 1].Value = worksheetTitle; //pathwayName + " - CPU Detail";
                xlWorkSheet.Cells["A2:Z2"].Merge = true;
                ExcelRange title = xlWorkSheet.Cells["A2"];
                AddTitle(title);

                int dataStartRow = rowCount;
                //Insert Columns.
                int x = 0;
                foreach (var item in detailView.OrderBy(i => i.Key)) {
                    xlWorkSheet.Cells[rowCount, (x + 2)].Value = item.Key;

                    if (x == 0) {
                        xlWorkSheet.Cells[(rowCount + 1), (x + 1)].Value = "Pathmon";
                        xlWorkSheet.Cells[(rowCount + 2), (x + 1)].Value = "TCPs";
                        xlWorkSheet.Cells[(rowCount + 3), (x + 1)].Value = "SERVERs";
                        xlWorkSheet.Cells["A" + (rowCount + 1) + ":" + "A" + (rowCount + 3)].Style.Font.Size = 10;
                        xlWorkSheet.Cells["A" + (rowCount + 1) + ":" + "A" + (rowCount + 3)].Style.Font.Name = "Courier New";
                    }

                    xlWorkSheet.Cells[(rowCount + 1), (x + 2)].Value = Math.Round(item.Value.Pathmon, 2);
                    xlWorkSheet.Cells[(rowCount + 2), (x + 2)].Value = Math.Round(item.Value.Tcp, 2);
                    xlWorkSheet.Cells[(rowCount + 3), (x + 2)].Value = Math.Round(item.Value.Servers, 2);

                    if (x % 2 == 0) {
                        xlWorkSheet.Cells[ConvertNumberToChar((x + 2)) + rowCount + ":" + ConvertNumberToChar((x + 2)) + (rowCount + 3)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        xlWorkSheet.Cells[ConvertNumberToChar((x + 2)) + rowCount + ":" + ConvertNumberToChar((x + 2)) + (rowCount + 3)].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    }
                    x++;
                }

                //Format the Header.
                ExcelRange header = xlWorkSheet.Cells["B" + rowCount + ":" + ConvertNumberToChar(detailView.Count + 1) + rowCount];
                AddHeaderNoFormat(header);

                //Border
                ExcelRange border = xlWorkSheet.Cells["A" + dataStartRow + ":" + ConvertNumberToChar(detailView.Count + 1) + (rowCount + 3)]; //3 is Pathmon, TCPs, and Servers.
                AddBorder(border);
                border.Style.Numberformat.Format = "#0.00";

                xlWorkSheet.Cells["A" + (dataStartRow + 1) + ":" + ConvertNumberToChar(detailView.Count + 1) + (rowCount + 3)].ConditionalFormatting.AddDatabar(System.Drawing.Color.RoyalBlue);

                xlWorkSheet.Cells["A" + dataStartRow + ":" + ConvertNumberToChar(detailView.Count + 1) + dataStartRow].Style.Numberformat.Format = "00";
                //CreateStackedBarChart(xlWorkSheet, chartStartRow, chartLastRow, dataStartRow, (rowCount + 3), detailView.Count + 1, true, "", "% Busy");

                InsertFreezeRow(xlWorkSheet);
                InsertFreezeColumn(xlWorkSheet);

                //Change Font.
                var sheetFontRange = xlWorkSheet.Cells["B" + (dataStartRow + 1) + ":" + "Z" + (rowCount + detailView.Count)];
                ChangeFontAndWidth(sheetFontRange);

                InsertNavigate(xlWorkSheet, saveLocation, previousSection, previousWorksheet, nextWorksheet, nextSection, worksheetToc);

                InsertPathwayHelpNavigate(xlWorkSheet, saveLocation, Enums.PathwayReportTypes.CpuBusy, isLocalAnalyst);

                xlWorkSheet.Name = worksheetName.Length > 31 ? worksheetName.Substring(0, 31) : worksheetName; //pathwayName + " - CPU Detail";
            } catch (Exception ex) {
                _log.ErrorFormat("Error: InsertWorksheetCPUDetailPerPathway: {0}: {1}", worksheetName, ex);
                _reportDownloadLogs.InsertNewLog(_reportDownloadId, DateTime.Now, "There was an error generating the following worksheet: " + worksheetName);
            }

        }

        internal void InsertHourlyTcpCounts(ExcelWorkbook xlWorkBook, int worksheetCount, string worksheetName, string saveLocation, string prevSection, string prevWorksheet, string nextSection, string nextWorksheet, string worksheetToc, string worksheetTitle, bool isLocalAnalyst, DateTime fromTimestamp, DateTime toTimestamp, List<string> pathwayList, Dictionary<string, PathwayHourlyView> dictionary) {
            try {
                if (xlWorkBook.Worksheets.Count < worksheetCount)
                    xlWorkBook.Worksheets.Add(worksheetName);
                //var xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.Item["Sheet" + Convert.ToString(worksheetCount)];
                var xlWorkSheet = (ExcelWorksheet)xlWorkBook.Worksheets[xlWorkBook.Worksheets.Count];

                int rowCount = 3;
                int pathwayCount = 0;

                //Insert Sheet Title.
                xlWorkSheet.Cells[2, 1].Value = worksheetTitle;
                if (pathwayList.Count > 25) {
                    xlWorkSheet.Cells["A2:" + ConvertNumberToChar(pathwayList.Count + 1) + "2"].Merge = true;
                } else {
                    xlWorkSheet.Cells["A2:Z2"].Merge = true;
                }
                var title = xlWorkSheet.Cells["A2"];
                AddTitle(title);

                //fill pathway name, skip cell[3,1]
                for (int i = 2; i < pathwayList.Count + 2; i++) {
                    xlWorkSheet.Cells[rowCount, i].Value = pathwayList[i - 2];
                }
                rowCount++;

                //populate data in excel
                for (DateTime index = fromTimestamp; index < toTimestamp; index = index.AddHours(1)) {
                    for (int i = 1; i < pathwayList.Count + 2; i++) {
                        if (i == 1) {
                            xlWorkSheet.Cells[rowCount, i].Value = index.ToString("MM/dd/yyyy HH:mm");
                        } else {
                            if (dictionary.ContainsKey(index.ToString() + " " + pathwayList[i - 2])) {
                                xlWorkSheet.Cells[rowCount, i].Value = dictionary[index.ToString() + " " + pathwayList[i - 2]].AverageTCPTransaction;
                            } else {
                                xlWorkSheet.Cells[rowCount, i].Value = 0;
                            }
                        }
                    }
                    rowCount++;
                }

                ExcelRange chartRange = xlWorkSheet.Cells["A3:A" + (rowCount - 1) + "," + "B3:" + ConvertNumberToChar(pathwayList.Count + 1) + (rowCount - 1)];
                DrawLineGraphTransaction(xlWorkSheet, "B4", pathwayList, chartRange, "TCP Transaction Counts, Hourly", "Transaction Counts", rowCount);

                xlWorkSheet.Cells.AutoFitColumns();

                //Change Font.
                var sheetFontRange = xlWorkSheet.Cells["B" + (rowCount + 1) + ":" + "Z" + (rowCount + pathwayCount + 1)];
                ChangeFontAndWidth(sheetFontRange);
                InsertFreezeRow(xlWorkSheet);

                InsertNavigate(xlWorkSheet, saveLocation, prevSection, prevWorksheet, nextWorksheet, nextSection, worksheetToc, pathwayList.Count);

                InsertPathwayHelpNavigate(xlWorkSheet, saveLocation, Enums.PathwayReportTypes.CpuBusy, isLocalAnalyst);

                xlWorkSheet.Name = worksheetName.Length > 31 ? worksheetName.Substring(0, 31) : worksheetName;
            } catch (Exception ex) {
                _log.ErrorFormat("Error: InsertHourlyTcpCounts: {0}: {1}", worksheetName, ex);
                _reportDownloadLogs.InsertNewLog(_reportDownloadId, DateTime.Now, "There was an error generating the following worksheet: " + worksheetName);
            }

        }

        internal void InsertHourlyLinkmonCounts(ExcelWorkbook xlWorkBook, int worksheetCount, string worksheetName, string saveLocation, string prevSection, string prevWorksheet, string nextSection, string nextWorksheet, string worksheetToc, string worksheetTitle, bool isLocalAnalyst, DateTime fromTimestamp, DateTime toTimestamp, List<string> pathwayList, Dictionary<string, PathwayHourlyView> dictionary) {
            try {
                if (xlWorkBook.Worksheets.Count < worksheetCount)
                    xlWorkBook.Worksheets.Add(worksheetName);
                //var xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.Item["Sheet" + Convert.ToString(worksheetCount)];
                var xlWorkSheet = (ExcelWorksheet)xlWorkBook.Worksheets[xlWorkBook.Worksheets.Count];

                int rowCount = 3;
                int pathwayCount = 0;

                //Insert Sheet Title.
                xlWorkSheet.Cells[2, 1].Value = worksheetTitle;
                if (pathwayList.Count > 25) {
                    xlWorkSheet.Cells["A2:" + ConvertNumberToChar(pathwayList.Count + 1) + "2"].Merge = true;
                } else {
                    xlWorkSheet.Cells["A2:Z2"].Merge = true;
                }
                var title = xlWorkSheet.Cells["A2"];
                AddTitle(title);

                //fill pathway name, skip cell[3,1]
                for (int i = 2; i < pathwayList.Count + 2; i++) {
                    xlWorkSheet.Cells[rowCount, i].Value = pathwayList[i - 2];
                }
                rowCount++;

                //populate data in excel
                for (DateTime index = fromTimestamp; index < toTimestamp; index = index.AddHours(1)) {
                    for (int i = 1; i < pathwayList.Count + 2; i++) {
                        if (i == 1) {
                            xlWorkSheet.Cells[rowCount, i].Value = index.ToString("MM/dd/yyyy HH:mm");
                        } else {
                            if (dictionary.ContainsKey(index.ToString() + " " + pathwayList[i - 2])) {
                                xlWorkSheet.Cells[rowCount, i].Value = dictionary[index.ToString() + " " + pathwayList[i - 2]].AverageLinkmonTransaction;
                            } else {
                                xlWorkSheet.Cells[rowCount, i].Value = 0;
                            }
                        }
                    }
                    rowCount++;
                }

                ExcelRange chartRange = xlWorkSheet.Cells["A3:A" + (rowCount - 1) + "," + "B3:" + ConvertNumberToChar(pathwayList.Count + 1) + (rowCount - 1)];
                DrawLineGraphTransaction(xlWorkSheet, "B4", pathwayList, chartRange, "Linkmon Transaction Counts, Hourly", "Transaction Counts", rowCount);

                xlWorkSheet.Cells.AutoFitColumns();

                //Change Font.
                var sheetFontRange = xlWorkSheet.Cells["B" + (rowCount + 1) + ":" + "Z" + (rowCount + pathwayCount + 1)];
                ChangeFontAndWidth(sheetFontRange);

                InsertFreezeRow(xlWorkSheet);

                InsertNavigate(xlWorkSheet, saveLocation, prevSection, prevWorksheet, nextWorksheet, nextSection, worksheetToc, pathwayList.Count);

                InsertPathwayHelpNavigate(xlWorkSheet, saveLocation, Enums.PathwayReportTypes.CpuBusy, isLocalAnalyst);

                xlWorkSheet.Name = worksheetName.Length > 31 ? worksheetName.Substring(0, 31) : worksheetName;
            } catch (Exception ex) {
                _log.ErrorFormat("Error: InsertHourlyLinkmonCounts: {0}: {1}", worksheetName, ex);
                _reportDownloadLogs.InsertNewLog(_reportDownloadId, DateTime.Now, "There was an error generating the following worksheet: " + worksheetName);
            }

        }

        internal void InsertHourlyTransactionCounts(ExcelWorkbook xlWorkBook, int worksheetCount, string worksheetName, string saveLocation, string prevSection, string prevWorksheet, string nextSection, string nextWorksheet, string worksheetToc, string worksheetTitle, bool isLocalAnalyst, DateTime fromTimestamp, DateTime toTimestamp, List<string> pathwayList, Dictionary<string, PathwayHourlyView> dictionary) {
            try {
                if (xlWorkBook.Worksheets.Count < worksheetCount)
                    xlWorkBook.Worksheets.Add(worksheetName);
                //var xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.Item["Sheet" + Convert.ToString(worksheetCount)];
                var xlWorkSheet = (ExcelWorksheet)xlWorkBook.Worksheets[xlWorkBook.Worksheets.Count];

                int rowCount = 3;
                int pathwayCount = 0;

                //Insert Sheet Title.
                xlWorkSheet.Cells[2, 1].Value = worksheetTitle;
                if (pathwayList.Count > 25) {
                    xlWorkSheet.Cells["A2" + ":" + ConvertNumberToChar(pathwayList.Count + 1) + "2"].Merge = true;
                } else {
                    xlWorkSheet.Cells["A2:Z2"].Merge = true;
                }
                var title = xlWorkSheet.Cells["A2"];
                AddTitle(title);

                //fill pathway name, skip cell[3,1]
                for (int i = 2; i < pathwayList.Count + 2; i++) {
                    xlWorkSheet.Cells[rowCount, i].Value = pathwayList[i - 2];
                }
                rowCount++;

                //populate data in excel
                for (DateTime index = fromTimestamp; index < toTimestamp; index = index.AddHours(1)) {
                    for (int i = 1; i < pathwayList.Count + 2; i++) {
                        if (i == 1) {
                            xlWorkSheet.Cells[rowCount, i].Value = index.ToString("MM/dd/yyyy HH:mm");
                        } else {
                            if (dictionary.ContainsKey(index.ToString() + " " + pathwayList[i - 2])) {
                                xlWorkSheet.Cells[rowCount, i].Value = dictionary[index.ToString() + " " + pathwayList[i - 2]].TransactionCount;
                            } else {
                                xlWorkSheet.Cells[rowCount, i].Value = 0;
                            }
                        }
                    }
                    rowCount++;
                }

                //xlWorkSheet.Cells["A4:A" + rowCount].Style.Numberformat.Format = "MM/dd/yyyy";
                xlWorkSheet.Column(1).Style.Numberformat.Format = "MM-dd-yyyy h:m";

                ExcelRange chartRange = xlWorkSheet.Cells["A3:A" + (rowCount - 1) + "," + "B3:" + ConvertNumberToChar(pathwayList.Count + 1) + (rowCount - 1)];
                DrawLineGraphTransaction(xlWorkSheet, "B4", pathwayList, chartRange, "Transaction Counts, Hourly", "Transaction Counts", rowCount);
                ExcelRange dataRange = xlWorkSheet.Cells["B4:" + ConvertNumberToChar(pathwayList.Count + 1) + (rowCount - 1)];

                xlWorkSheet.Cells.AutoFitColumns();

                //Change Font.
                var sheetFontRange = xlWorkSheet.Cells["B" + (rowCount + 1) + ":" + "Z" + (rowCount + pathwayCount + 1)];
                ChangeFontAndWidth(sheetFontRange);

                InsertFreezeRow(xlWorkSheet);

                InsertNavigate(xlWorkSheet, saveLocation, prevSection, prevWorksheet, nextWorksheet, nextSection, worksheetToc, pathwayList.Count);

                InsertPathwayHelpNavigate(xlWorkSheet, saveLocation, Enums.PathwayReportTypes.CpuBusy, isLocalAnalyst);

                xlWorkSheet.Name = worksheetName.Length > 31 ? worksheetName.Substring(0, 31) : worksheetName;
            } catch (Exception ex) {
                _log.ErrorFormat("Error: InsertHourlyTransactionCounts: {0}: {1}", worksheetName, ex);
                _reportDownloadLogs.InsertNewLog(_reportDownloadId, DateTime.Now, "There was an error generating the following worksheet: " + worksheetName);
            }

        }

        internal void InsertPeakHourlyCPUBusyPerPathway(ExcelWorkbook xlWorkBook, int worksheetCount, string worksheetName, string saveLocation, string prevSection, string prevWorksheet, string nextSection, string nextWorksheet, string worksheetToc, string worksheetTitle, bool isLocalAnalyst, DateTime fromTimestamp, DateTime toTimestamp, List<string> pathwayList, Dictionary<string, PathwayHourlyView> dictionary) {
            try {
                if (xlWorkBook.Worksheets.Count < worksheetCount)
                    xlWorkBook.Worksheets.Add(worksheetName);
                //var xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.Item["Sheet" + Convert.ToString(worksheetCount)];
                var xlWorkSheet = (ExcelWorksheet)xlWorkBook.Worksheets[xlWorkBook.Worksheets.Count];

                //Insert Sheet Title.
                xlWorkSheet.Cells[2, 1].Value = worksheetTitle;
                if (pathwayList.Count > 25) {
                    xlWorkSheet.Cells["A2:" + ConvertNumberToChar(pathwayList.Count + 1) + "2"].Merge = true;
                } else {
                    xlWorkSheet.Cells["A2:Z2"].Merge = true;
                }
                var title = xlWorkSheet.Cells["A2"];
                AddTitle(title);

                int rowCount = 3;
                int pathwayCount = 0;

                //fill pathway name, skip cell[3,1]
                for (int i = 2; i < pathwayList.Count + 2; i++) {
                    xlWorkSheet.Cells[rowCount, i].Value = pathwayList[i - 2];
                }
                rowCount++;

                //populate data in excel
                for (DateTime index = fromTimestamp; index < toTimestamp; index = index.AddHours(1)) {
                    for (int i = 1; i < pathwayList.Count + 2; i++) {
                        if (i == 1) {
                            xlWorkSheet.Cells[rowCount, i].Value = index.ToString("MM/dd/yyyy HH:mm");
                        } else {
                            if (dictionary.ContainsKey(index.ToString() + " " + pathwayList[i - 2])) {
                                xlWorkSheet.Cells[rowCount, i].Value = dictionary[index.ToString() + " " + pathwayList[i - 2]].PeakCpuBusy;
                            } else {
                                xlWorkSheet.Cells[rowCount, i].Value = 0;
                            }
                        }
                    }
                    rowCount++;
                }

                ExcelRange chartRange = xlWorkSheet.Cells["A3:A" + (rowCount - 1) + "," + "B3:" + ConvertNumberToChar(pathwayList.Count + 1) + (rowCount - 1)];
                //formatting number to prevent excel turn on scientific notation automatically
                ExcelRange dataRange = xlWorkSheet.Cells["B4:" + ConvertNumberToChar(pathwayList.Count + 1) + (rowCount - 1)];
                dataRange.Style.Numberformat.Format = "0.000000000";
                DrawLineGraphCPU(xlWorkSheet, "B4", pathwayList, chartRange, "Peak CPU Busy %, Hourly", "CPU Busy %", rowCount);

                xlWorkSheet.Cells.AutoFitColumns();

                //Change Font.
                var sheetFontRange = xlWorkSheet.Cells["B" + (rowCount + 1) + ":" + "Z" + (rowCount + pathwayCount + 1)];
                ChangeFontAndWidth(sheetFontRange);

                for (int i = 2; i <= pathwayList.Count + 1; i++) {
                    xlWorkSheet.Column(i).Width = 12;
                }

                InsertFreezeRow(xlWorkSheet);

                InsertNavigate(xlWorkSheet, saveLocation, prevSection, prevWorksheet, nextWorksheet, nextSection, worksheetToc, pathwayList.Count);

                InsertPathwayHelpNavigate(xlWorkSheet, saveLocation, Enums.PathwayReportTypes.CpuBusy, isLocalAnalyst);

                xlWorkSheet.Name = worksheetName.Length > 31 ? worksheetName.Substring(0, 31) : worksheetName;
            } catch (Exception ex) {
                _log.ErrorFormat("Error: InsertPeakHourlyCPUBusyPerPathway: {0}: {1}", worksheetName, ex);
                _reportDownloadLogs.InsertNewLog(_reportDownloadId, DateTime.Now, "There was an error generating the following worksheet: " + worksheetName);
            }

        }

        internal void InsertHourlyCPUBusyPerPathway(ExcelWorkbook xlWorkBook, int worksheetCount, string worksheetName, string saveLocation, string prevSection, string prevWorksheet, string nextSection, string nextWorksheet, string worksheetToc, string worksheetTitle, bool isLocalAnalyst, DateTime fromTimestamp, DateTime toTimestamp, List<string> pathwayList, Dictionary<string, PathwayHourlyView> dictionary) {
            try {
                if (xlWorkBook.Worksheets.Count < worksheetCount)
                    xlWorkBook.Worksheets.Add(worksheetName);
                //var xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.Item["Sheet" + Convert.ToString(worksheetCount)];
                var xlWorkSheet = (ExcelWorksheet)xlWorkBook.Worksheets[xlWorkBook.Worksheets.Count];

                //Insert Sheet Title.
                xlWorkSheet.Cells[2, 1].Value = worksheetTitle;
                if (pathwayList.Count > 25) {
                    xlWorkSheet.Cells["A2" + ":" + ConvertNumberToChar(pathwayList.Count + 1) + "2"].Merge = true;
                } else {
                    xlWorkSheet.Cells["A2:Z2"].Merge = true;
                }
                var title = xlWorkSheet.Cells["A2"];
                AddTitle(title);

                int rowCount = 3;
                int pathwayCount = 0;

                //fill pathway name, skip cell[3,1]
                for (int i = 2; i < pathwayList.Count + 2; i++) {
                    xlWorkSheet.Cells[rowCount, i].Value = pathwayList[i - 2];
                }
                rowCount++;

                //populate data in excel
                for (DateTime index = fromTimestamp; index < toTimestamp; index = index.AddHours(1)) {
                    for (int i = 1; i < pathwayList.Count + 2; i++) {
                        if (i == 1) {
                            xlWorkSheet.Cells[rowCount, i].Value = index.ToString("MM/dd/yyyy HH:mm");
                        } else {
                            if (dictionary.ContainsKey(index.ToString() + " " + pathwayList[i - 2])) {
                                xlWorkSheet.Cells[rowCount, i].Value = dictionary[index.ToString() + " " + pathwayList[i - 2]].CpuBusy;
                            } else {
                                xlWorkSheet.Cells[rowCount, i].Value = 0;
                            }
                        }
                    }
                    rowCount++;
                }

                xlWorkSheet.Cells["A4:A" + rowCount].Style.Numberformat.Format = "MM/dd/yyyy hh:mm";

                ExcelRange chartRange = xlWorkSheet.Cells["A3:A" + (rowCount - 1) + "," + "B3:" + ConvertNumberToChar(pathwayList.Count + 1) + (rowCount - 1)];
                //formatting number to prevent excel turn on scientific notation automatically
                ExcelRange dataRange = xlWorkSheet.Cells["B4:" + ConvertNumberToChar(pathwayList.Count + 1) + (rowCount - 1)];
                dataRange.Style.Numberformat.Format = "0.000000000";
                DrawLineGraphCPU(xlWorkSheet, "B4", pathwayList, chartRange, "Avg CPU Busy %, Hourly", "CPU Busy %", rowCount);

                xlWorkSheet.Cells.AutoFitColumns();

                //Change Font.
                var sheetFontRange = xlWorkSheet.Cells["B" + (rowCount + 1) + ":" + "Z" + (rowCount + pathwayCount + 1)];
                ChangeFontAndWidth(sheetFontRange);

                for (int i = 2; i <= pathwayList.Count + 1; i++) {
                    xlWorkSheet.Column(i).Width = 12;
                }

                sheetFontRange.ConditionalFormatting.AddDatabar(System.Drawing.Color.RoyalBlue);
                InsertFreezeRow(xlWorkSheet);
                InsertNavigate(xlWorkSheet, saveLocation, prevSection, prevWorksheet, nextWorksheet, nextSection, worksheetToc, pathwayList.Count);

                InsertPathwayHelpNavigate(xlWorkSheet, saveLocation, Enums.PathwayReportTypes.CpuBusy, isLocalAnalyst);

                xlWorkSheet.Name = worksheetName.Length > 31 ? worksheetName.Substring(0, 31) : worksheetName;
            }
            catch (Exception ex) {
                _log.ErrorFormat("Error: InsertHourlyCPUBusyPerPathway: {0}: {1}", worksheetName, ex);
                _reportDownloadLogs.InsertNewLog(_reportDownloadId, DateTime.Now, "There was an error generating the following worksheet: " + worksheetName);
            }

        }

        internal void InsertWorksheetTransactionServerAllPathway(ExcelWorkbook xlWorkBook, int worksheetCount,
                                                            Core.Infrastructure.AllPathway.TransactionServerView transactionViewServer,
                                                            Core.Infrastructure.AllPathway.TransactionTcpView transactionViewTcp,
                                                            string worksheetName, string saveLocation, string previousSection, string previousWorksheet, string nextWorksheet, string nextSection, string worksheetToc, string worksheetTitle, bool isLocalAnalyst) {
            try {
                ExcelWorksheet xlWorkSheet = null;
                if (xlWorkBook.Worksheets.Count < worksheetCount)
                    xlWorkSheet = xlWorkBook.Worksheets.Add(worksheetName);
                //var xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.Item["Sheet" + Convert.ToString(worksheetCount)];

                int rowCount = 3;
                int chartStartRow = 3;
                int chartLastRow = 19;

                //Insert Sheet Title.
                xlWorkSheet.Cells[2, 1].Value = worksheetTitle;
                xlWorkSheet.Cells["A2:Z2"].Merge = true;
                ExcelRange title = xlWorkSheet.Cells["A2"];
                AddTitle(title);

                int dataStartRow = rowCount;
                /*//Insert All Pathway.
                xlWorkSheet.Cells[rowCount, 1] = "LINKMON-SERVER Transactions";
                xlWorkSheet.Range["A" + rowCount, "B" + rowCount].MergeCells = true;

                rowCount++;*/
                xlWorkSheet.Cells[rowCount, 1].Value = "Pathway";
                xlWorkSheet.Cells[rowCount, 2].Value = "Linkmon Transactions";
                xlWorkSheet.Cells[rowCount, 3].Value = "TCP Transactions";

                //Format the Header.
                var header = xlWorkSheet.Cells["A" + rowCount + ":" + "C" + rowCount];
                AddHeader(header);
                rowCount++;

                var items = transactionViewServer.LinkmonToServer.OrderByDescending(x => x.Value.TotalIrReqCnt).ToList();

                foreach (var view in items) {
                    xlWorkSheet.Cells[rowCount, 1].Value = view.Key;
                    xlWorkSheet.Cells[rowCount, 2].Value = view.Value.TotalIrReqCnt;
                    //Add TCP - SERVER Trans.
                    xlWorkSheet.Cells[rowCount, 3].Value = transactionViewServer.TcptoServer[view.Key].TotalIsReqCnt;
                    rowCount++;
                }

                //Get max value.
                var maxLinkmonValue = items.Max(x => x.Value.TotalIrReqCnt);
                var maxServerValue = transactionViewServer.TcptoServer.Max(x => x.Value.TotalIsReqCnt);
                var totalMax = Math.Max(maxLinkmonValue, maxServerValue);

                //CreateStackedBarChart(xlWorkSheet, chartStartRow, chartLastRow, dataStartRow, (rowCount - 1), 3, false, "SERVER Transaction Counts", "Total Transaction Counts", totalMax);
                //CreatePieChart(xlWorkSheet, chartStartRow, chartLastRow, dataStartRow, rowCount - 1, 2, "All Pathways SERVER, Linkmon Transactions - Collection");
                //Border
                var border = xlWorkSheet.Cells["A" + dataStartRow + ":" + "C" + (rowCount - 1)];
                AddBorder(border);
                border.Style.Numberformat.Format = "#,##0";
                border = xlWorkSheet.Cells["B" + dataStartRow + ":" + "B" + (rowCount - 1)];
                border.ConditionalFormatting.AddDatabar(System.Drawing.Color.RoyalBlue);
                border = xlWorkSheet.Cells["C" + dataStartRow + ":" + "C" + (rowCount - 1)];
                border.ConditionalFormatting.AddDatabar(System.Drawing.Color.RoyalBlue);

                //Change Font.
                var sheetFontRange = xlWorkSheet.Cells["A" + (dataStartRow + 1) + ":" + "C" + (rowCount - 1)];
                ChangeFontAndWidth(sheetFontRange);

                xlWorkSheet.Cells["A3:A" + rowCount].AutoFitColumns();
                xlWorkSheet.Cells.AutoFitColumns();
                xlWorkSheet.Column(1).AutoFit();
                xlWorkSheet.Cells.AutoFitColumns(0, 150);

                //Freeze first row.
                InsertFreezeRow(xlWorkSheet);

                InsertNavigate(xlWorkSheet, saveLocation, previousSection, previousWorksheet, nextWorksheet, nextSection, worksheetToc);

                InsertPathwayHelpNavigate(xlWorkSheet, saveLocation, Enums.PathwayReportTypes.Transactions, isLocalAnalyst);

                //xlWorkSheet.Name = "Collection - Transaction SERVER";
                xlWorkSheet.Name = worksheetName.Length > 31 ? worksheetName.Substring(0, 31) : worksheetName;
            } catch (Exception ex) {
                _log.ErrorFormat("Error: InsertWorksheetTransactionServerAllPathway: {0}: {1}", worksheetName, ex);
                _reportDownloadLogs.InsertNewLog(_reportDownloadId, DateTime.Now, "There was an error generating the following worksheet: " + worksheetName);
            }

        }

        internal void InsertWorksheetTransactionPerPathwayInterval(ExcelWorkbook xlWorkBook, int worksheetCount, string pathwayName,
                                        Dictionary<string, Pathway.Core.Infrastructure.PerPathway.TransactionServerView> transactionViewServers,
                                        Dictionary<string, Pathway.Core.Infrastructure.PerPathway.TransactionTcpView> transactionViewTcps,
                                        string worksheetName, string saveLocation, string previousSection,
                                        string previousWorksheet, string nextWorksheet, string nextSection, string worksheetToc, string worksheetTitle, bool isLocalAnalyst) {
            try {
                if (xlWorkBook.Worksheets.Count < worksheetCount)
                    xlWorkBook.Worksheets.Add(worksheetName);
                //var xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.Item["Sheet" + Convert.ToString(worksheetCount)];
                var xlWorkSheet = (ExcelWorksheet)xlWorkBook.Worksheets[xlWorkBook.Worksheets.Count];

                var rowCount = 3;
                var chartStartRow = 3;
                var chartLastRow = 19;

                //Insert Sheet Title.
                xlWorkSheet.Cells[2, 1].Value = worksheetTitle; //pathwayName + " - SERVER Transactions";
                xlWorkSheet.Cells["A2:Z2"].Merge = true;
                var title = xlWorkSheet.Cells["A2"];
                AddTitle(title);

                var dataStartRow = rowCount;
                var serverTitle = xlWorkSheet.Cells["A" + dataStartRow + ":" + ConvertNumberToChar(transactionViewServers.Count + 1) + dataStartRow];//.NumberFormat = "@";
                serverTitle.Style.Numberformat.Format = "@";

                for (var x = 0; x < transactionViewServers.Count; x++) {
                    var hour = transactionViewServers.Keys.ElementAt(x);
                    xlWorkSheet.Cells[rowCount, (x + 2)].Value = hour;
                    if (x % 2 == 0) {
                        xlWorkSheet.Cells[ConvertNumberToChar((x + 2)) + rowCount].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        xlWorkSheet.Cells[ConvertNumberToChar((x + 2)) + rowCount].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    }
                }
                rowCount++;

                //Insert All Pathway.
                xlWorkSheet.Cells[rowCount, 1].Value = "Linkmon Transactions";
                xlWorkSheet.Cells[rowCount, 1].AutoFitColumns();
                for (var x = 0; x < transactionViewServers.Count; x++) {
                    var hour = transactionViewServers.Keys.ElementAt(x);
                    xlWorkSheet.Cells[rowCount, (x + 2)].Value = transactionViewServers[hour].LinkmonToServer;
                    if (x % 2 == 0) {
                        xlWorkSheet.Cells[ConvertNumberToChar((x + 2)) + rowCount].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        xlWorkSheet.Cells[ConvertNumberToChar((x + 2)) + rowCount].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    }
                }
                //xlWorkSheet.Cells[rowCount, 2] = transactionViewServer.LinkmonToServer;
                rowCount++;

                xlWorkSheet.Cells[rowCount, 1].Value = "TCP Transactions";
                xlWorkSheet.Cells[rowCount, 1].AutoFitColumns();
                for (var x = 0; x < transactionViewServers.Count; x++) {
                    var hour = transactionViewServers.Keys.ElementAt(x);
                    xlWorkSheet.Cells[rowCount, (x + 2)].Value = transactionViewServers[hour].TcptoServer;
                    if (x % 2 == 0) {
                        xlWorkSheet.Cells[ConvertNumberToChar((x + 2)) + rowCount].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        xlWorkSheet.Cells[ConvertNumberToChar((x + 2)) + rowCount].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    }
                }
                //xlWorkSheet.Cells[rowCount, 2] = transactionViewServer.TcptoServer;

                xlWorkSheet.Column(1).Width = 190;

                //Format the Header.
                var header = xlWorkSheet.Cells["A" + (rowCount - 1) + ":" + "A" + rowCount];
                //AddHeader(header);
                AddHeaderNoFormat(header);
                AddHeaderNoFormat(serverTitle);
                header.AutoFitColumns();
                serverTitle.AutoFitColumns();

                var maxLinkmonValue = transactionViewServers.Max(x => x.Value.LinkmonToServer);
                var maxTCPValue = transactionViewServers.Max(x => x.Value.TcptoServer);
                var maxValue = Convert.ToInt64(Math.Max(maxLinkmonValue, maxTCPValue));
                if (maxValue == 0)
                    maxValue = 1;

                //  CreateStackedBarChart(xlWorkSheet, chartStartRow, chartLastRow, dataStartRow, rowCount, transactionViewServers.Count + 1, false, "", "Total Transaction Counts", maxValue);
                //CreatePieChart(xlWorkSheet, chartStartRow, chartLastRow, dataStartRow, rowCount, 2, pathwayName + " Pathway SERVER Transactions - Collection");
                //Border
                var border = xlWorkSheet.Cells["A" + dataStartRow + ":" + ConvertNumberToChar(transactionViewServers.Count + 1) + rowCount];
                AddBorder(border);
                border.Style.Numberformat.Format = "#,##0";

                border = xlWorkSheet.Cells["B" + (dataStartRow + 1) + ":" + "Y" + rowCount];
                border.ConditionalFormatting.AddDatabar(System.Drawing.Color.RoyalBlue);

                //Change Font.
                ChangeFontAndWidth(border);

                //Freeze first row.InsertWorksheetTransactionTcpPerPathway
                InsertFreezeRow(xlWorkSheet);

                InsertFreezeColumn(xlWorkSheet);
                xlWorkSheet.Cells.AutoFitColumns();
                //Change Font.
                ChangeFontAndWidth(border);

                for (int i = 2; i <= transactionViewServers.Count + 1; i++) {
                    xlWorkSheet.Column(i).Width = 12;
                }

                InsertNavigate(xlWorkSheet, saveLocation, previousSection, previousWorksheet, nextWorksheet, nextSection, worksheetToc);


                InsertPathwayHelpNavigate(xlWorkSheet, saveLocation, Enums.PathwayReportTypes.Transactions, isLocalAnalyst);

                xlWorkSheet.Name = worksheetName.Length > 31 ? worksheetName.Substring(0, 31) : worksheetName; //pathwayName + " - Transaction SERVER";
            } catch (Exception ex) {
                _log.ErrorFormat("Error: InsertWorksheetTransactionPerPathwayInterval: {0}: {1}", worksheetName, ex);
                _reportDownloadLogs.InsertNewLog(_reportDownloadId, DateTime.Now, "There was an error generating the following worksheet: " + worksheetName);
            }
        }

        private void DrawLineGraphCPU(ExcelWorksheet xlWorkSheet, string position, List<string> pathwayList, ExcelRange chartRange, string graphTitle, string yAixsName, int rowCount) {

            try {
                var lineChart = xlWorkSheet.Drawings.AddChart(graphTitle, OfficeOpenXml.Drawing.Chart.eChartType.Line);

                var rangeLabel = xlWorkSheet.Cells["A4:A" + rowCount];
                for (int i = 2; i <= pathwayList.Count + 1; i++) {
                    var dataRange = xlWorkSheet.Cells[ConvertNumberToChar(i) + "4:" + ConvertNumberToChar(i) + rowCount];
                    lineChart.Series.Add(dataRange, rangeLabel);
                    lineChart.Series[i - 2].Header = pathwayList[i - 2];

                };

                lineChart.SetPosition(3, 0, 1, 0);
                lineChart.Title.Text = graphTitle;
                lineChart.Title.Font.Size = 12;
                lineChart.XAxis.Format = "yyyy-MM-dd HH:mm";
                lineChart.RoundedCorners = false;
                lineChart.PlotArea.Fill.Color = System.Drawing.ColorTranslator.FromHtml("#505050");
                lineChart.Fill.Color = System.Drawing.Color.Black;
                lineChart.Border.Fill.Color = System.Drawing.Color.White;
                lineChart.Legend.Font.Color = System.Drawing.Color.White;
                lineChart.YAxis.MajorGridlines.Fill.Color = System.Drawing.Color.White;
                lineChart.YAxis.Title.Font.Size = 10;
                lineChart.YAxis.Title.Font.Color = System.Drawing.Color.White;
                lineChart.YAxis.Title.Text = yAixsName;
                lineChart.YAxis.Font.Color = System.Drawing.Color.White;
                lineChart.XAxis.Font.Color = System.Drawing.Color.White;
                lineChart.Title.Font.Size = 12;
                lineChart.Title.Font.Color = System.Drawing.Color.White;
                lineChart.SetSize(800, 500);
            }
            catch (Exception ex) {
                _log.ErrorFormat("Error: DrawLineGraphCPU: {0}", ex);
            }
        }
		private void DrawLineGraphTransaction(ExcelWorksheet xlWorkSheet, string position, List<string> pathwayList, ExcelRange chartRange, string graphTitle, string yAixsName, int rowCount) {
            try {
                var lineChart = xlWorkSheet.Drawings.AddChart(graphTitle, OfficeOpenXml.Drawing.Chart.eChartType.Line);

                var rangeLabel = xlWorkSheet.Cells["A4:A" + rowCount];
                for (int i = 2; i <= pathwayList.Count + 1; i++) {
                    var dataRange = xlWorkSheet.Cells[ConvertNumberToChar(i) + "4:" + ConvertNumberToChar(i) + rowCount];
                    lineChart.Series.Add(dataRange, rangeLabel);
                    lineChart.Series[i - 2].Header = pathwayList[i - 2];

                };

                lineChart.SetPosition(3, 0, 1, 0);
                lineChart.Title.Text = graphTitle;
                lineChart.Title.Font.Size = 12;
                lineChart.XAxis.Format = "yyyy-MM-dd HH:mm";
                lineChart.RoundedCorners = false;
                lineChart.PlotArea.Fill.Color = System.Drawing.ColorTranslator.FromHtml("#505050");
                lineChart.Fill.Color = System.Drawing.Color.Black;
                lineChart.Border.Fill.Color = System.Drawing.Color.White;
                lineChart.Legend.Font.Color = System.Drawing.Color.White;
                lineChart.YAxis.MajorGridlines.Fill.Color = System.Drawing.Color.White;
                lineChart.YAxis.Title.Font.Size = 10;
                lineChart.YAxis.Title.Font.Color = System.Drawing.Color.White;
                lineChart.YAxis.Title.Text = yAixsName;
                lineChart.YAxis.Font.Color = System.Drawing.Color.White;
                lineChart.XAxis.Font.Color = System.Drawing.Color.White;
                lineChart.Title.Font.Size = 12;
                lineChart.Title.Font.Color = System.Drawing.Color.White;
                lineChart.SetSize(800, 500);
            }
            catch (Exception ex) {
                _log.ErrorFormat("Error: DrawLineGraphTransaction: {0}", ex);
            }
        }
	}
}