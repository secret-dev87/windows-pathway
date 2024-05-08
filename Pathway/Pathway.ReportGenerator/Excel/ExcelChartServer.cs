using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pathway.Core.Infrastructure;
using Pathway.Core.Infrastructure.PerPathway.Server;
using Pathway.Core.RemoteAnalyst.Concrete;
using Pathway.ReportGenerator.Infrastructure;
using OfficeOpenXml;
using log4net;

namespace Pathway.ReportGenerator.Excel {
    class ExcelChartServer : ExcelChartBase
    {
        private readonly ILog _log;
        private readonly int _reportDownloadId;
        private readonly ReportDownloadLogs _reportDownloadLogs;

        public ExcelChartServer(int reportDownloadId, ReportDownloadLogs reportDownloadLogs, ILog log) {
            _reportDownloadId = reportDownloadId;
            _reportDownloadLogs = reportDownloadLogs;
            _log = log;
        }

        internal void InsertWorksheetCPUBusy(ExcelWorkbook xlWorkBook, int worksheetCount, string pathwayName, List<ServerCPUBusyView> detailView, string worksheetName, string saveLocation, string previousSection, string previousWorksheet, string nextWorksheet, string nextSection, string worksheetToc, string worksheetTitle, bool isCPUBusy, bool isLocalAnalyst) {
            try {
                if (xlWorkBook.Worksheets.Count < worksheetCount)
                    xlWorkBook.Worksheets.Add(worksheetName);
                //var xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.Item["Sheet" + Convert.ToString(worksheetCount)];
                var xlWorkSheet = (ExcelWorksheet)xlWorkBook.Worksheets[xlWorkBook.Worksheets.Count];

                int rowCount = 3;
                //int chartStartRow = 3;
                // int chartLastRow = 19;

                if (detailView.Count == 0)
                    rowCount = 3;



                //Insert Sheet Title.
                xlWorkSheet.Cells[2, 1].Value = worksheetTitle; //pathwayName + " - CPU Detail";
                xlWorkSheet.Cells["A2:Z2"].Merge = true;
                var title = xlWorkSheet.Cells["A2"];
                AddTitle(title);
                string endColume = "I";
                int dataStartRow = rowCount;

                if (detailView.Where(x => x.TcpTransaction > 0).Count() == 0) {

                    endColume = "G";
                    //Insert Columns.
                    xlWorkSheet.Cells[rowCount, 1].Value = "SERVER Class";
                    xlWorkSheet.Cells[rowCount, 2].Value = "CPU Busy %, Average";
                    xlWorkSheet.Cells[rowCount, 3].Value = "Linkmon Transactions";
                    xlWorkSheet.Cells[rowCount, 4].Value = "Process Count, Average";
                    xlWorkSheet.Cells[rowCount, 5].Value = "Process Count, Max.";
                    xlWorkSheet.Cells[rowCount, 6].Value = "NUMSTATIC";
                    xlWorkSheet.Cells[rowCount, 7].Value = "MAXSERVERS";
                } else {

                    //Insert Columns.
                    xlWorkSheet.Cells[rowCount, 1].Value = "SERVER Class";
                    xlWorkSheet.Cells[rowCount, 2].Value = "CPU Busy %, Average";
                    xlWorkSheet.Cells[rowCount, 3].Value = "All Transactions";
                    xlWorkSheet.Cells[rowCount, 4].Value = "TCP Transactions";
                    xlWorkSheet.Cells[rowCount, 5].Value = "Linkmon Transactions";
                    xlWorkSheet.Cells[rowCount, 6].Value = "Process Count, Average";
                    xlWorkSheet.Cells[rowCount, 7].Value = "Process Count, Max.";
                    xlWorkSheet.Cells[rowCount, 8].Value = "NUMSTATIC";
                    xlWorkSheet.Cells[rowCount, 9].Value = "MAXSERVERS";

                }




                //Format the Header.
                ExcelRange header = xlWorkSheet.Cells["A" + rowCount + ":" + endColume + rowCount];
                AddHeaderNoFormat(header);

                rowCount++;

                if (detailView.Where(x => x.TcpTransaction > 0).Count() == 0) {
                    foreach (var item in detailView.OrderByDescending(i => i.CPUBusy)) {
                        xlWorkSheet.Cells[rowCount, 1].Value = item.ServerClass;
                        xlWorkSheet.Cells[rowCount, 2].Value = item.CPUBusy;
                        xlWorkSheet.Cells[rowCount, 3].Value = item.LinkmonTransaction;
                        xlWorkSheet.Cells[rowCount, 4].Value = item.ProcessCount;
                        xlWorkSheet.Cells[rowCount, 5].Value = item.ProcessMaxCount;
                        xlWorkSheet.Cells[rowCount, 6].Value = item.NumStatic;
                        xlWorkSheet.Cells[rowCount, 7].Value = item.MaxServers;
                        if (item.ProcessCount > item.NumStatic) {
                            xlWorkSheet.Cells["D" + rowCount].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                        }
                        if (item.ProcessMaxCount > item.MaxServers * 0.9) {
                            xlWorkSheet.Cells["G" + rowCount].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                        }

                        rowCount++;
                    }

                } else {

                    foreach (var item in detailView.OrderByDescending(i => i.CPUBusy)) {
                        xlWorkSheet.Cells[rowCount, 1].Value = item.ServerClass;
                        xlWorkSheet.Cells[rowCount, 2].Value = item.CPUBusy;
                        xlWorkSheet.Cells[rowCount, 3].Value = item.TcpTransaction + item.LinkmonTransaction;
                        xlWorkSheet.Cells[rowCount, 4].Value = item.TcpTransaction;
                        xlWorkSheet.Cells[rowCount, 5].Value = item.LinkmonTransaction;
                        xlWorkSheet.Cells[rowCount, 6].Value = item.ProcessCount;
                        xlWorkSheet.Cells[rowCount, 7].Value = item.ProcessMaxCount;
                        xlWorkSheet.Cells[rowCount, 8].Value = item.NumStatic;
                        xlWorkSheet.Cells[rowCount, 9].Value = item.MaxServers;
                        if (item.ProcessCount > item.NumStatic) {
                            xlWorkSheet.Cells["F" + rowCount].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                        }
                        if (item.ProcessMaxCount > item.MaxServers * 0.9) {
                            xlWorkSheet.Cells["I" + rowCount].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                        }
                        rowCount++;
                    }

                }

                if (detailView.Count == 0) {
                    //Insert No data.
                    xlWorkSheet.Cells[rowCount, 1].Value = "No data found!";

                    var noData = xlWorkSheet.Cells["A" + rowCount + ":" + endColume + rowCount];
                    noData.Merge = true;
                    noData.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    noData.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    noData.Style.Font.Color.SetColor(System.Drawing.Color.Red);
                    rowCount++;
                }

                //Change background color.
                /*for (var x = 1; x <= 6; x++) {  //6 is nu,ber of columns in this worksheet.
                    if (x % 2 == 0) {
                        xlWorkSheet.Range[ConvertNumberToChar(x) + dataStartRow,
                            ConvertNumberToChar(x) + (rowCount - 1)].Interior.ColorIndex = Helper.Color.LightGray;
                    }
                }*/

                //Border
                var border = xlWorkSheet.Cells["A" + dataStartRow + ":" + endColume + (rowCount - 1)];
                AddBorder(border);

                //CPU Busy needs a decimal point.
                var cpuFormat = xlWorkSheet.Cells["B" + dataStartRow + ":" + "B" + (rowCount - 1)];
                cpuFormat.Style.Numberformat.Format = "0.00";
                /*var avgRt = xlWorkSheet.Range[endColume + dataStartRow, endColume + (rowCount - 1)];
                cpuFormat.NumberFormat = "0.000";*/

                //Another Cell Format.
                var anotherFormat = xlWorkSheet.Cells["C" + dataStartRow + ":" + "I" + (rowCount - 1)];
                anotherFormat.Style.Numberformat.Format = "#,##0";

                //Format Conditions are for each column.
                cpuFormat.ConditionalFormatting.AddDatabar(System.Drawing.Color.RoyalBlue);
                xlWorkSheet.Cells["B" + (dataStartRow + 1) + ":" + "B" + (rowCount - 1)].ConditionalFormatting.AddDatabar(System.Drawing.Color.RoyalBlue);
                xlWorkSheet.Cells["C" + (dataStartRow + 1) + ":" + "C" + (rowCount - 1)].ConditionalFormatting.AddDatabar(System.Drawing.Color.RoyalBlue);
                xlWorkSheet.Cells["D" + (dataStartRow + 1) + ":" + "D" + (rowCount - 1)].ConditionalFormatting.AddDatabar(System.Drawing.Color.RoyalBlue);
                xlWorkSheet.Cells["E" + (dataStartRow + 1) + ":" + "E" + (rowCount - 1)].ConditionalFormatting.AddDatabar(System.Drawing.Color.RoyalBlue);
                xlWorkSheet.Cells["F" + (dataStartRow + 1) + ":" + "F" + (rowCount - 1)].ConditionalFormatting.AddDatabar(System.Drawing.Color.RoyalBlue);
                xlWorkSheet.Cells["G" + (dataStartRow + 1) + ":" + "G" + (rowCount - 1)].ConditionalFormatting.AddDatabar(System.Drawing.Color.RoyalBlue);
                xlWorkSheet.Cells["H" + (dataStartRow + 1) + ":" + "H" + (rowCount - 1)].ConditionalFormatting.AddDatabar(System.Drawing.Color.RoyalBlue);
                xlWorkSheet.Cells["I" + (dataStartRow + 1) + ":" + "I" + (rowCount - 1)].ConditionalFormatting.AddDatabar(System.Drawing.Color.RoyalBlue);

                //if (detailView.Count > 0) {
                //    if (isCPUBusy)
                //        CreateBarChart(xlWorkSheet, chartStartRow, chartLastRow, dataStartRow, (rowCount - 1), 2, 6, true, "", "% Busy");
                //    else {
                //        string buildRange = "A" + dataStartRow + ":" + "A" + (rowCount - 1) + "," + "D" + dataStartRow + ":" + "E" + (rowCount - 1);
                //        //string buildRangeData = "D" + dataStartRow + ":" + "E" + (rowCount - 1);
                //        CreateStackedBarChartCustomRange(xlWorkSheet, chartStartRow, chartLastRow, dataStartRow, (rowCount - 1), 9, buildRange, false, "", "Total Transaction Counts");
                //    }
                //}
                InsertFreezeRow(xlWorkSheet);
                InsertFreezeColumn(xlWorkSheet);
                //Change Font.
                var sheetFontRange = xlWorkSheet.Cells["A" + (dataStartRow + 1) + ":" + endColume + (rowCount + detailView.Count)];
                ChangeFontAndWidth(sheetFontRange);

                InsertNavigate(xlWorkSheet, saveLocation, previousSection, previousWorksheet, nextWorksheet, nextSection, worksheetToc);

                if (isCPUBusy)
                    InsertPathwayHelpNavigate(xlWorkSheet, saveLocation, Enums.PathwayReportTypes.ServerCpuBusy, isLocalAnalyst);
                else
                    InsertPathwayHelpNavigate(xlWorkSheet, saveLocation, Enums.PathwayReportTypes.ServerTransactions, isLocalAnalyst);

                xlWorkSheet.Name = worksheetName; //pathwayName + " - CPU Detail";
            } catch (Exception ex) {
                _log.ErrorFormat("Error: InsertWorksheetCPUBusy: {0}: {1}", worksheetName, ex);
                _reportDownloadLogs.InsertNewLog(_reportDownloadId, DateTime.Now, "There was an error generating the following worksheet: " + worksheetName);
            }

        }

