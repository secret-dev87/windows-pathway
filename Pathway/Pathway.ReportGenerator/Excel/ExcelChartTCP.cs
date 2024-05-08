using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Pathway.Core.Infrastructure;
using Pathway.Core.Infrastructure.PerPathway.Server;
using Pathway.Core.Infrastructure.PerPathway.Tcp;
using Pathway.Core.RemoteAnalyst.Concrete;
using OfficeOpenXml;
using log4net;

namespace Pathway.ReportGenerator.Excel {
    class ExcelChartTCP : ExcelChartBase
    {
        private readonly ILog _log;
        private readonly int _reportDownloadId;
        private readonly ReportDownloadLogs _reportDownloadLogs;

        public ExcelChartTCP(int reportDownloadId, ReportDownloadLogs reportDownloadLogs, ILog log) {
            _reportDownloadId = reportDownloadId;
            _reportDownloadLogs = reportDownloadLogs;
            _log = log;
        }

        internal void InsertWorksheetTcpTransaction(ExcelWorkbook xlWorkBook, int worksheetCount, string pathwayName, List<TcpTransactionView> tcpTransactionViews, string worksheetName, string saveLocation, string previousSection, string previousWorksheet, string nextWorksheet, string nextSection, string worksheetToc, string worksheetTitle, bool isLocalAnalyst)
        {
            try {
                if (xlWorkBook.Worksheets.Count < worksheetCount)
                    xlWorkBook.Worksheets.Add(worksheetName);
                //var xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.Item["Sheet" + Convert.ToString(worksheetCount)];
                var xlWorkSheet = (ExcelWorksheet)xlWorkBook.Worksheets[xlWorkBook.Worksheets.Count];

                int rowCount = 3;
                int chartStartRow = 3;
                int chartLastRow = 19;

                if (tcpTransactionViews.Count == 0)
                    rowCount = 3;

                string endColume = "C";

                //Insert Sheet Title.
                xlWorkSheet.Cells[2, 1].Value = worksheetTitle; //pathwayName + " - CPU Detail";
                xlWorkSheet.Cells["A2:Z2"].Merge = true;
                var title = xlWorkSheet.Cells["A2"];
                AddTitle(title);

                int dataStartRow = rowCount;
                //Insert Columns.
                xlWorkSheet.Cells[rowCount, 1].Value = "TCP";
                xlWorkSheet.Cells[rowCount, 2].Value = "Transactions";
                //xlWorkSheet.Cells[rowCount, 3] = "CPU Busy";
                //xlWorkSheet.Cells[rowCount, 4] = "SERVER Trans.";
                xlWorkSheet.Cells[rowCount, 3].Value = "TERMs";
                //xlWorkSheet.Cells[rowCount, 6] = "Avg R/T";
                //Format the Header.
                ExcelRange header = xlWorkSheet.Cells["A" + rowCount + ":" + endColume + rowCount];
                AddHeaderNoFormat(header);

                rowCount++;

                foreach (var item in tcpTransactionViews.OrderByDescending(i => i.TermTranscaction)) {
                    xlWorkSheet.Cells[rowCount, 1].Value = item.Tcp;
                    xlWorkSheet.Cells[rowCount, 2].Value = item.TermTranscaction;
                    //xlWorkSheet.Cells[rowCount, 3] = item.CPUBusy;
                    //xlWorkSheet.Cells[rowCount, 4] = item.ServerTransaction;
                    xlWorkSheet.Cells[rowCount, 3].Value = item.TermCount;
                    //xlWorkSheet.Cells[rowCount, 6] = item.AverageRt;
                    rowCount++;
                }
                if (tcpTransactionViews.Count == 0) {
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
                for (var x = 1; x <= 3; x++) {  //6 is nu,ber of columns in this worksheet.
                    if (x % 2 == 0) {
                        xlWorkSheet.Cells[ConvertNumberToChar(x) + dataStartRow + ":" + ConvertNumberToChar(x) + (rowCount - 1)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        xlWorkSheet.Cells[ConvertNumberToChar(x) + dataStartRow + ":" + ConvertNumberToChar(x) + (rowCount - 1)].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    }
                }
                //Border
                var border = xlWorkSheet.Cells["A" + dataStartRow + ":" + endColume + (rowCount - 1)];
                AddBorder(border);

                //Trans Format.
                var numberFormat = xlWorkSheet.Cells["B" + dataStartRow + ":" + "C" + (rowCount - 1)];
                numberFormat.Style.Numberformat.Format = "#,##0";

                /*//CPU Busy Format.
                numberFormat = xlWorkSheet.Range["C" + dataStartRow, "C" + (rowCount - 1)];
                numberFormat.NumberFormat = "0.000";

                //Server Trans & Term Count Format.
                numberFormat = xlWorkSheet.Range["D" + dataStartRow, "E" + (rowCount - 1)];
                numberFormat.NumberFormat = "#,##0";

                //Avg R/T Format.
                numberFormat = xlWorkSheet.Range[endColume + dataStartRow, endColume + (rowCount - 1)];
                numberFormat.NumberFormat = "0.000";*/

                //Format Conditions are for each column.
                //cpuFormat.FormatConditions.AddDatabar();
                xlWorkSheet.Cells["B" + (dataStartRow + 1) + ":" + "B" + (rowCount - 1)].ConditionalFormatting.AddDatabar(System.Drawing.Color.RoyalBlue);
                xlWorkSheet.Cells["C" + (dataStartRow + 1) + ":" + "C" + (rowCount - 1)].ConditionalFormatting.AddDatabar(System.Drawing.Color.RoyalBlue);
                //xlWorkSheet.Range["D" + (dataStartRow + 1), "D" + (rowCount - 1)].FormatConditions.AddDatabar();
                //xlWorkSheet.Range["E" + (dataStartRow + 1), "E" + (rowCount - 1)].FormatConditions.AddDatabar();
                //xlWorkSheet.Range["F" + (dataStartRow + 1), "F" + (rowCount - 1)].FormatConditions.AddDatabar();

                //if (tcpTransactionViews.Count > 0) {
                //    //Get max value.
                //    var maxTransValue = tcpTransactionViews.Max(x => x.TermTranscaction);

                //    CreateBarChart(xlWorkSheet, chartStartRow, chartLastRow, dataStartRow, (rowCount - 1), 2, 6, false, "", "Total Transaction Counts", maxTransValue);
                //    //CreateStackedBarChart(xlWorkSheet, chartStartRow, chartLastRow, dataStartRow, (rowCount - 1), 6, false, pathwayName + " Pathway Top 10 TCPs of a day, based on TERM transactions", "");
                //}
                InsertFreezeRow(xlWorkSheet);

                //Change Font.
                var sheetFontRange = xlWorkSheet.Cells["A" + (dataStartRow + 1) + ":" + endColume + (rowCount + tcpTransactionViews.Count)];
                ChangeFontAndWidth(sheetFontRange);

                InsertNavigate(xlWorkSheet, saveLocation, previousSection, previousWorksheet, nextWorksheet, nextSection, worksheetToc);


                InsertPathwayHelpNavigate(xlWorkSheet, saveLocation, Enums.PathwayReportTypes.TCPTransactions, isLocalAnalyst);

                xlWorkSheet.Name = worksheetName; //pathwayName + " - CPU Detail";
            } catch (Exception ex)
            {
                _log.ErrorFormat("Error: InsertWorksheetTcpTransaction: {0}: {1}", worksheetName, ex);
                _reportDownloadLogs.InsertNewLog(_reportDownloadId, DateTime.Now, "There was an error generating the following worksheet: " + worksheetName);
            }
        }

        internal void InsertWorksheetQueues(ExcelWorkbook xlWorkBook, int worksheetCount, string pathwayName, string worksheetName, 
                                            string saveLocation, string previousSection, string previousWorksheet, string nextWorksheet, 
                                            string nextSection, string worksheetToc, string worksheetTitle, 
                                            Dictionary<string, List<TcpQueuedTransactionView>> tcpView,
                                            Dictionary<string, List<ServerQueueTcpSubView>> subDatas,
                                            Enums.IntervalTypes intervalTypes, bool isLocalAnalyst) {
            try {
                if (xlWorkBook.Worksheets.Count < worksheetCount)
                    xlWorkBook.Worksheets.Add(worksheetName);
                //var xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.Item["Sheet" + Convert.ToString(worksheetCount)];
                var xlWorkSheet = (ExcelWorksheet)xlWorkBook.Worksheets[xlWorkBook.Worksheets.Count];

                var rowCount = 3;
                var columnCount = 5;

                //Insert Sheet Title.
                xlWorkSheet.Cells[2, 1].Value = worksheetTitle; //pathwayName + " - CPU Detail";
                xlWorkSheet.Cells["A2:Z2"].Merge = true;
                var title = xlWorkSheet.Cells["A2"];
                AddTitle(title);

                var dataStartRow = 3;

                var dayLists = new List<string>();

                var dataStartrowCount = 3;
                //Insert Columns.
                dayLists = tcpView.Select((t, x) => tcpView.Keys.ElementAt(x)).ToList();
                var serverTitle = xlWorkSheet.Cells["A" + dataStartrowCount + ":" + ConvertNumberToChar((tcpView.Count * columnCount) + 1) + (dataStartRow + 1)];
                serverTitle.Style.Numberformat.Format = "@";
                AddHeaderNoFormat(serverTitle);

                /*//Format the Header.
                var header = xlWorkSheet.Range["A" + rowCount, endColume + rowCount];
                AddBorder(header);*/

                var column = 0;
                foreach (var day in dayLists) {
                    //Get ServerClass and count.
                    var serverInfo = (from x in subDatas[day]
                                      group x.TcpName by x.TcpName
                                          into g
                                      let count = g.Count()
                                      orderby g.Key
                                      select new { Value = g.Key, Count = count }).ToDictionary(x => x.Value, x => x.Count);

                    dataStartrowCount = 3;
                    xlWorkSheet.Cells[dataStartrowCount, (column + 1)].Value = day;

                    //Merge the cells.
                    xlWorkSheet.Cells[ConvertNumberToChar(column + 1) + rowCount + ":" + ConvertNumberToChar(column + columnCount) + rowCount].Merge = true;
                    dataStartrowCount++;

                    //Insert Columns.
                    xlWorkSheet.Cells[dataStartrowCount, column + 1].Value = "TCP";
                    xlWorkSheet.Cells[dataStartrowCount, column + 2].Value = "Peak Queue Length";
                    xlWorkSheet.Cells[dataStartrowCount, column + 3].Value = "Peak Time";
                    xlWorkSheet.Cells[dataStartrowCount, column + 4].Value = "Instances";
                    xlWorkSheet.Cells[dataStartrowCount, column + 5].Value = "TERMs";

                    dataStartrowCount++;
                    foreach (var item in tcpView[day]) {
                        //Calcuate the row number on the detail page.
                        var tempDetailCount = 0;
                        for (var i = 0; i < serverInfo.Count; i++) {
                            var tempValue = serverInfo.Keys.ElementAt(i);
                            if (!tempValue.Equals(item.Tcp))
                                tempDetailCount += serverInfo[tempValue];
                            else {
                                tempDetailCount += serverInfo[tempValue];
                                break;
                            }
                        }

                        xlWorkSheet.Cells[dataStartrowCount, column + 1].Value = item.Tcp;
                        xlWorkSheet.Cells[dataStartrowCount, column + 2].Value = item.PeakQueueLength;
                        xlWorkSheet.Cells[dataStartrowCount, column + 3].Value = item.PeakTime;
                        xlWorkSheet.Cells[dataStartrowCount, column + 4].Value = item.Instances;
                        if (tempDetailCount > 0) {
                            string linkName = "";
                            if (Enums.IntervalTypes.Daily == intervalTypes)
                                linkName = Convert.ToDateTime(day).ToString("yy-MM-dd");
                            else
                                linkName = Convert.ToDateTime(day).ToString("HHmm");
                            //Add hyper link.
                            xlWorkSheet.Cells[dataStartrowCount, column + 4].Hyperlink = new Uri("#'" + pathwayName + " TCP Que Ins. " + linkName + "'!A" + (tempDetailCount + 3), UriKind.Relative);

                        }
                        xlWorkSheet.Cells[dataStartrowCount, column + 5].Value = item.TermCount;
                        dataStartrowCount++;
                    }

                    if (tcpView[day].Count == 0) {
                        //Insert No data.
                        xlWorkSheet.Cells[dataStartrowCount, column + 1].Value = "No data found!";

                        var noData = xlWorkSheet.Cells[ConvertNumberToChar(column + 1) + dataStartrowCount + ":" + ConvertNumberToChar(column + columnCount) + dataStartrowCount];
                        noData.Merge = true;
                        noData.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        noData.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        noData.Style.Font.Color.SetColor(System.Drawing.Color.Red);
                        dataStartrowCount++;
                    }

                    //Border
                    var border = xlWorkSheet.Cells[ConvertNumberToChar(column + 1) + dataStartRow + ":" + ConvertNumberToChar(column + columnCount) + (dataStartrowCount - 1)];
                    AddBorder(border);
                    if (dayLists.IndexOf(day) % 2 == 0) {
                        //border.Interior.ColorIndex = Helper.Color.LightGray;
                        border.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        border.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    }


                    //Change Font.
                    var sheetFontRange = xlWorkSheet.Cells[ConvertNumberToChar(column + 1) + dataStartRow + ":" + ConvertNumberToChar(column + columnCount) + (dataStartrowCount - 1)];
                    ChangeFontAndWidth(sheetFontRange);

                    //Cell Format.
                    var formatCell = xlWorkSheet.Cells[ConvertNumberToChar(column + 2) + dataStartRow + ":" + ConvertNumberToChar(column + 2) + (dataStartrowCount - 1)];
                    formatCell.Style.Numberformat.Format = "#,##0";
                    formatCell.ConditionalFormatting.AddDatabar(System.Drawing.Color.RoyalBlue);

                    formatCell = xlWorkSheet.Cells[ConvertNumberToChar(column + 3) + dataStartRow + ":" + ConvertNumberToChar(column + 3) + (dataStartrowCount - 1)];
                    formatCell.Style.Numberformat.Format = "hh:mm";

                    formatCell = xlWorkSheet.Cells[ConvertNumberToChar(column + 4) + dataStartRow + ":" + ConvertNumberToChar(column + 5) + (dataStartrowCount - 1)];
                    formatCell.Style.Numberformat.Format = "#,##0";
                    formatCell.ConditionalFormatting.AddDatabar(System.Drawing.Color.RoyalBlue);

                    column += columnCount;
                }

                InsertFreezeRowFirstFour(xlWorkSheet);


                InsertNavigate(xlWorkSheet, saveLocation, previousSection, previousWorksheet, nextWorksheet, nextSection, worksheetToc);
                //InsertAlertHelpNavigate(xlWorkSheet, saveLocation, Enums.AlertReportTypes.QueueTCP);

                InsertPathwayHelpNavigate(xlWorkSheet, saveLocation, Enums.PathwayReportTypes.TCPQueues, isLocalAnalyst);
                xlWorkSheet.Name = worksheetName;
            } catch (Exception ex) {
                _log.ErrorFormat("Error: InsertWorksheetQueues: {0}: {1}", worksheetName, ex);
                _reportDownloadLogs.InsertNewLog(_reportDownloadId, DateTime.Now, "There was an error generating the following worksheet: " + worksheetName);
            }
        }

        internal void InsertWorksheetQueueSubs(ExcelWorkbook xlWorkBook, int worksheetCount, string pathwayName, string worksheetName, string saveLocation, string previousSection, string previousWorksheet, string nextWorksheet, string nextSection, string worksheetToc, string worksheetTitle, List<ServerQueueTcpSubView> tcpView, bool isLocalAnalyst) {
            try {
                if (xlWorkBook.Worksheets.Count < worksheetCount)
                    xlWorkBook.Worksheets.Add(worksheetName);
                //var xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.Item["Sheet" + Convert.ToString(worksheetCount)];
                var xlWorkSheet = (ExcelWorksheet)xlWorkBook.Worksheets[xlWorkBook.Worksheets.Count];

                var rowCount = 3;
                var columnCount = 4;
                //Insert Sheet Title.
                xlWorkSheet.Cells[2, 1].Value = worksheetTitle;
                xlWorkSheet.Cells["A2:Z2"].Merge = true;
                var title = xlWorkSheet.Cells["A2"];
                AddTitle(title);

                var column = 0;
                var dataStartrowCount = 3;

                //Add Sub Title.
                xlWorkSheet.Cells[dataStartrowCount, (column + 1)].Value = "Server Class";
                xlWorkSheet.Cells[dataStartrowCount, (column + 2)].Value = "Time";
                xlWorkSheet.Cells[dataStartrowCount, (column + 3)].Value = "Queue Length";
                xlWorkSheet.Cells[dataStartrowCount, (column + 4)].Value = "Queue Wait";

                var serverTitle = xlWorkSheet.Cells["A" + dataStartrowCount + ":" + "D" + dataStartrowCount];
                AddHeaderNoFormat(serverTitle);

                if (tcpView.Count == 0) {
                    dataStartrowCount++;
                    //Insert No data.
                    xlWorkSheet.Cells[dataStartrowCount, (column + 1)].Value = "No data found!";

                    var noData = xlWorkSheet.Cells[ConvertNumberToChar((column + 1)) + dataStartrowCount + ":" + ConvertNumberToChar(column + columnCount) + dataStartrowCount];
                    noData.Merge = true;
                    noData.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    noData.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    noData.Style.Font.Color.SetColor(System.Drawing.Color.Red);

                    var border = xlWorkSheet.Cells[ConvertNumberToChar((column + 1)) + rowCount + ":" + ConvertNumberToChar((column + columnCount)) + dataStartrowCount];
                    AddBorder(border);
                    ChangeFontAndWidth(border);
                    xlWorkSheet.Column(column + 1).Width = 9;
                    xlWorkSheet.Column(column + 2).Width = 9;
                    xlWorkSheet.Column(column + 3).Width = 9;
                    xlWorkSheet.Column(column + 4).Width = 9;
                } else {
                    var range = xlWorkSheet.Cells["A" + (dataStartrowCount + 1)];
                    long rangeCount = tcpView.Count;
                    range = xlWorkSheet.Cells[Convert.ToInt32(rangeCount), columnCount];
                    var saRet = new object[rangeCount, columnCount];

                    var tempTcp = "";
                    var dataRowInfo = new Dictionary<int, int>();
                    var tempStartRow = 0;
                    for (var x = 0; x < rangeCount; x++) {
                        dataStartrowCount++;
                        if (tempTcp.Length == 0) {
                            tempTcp = tcpView[x].TcpName;
                            saRet[x, 0] = tempTcp;
                            tempStartRow = x;
                        } else if (tempTcp.Equals(tcpView[x].TcpName)) {
                            saRet[x, 0] = "";
                        } else {
                            tempTcp = tcpView[x].TcpName;
                            saRet[x, 0] = tempTcp;
                            dataRowInfo.Add(tempStartRow, (x - 1));
                            tempStartRow = x;
                        }

                        saRet[x, 1] = tcpView[x].Time;
                        saRet[x, 2] = tcpView[x].RequestCount;
                        saRet[x, 3] = tcpView[x].PercentWait;
                    }
                    dataRowInfo.Add(tempStartRow, ((int)rangeCount - 1));

                    range.Value = saRet;

                    //Base on the dataRowInfo, change the back ground color.
                    var i = 0;
                    foreach (var rows in dataRowInfo) {
                        var subRange = xlWorkSheet.Cells["A" + (rows.Key + (rowCount + 1)) + ":" + "A" + (rows.Value + (rowCount + 1))];
                        subRange.Merge = true;
                        subRange.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        subRange.Style.Font.Bold = true;

                        if (i % 2 == 0) {
                            subRange = xlWorkSheet.Cells["A" + (rows.Key + (rowCount + 1)) + ":" + ConvertNumberToChar(columnCount) + (rows.Value + (rowCount + 1))];
                            //subRange.Interior.ColorIndex = Helper.Color.LightGray;
                            subRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            subRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                        }
                        i++;
                    }

                    var border = xlWorkSheet.Cells[ConvertNumberToChar((column + 1)) + rowCount + ":" + ConvertNumberToChar((column + columnCount)) + dataStartrowCount];
                    AddBorder(border);

                    //DateTime format.
                    border = xlWorkSheet.Cells["B" + (rowCount + 1) + ":" + "B" + dataStartrowCount];
                    border.Style.Numberformat.Format = "hh:mm";

                    //Number format.
                    border = xlWorkSheet.Cells["C" + (rowCount + 1) + ":" + "D" + dataStartrowCount];
                    border.Style.Numberformat.Format = "#,##0";

                    border = xlWorkSheet.Cells[ConvertNumberToChar((column + 1)) + (rowCount + 1) + ":" + ConvertNumberToChar((column + columnCount)) + dataStartrowCount];
                    ChangeFontAndWidth(border);
                }
                serverTitle.AutoFitColumns();

                //Freeze first row.InsertWorksheetTransactionTcpPerPathway
                InsertFreezeRowFirstThree(xlWorkSheet);

                InsertNavigate(xlWorkSheet, saveLocation, previousSection, previousWorksheet, nextWorksheet, nextSection, worksheetToc);


                InsertPathwayHelpNavigate(xlWorkSheet, saveLocation, Enums.PathwayReportTypes.TCPQueues, isLocalAnalyst);

                xlWorkSheet.Name = worksheetName;
            } catch (Exception ex) {
                _log.ErrorFormat("Error: InsertWorksheetQueueSubs: {0}: {1}", worksheetName, ex);
                _reportDownloadLogs.InsertNewLog(_reportDownloadId, DateTime.Now, "There was an error generating the following worksheet: " + worksheetName);
            }
        }
    }
}
