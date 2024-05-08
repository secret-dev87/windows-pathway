using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Pathway.Core.Infrastructure;
using Pathway.Core.Infrastructure.PerPathway.Server;
using Pathway.ReportGenerator.Infrastructure;
using Pathway.Core.RemoteAnalyst.Concrete;
using OfficeOpenXml;
using System.Data;
using log4net;

namespace Pathway.ReportGenerator.Excel {
    class ExcelChartAlert : ExcelChartBase {
        private readonly ILog _log;
        private readonly int _reportDownloadId;
        private readonly ReportDownloadLogs _reportDownloadLogs;

        public ExcelChartAlert(int reportDownloadId, ReportDownloadLogs reportDownloadLogs, ILog log) {
            _reportDownloadId = reportDownloadId;
            _reportDownloadLogs = reportDownloadLogs;
            _log = log;
        }

        internal void InsertWorksheetAlert(ExcelWorkbook xlWorkBook, string saveLocation, int worksheetCount, Dictionary<string, AlertView> alertView, string worksheetName, string worksheetTitle, Dictionary<string, Collection> worksheetNames, bool isLocalAnalyst) {
            try {
                if (xlWorkBook.Worksheets.Count < worksheetCount)
                    xlWorkBook.Worksheets.Add(worksheetName);

                //var xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.Item["Sheet" + Convert.ToString(worksheetCount)];
                var xlWorkSheet = (ExcelWorksheet)xlWorkBook.Worksheets[xlWorkBook.Worksheets.Count];

                var rowCount = 3;
                InsertAlertHelpNavigate(xlWorkSheet, saveLocation, Enums.AlertReportTypes.AlertCollection, isLocalAnalyst);

                //Insert Sheet Title.
                xlWorkSheet.Cells[2, 1].Value = worksheetTitle;
                xlWorkSheet.Cells["A2:Z2"].Merge = true;
                var title = xlWorkSheet.Cells["A2"];
                AddTitle(title);

                //Insert Alert Titles
                xlWorkSheet.Cells[rowCount + 1, 1].Value = "SERVER";
                xlWorkSheet.Cells[rowCount + 2, 1].Value = "Pending - Class";
                xlWorkSheet.Cells[rowCount + 3, 1].Value = "Pending - Process";
                xlWorkSheet.Cells[rowCount + 4, 1].Value = "Queue - TCP";
                xlWorkSheet.Cells[rowCount + 5, 1].Value = "Queue - Linkmon";
                xlWorkSheet.Cells[rowCount + 6, 1].Value = "Unused - Class";
                xlWorkSheet.Cells[rowCount + 7, 1].Value = "Unused - Process";
                xlWorkSheet.Cells[rowCount + 8, 1].Value = "High MaxLinks";
                xlWorkSheet.Cells[rowCount + 9, 1].Value = "Check Directory On";
                xlWorkSheet.Cells[rowCount + 10, 1].Value = "High Dynamic Servers";
                xlWorkSheet.Cells[rowCount + 11, 1].Value = "Error List";

                //Insert Data
                for (var x = 0; x < alertView.Count; x++) {
                    var view = alertView.Keys.ElementAt(x);
                    var heading = xlWorkSheet.Cells[ConvertNumberToChar((x + 1) + 1) + rowCount];
                    heading.Style.Font.Size = 10;
                    heading.Style.Font.Bold = true;
                    heading.Style.Font.Name = "Courier New";
                    heading.Style.Numberformat.Format = "@";
                    xlWorkSheet.Cells[rowCount, (x + 1) + 1].Value = view;

                    xlWorkSheet.Cells[rowCount + 2, (x + 1) + 1].Value = alertView[view].ServerPendingClass;
                    if (alertView[view].ServerPendingClass > 0) {
                        xlWorkSheet.Cells[rowCount + 2, (x + 1) + 1].Hyperlink = new Uri("#'" + worksheetNames[view].ServerPendingClass + "'!A3", UriKind.Relative);
                    }
                    xlWorkSheet.Cells[rowCount + 3, (x + 1) + 1].Value = alertView[view].ServerPendingProcess;
                    if (alertView[view].ServerPendingProcess > 0) {
                        xlWorkSheet.Cells[rowCount + 3, (x + 1) + 1].Hyperlink = new Uri("#'" + worksheetNames[view].ServerPendingProcess + "'!A3", UriKind.Relative);
                    }
                    xlWorkSheet.Cells[rowCount + 4, (x + 1) + 1].Value = alertView[view].ServerQueueTcp;
                    if (alertView[view].ServerQueueTcp > 0) {
                        xlWorkSheet.Cells[rowCount + 4, (x + 1) + 1].Hyperlink = new Uri("#'" + worksheetNames[view].ServerQueuedTCP + "'!A3", UriKind.Relative);
                    }
                    xlWorkSheet.Cells[rowCount + 5, (x + 1) + 1].Value = alertView[view].ServerQueueLinkmon;
                    if (alertView[view].ServerQueueLinkmon > 0) {
                        xlWorkSheet.Cells[rowCount + 5, (x + 1) + 1].Hyperlink = new Uri("#'" + worksheetNames[view].ServerQueuedLinkmon + "'!A3", UriKind.Relative);
                    }
                    xlWorkSheet.Cells[rowCount + 6, (x + 1) + 1].Value = alertView[view].ServerUnusedClass;
                    if (alertView[view].ServerUnusedClass > 0) {
                        xlWorkSheet.Cells[rowCount + 6, (x + 1) + 1].Hyperlink = new Uri("#'" + worksheetNames[view].ServerUnusedServerClasses + "'!A3", UriKind.Relative);
                    }
                    xlWorkSheet.Cells[rowCount + 7, (x + 1) + 1].Value = alertView[view].ServerUnusedProcess;
                    if (alertView[view].ServerUnusedProcess > 0) {
                        xlWorkSheet.Cells[rowCount + 7, (x + 1) + 1].Hyperlink = new Uri("#'" + worksheetNames[view].ServerUnusedServerProcesses + "'!A3", UriKind.Relative);
                    }
                    xlWorkSheet.Cells[rowCount + 8, (x + 1) + 1].Value = alertView[view].ServerMaxLinks;
                    if (alertView[view].ServerMaxLinks > 0) {
                        xlWorkSheet.Cells[rowCount + 8, (x + 1) + 1].Hyperlink = new Uri("#'" + worksheetNames[view].ServerMaxLinks + "'!A3", UriKind.Relative);
                    }
                    xlWorkSheet.Cells[rowCount + 9, (x + 1) + 1].Value = alertView[view].DirectoryOnLinks;
                    if (alertView[view].DirectoryOnLinks > 0) {
                        xlWorkSheet.Cells[rowCount + 9, (x + 1) + 1].Hyperlink = new Uri("#'" + worksheetNames[view].CheckDirectoryOnLinks + "'!A3", UriKind.Relative);
                    }
                    xlWorkSheet.Cells[rowCount + 10, (x + 1) + 1].Value = alertView[view].HighDynamicServers;
                    if (alertView[view].HighDynamicServers > 0) {
                        xlWorkSheet.Cells[rowCount + 10, (x + 1) + 1].Hyperlink = new Uri("#'" + worksheetNames[view].HighDynamicServers + "'!A3", UriKind.Relative);
                    }
                    xlWorkSheet.Cells[rowCount + 11, (x + 1) + 1].Value = alertView[view].ServerErrorList;
                    if (alertView[view].ServerErrorList > 0) {
                        xlWorkSheet.Cells[rowCount + 11, (x + 1) + 1].Hyperlink = new Uri("#'" + worksheetNames[view].ServerErrorList + "'!A3", UriKind.Relative);
                    }

                    //Add backgroud.
                    if (x % 2 == 0) {
                        xlWorkSheet.Cells[ConvertNumberToChar((x + 1) + 1) + rowCount + ":" +
                                          ConvertNumberToChar((x + 1) + 1) + (rowCount + 11)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        xlWorkSheet.Cells[ConvertNumberToChar((x + 1) + 1) + rowCount + ":" +
                                          ConvertNumberToChar((x + 1) + 1) + (rowCount + 11)].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    }
                }

                //Freeze first row.
                InsertFreezeRow(xlWorkSheet);

                //Format the Header.
                ExcelRange header;
                if (alertView.Count > 1) {
                    header = xlWorkSheet.Cells["B" + rowCount + ":" + ConvertNumberToChar(alertView.Count + 1) + rowCount];
                    //BUG: if single cell is selected, it changes entire cell's background.
                    //header.AutoFormat(XlRangeAutoFormat.xlRangeAutoFormat3DEffects1, true, false, true, true, true, true);
                } else {
                    header = xlWorkSheet.Cells["B" + rowCount];
                }
                AddHeaderNoFormat(header);
                AddBorder(header);

                //Border TERM
                var border = xlWorkSheet.Cells["A" + (rowCount + 2) + ":" + ConvertNumberToChar(alertView.Count + 1) + (rowCount + 11)];
                AddBorder(border);
                border.Style.Numberformat.Format = "#,##0";
                border.ConditionalFormatting.AddDatabar(System.Drawing.Color.RoyalBlue);

                //Change Font.
                var sheetFontRange = xlWorkSheet.Cells["A4:Z" + (rowCount + 20)];
                ChangeFontAndWidth(sheetFontRange);

                //Format Sub Categoty.
                var sub = xlWorkSheet.Cells["A" + (rowCount + 1) + ":" + ConvertNumberToChar(alertView.Count + 1) + (rowCount + 1)];
                AddAlertSubTitle(sub);

                sub = xlWorkSheet.Cells["A" + (rowCount + 12) + ":" + ConvertNumberToChar(alertView.Count + 1) + (rowCount + 12)];
                AddAlertSubTitle(sub);

                xlWorkSheet.Name = worksheetName.Length > 31 ? worksheetName.Substring(0, 31) : worksheetName;
            } catch (Exception ex) {
                _log.ErrorFormat("Error: InsertWorksheetAlert: {0}", ex);
                _reportDownloadLogs.InsertNewLog(_reportDownloadId, DateTime.Now, "There was an error generating the following worksheet: " + worksheetName);
            }
        }

        internal void InsertWorksheetUnusedProcessInterval(ExcelWorkbook xlWorkBook, int worksheetCount, string list, Dictionary<string, List<UnusedServerProcesses>> unusedClass, string worksheetName, string saveLocation, string previousSection, string previousWorksheet, string nextWorksheet, string nextSection, string worksheetToc, string worksheetTitle, bool isLocalAnalyst) {
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
                var serverTitle = xlWorkSheet.Cells["A" + dataStartRow + ":" + ConvertNumberToChar((unusedClass.Count * 4) + 1) + (dataStartRow + 1)];
                serverTitle.Style.Numberformat.Format = "@";

                var hourList = unusedClass.Select((t, x) => unusedClass.Keys.ElementAt(x)).ToList();

                var column = 0;
                foreach (var hour in hourList) {
                    var dataStartrowCount = 3;
                    xlWorkSheet.Cells[rowCount, (column + 1)].Value = hour;

                    //Merge the cells.
                    xlWorkSheet.Cells[ConvertNumberToChar(column + 1) + rowCount + ":" + ConvertNumberToChar(column + 4) + rowCount].Merge = true;
                    dataStartrowCount++;

                    //Add Sub Title.
                    xlWorkSheet.Cells[dataStartrowCount, (column + 1)].Value = "SERVER Class";
                    xlWorkSheet.Cells[dataStartrowCount, (column + 2)].Value = "Process";
                    xlWorkSheet.Cells[dataStartrowCount, (column + 3)].Value = "MAXSERVERS";
                    xlWorkSheet.Cells[dataStartrowCount, (column + 4)].Value = "NUMSTATIC";
                    if (unusedClass[hour].Count == 0) {
                        dataStartrowCount++;
                        //Insert No data.
                        xlWorkSheet.Cells[dataStartrowCount, (column + 1)].Value = "No data found!";

                        var noData = xlWorkSheet.Cells[ConvertNumberToChar((column + 1)) + dataStartrowCount + ":" + ConvertNumberToChar(column + 4) + dataStartrowCount];
                        noData.Merge = true;
                        noData.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        noData.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        noData.Style.Font.Color.SetColor(System.Drawing.Color.Red);

                        var border = xlWorkSheet.Cells[ConvertNumberToChar((column + 1)) + rowCount + ":" + ConvertNumberToChar((column + 4)) + dataStartrowCount];
                        AddBorder(border);
                        ChangeFontAndWidth(border);
                        xlWorkSheet.Column(column + 1).Width = 9;
                        xlWorkSheet.Column(column + 2).Width = 9;
                        xlWorkSheet.Column(column + 3).Width = 9;
                        xlWorkSheet.Column(column + 4).Width = 9;
                    } else {
                        foreach (var view in unusedClass[hour]) {
                            dataStartrowCount++;
                            xlWorkSheet.Cells[dataStartrowCount, (column + 1)].Value = view.ServerClass;
                            xlWorkSheet.Cells[dataStartrowCount, (column + 2)].Value = view.Process;
                            xlWorkSheet.Cells[dataStartrowCount, (column + 3)].Value = view.MaxServers;
                            xlWorkSheet.Cells[dataStartrowCount, (column + 4)].Value = view.NumStatic;
                        }

                        var border = xlWorkSheet.Cells[ConvertNumberToChar((column + 1)) + rowCount + ":" + ConvertNumberToChar((column + 4)) + dataStartrowCount];
                        if (hourList.IndexOf(hour) % 2 == 0) {
                            border.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            border.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                        }
                        AddBorder(border);
                        ChangeFontAndWidth(border);
                    }
                    column += 4;
                }

                AddHeaderNoFormat(serverTitle);
                serverTitle.AutoFitColumns();

                //Freeze first row.InsertWorksheetTransactionTcpPerPathway
                InsertFreezeRow(xlWorkSheet);

                InsertNavigate(xlWorkSheet, saveLocation, previousSection, previousWorksheet, nextWorksheet, nextSection, worksheetToc);
                InsertAlertHelpNavigate(xlWorkSheet, saveLocation, Enums.AlertReportTypes.UnusedProcess, isLocalAnalyst);

                xlWorkSheet.Name = worksheetName.Length > 31 ? worksheetName.Substring(0, 31) : worksheetName;
            } catch (Exception ex) {
                _log.ErrorFormat("Error: InsertWorksheetUnusedProcessInterval: {0}", ex);
                _reportDownloadLogs.InsertNewLog(_reportDownloadId, DateTime.Now, "There was an error generating the following worksheet: " + worksheetName);
            }
        }

