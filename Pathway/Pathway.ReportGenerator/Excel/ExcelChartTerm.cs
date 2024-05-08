using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Pathway.Core.Infrastructure;
using Pathway.Core.Infrastructure.PerPathway.Server;
using Pathway.Core.Infrastructure.PerPathway.Term;
using Pathway.ReportGenerator.Infrastructure;
using Pathway.Core.RemoteAnalyst.Concrete;
using OfficeOpenXml;
using log4net;

namespace Pathway.ReportGenerator.Excel {
    class ExcelChartTerm : ExcelChartBase {
        private readonly ILog _log;
        private readonly int _reportDownloadId;
        private readonly ReportDownloadLogs _reportDownloadLogs;

        public ExcelChartTerm(int reportDownloadId, ReportDownloadLogs reportDownloadLogs, ILog log) {
            _reportDownloadId = reportDownloadId;
            _reportDownloadLogs = reportDownloadLogs;
            _log = log;
        }

        internal void InsertWorksheetTermTop20(ExcelWorkbook xlWorkBook, int worksheetCount, string list, Dictionary<string, List<TermTop20View>> views, string worksheetName, string saveLocation, string previousSection, string previousWorksheet, string nextWorksheet, string nextSection, string worksheetToc, string worksheetTitle, bool isLocalAnalyst) {
            try {
                if (xlWorkBook.Worksheets.Count < worksheetCount)
                    xlWorkBook.Worksheets.Add(worksheetName);
                //var xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.Item["Sheet" + Convert.ToString(worksheetCount)];
                var xlWorkSheet = (ExcelWorksheet)xlWorkBook.Worksheets[xlWorkBook.Worksheets.Count];

                var rowCount = 3;
                //Insert Sheet Title.
                xlWorkSheet.Cells[2, 1].Value = worksheetTitle;
                xlWorkSheet.Cells["A2:Z2"].Merge = true;
                var title = xlWorkSheet.Cells["A2"];
                AddTitle(title);

                var intervalList = views.Select((t, x) => views.Keys.ElementAt(x)).ToList();
                //var reports = new List<string> { "Transactions", "Avg. Response Time", "Max Response Time" };

                //xlWorkSheet.Cells[rowCount, 1] = "Transactions";
                #region Transactions
                //var subTitle = xlWorkSheet.Range["A" + rowCount, ConvertNumberToChar((views.Count * 3)) + rowCount];
                //subTitle.MergeCells = true;
                //AddHeader(subTitle);
                //rowCount++;

                var dataStartRow = rowCount;
                var serverTitle = xlWorkSheet.Cells["A" + dataStartRow + ":" + ConvertNumberToChar((views.Count * 3) + 1) + (dataStartRow + 1)];
                serverTitle.Style.Numberformat.Format = "@";
                var column = 0;
                int subDataStartRow = 0;
                foreach (var hour in intervalList) {

                    // Do not show the TERM colume at this hour if all transaction values are 0
                    if (views[hour].Where(x => x.TransactionValue > 0).Count() == 0)
                        continue;

                    subDataStartRow = rowCount;
                    xlWorkSheet.Cells[rowCount, (column + 1)].Value = hour;

                    //Merge the cells.
                    xlWorkSheet.Cells[ConvertNumberToChar(column + 1) + rowCount + ":" + ConvertNumberToChar(column + 3) + rowCount].Merge = true;
                    subDataStartRow++;

                    //Add Sub Title.
                    xlWorkSheet.Cells[subDataStartRow, (column + 1)].Value = "TERM";
                    xlWorkSheet.Cells[subDataStartRow, (column + 2)].Value = "Transaction";
                    xlWorkSheet.Cells[subDataStartRow, (column + 3)].Value = "TCP";



                    if (views[hour].Count == 0) {
                        subDataStartRow++;
                        //Insert No data.
                        xlWorkSheet.Cells[subDataStartRow, (column + 1)].Value = "No data found!";

                        var noData = xlWorkSheet.Cells[ConvertNumberToChar((column + 1)) + subDataStartRow + ":" + ConvertNumberToChar(column + 3) + subDataStartRow];
                        noData.Merge = true;
                        noData.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        noData.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        noData.Style.Font.Color.SetColor(System.Drawing.Color.Red);

                        var border = xlWorkSheet.Cells[ConvertNumberToChar((column + 1)) + rowCount + ":" + ConvertNumberToChar((column + 3)) + subDataStartRow];
                        if (intervalList.IndexOf(hour) % 2 == 0) {
                            border.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            border.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                        }
                        AddBorder(border);
                        ChangeFontAndWidth(border);
                    } else {
                        foreach (var view in views[hour]) {
                            subDataStartRow++;
                            xlWorkSheet.Cells[subDataStartRow, (column + 1)].Value = view.TransactionTerm;
                            xlWorkSheet.Cells[subDataStartRow, (column + 2)].Value = view.TransactionValue;
                            xlWorkSheet.Cells[subDataStartRow, (column + 3)].Value = view.TransactionTcp;
                        }
                        var border = xlWorkSheet.Cells[ConvertNumberToChar((column + 1)) + rowCount + ":" + ConvertNumberToChar((column + 3)) + subDataStartRow];
                        if (intervalList.IndexOf(hour) % 2 == 0) {
                            border.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            border.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                        }
                        AddBorder(border);
                        ChangeFontAndWidth(border);
                    }


                    var dataColumn = xlWorkSheet.Cells[ConvertNumberToChar((column + 2)) + rowCount + ":" + ConvertNumberToChar((column + 2)) + subDataStartRow];
                    dataColumn.Style.Numberformat.Format = "#,##0";
                    dataColumn.ConditionalFormatting.AddDatabar(System.Drawing.Color.RoyalBlue);
                    column += 3;
                }

                rowCount = subDataStartRow;
                AddHeaderNoFormat(serverTitle);
                serverTitle.AutoFitColumns();
                rowCount++;
                #endregion

                //Freeze first row.InsertWorksheetTransactionTcpPerPathway
                InsertFreezeRowFirstFour(xlWorkSheet);

                InsertNavigate(xlWorkSheet, saveLocation, previousSection, previousWorksheet, nextWorksheet, nextSection, worksheetToc);


                InsertPathwayHelpNavigate(xlWorkSheet, saveLocation, Enums.PathwayReportTypes.TermTransactions, isLocalAnalyst);

                xlWorkSheet.Name = worksheetName;
            } catch (Exception ex)
            {
                _log.ErrorFormat("Error: InsertWorksheetTermTop20: {0}: {1}", worksheetName, ex);
                _reportDownloadLogs.InsertNewLog(_reportDownloadId, DateTime.Now, "There was an error generating the following worksheet: " + worksheetName);
            }

        }