        internal void InsertWorksheetUnusedClassInterval(ExcelWorkbook xlWorkBook, int worksheetCount, string list, Dictionary<string, List<ServerUnusedServerClassView>> unusedClass, string worksheetName, string saveLocation, string previousSection, string previousWorksheet, string nextWorksheet, string nextSection, string worksheetToc, string worksheetTitle, bool isLocalAnalyst, bool isTCP = false, List<DateTime> datesWithData = null) {
            try {
                if (xlWorkBook.Worksheets.Count < worksheetCount)
                    xlWorkBook.Worksheets.Add(worksheetName);
                //var xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.Item["Sheet" + Convert.ToString(worksheetCount)];
                var xlWorkSheet = (ExcelWorksheet)xlWorkBook.Worksheets[xlWorkBook.Worksheets.Count];

                var rowCount = 3;
                var columnCount = 1;

                if (isTCP)
                    columnCount = 3;
                else
                    columnCount = 1;

                //Insert Sheet Title.
                xlWorkSheet.Cells[2, 1].Value = worksheetTitle; //pathwayName + " - SERVER Transactions";
                xlWorkSheet.Cells["A2:Z2"].Merge = true;
                var title = xlWorkSheet.Cells["A2"];
                AddTitle(title);

                var dataStartRow = rowCount;
                var serverTitle = xlWorkSheet.Cells["A" + dataStartRow + ":" + ConvertNumberToChar((unusedClass.Count * columnCount) + 1) + (dataStartRow + 1)];
                serverTitle.Style.Numberformat.Format = "@";

                var hourList = unusedClass.Select((t, x) => unusedClass.Keys.ElementAt(x)).ToList();
                var column = 0;
                foreach (var hour in hourList) {
                    var dataStartrowCount = 3;
                    xlWorkSheet.Cells[rowCount, (column + 1)].Value = hour;

                    //Merge the cells.
                    xlWorkSheet.Cells[ConvertNumberToChar(column + 1) + rowCount + ":" + ConvertNumberToChar(column + columnCount) + rowCount].Merge = true;
                    dataStartrowCount++;

                    //Add Sub Title.
                    if (isTCP) {
                        xlWorkSheet.Cells[dataStartrowCount, (column + 1)].Value = "TCP";
                        xlWorkSheet.Cells[dataStartrowCount, (column + 2)].Value = "TERMs";
                        xlWorkSheet.Cells[dataStartrowCount, (column + 3)].Value = "Unused";
                    } else {
                        xlWorkSheet.Cells[dataStartrowCount, (column + 1)].Value = "SERVER Class";
                    }


                    if (unusedClass[hour].Count == 0) {
                        dataStartrowCount++;
                        //Insert No data.
                        xlWorkSheet.Cells[dataStartrowCount, (column + 1)].Value = "No data found!";

                        var noData = xlWorkSheet.Cells[ConvertNumberToChar((column + 1)) + dataStartrowCount + ":" + ConvertNumberToChar(column + columnCount) + dataStartrowCount];
                        noData.Merge = true;
                        noData.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        noData.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        noData.Style.Font.Color.SetColor(System.Drawing.Color.Red);

                        var border = xlWorkSheet.Cells[ConvertNumberToChar((column + 1)) + rowCount + ":" + ConvertNumberToChar((column + columnCount)) + dataStartrowCount];
                        if (hourList.IndexOf(hour) % 2 == 0) {
                            //border.Interior.ColorIndex = Helper.Color.LightGray;
                            border.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            border.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                        }
                        AddBorder(border);
                        ChangeFontAndWidth(border);
                        xlWorkSheet.Column(column + 1).Width = 9;
                        xlWorkSheet.Column(column + 2).Width = 9;
                        xlWorkSheet.Column(column + 3).Width = 9;
                    } else {
                        foreach (var view in unusedClass[hour]) {
                            dataStartrowCount++;
                            if (isTCP) {
                                xlWorkSheet.Cells[dataStartrowCount, (column + 1)].Value = view.ServerClass;
                                xlWorkSheet.Cells[dataStartrowCount, (column + 2)].Value = view.TermCount;
                                xlWorkSheet.Cells[dataStartrowCount, (column + 3)].Value = view.Unused;
                            } else {
                                xlWorkSheet.Cells[dataStartrowCount, (column + 1)].Value = view.ServerClass;
                            }

                        }

                        var border = xlWorkSheet.Cells[ConvertNumberToChar((column + 1)) + rowCount + ":" + ConvertNumberToChar((column + columnCount)) + dataStartrowCount];
                        if (hourList.IndexOf(hour) % 2 == 0) {
                            //border.Interior.ColorIndex = Helper.Color.LightGray;
                            border.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            border.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                        }
                        AddBorder(border);
                        ChangeFontAndWidth(border);
                    }


                    //Cell Format.
                    var formatCell = xlWorkSheet.Cells[ConvertNumberToChar((column + 1)) + rowCount + ":" + ConvertNumberToChar((column + columnCount)) + dataStartrowCount];
                    formatCell.Style.Numberformat.Format = "#,##0";

                    if (columnCount >= 2) {
                        xlWorkSheet.Cells[ConvertNumberToChar((column + 2)) + rowCount + ":" + ConvertNumberToChar((column + columnCount)) + dataStartrowCount].ConditionalFormatting.AddDatabar(System.Drawing.Color.RoyalBlue);
                    }

                    column += columnCount;
                }

                for (int i = 1; i <= xlWorkSheet.Dimension.Columns; i++) {
                    xlWorkSheet.Column(i).Width = 16;
                }

                AddHeaderNoFormat(serverTitle);
                //serverTitle.EntireColumn.AutoFit();

                //Freeze first row.InsertWorksheetTransactionTcpPerPathway
                InsertFreezeRowFirstFour(xlWorkSheet);

                InsertNavigate(xlWorkSheet, saveLocation, previousSection, previousWorksheet, nextWorksheet, nextSection, worksheetToc);

                if (isTCP)
                    InsertPathwayHelpNavigate(xlWorkSheet, saveLocation, Enums.PathwayReportTypes.TCPUnused, isLocalAnalyst);
                else
                    InsertPathwayHelpNavigate(xlWorkSheet, saveLocation, Enums.PathwayReportTypes.ServerUnusedClass, isLocalAnalyst);

                xlWorkSheet.Name = worksheetName;
            } catch (Exception ex)
            {
                _log.ErrorFormat("Error: InsertWorksheetUnusedClassInterval: {0}: {1}", worksheetName, ex);
                _reportDownloadLogs.InsertNewLog(_reportDownloadId, DateTime.Now, "There was an error generating the following worksheet: " + worksheetName);
            }
        }