        internal void InsertWorksheetServerMaxLinksInterval(ExcelWorkbook xlWorkBook, int worksheetCount, string list, Dictionary<string, List<ServerMaxLinks>> serverMaxLinks, string worksheetName, string saveLocation, string previousSection, string previousWorksheet, string nextWorksheet, string nextSection, string worksheetToc, string worksheetTitle, bool isLocalAnalyst)
        {
            try {
                if (xlWorkBook.Worksheets.Count < worksheetCount) xlWorkBook.Worksheets.Add(worksheetName);
                var xlWorkSheet = (ExcelWorksheet)xlWorkBook.Worksheets[xlWorkBook.Worksheets.Count];

                var rowCount = 3;
                //Insert Sheet Title.
                xlWorkSheet.Cells[2, 1].Value = worksheetTitle; //pathwayName + " - SERVER Transactions";
                xlWorkSheet.Cells["A2:Z2"].Merge = true;
                var title = xlWorkSheet.Cells["A2"];
                AddTitle(title);

                var dataStartRow = rowCount;
                var serverTitle = xlWorkSheet.Cells["A" + dataStartRow + ":" + ConvertNumberToChar((serverMaxLinks.Count * 4) + 1) + (dataStartRow + 1)];
                serverTitle.Style.Numberformat.Format = "@";

                var hourList = serverMaxLinks.Select((t, x) => serverMaxLinks.Keys.ElementAt(x)).ToList();
                var column = 0;
                foreach (var hour in hourList) {
                    var dataStartrowCount = 3;
                    xlWorkSheet.Cells[rowCount, (column + 1)].Value = hour;

                    //Merge the cells.
                    xlWorkSheet.Cells[ConvertNumberToChar(column + 1) + rowCount + ":" + ConvertNumberToChar(column + 3) + rowCount].Merge = true;
                    dataStartrowCount++;

                    //Add Sub Title.
                    xlWorkSheet.Cells[dataStartrowCount, (column + 1)].Value = "SERVER Class";
                    xlWorkSheet.Cells[dataStartrowCount, (column + 2)].Value = "Links Used";
                    xlWorkSheet.Cells[dataStartrowCount, (column + 3)].Value = "Max Links";
                    if (serverMaxLinks[hour].Count == 0) {
                        dataStartrowCount++;
                        //Insert No data.
                        xlWorkSheet.Cells[dataStartrowCount, (column + 1)].Value = "No data found!";

                        var noData = xlWorkSheet.Cells[ConvertNumberToChar((column + 1)) + dataStartrowCount + ":" + ConvertNumberToChar(column + 3) + dataStartrowCount];
                        noData.Merge = true;
                        noData.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        noData.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        noData.Style.Font.Color.SetColor(System.Drawing.Color.Red);

                        var border = xlWorkSheet.Cells[ConvertNumberToChar((column + 1)) + rowCount + ":" + ConvertNumberToChar((column + 3)) + dataStartrowCount];
                        AddBorder(border);
                        ChangeFontAndWidth(border);
                        xlWorkSheet.Column(column + 1).Width = 9;
                        xlWorkSheet.Column(column + 2).Width = 9;
                        xlWorkSheet.Column(column + 3).Width = 9;
                    } else {
                        foreach (var view in serverMaxLinks[hour]) {
                            dataStartrowCount++;
                            xlWorkSheet.Cells[dataStartrowCount, (column + 1)].Value = view.ServerClass;
                            xlWorkSheet.Cells[dataStartrowCount, (column + 2)].Value = view.LinksUsed;
                            xlWorkSheet.Cells[dataStartrowCount, (column + 3)].Value = view.MaxLinks;
                        }

                        var border = xlWorkSheet.Cells[ConvertNumberToChar((column + 1)) + rowCount + ":" + ConvertNumberToChar((column + 3)) + dataStartrowCount];
                        if (hourList.IndexOf(hour) % 2 == 0) {
                            //border.Interior.ColorIndex = Helper.Color.LightGray;
                            border.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            border.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                        }
                        AddBorder(border);
                        ChangeFontAndWidth(border);
                    }
                    //INCREASE "column" BY NUMBER OF COLUMNS IN REPORT
                    column += 3;
                }

                AddHeaderNoFormat(serverTitle);
                serverTitle.AutoFitColumns();

                //Freeze first row.InsertWorksheetTransactionTcpPerPathway
                InsertFreezeRow(xlWorkSheet);

                InsertNavigate(xlWorkSheet, saveLocation, previousSection, previousWorksheet, nextWorksheet, nextSection, worksheetToc);
                InsertAlertHelpNavigate(xlWorkSheet, saveLocation, Enums.AlertReportTypes.HighMaxLinks, isLocalAnalyst);

                xlWorkSheet.Name = worksheetName.Length > 31 ? worksheetName.Substring(0, 31) : worksheetName;
            } catch (Exception ex) {
                _log.ErrorFormat("Error: InsertWorksheetServerMaxLinksInterval: {0}", ex);
                _reportDownloadLogs.InsertNewLog(_reportDownloadId, DateTime.Now, "There was an error generating the following worksheet: " + worksheetName);
            }

        }