        internal void InsertWorksheetTermUnused(ExcelWorkbook xlWorkBook, int worksheetCount, string list, Dictionary<string, List<TermUnusedView>> views, string worksheetName, string saveLocation, string previousSection, string previousWorksheet,
            string nextWorksheet, string nextSection, string worksheetToc, string worksheetTitle, bool isLocalAnalyst) {
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
                var serverTitle = xlWorkSheet.Cells["A" + dataStartRow + ":" + ConvertNumberToChar((views.Count * 2) + 1) + (dataStartRow + 1)];
                serverTitle.Style.Numberformat.Format = "@";

                var hourList = views.Select((t, x) => views.Keys.ElementAt(x)).ToList();

                var column = 0;
                foreach (var hour in hourList) {
                    var dataStartrowCount = 3;
                    xlWorkSheet.Cells[rowCount, (column + 1)].Value = hour;

                    //Merge the cells.
                    xlWorkSheet.Cells[ConvertNumberToChar(column + 1) + rowCount + ":" + ConvertNumberToChar(column + 2) + rowCount].Merge = true;
                    dataStartrowCount++;

                    //Add Sub Title.
                    xlWorkSheet.Cells[dataStartrowCount, (column + 1)].Value = "TCP";
                    xlWorkSheet.Cells[dataStartrowCount, (column + 2)].Value = "TERM";

                    if (views[hour].Count == 0) {
                        dataStartrowCount++;
                        //Insert No data.
                        xlWorkSheet.Cells[dataStartrowCount, (column + 1)].Value = "No data found!";

                        var noData = xlWorkSheet.Cells[ConvertNumberToChar((column + 1)) + dataStartrowCount + ":" + ConvertNumberToChar(column + 2) + dataStartrowCount];
                        noData.Merge = true;
                        noData.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        noData.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        noData.Style.Font.Color.SetColor(System.Drawing.Color.Red);

                        var border = xlWorkSheet.Cells[ConvertNumberToChar((column + 1)) + rowCount + ":" + ConvertNumberToChar((column + 2)) + dataStartrowCount];
                        if (hourList.IndexOf(hour) % 2 == 0) {
                            border.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            border.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                        }
                        AddBorder(border);
                        ChangeFontAndWidth(border);
                        xlWorkSheet.Column(column + 1).Width = 10;
                        xlWorkSheet.Column(column + 2).Width = 10;
                    } else {
                        foreach (var view in views[hour]) {
                            dataStartrowCount++;
                            xlWorkSheet.Cells[dataStartrowCount, (column + 1)].Value = view.Tcp;
                            xlWorkSheet.Cells[dataStartrowCount, (column + 2)].Value = view.Term;
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
                //serverTitle.EntireColumn.AutoFit();

                //Freeze first row.InsertWorksheetTransactionTcpPerPathway
                InsertFreezeRowFirstFour(xlWorkSheet);

                InsertNavigate(xlWorkSheet, saveLocation, previousSection, previousWorksheet, nextWorksheet, nextSection, worksheetToc);


                InsertPathwayHelpNavigate(xlWorkSheet, saveLocation, Enums.PathwayReportTypes.TermUnused, isLocalAnalyst);

                xlWorkSheet.Name = worksheetName;
            } catch (Exception ex) {
                _log.ErrorFormat("Error: InsertWorksheetTermUnused: {0}: {1}", worksheetName, ex);
                _reportDownloadLogs.InsertNewLog(_reportDownloadId, DateTime.Now, "There was an error generating the following worksheet: " + worksheetName);
            }

        }
    }
}