        internal void InsertWorksheetUnusedProcessInterval(ExcelWorkbook xlWorkBook, int worksheetCount, string list, Dictionary<string, List<ServerUnusedServerProcessesView>> unusedClass, string worksheetName, string saveLocation, string previousSection, string previousWorksheet, string nextWorksheet, string nextSection, string worksheetToc, string worksheetTitle, bool isLocalAnalyst)
        {
            try {
                if (xlWorkBook.Worksheets.Count < worksheetCount)
                    xlWorkBook.Worksheets.Add(worksheetName);
                //var xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.Item["Sheet" + Convert.ToString(worksheetCount)];
                var xlWorkSheet = (ExcelWorksheet)xlWorkBook.Worksheets[xlWorkBook.Worksheets.Count];

                var rowCount = 3;
                //Insert Sheet Title.
                xlWorkSheet.Cells[2, 1].Value = worksheetTitle; //pathwayName + " - SERVER Transactions";
                xlWorkSheet.Cells["A2:Z2"].Merge = true;
                var title = xlWorkSheet.Cells["A2"];
                AddTitle(title);

                var dataStartRow = rowCount;
                var serverTitle = xlWorkSheet.Cells["A" + dataStartRow + ":" + ConvertNumberToChar((unusedClass.Count * 2) + 1) + (dataStartRow + 1)];
                serverTitle.Style.Numberformat.Format = "@";

                var hourList = unusedClass.Select((t, x) => unusedClass.Keys.ElementAt(x)).ToList();

                var column = 0;
                foreach (var hour in hourList) {
                    var dataStartrowCount = 3;
                    xlWorkSheet.Cells[rowCount, (column + 1)].Value = hour;

                    //Merge the cells.
                    xlWorkSheet.Cells[ConvertNumberToChar(column + 1) + rowCount + ":" + ConvertNumberToChar(column + 2) + rowCount].Merge = true;
                    dataStartrowCount++;

                    //Add Sub Title.
                    xlWorkSheet.Cells[dataStartrowCount, (column + 1)].Value = "SERVER Class";
                    xlWorkSheet.Cells[dataStartrowCount, (column + 2)].Value = "Process";
                    if (unusedClass[hour].Count == 0) {
                        dataStartrowCount++;
                        //Insert No data.
                        xlWorkSheet.Cells[dataStartrowCount, (column + 1)].Value = "No data found!";

                        var noData = xlWorkSheet.Cells[ConvertNumberToChar((column + 1)) + dataStartrowCount + ":" + ConvertNumberToChar(column + 2) + dataStartrowCount];
                        noData.Merge = true;
                        noData.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        noData.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        noData.Style.Font.Color.SetColor(System.Drawing.Color.Red);

                        var border = xlWorkSheet.Cells[ConvertNumberToChar((column + 1)) + rowCount + ":" + ConvertNumberToChar((column + 2)) + dataStartrowCount];
                        AddBorder(border);
                        ChangeFontAndWidth(border);
                        xlWorkSheet.Column(column + 1).Width = 9;
                        xlWorkSheet.Column(column + 2).Width = 9;
                    } else {
                        foreach (var view in unusedClass[hour]) {
                            dataStartrowCount++;
                            xlWorkSheet.Cells[dataStartrowCount, (column + 1)].Value = view.ServerClass;
                            xlWorkSheet.Cells[dataStartrowCount, (column + 2)].Value = view.Process;
                        }

                        var border = xlWorkSheet.Cells[ConvertNumberToChar((column + 1)) + rowCount + ":" + ConvertNumberToChar((column + 2)) + dataStartrowCount];
                        if (hourList.IndexOf(hour) % 2 == 0) {
                            //border.Interior.ColorIndex = Helper.Color.LightGray;
                            border.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            border.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                        }
                        AddBorder(border);
                        ChangeFontAndWidth(border);
                    }
                    column += 2;
                }

                AddHeaderNoFormat(serverTitle);
                serverTitle.AutoFitColumns();

                //Freeze first row.InsertWorksheetTransactionTcpPerPathway
                InsertFreezeRow(xlWorkSheet);

                InsertNavigate(xlWorkSheet, saveLocation, previousSection, previousWorksheet, nextWorksheet, nextSection, worksheetToc);


                InsertPathwayHelpNavigate(xlWorkSheet, saveLocation, Enums.PathwayReportTypes.ServerUnusedProcesses, isLocalAnalyst);

                xlWorkSheet.Name = worksheetName;
            } catch (Exception ex) {
                _log.ErrorFormat("Error: InsertWorksheetUnusedProcessInterval: {0}: {1}", worksheetName, ex);
                _reportDownloadLogs.InsertNewLog(_reportDownloadId, DateTime.Now, "There was an error generating the following worksheet: " + worksheetName);
            }
        }
    }
}