        internal void InsertWorksheetCheckDirectoryOnDetail(ExcelWorkbook xlWorkBook, int worksheetCount, string list, Dictionary<string, List<CheckDirectoryON>> serverMaxLinks, string worksheetName, string saveLocation, string previousSection, string previousWorksheet, string nextWorksheet, string nextSection, string worksheetToc, string worksheetTitle, bool isLocalAnalyst)
        {
            try {
                if (xlWorkBook.Worksheets.Count < worksheetCount) xlWorkBook.Worksheets.Add(worksheetName);
                var xlWorkSheet = (ExcelWorksheet)xlWorkBook.Worksheets[xlWorkBook.Worksheets.Count];

                var rowCount = 3;
                //Insert Sheet Title.
                xlWorkSheet.Cells[2, 1].Value = worksheetTitle; //pathwayName + " - SERVER Transactions";
                xlWorkSheet.Cells["A2:Z2"].Merge = true;
                var title = xlWorkSheet.Cells["A2"];
                AddTitle(title);

                var dataStartRow = rowCount;
                var serverTitle = xlWorkSheet.Cells["A" + dataStartRow + ":" + ConvertNumberToChar((serverMaxLinks.Count * 4) + 1) + (dataStartRow + 1)];
                serverTitle.Style.Numberformat.Format = "@";

                var hourList = serverMaxLinks.Select((t, x) => serverMaxLinks.Keys.ElementAt(x)).ToList();
                var column = 0;
                foreach (var hour in hourList) {
                    var dataStartrowCount = 3;
                    xlWorkSheet.Cells[rowCount, (column + 1)].Value = hour;

                    //Merge the cells.
                    xlWorkSheet.Cells[ConvertNumberToChar(column + 1) + rowCount + ":" + ConvertNumberToChar(column + 1) + rowCount].Merge = true;
                    dataStartrowCount++;

                    //Add Sub Title.
                    xlWorkSheet.Cells[dataStartrowCount, (column + 1)].Value = "TCP";
                    if (serverMaxLinks[hour].Count == 0) {
                        dataStartrowCount++;
                        //Insert No data.
                        xlWorkSheet.Cells[dataStartrowCount, (column + 1)].Value = "No data found!";

                        var noData = xlWorkSheet.Cells[ConvertNumberToChar((column + 1)) + dataStartrowCount + ":" + ConvertNumberToChar(column + 1) + dataStartrowCount];
                        noData.Merge = true;
                        noData.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        noData.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        noData.Style.Font.Color.SetColor(System.Drawing.Color.Red);

                        var border = xlWorkSheet.Cells[ConvertNumberToChar((column + 1)) + rowCount + ":" + ConvertNumberToChar((column + 1)) + dataStartrowCount];
                        AddBorder(border);
                        ChangeFontAndWidth(border);
                        //xlWorkSheet.Column(column + 1).Width = 9;
                    } else {
                        foreach (var view in serverMaxLinks[hour]) {
                            dataStartrowCount++;
                            xlWorkSheet.Cells[dataStartrowCount, (column + 1)].Value = view.ServerClass;
                            xlWorkSheet.Cells[dataStartrowCount, (column + 1)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        }

                        var border = xlWorkSheet.Cells[ConvertNumberToChar((column + 1)) + rowCount + ":" + ConvertNumberToChar((column + 1)) + dataStartrowCount];
                        if (hourList.IndexOf(hour) % 2 == 0) {
                            //border.Interior.ColorIndex = Helper.Color.LightGray;
                            border.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            border.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                        }
                        AddBorder(border);
                        ChangeFontAndWidth(border);
                        xlWorkSheet.Column(column + 1).Width = 17;
                    }
                    //INCREASE "column" BY NUMBER OF COLUMNS IN REPORT
                    column += 1;
                }

                AddHeaderNoFormat(serverTitle);
                //serverTitle.AutoFitColumns();
                //xlWorkSheet.Cells.AutoFitColumns();

                //Freeze first row.InsertWorksheetTransactionTcpPerPathway
                InsertFreezeRow(xlWorkSheet);

                InsertNavigate(xlWorkSheet, saveLocation, previousSection, previousWorksheet, nextWorksheet, nextSection, worksheetToc);
                InsertAlertHelpNavigate(xlWorkSheet, saveLocation, Enums.AlertReportTypes.CheckDirectoryOn, isLocalAnalyst);

                xlWorkSheet.Name = worksheetName.Length > 31 ? worksheetName.Substring(0, 31) : worksheetName;
            } catch (Exception ex) {
                _log.ErrorFormat("Error: InsertWorksheetCheckDirectoryOnDetail: {0}", ex);
                _reportDownloadLogs.InsertNewLog(_reportDownloadId, DateTime.Now, "There was an error generating the following worksheet: " + worksheetName);
            }
        }

        internal void InsertWorksheetHighDynamicServersDetail(ExcelWorkbook xlWorkBook, int worksheetCount, string list, Dictionary<string, List<HighDynamicServers>> highDynamicServers, string worksheetName, string saveLocation, string previousSection, string previousWorksheet, string nextWorksheet, string nextSection, string worksheetToc, string worksheetTitle, bool isLocalAnalyst)
        {
            try {
                if (xlWorkBook.Worksheets.Count < worksheetCount) xlWorkBook.Worksheets.Add(worksheetName);
                var xlWorkSheet = (ExcelWorksheet)xlWorkBook.Worksheets[xlWorkBook.Worksheets.Count];

                var rowCount = 3;
                //Insert Sheet Title.
                xlWorkSheet.Cells[2, 1].Value = worksheetTitle; //pathwayName + " - SERVER Transactions";
                xlWorkSheet.Cells["A2:Z2"].Merge = true;
                var title = xlWorkSheet.Cells["A2"];
                AddTitle(title);

                var dataStartRow = rowCount;
                var serverTitle = xlWorkSheet.Cells["A" + dataStartRow + ":" + ConvertNumberToChar((highDynamicServers.Count * 4) + 1) + (dataStartRow + 1)];
                serverTitle.Style.Numberformat.Format = "@";

                var hourList = highDynamicServers.Select((t, x) => highDynamicServers.Keys.ElementAt(x)).ToList();
                var column = 0;
                foreach (var hour in hourList) {
                    var dataStartrowCount = 3;
                    xlWorkSheet.Cells[rowCount, (column + 1)].Value = hour;

                    //Merge the cells.
                    xlWorkSheet.Cells[ConvertNumberToChar(column + 1) + rowCount + ":" + ConvertNumberToChar(column + 4) + rowCount].Merge = true;
                    dataStartrowCount++;

                    //Add Sub Title.
                    xlWorkSheet.Cells[dataStartrowCount, (column + 1)].Value = "SERVER Class";
                    xlWorkSheet.Cells[dataStartrowCount, (column + 2)].Value = "Process Count";
                    xlWorkSheet.Cells[dataStartrowCount, (column + 3)].Value = "NUMSTATIC";
                    xlWorkSheet.Cells[dataStartrowCount, (column + 4)].Value = "MAXSERVERS";

                    if (highDynamicServers[hour].Count == 0) {
                        dataStartrowCount++;
                        //Insert No data.
                        xlWorkSheet.Cells[dataStartrowCount, (column + 1)].Value = "No data found!";

                        var noData = xlWorkSheet.Cells[ConvertNumberToChar((column + 1)) + dataStartrowCount + ":" + ConvertNumberToChar(column + 4) + dataStartrowCount];
                        noData.Merge = true;
                        noData.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        noData.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        noData.Style.Font.Color.SetColor(System.Drawing.Color.Red);

                        var border = xlWorkSheet.Cells[ConvertNumberToChar((column + 1)) + rowCount + ":" + ConvertNumberToChar((column + 4)) + dataStartrowCount];
                        AddBorder(border);
                        ChangeFontAndWidth(border);
                        xlWorkSheet.Column(column + 1).Width = 9;
                    } else {
                        foreach (var view in highDynamicServers[hour]) {
                            dataStartrowCount++;
                            xlWorkSheet.Cells[dataStartrowCount, (column + 1)].Value = view.ServerClass;
                            xlWorkSheet.Cells[dataStartrowCount, (column + 2)].Value = view.ProcessCount;
                            xlWorkSheet.Cells[dataStartrowCount, (column + 3)].Value = view.NumStatic;
                            xlWorkSheet.Cells[dataStartrowCount, (column + 4)].Value = view.MaxServers;
                        }

                        var border = xlWorkSheet.Cells[ConvertNumberToChar((column + 1)) + rowCount + ":" + ConvertNumberToChar((column + 4)) + dataStartrowCount];
                        if (hourList.IndexOf(hour) % 2 == 0) {
                            //border.Interior.ColorIndex = Helper.Color.LightGray;
                            border.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            border.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                        }
                        AddBorder(border);
                        ChangeFontAndWidth(border);
                    }
                    //INCREASE "column" BY NUMBER OF COLUMNS IN REPORT
                    column += 4;
                }

                AddHeaderNoFormat(serverTitle);
                serverTitle.AutoFitColumns();
                //serverTitle.EntireColumn.AutoFit();

                //Freeze first row.InsertWorksheetTransactionTcpPerPathway
                InsertFreezeRow(xlWorkSheet);

                InsertNavigate(xlWorkSheet, saveLocation, previousSection, previousWorksheet, nextWorksheet, nextSection, worksheetToc);
                InsertAlertHelpNavigate(xlWorkSheet, saveLocation, Enums.AlertReportTypes.HighDynamicServers, isLocalAnalyst);

                xlWorkSheet.Name = worksheetName.Length > 31 ? worksheetName.Substring(0, 31) : worksheetName;
            } catch (Exception ex)
            {
                _log.ErrorFormat("Error: InsertWorksheetHighDynamicServersDetail: {0}", ex);
                _reportDownloadLogs.InsertNewLog(_reportDownloadId, DateTime.Now, "There was an error generating the following worksheet: " + worksheetName);
            }
        }

        internal void InsertWorksheetQueues(ExcelWorkbook xlWorkBook, int worksheetCount, string pathwayName, string worksheetName, string saveLocation, 
                                            string previousSection, string previousWorksheet, string nextWorksheet, string nextSection, 
                                            string worksheetToc, string worksheetTitle, Dictionary<string, List<ServerQueueTcpSubView>> subDatas, 
                                            Enums.IntervalTypes intervalTypes, bool isLocalAnalyst, Dictionary<string, List<ServerQueueTcpView>> tcpView = null, 
                                            Dictionary<string, List<ServerQueueLinkmonView>> linkmonView = null, bool isAlert = true) {
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
                if (tcpView != null) {
                    dayLists = tcpView.Select((t, x) => tcpView.Keys.ElementAt(x)).ToList();
                    var serverTitle = xlWorkSheet.Cells["A" + dataStartrowCount + ":" + ConvertNumberToChar((tcpView.Count * columnCount) + 1) + (dataStartRow + 1)];
                    serverTitle.Style.Numberformat.Format = "@";
                    AddHeaderNoFormat(serverTitle);
                } else if (linkmonView != null) {
                    dayLists = linkmonView.Select((t, x) => linkmonView.Keys.ElementAt(x)).ToList();
                    var serverTitle = xlWorkSheet.Cells["A" + dataStartrowCount + ":" + ConvertNumberToChar((linkmonView.Count * columnCount) + 1) + (dataStartRow + 1)];
                    serverTitle.Style.Numberformat.Format = "@";
                    AddHeaderNoFormat(serverTitle);
                }

                /*//Format the Header.
                var header = xlWorkSheet.Range["A" + rowCount, endColume + rowCount];
                AddBorder(header);*/

                var column = 0;
                foreach (var day in dayLists) {
                    //Get ServerClass and count.
                    var serverInfo = (from x in subDatas[day]
                                      group x.ServerClass by x.ServerClass
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
                    if (tcpView != null) {
                        xlWorkSheet.Cells[dataStartrowCount, column + 1].Value = "SERVER Class";
                        xlWorkSheet.Cells[dataStartrowCount, column + 2].Value = "Peak Queue Count";
                        xlWorkSheet.Cells[dataStartrowCount, column + 3].Value = "TCP";
                        xlWorkSheet.Cells[dataStartrowCount, column + 4].Value = "Peaked Time";
                        xlWorkSheet.Cells[dataStartrowCount, column + 5].Value = "Instances";
                    } else {
                        xlWorkSheet.Cells[dataStartrowCount, column + 1].Value = "SERVER Class";
                        xlWorkSheet.Cells[dataStartrowCount, column + 2].Value = "Peak Queue Count";
                        xlWorkSheet.Cells[dataStartrowCount, column + 3].Value = "Linkmon";
                        xlWorkSheet.Cells[dataStartrowCount, column + 4].Value = "Peaked Time";
                        xlWorkSheet.Cells[dataStartrowCount, column + 5].Value = "Instances";
                    }

                    dataStartrowCount++;
                    if (tcpView != null) {
                        foreach (var item in tcpView[day]) {
                            //Calcuate the row number on the detail page.
                            var tempDetailCount = 0;
                            for (var i = 0; i < serverInfo.Count; i++) {
                                var tempValue = serverInfo.Keys.ElementAt(i);
                                if (!tempValue.Equals(item.Server))
                                    tempDetailCount += serverInfo[tempValue];
                                else {
                                    tempDetailCount += serverInfo[tempValue];
                                    break;
                                }
                            }

                            xlWorkSheet.Cells[dataStartrowCount, column + 1].Value = item.Server;
                            xlWorkSheet.Cells[dataStartrowCount, column + 2].Value = item.PeakQueueLength;
                            xlWorkSheet.Cells[dataStartrowCount, column + 3].Value = item.Tcp;
                            xlWorkSheet.Cells[dataStartrowCount, column + 4].Value = item.PeakTime;
                            xlWorkSheet.Cells[dataStartrowCount, column + 5].Value = item.Instances;
                            if (tempDetailCount > 0) {
                                string linkName;
                                if (Enums.IntervalTypes.Daily.Equals(intervalTypes))
                                    linkName = Convert.ToDateTime(day).ToString("yy-MM-dd");
                                else
                                    linkName = Convert.ToDateTime(day).ToString("HHmm");

                                //Add hyper link.
                                xlWorkSheet.Cells[dataStartrowCount, column + 5].Hyperlink = new Uri("#'" + pathwayName + " Que TCP Ins. " + linkName + "'!A" + (tempDetailCount + 3), UriKind.Relative);
                                xlWorkSheet.Cells[dataStartrowCount, column + 5].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                                xlWorkSheet.Cells[dataStartrowCount, column + 5].Style.Font.UnderLine = true;
                            }
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
                    } else {
                        foreach (var item in linkmonView[day]) {
                            //Calcuate the row number on the detail page.
                            var tempDetailCount = 0;
                            for (var i = 0; i < serverInfo.Count; i++) {
                                var tempValue = serverInfo.Keys.ElementAt(i);
                                if (!tempValue.Equals(item.Server))
                                    tempDetailCount += serverInfo[tempValue];
                                else {
                                    tempDetailCount += serverInfo[tempValue];
                                    break;
                                }
                            }
                            xlWorkSheet.Cells[dataStartrowCount, column + 1].Value = item.Server;
                            if (tempDetailCount > 0) {
                                string linkName;
                                if (Enums.IntervalTypes.Daily.Equals(intervalTypes))
                                    linkName = Convert.ToDateTime(day).ToString("yy-MM-dd");
                                else
                                    linkName = Convert.ToDateTime(day).ToString("HHmm");
                                //Add hyper link.
                                xlWorkSheet.Cells[dataStartrowCount, column + 1].Hyperlink = new Uri("#'" + pathwayName + " Que Linkmon Ins. " + linkName + "'!A" +
                                    (tempDetailCount + 3), UriKind.Relative);
                                xlWorkSheet.Cells[dataStartrowCount, column + 1].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                                xlWorkSheet.Cells[dataStartrowCount, column + 1].Style.Font.UnderLine = true;

                            }
                            xlWorkSheet.Cells[dataStartrowCount, column + 2].Value = item.PeakQueueLength;
                            xlWorkSheet.Cells[dataStartrowCount, column + 3].Value = item.Linkmon;
                            xlWorkSheet.Cells[dataStartrowCount, column + 4].Value = item.PeakTime;
                            xlWorkSheet.Cells[dataStartrowCount, column + 5].Value = item.Instances;
                            dataStartrowCount++;
                        }

                        if (linkmonView[day].Count == 0) {
                            //Insert No data.
                            xlWorkSheet.Cells[dataStartrowCount, column + 1].Value = "No data found!";

                            var noData = xlWorkSheet.Cells[ConvertNumberToChar(column + 1) + dataStartrowCount + ":" + ConvertNumberToChar(column + columnCount) + dataStartrowCount];
                            noData.Merge = true;
                            noData.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                            noData.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            noData.Style.Font.Color.SetColor(System.Drawing.Color.Red);
                            dataStartrowCount++;
                        }
                    }

                    //Border
                    var border = xlWorkSheet.Cells[ConvertNumberToChar(column + 1) + dataStartRow + ":" + ConvertNumberToChar(column + columnCount) + (dataStartrowCount - 1)];
                    AddBorder(border);
                    if (dayLists.IndexOf(day) % 2 == 0) {
                        //border.Interior.ColorIndex = Helper.Color.LightGray;
                        border.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        border.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    }

                    //Cell Format.
                    var formatCell = xlWorkSheet.Cells[ConvertNumberToChar(column + 2) + dataStartRow + ":" + ConvertNumberToChar(column + 2) + (dataStartrowCount - 1)];
                    formatCell.Style.Numberformat.Format = "#,##0";
                    formatCell.ConditionalFormatting.AddDatabar(System.Drawing.Color.RoyalBlue);

                    formatCell = xlWorkSheet.Cells[ConvertNumberToChar(column + 4) + dataStartRow + ":" + ConvertNumberToChar(column + 4) + (dataStartrowCount - 1)];
                    formatCell.Style.Numberformat.Format = "hh:mm";

                    formatCell = xlWorkSheet.Cells[ConvertNumberToChar(column + 5) + dataStartRow + ":" + ConvertNumberToChar(column + 5) + (dataStartrowCount - 1)];
                    formatCell.Style.Numberformat.Format = "#,##0";
                    formatCell.ConditionalFormatting.AddDatabar(System.Drawing.Color.RoyalBlue);

                    column += columnCount;
                }



                InsertFreezeRowFirstFour(xlWorkSheet);

                //Change Font.
                ExcelRange sheetFontRange;
                if (tcpView != null)
                    sheetFontRange = xlWorkSheet.Cells["A" + (dataStartRow + 2) + ":" + ConvertNumberToChar(column + 5) + (dataStartrowCount + tcpView.Count)];
                else
                    sheetFontRange = xlWorkSheet.Cells["A" + (dataStartRow + 2) + ":" + ConvertNumberToChar(column + 5) + (dataStartrowCount + linkmonView.Count)];

                ChangeFontAndWidth(sheetFontRange);

                xlWorkSheet.Column(1).Width = 16;
                xlWorkSheet.Column(2).Width = 21;
                xlWorkSheet.Column(3).Width = 11;
                xlWorkSheet.Column(4).Width = 16;
                xlWorkSheet.Column(5).Width = 14;

                InsertNavigate(xlWorkSheet, saveLocation, previousSection, previousWorksheet, nextWorksheet, nextSection, worksheetToc);

                if (isAlert) {
                    if (tcpView != null)
                        InsertAlertHelpNavigate(xlWorkSheet, saveLocation, Enums.AlertReportTypes.QueueTCP, isLocalAnalyst);
                    else
                        InsertAlertHelpNavigate(xlWorkSheet, saveLocation, Enums.AlertReportTypes.QueueLinkmon, isLocalAnalyst);
                } else {
                    if (tcpView != null)
                        InsertPathwayHelpNavigate(xlWorkSheet, saveLocation, Enums.PathwayReportTypes.ServerQueuesTCP, isLocalAnalyst);
                    else
                        InsertPathwayHelpNavigate(xlWorkSheet, saveLocation, Enums.PathwayReportTypes.ServerQueuesLinkmon, isLocalAnalyst);
                }

                xlWorkSheet.Name = worksheetName.Length > 31 ? worksheetName.Substring(0, 31) : worksheetName;
            } catch (Exception ex)
            {
                _log.ErrorFormat("Error: InsertWorksheetQueues: {0}", ex);
                _reportDownloadLogs.InsertNewLog(_reportDownloadId, DateTime.Now, "There was an error generating the following worksheet: " + worksheetName);
            }
        }

        internal void InsertWorksheetErrorList(ExcelWorkbook xlWorkBook, int worksheetCount, string pathwayName, Dictionary<string, List<ServerErrorListView>> errorLists, Dictionary<string, List<ServerErrorView>> subDatas, string worksheetName, string saveLocation, string previousSection, string previousWorksheet, string nextWorksheet, string nextSection, string worksheetToc, string worksheetTitle, bool isLocalAnalyst) {
            try {

                if (xlWorkBook.Worksheets.Count < worksheetCount)
                    xlWorkBook.Worksheets.Add(worksheetName);
                //var xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.Item["Sheet" + Convert.ToString(worksheetCount)];
                var xlWorkSheet = (ExcelWorksheet)xlWorkBook.Worksheets[xlWorkBook.Worksheets.Count];

                var columnCount = 3;
                var rowCount = 3;

                //Insert Sheet Title.
                xlWorkSheet.Cells[2, 1].Value = worksheetTitle; //pathwayName + " - SERVER Transactions";
                xlWorkSheet.Cells["A2:Z2"].Merge = true;
                var title = xlWorkSheet.Cells["A2"];
                AddTitle(title);

                var dataStartRow = rowCount;
                var serverTitle = xlWorkSheet.Cells["A" + dataStartRow + ":" + ConvertNumberToChar((errorLists.Count * columnCount) + 1) + (dataStartRow + 1)];
                serverTitle.Style.Numberformat.Format = "@";

                var dayLists = errorLists.Select((t, x) => errorLists.Keys.ElementAt(x)).ToList();

                var column = 0;
                foreach (var day in dayLists) {
                    //Get ServerClass and count.
                    var serverInfo = (from x in subDatas[day]
                                      group x.ServerClass by x.ServerClass
                                        into g
                                      let count = g.Count()
                                      orderby g.Key
                                      select new { Value = g.Key, Count = count }).ToDictionary(x => x.Value, x => x.Count);

                    var dataStartrowCount = 3;
                    xlWorkSheet.Cells[rowCount, (column + 1)].Value = day;

                    //Merge the cells.
                    xlWorkSheet.Cells[ConvertNumberToChar(column + 1) + rowCount + ":" + ConvertNumberToChar(column + columnCount) + rowCount].Merge = true;
                    dataStartrowCount++;

                    //Add Sub Title.
                    xlWorkSheet.Cells[dataStartrowCount, (column + 1)].Value = "SERVER Class";
                    xlWorkSheet.Cells[dataStartrowCount, (column + 2)].Value = "Most Recent Time";
                    xlWorkSheet.Cells[dataStartrowCount, (column + 3)].Value = "Instances";
                    if (errorLists[day].Count == 0) {
                        dataStartrowCount++;
                        //Insert No data.
                        xlWorkSheet.Cells[dataStartrowCount, (column + 1)].Value = "No data found!";

                        var noData = xlWorkSheet.Cells[ConvertNumberToChar((column + 1)) + dataStartrowCount + ":" + ConvertNumberToChar(column + columnCount) + dataStartrowCount];
                        noData.Merge = true;
                        noData.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        noData.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        noData.Style.Font.Color.SetColor(System.Drawing.Color.Red);

                        /*var border = xlWorkSheet.Range[ConvertNumberToChar((column + 1)) + rowCount, ConvertNumberToChar((column + columnCount)) + dataStartrowCount];
                        AddBorder(border);
                        ChangeFontAndWidth(border);*/
                        xlWorkSheet.Column(column + 1).Width = 9;
                        xlWorkSheet.Column(column + 2).Width = 9;
                        xlWorkSheet.Column(column + 3).Width = 9;
                    } else {
                        foreach (var view in errorLists[day]) {//Calcuate the row number on the detail page.
                            var tempDetailCount = 0;
                            for (var i = 0; i < serverInfo.Count; i++) {
                                var tempValue = serverInfo.Keys.ElementAt(i);
                                if (!tempValue.Equals(view.ServerClass))
                                    tempDetailCount += serverInfo[tempValue];
                                else {
                                    tempDetailCount += serverInfo[tempValue];
                                    break;
                                }
                            }

                            dataStartrowCount++;
                            xlWorkSheet.Cells[dataStartrowCount, (column + 1)].Value = view.ServerClass;
                            if (tempDetailCount > 0) {
                                //Add hyper link.
                                xlWorkSheet.Cells[dataStartrowCount, column + 1].Hyperlink = new Uri("#'" + pathwayName + " Error List Ins. " + Convert.ToDateTime(day).ToString("yy-MM-dd") + "'!A" +
                                    (tempDetailCount + 3), UriKind.Relative);
                                xlWorkSheet.Cells[dataStartrowCount, column + 1].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                                xlWorkSheet.Cells[dataStartrowCount, column + 1].Style.Font.UnderLine = true;
                            }

                            xlWorkSheet.Cells[dataStartrowCount, (column + 2)].Value = view.MostRecentTime;
                            xlWorkSheet.Cells[dataStartrowCount, (column + 3)].Value = view.Instances;
                        }

                        //cell format
                        var formatCell = xlWorkSheet.Cells[ConvertNumberToChar(column + 2) + "5" + ":" + ConvertNumberToChar(column + 2) + dataStartrowCount];
                        formatCell.Style.Numberformat.Format = "hh:mm";

                        formatCell = xlWorkSheet.Cells[ConvertNumberToChar(column + 3) + "5" + ":" + ConvertNumberToChar(column + 4) + dataStartrowCount];
                        formatCell.Style.Numberformat.Format = "#,##0";

                    }
                    var border = xlWorkSheet.Cells[ConvertNumberToChar((column + 1)) + rowCount + ":" + ConvertNumberToChar((column + columnCount)) + dataStartrowCount];
                    if (dayLists.IndexOf(day) % 2 == 0) {
                        //border.Interior.ColorIndex = Helper.Color.LightGray;
                        border.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        border.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    }
                    AddBorder(border);
                    ChangeFontAndWidth(border);
                    column += columnCount;
                }

                AddHeaderNoFormat(serverTitle);
                serverTitle.AutoFitColumns();
                //serverTitle.EntireColumn.AutoFit();

                //Freeze first row.InsertWorksheetTransactionTcpPerPathway
                InsertFreezeRow(xlWorkSheet);

                InsertNavigate(xlWorkSheet, saveLocation, previousSection, previousWorksheet, nextWorksheet, nextSection, worksheetToc);
                InsertAlertHelpNavigate(xlWorkSheet, saveLocation, Enums.AlertReportTypes.ErrorList, isLocalAnalyst);

                xlWorkSheet.Name = worksheetName.Length > 31 ? worksheetName.Substring(0, 31) : worksheetName;
            } catch (Exception ex) {
                _log.ErrorFormat("Error: InsertWorksheetErrorList: {0}", ex);
                _reportDownloadLogs.InsertNewLog(_reportDownloadId, DateTime.Now, "There was an error generating the following worksheet: " + worksheetName);
            }

        }

        internal void InsertWorksheetQueueSubs(ExcelWorkbook xlWorkBook, int worksheetCount, string pathwayName, string worksheetName, string saveLocation, string previousSection, string previousWorksheet, string nextWorksheet, string nextSection, string worksheetToc, string worksheetTitle, List<ServerQueueTcpSubView> tcpView, bool isLinkmon, bool isLocalAnalyst) {

            try {
                if (xlWorkBook.Worksheets.Count < worksheetCount)
                    xlWorkBook.Worksheets.Add(worksheetName);
                //var xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.Item["Sheet" + Convert.ToString(worksheetCount)];
                var xlWorkSheet = (ExcelWorksheet)xlWorkBook.Worksheets[xlWorkBook.Worksheets.Count];

                var rowCount = 3;
                var columnCount = 9;
                //Insert Sheet Title.
                xlWorkSheet.Cells[2, 1].Value = worksheetTitle;
                xlWorkSheet.Cells["A2:Z2"].Merge = true;
                var title = xlWorkSheet.Cells["A2"];
                AddTitle(title);

                var column = 0;
                var dataStartrowCount = 3;

                //Add Sub Title.
                xlWorkSheet.Cells[dataStartrowCount, (column + 1)].Value = "SERVER Class";
                xlWorkSheet.Cells[dataStartrowCount, (column + 2)].Value = "Time";
                if (!isLinkmon)
                    xlWorkSheet.Cells[dataStartrowCount, (column + 3)].Value = "TCP";
                else
                    xlWorkSheet.Cells[dataStartrowCount, (column + 3)].Value = "Linkmon";
                xlWorkSheet.Cells[dataStartrowCount, (column + 4)].Value = "Request Count";
                xlWorkSheet.Cells[dataStartrowCount, (column + 5)].Value = "% Wait";
                xlWorkSheet.Cells[dataStartrowCount, (column + 6)].Value = "% Dynamic";
                xlWorkSheet.Cells[dataStartrowCount, (column + 7)].Value = "Maximum Waits";
                xlWorkSheet.Cells[dataStartrowCount, (column + 8)].Value = "Average Waits";
                xlWorkSheet.Cells[dataStartrowCount, (column + 9)].Value = "Sent Request Count";

                var serverTitle = xlWorkSheet.Cells["A" + dataStartrowCount + ":" + "I" + dataStartrowCount];
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
                    xlWorkSheet.Column(column + 5).Width = 9;
                    xlWorkSheet.Column(column + 6).Width = 9;
                    xlWorkSheet.Column(column + 7).Width = 9;
                    xlWorkSheet.Column(column + 8).Width = 9;
                    xlWorkSheet.Column(column + 9).Width = 9;
                } else {
                    var range = xlWorkSheet.Cells["A" + (dataStartrowCount + 1)];
                    long rangeCount = tcpView.Count;
                    //range = range.Resize[rangeCount, columnCount];
                    range = xlWorkSheet.Cells[Convert.ToInt32(rangeCount), columnCount];
                    var saRet = new object[rangeCount, columnCount];

                    var tempServerClass = "";
                    var dataRowInfo = new Dictionary<int, int>();
                    var tempStartRow = 0;
                    for (var x = 0; x < rangeCount; x++) {
                        dataStartrowCount++;
                        if (tempServerClass.Length == 0) {
                            tempServerClass = tcpView[x].ServerClass;
                            saRet[x, 0] = tempServerClass;
                            tempStartRow = x;
                        } else if (tempServerClass.Equals(tcpView[x].ServerClass)) {
                            saRet[x, 0] = "";
                        } else {
                            tempServerClass = tcpView[x].ServerClass;
                            saRet[x, 0] = tempServerClass;
                            dataRowInfo.Add(tempStartRow, (x - 1));
                            tempStartRow = x;
                        }

                        saRet[x, 1] = tcpView[x].Time;
                        saRet[x, 2] = tcpView[x].TcpName;
                        saRet[x, 3] = tcpView[x].RequestCount;
                        saRet[x, 4] = tcpView[x].PercentWait;
                        saRet[x, 5] = tcpView[x].PercentDynamic;
                        saRet[x, 6] = tcpView[x].MaxWaits;
                        saRet[x, 7] = tcpView[x].AvgWaits;
                        saRet[x, 8] = tcpView[x].SentRequestCount;
                    }
                    dataRowInfo.Add(tempStartRow, ((int)rangeCount - 1));

                    //range.set_Value(MisValue, saRet);
                    //range.Value = saRet;

                    for (int row = 4; row < ((saRet.Length / columnCount) + 4); row++) {
                        for (int col = 0; col < columnCount; col++) {
                            xlWorkSheet.Cells[row, col + 1].Value = saRet[row - 4, col];
                        }
                    }

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
                    border = xlWorkSheet.Cells["D" + (rowCount + 1) + ":" + "D" + dataStartrowCount];
                    border.Style.Numberformat.Format = "#,##0";

                    border = xlWorkSheet.Cells["E" + (rowCount + 1) + ":" + "F" + dataStartrowCount];
                    border.Style.Numberformat.Format = "0.00";

                    border = xlWorkSheet.Cells["G" + (rowCount + 1) + ":" + "G" + dataStartrowCount];
                    border.Style.Numberformat.Format = "#,##0";

                    border = xlWorkSheet.Cells["I" + (rowCount + 1) + ":" + "I" + dataStartrowCount];
                    border.Style.Numberformat.Format = "#,##0";

                    border = xlWorkSheet.Cells["H" + (rowCount + 1) + ":" + "H" + dataStartrowCount];
                    border.Style.Numberformat.Format = "0.00";

                    border = xlWorkSheet.Cells[ConvertNumberToChar((column + 1)) + (rowCount + 1) + ":" + ConvertNumberToChar((column + columnCount)) + dataStartrowCount];
                    ChangeFontAndWidth(border);
                }

                serverTitle.AutoFitColumns();

                //Freeze first row.InsertWorksheetTransactionTcpPerPathway
                InsertFreezeRowFirstThree(xlWorkSheet);

                InsertNavigate(xlWorkSheet, saveLocation, previousSection, previousWorksheet, nextWorksheet, nextSection, worksheetToc);
                if (!isLinkmon)
                    InsertAlertHelpNavigate(xlWorkSheet, saveLocation, Enums.AlertReportTypes.QueueTCPSub, isLocalAnalyst);
                else
                    InsertAlertHelpNavigate(xlWorkSheet, saveLocation, Enums.AlertReportTypes.QueueLinkmomSub, isLocalAnalyst);


                xlWorkSheet.Name = worksheetName.Length > 31 ? worksheetName.Substring(0, 31) : worksheetName;


            }
            catch (Exception ex) {
                _log.ErrorFormat("Error: InsertWorksheetQueueSubs: {0}", ex);
                _reportDownloadLogs.InsertNewLog(_reportDownloadId, DateTime.Now, "There was an error generating the following worksheet: " + worksheetName);
            }
        }

        internal void InsertWorksheetErrorListSubs(ExcelWorkbook xlWorkBook, int worksheetCount, string pathwayName, List<ServerErrorView> errorLists, string worksheetName, string saveLocation, string previousSection, string previousWorksheet, string nextWorksheet, string nextSection, string worksheetToc, string worksheetTitle, List<long> errorInfos, bool isLocalAnalyst) {
            try {
                if (xlWorkBook.Worksheets.Count < worksheetCount)
                    xlWorkBook.Worksheets.Add(worksheetName);
                //var xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.Item["Sheet" + Convert.ToString(worksheetCount)];
                var xlWorkSheet = (ExcelWorksheet)xlWorkBook.Worksheets[xlWorkBook.Worksheets.Count];

                var rowCount = 3;
                var columnCount = 3;
                //Insert Sheet Title.
                xlWorkSheet.Cells[2, 1].Value = worksheetTitle;
                xlWorkSheet.Cells["A2:Z2"].Merge = true;
                var title = xlWorkSheet.Cells["A2"];
                AddTitle(title);

                var column = 0;
                var dataStartrowCount = 3;

                //Add Sub Title.
                xlWorkSheet.Cells[dataStartrowCount, (column + 1)].Value = "SERVER Class";
                xlWorkSheet.Cells[dataStartrowCount, (column + 2)].Value = "Time";
                xlWorkSheet.Cells[dataStartrowCount, (column + 3)].Value = "Error Number";

                var serverTitle = xlWorkSheet.Cells["A" + dataStartrowCount + ":" + "C" + dataStartrowCount];
                AddHeaderNoFormat(serverTitle);

                if (errorLists.Count == 0) {
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
                } else {
                    var range = xlWorkSheet.Cells["A" + (dataStartrowCount + 1)];
                    long rangeCount = errorLists.Count;
                    range = xlWorkSheet.Cells["A" + (dataStartrowCount + 1)];
                    var saRet = new object[rangeCount, columnCount];

                    var tempServerClass = "";
                    var dataRowInfo = new Dictionary<int, int>();
                    var tempStartRow = 0;
                    for (var x = 0; x < rangeCount; x++) {
                        dataStartrowCount++;
                        if (tempServerClass.Length == 0) {
                            tempServerClass = errorLists[x].ServerClass;
                            saRet[x, 0] = tempServerClass;
                            tempStartRow = x;
                        } else if (tempServerClass.Equals(errorLists[x].ServerClass)) {
                            saRet[x, 0] = "";
                        } else {
                            tempServerClass = errorLists[x].ServerClass;
                            saRet[x, 0] = tempServerClass;
                            dataRowInfo.Add(tempStartRow, (x - 1));
                            tempStartRow = x;
                        }

                        saRet[x, 1] = errorLists[x].MostRecentTime;
                        saRet[x, 2] = errorLists[x].ErrorNumber;
                    }

                    dataRowInfo.Add(tempStartRow, ((int)rangeCount - 1));

                    for (int row = 4; row < ((saRet.Length / columnCount) + 4); row++) {
                        for (int col = 0; col < columnCount; col++) {
                            xlWorkSheet.Cells[row, col + 1].Value = saRet[row - 4, col];
                        }
                    }
                    //Base on the dataRowInfo, change the back ground color.
                    var i = 0;
                    foreach (var rows in dataRowInfo) {
                        var subRange = xlWorkSheet.Cells["A" + (rows.Key + (rowCount + 1)) + ":" + "A" + (rows.Value + (rowCount + 1))];
                        subRange.Merge = true;
                        subRange.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        subRange.Style.Font.Bold = true;

                        if (i % 2 == 0) {
                            subRange = xlWorkSheet.Cells["A" + (rows.Key + (rowCount + 1)) + ":" + ConvertNumberToChar(columnCount) + (rows.Value + (rowCount + 1))];
                            //subRange.Interior.ColorIndex = Helper.Color.LightGray;
                            subRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            subRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                        }
                        i++;
                    }

                    try {
                        //Insert Hyperlink.
                        for (var rows = 0; rows < rangeCount; rows++) {
                            //Get error Index.
                            string errorValue = xlWorkSheet.Cells["C" + (rows + 4)].Text;
                            var errorCount = errorInfos.IndexOf(Convert.ToInt64(errorValue)) + 4;
                            if (errorCount > 3) {
                                //Add hyper link.
                                xlWorkSheet.Cells[(rows + 4), column + 3].Hyperlink = new Uri("#'Error Info'!A3", UriKind.Relative);
                                xlWorkSheet.Cells[(rows + 4), column + 3].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                                xlWorkSheet.Cells[(rows + 4), column + 3].Style.Font.UnderLine = true;

                            }
                        }
                    }
                    catch (Exception ex) {
                        throw new Exception(ex.Message);
                    }
                    var border = xlWorkSheet.Cells[ConvertNumberToChar((column + 1)) + rowCount + ":" + ConvertNumberToChar((column + columnCount)) + dataStartrowCount];
                    AddBorder(border);

                    //DateTime format.
                    border = xlWorkSheet.Cells["B" + (rowCount + 1) + ":" + "B" + dataStartrowCount];
                    border.Style.Numberformat.Format = "hh:mm";

                    border = xlWorkSheet.Cells[ConvertNumberToChar((column + 1)) + (rowCount + 1) + ":" + ConvertNumberToChar((column + columnCount)) + dataStartrowCount];
                    ChangeFontAndWidth(border);
                }
                serverTitle.AutoFitColumns();

                //Freeze first row.InsertWorksheetTransactionTcpPerPathway
                InsertFreezeRowFirstThree(xlWorkSheet);

                InsertNavigate(xlWorkSheet, saveLocation, previousSection, previousWorksheet, nextWorksheet, nextSection, worksheetToc);
                InsertAlertHelpNavigate(xlWorkSheet, saveLocation, Enums.AlertReportTypes.ErrorListSub, isLocalAnalyst);

                xlWorkSheet.Name = worksheetName.Length > 31 ? worksheetName.Substring(0, 31) : worksheetName;
            } catch (Exception ex) {
                _log.ErrorFormat("Error: InsertWorksheetErrorListSubs: {0}", ex);
                _reportDownloadLogs.InsertNewLog(_reportDownloadId, DateTime.Now, "There was an error generating the following worksheet: " + worksheetName);
            }
        }

        internal void InsertWorksheetErrorInfo(ExcelWorkbook xlWorkBook, int worksheetCount, List<ErrorInfo> errorLists, string saveLocation, string worksheetToc) {
            try {
                if (xlWorkBook.Worksheets.Count < worksheetCount)
                    xlWorkBook.Worksheets.Add("Error" + worksheetCount);
                //var xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.Item["Sheet" + Convert.ToString(worksheetCount)];
                var xlWorkSheet = (ExcelWorksheet)xlWorkBook.Worksheets[xlWorkBook.Worksheets.Count];

                var rowCount = 3;
                var columnCount = 5;
                //Insert Sheet Title.
                xlWorkSheet.Cells[2, 1].Value = "Error Information";
                xlWorkSheet.Cells["A2:Z2"].Merge = true;
                var title = xlWorkSheet.Cells["A2"];
                AddTitle(title);

                var column = 0;
                var dataStartrowCount = 3;

                //Add Sub Title.
                xlWorkSheet.Cells[dataStartrowCount, (column + 1)].Value = "Error Number";
                xlWorkSheet.Cells[dataStartrowCount, (column + 2)].Value = "Message";
                xlWorkSheet.Cells[dataStartrowCount, (column + 3)].Value = "Cause";
                xlWorkSheet.Cells[dataStartrowCount, (column + 4)].Value = "Effect";
                xlWorkSheet.Cells[dataStartrowCount, (column + 5)].Value = "Recovery";

                var serverTitle = xlWorkSheet.Cells["A" + dataStartrowCount + ":" + "E" + dataStartrowCount];
                AddHeaderNoFormat(serverTitle);

                var range = xlWorkSheet.Cells["A" + (dataStartrowCount + 1)];
                long rangeCount = errorLists.Count;
                var saRet = new object[rangeCount, columnCount];

                if (rangeCount > 0 ) {
                    //range = xlWorkSheet.Cells[Convert.ToInt32(rangeCount), columnCount];


                    for (var x = 0; x < rangeCount; x++) {
                        rowCount++;
                        saRet[x, 0] = errorLists[x].ErrorNumber;
                        saRet[x, 1] = errorLists[x].Message;
                        saRet[x, 2] = errorLists[x].Cause;
                        saRet[x, 3] = errorLists[x].Effect;
                        saRet[x, 4] = errorLists[x].Recovery;
                    }

                    for (int row = 4; row < ((saRet.Length / columnCount) + 4); row++) {
                        for (int col = 0; col < columnCount; col++) {
                            xlWorkSheet.Cells[row, col + 1].Value = saRet[row - 4, col];
                        }
                    }

                    //Base on the dataRowInfo, change the back ground color.
                    var i = 0;
                    for (var rows = 0; rows < rangeCount; rows++) {
                        var subRange = xlWorkSheet.Cells["A" + (rows + dataStartrowCount + 1) + ":" + ConvertNumberToChar(columnCount) + (rows + dataStartrowCount + 1)];
                        subRange.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                        if (i % 2 == 0) {
                            //subRange.Interior.ColorIndex = Helper.Color.LightGray;
                            subRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            subRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                        }
                        i++;
                    }

                    var border = xlWorkSheet.Cells[ConvertNumberToChar((column + 1)) + dataStartrowCount + ":" + ConvertNumberToChar((column + columnCount)) + rowCount];
                    AddBorder(border);

                    border = xlWorkSheet.Cells[ConvertNumberToChar((column + 1)) + (dataStartrowCount + 1) + ":" + ConvertNumberToChar((column + columnCount)) + rowCount];
                    ChangeFontAndWidth(border);
                    border.Style.WrapText = true;
                }


                xlWorkSheet.Column(1).Width = 20;
                xlWorkSheet.Column(2).Width = 40;
                xlWorkSheet.Column(3).Width = 40;
                xlWorkSheet.Column(4).Width = 40;
                xlWorkSheet.Column(5).Width = 40;

                //Freeze first row.InsertWorksheetTransactionTcpPerPathway
                InsertFreezeRowFirstThree(xlWorkSheet);

                InsertErrorNavigate(xlWorkSheet, saveLocation, worksheetToc);

                xlWorkSheet.Name = "Error Info";
            } catch (Exception ex) {
                _log.ErrorFormat("Error: InsertWorksheetErrorInfo: {0}", ex);
                _reportDownloadLogs.InsertNewLog(_reportDownloadId, DateTime.Now, "There was an error generating the following worksheet: Error Info");
            }
        }

        internal void InsertWorksheetPendingClassInterval(ExcelWorkbook xlWorkBook, int worksheetCount, string list, Dictionary<string, List<string>> pendingClass, string worksheetName, string saveLocation, string previousSection, string previousWorksheet, string nextWorksheet, string nextSection, string worksheetToc, string worksheetTitle, bool isLocalAnalyst) {
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
                var serverTitle = xlWorkSheet.Cells["A" + dataStartRow + ":" + ConvertNumberToChar((pendingClass.Count * 1) + 1) + (dataStartRow + 1)];
                serverTitle.Style.Numberformat.Format = "@";

                var hourList = pendingClass.Select((t, x) => pendingClass.Keys.ElementAt(x)).ToList();

                var column = 0;
                foreach (var hour in hourList) {
                    var dataStartrowCount = 3;
                    xlWorkSheet.Cells[rowCount, (column + 1)].Value = hour;

                    //Merge the cells.
                    xlWorkSheet.Cells[ConvertNumberToChar(column + 1) + rowCount + ":" + ConvertNumberToChar(column + 1) + rowCount].Merge = true;
                    dataStartrowCount++;

                    //Add Sub Title.
                    xlWorkSheet.Cells[dataStartrowCount, (column + 1)].Value = "SERVER Class";
                    if (pendingClass[hour].Count == 0) {
                        dataStartrowCount++;
                        //Insert No data.
                        xlWorkSheet.Cells[dataStartrowCount, (column + 1)].Value = "No data found!";

                        var noData = xlWorkSheet.Cells[ConvertNumberToChar((column + 1)) + dataStartrowCount + ":" + ConvertNumberToChar(column + 1) + dataStartrowCount];
                        noData.Merge = true;
                        noData.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        noData.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        noData.Style.Font.Color.SetColor(System.Drawing.Color.Red);

                        var border = xlWorkSheet.Cells[ConvertNumberToChar((column + 1)) + rowCount + ":" + ConvertNumberToChar((column + 1)) + dataStartrowCount];
                        AddBorder(border);
                        ChangeFontAndWidth(border);
                        xlWorkSheet.Column(column + 1).Width = 9;
                    } else {
                        foreach (var serverName in pendingClass[hour]) {
                            dataStartrowCount++;
                            xlWorkSheet.Cells[dataStartrowCount, (column + 1)].Value = serverName;
                        }

                        var border = xlWorkSheet.Cells[ConvertNumberToChar((column + 1)) + rowCount + ":" + ConvertNumberToChar((column + 1)) + dataStartrowCount];
                        if (hourList.IndexOf(hour) % 2 == 0) {
                            //border.Interior.ColorIndex = Helper.Color.LightGray;
                            border.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            border.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                        }
                        AddBorder(border);
                        ChangeFontAndWidth(border);
                    }
                    column += 1;
                }

                AddHeaderNoFormat(serverTitle);
                serverTitle.AutoFitColumns();

                //Freeze first row.InsertWorksheetTransactionTcpPerPathway
                InsertFreezeRow(xlWorkSheet);

                InsertNavigate(xlWorkSheet, saveLocation, previousSection, previousWorksheet, nextWorksheet, nextSection, worksheetToc);
                InsertAlertHelpNavigate(xlWorkSheet, saveLocation, Enums.AlertReportTypes.UnusedProcess, isLocalAnalyst);

                xlWorkSheet.Name = worksheetName.Length > 31 ? worksheetName.Substring(0, 31) : worksheetName;
            } catch (Exception ex) {
                _log.ErrorFormat("Error: InsertWorksheetPendingClassInterval: {0}", ex);
                _reportDownloadLogs.InsertNewLog(_reportDownloadId, DateTime.Now, "There was an error generating the following worksheet: " + worksheetName);
            }
        }

        internal void InsertWorksheetPendingProcessInterval(ExcelWorkbook xlWorkBook, int worksheetCount, string list, Dictionary<string, List<ServerUnusedServerProcessesView>> pendingProcess, string worksheetName, string saveLocation, string previousSection, string previousWorksheet, string nextWorksheet, string nextSection, string worksheetToc, string worksheetTitle, bool isLocalAnalyst) {
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
                int columnCount = 2;

                var dataStartRow = rowCount;
                var serverTitle = xlWorkSheet.Cells["A" + dataStartRow + ":" + ConvertNumberToChar((pendingProcess.Count * columnCount) + 1) + (dataStartRow + 1)];
                serverTitle.Style.Numberformat.Format = "@";

                var hourList = pendingProcess.Select((t, x) => pendingProcess.Keys.ElementAt(x)).ToList();

                var column = 0;
                foreach (var hour in hourList) {
                    var dataStartrowCount = 3;
                    xlWorkSheet.Cells[rowCount, (column + 1)].Value = hour;

                    //Merge the cells.
                    xlWorkSheet.Cells[ConvertNumberToChar(column + 1) + rowCount + ":" + ConvertNumberToChar(column + columnCount) + rowCount].Merge = true;
                    dataStartrowCount++;

                    //Add Sub Title.
                    xlWorkSheet.Cells[dataStartrowCount, (column + 1)].Value = "SERVER Class";
                    xlWorkSheet.Cells[dataStartrowCount, (column + 2)].Value = "Process";
                    if (pendingProcess[hour].Count == 0) {
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
                    } else {
                        foreach (var view in pendingProcess[hour]) {
                            dataStartrowCount++;
                            xlWorkSheet.Cells[dataStartrowCount, (column + 1)].Value = view.ServerClass;
                            xlWorkSheet.Cells[dataStartrowCount, (column + 2)].Value = view.Process;
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
                    column += columnCount;
                }

                AddHeaderNoFormat(serverTitle);
                serverTitle.AutoFitColumns();

                //Freeze first row.InsertWorksheetTransactionTcpPerPathway
                InsertFreezeRow(xlWorkSheet);

                InsertNavigate(xlWorkSheet, saveLocation, previousSection, previousWorksheet, nextWorksheet, nextSection, worksheetToc);
                InsertAlertHelpNavigate(xlWorkSheet, saveLocation, Enums.AlertReportTypes.UnusedProcess, isLocalAnalyst);

                xlWorkSheet.Name = worksheetName.Length > 31 ? worksheetName.Substring(0, 31) : worksheetName;
            } catch (Exception ex) {
                _log.ErrorFormat("Error: InsertWorksheetPendingProcessInterval: {0}", ex);
            }
        }
    }
}
