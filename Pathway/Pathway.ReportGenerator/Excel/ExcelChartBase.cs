using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text.RegularExpressions;
using Pathway.ReportGenerator.Infrastructure;
using OfficeOpenXml;
using Pathway.Core.Infrastructure;
using Pathway.Core.RemoteAnalyst.Concrete;

namespace Pathway.ReportGenerator.Excel {
    internal class ExcelChartBase {
        protected readonly object MisValue = System.Reflection.Missing.Value;

        internal string ConvertNumberToChar(int columnNumber) {
            string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string returnValue = "";

            if (columnNumber > 0) {
                if ((columnNumber % 26) == 0) {
                    returnValue = ConvertNumberToChar((columnNumber / 26) - 1) + alphabet.Substring(25, 1);
                }
                else {
                    returnValue = ConvertNumberToChar((columnNumber / 26)) + alphabet.Substring((columnNumber % 26) - 1, 1);
                }
            }

            return returnValue;
        }

        internal void AddBorder(ExcelRange border) {
            border.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thick);
            border.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            border.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            border.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            border.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        }

        internal void AddTitle(ExcelRange title) {
            title.Style.Font.Bold = true;
            title.Style.Font.Size = 10;
            title.Style.Font.Color.SetColor(System.Drawing.Color.White);
            title.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            title.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Black);
            title.Style.Font.Name = "Courier New";
            title.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            title.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
        }

        internal void AddHeader(ExcelRange header) {
            header.Style.Font.Bold = true;
            header.Style.Font.Size = 10;
            header.Style.Font.Name = "Courier New";
            header.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            header.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            header.Style.Font.Color.SetColor(System.Drawing.Color.White);
            header.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            header.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Black);

        }
        internal void AddHeaderNoFormat(ExcelRange header) {
            header.Style.Font.Bold = true;
            header.Style.Font.Size = 10;
            header.Style.Font.Name = "Courier New";
            header.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            header.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        }

        internal void AddAlertSubTitle(ExcelRange sub) {
            sub.Style.Font.Bold = true;
            sub.Style.Font.Size = 10;
            sub.Style.Font.Name = "Courier New";
            sub.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            sub.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            sub.Style.Font.Color.SetColor(System.Drawing.Color.White);
            sub.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            sub.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Black);
            AddBorder(sub);
        }

        internal void ChangeFontAndWidth(ExcelRange font) {
            font.Style.Font.Name = "Courier New";
            font.Style.Font.Size = 8;
            font.AutoFitColumns();
        }

        internal void ChangeFont(ExcelRange font) {
            font.Style.Font.Name = "Courier New";
            font.Style.Font.Size = 8;
        }

        internal void InsertFreezeRow(ExcelWorksheet xlWorkSheet) {
            //Freeze first row.
            try
            {
                xlWorkSheet.View.FreezePanes(3, 1);
            }
            catch { }
        }

        internal void InsertFreezeColumn(ExcelWorksheet xlWorkSheet)
        {
            //Freeze first row.
            try
            {
                //todo
                //xlWorkSheet.Application.ActiveWindow.SplitColumn = 1;
                //xlWorkSheet.Application.ActiveWindow.FreezePanes = true;
            }
            catch { }
        }
        internal void InsertFreezeRowFirstThree(ExcelWorksheet xlWorkSheet) {
            //Freeze first row.
            try
            {
                xlWorkSheet.View.FreezePanes(4, 1);
            }
            catch { }
            
        }
        internal void InsertFreezeRowFirstFour(ExcelWorksheet xlWorkSheet) {
            //Freeze first row.
            try
            {
                xlWorkSheet.View.FreezePanes(5, 1);
            }
            catch { }
            
        }

        protected void InsertImage(ExcelWorksheet xlWorkSheet, int cellCount, int rowCount, IEnumerable<string> pathways,
                        string saveLocation, string dataDate,
                        Dictionary<string, List<ExcelFileInfo>> perPathwayDailies, string title) {

            var imageCellCount = cellCount;
            //Insert view image.
            foreach (var pathway in pathways) {
                imageCellCount++;
                var oRange = xlWorkSheet.Cells[ConvertNumberToChar(imageCellCount) + rowCount];

                #region Insert Hyperlink
                if (perPathwayDailies != null && perPathwayDailies.ContainsKey(pathway)) {
                    var reportDate = DateTime.MinValue;

                    if (!dataDate.Equals("Collection"))
                        reportDate = Convert.ToDateTime(dataDate);

                    //Find the matching entry on perPathwayDailies
                    var link = perPathwayDailies[pathway].Find(x => x.ExcelReportDate.Date == reportDate.Date);
                    if (link != null) {

                        System.Drawing.Image shape = System.Drawing.Image.FromFile(saveLocation + @"\view.gif");

                        var linkName = "";
                        switch (title) {
                            case "Alerts":
                                if (link.AlertWorksheetName.Length > 0) {
                                    linkName = "#'" + link.AlertWorksheetName + "'!A3";
                                }
                                break;
                            case "CPU Busy - Summary":
                                if (link.CPUSummaryWorksheetName.Length > 0) {
                                    linkName = "#'" + link.CPUSummaryWorksheetName + "'!A3";
                                }
                                break;
                            case "CPU Busy - Detail":
                                if (link.CPUDetailWorksheetName.Length > 0) {
                                    linkName = "#'" + link.CPUDetailWorksheetName + "'!A3";
                                }
                                break;
                            case "CPU Busy, Average":
                                if (link.CPUDetailWorksheetName.Length > 0) {
                                    linkName = "#'" + link.CPUDetailWorksheetName + "'!A3";
                                }
                                break;
                            case "SERVER Transactions":
                                if (link.ServerTransactionWorksheetName.Length > 0) {
                                    linkName = "#'" + link.ServerTransactionWorksheetName + "'!A3";
                                }
                                break;
                            case "Transactions, Total":
                                if (link.ServerTransactionWorksheetName.Length > 0) {
                                    linkName = "#'" + link.ServerTransactionWorksheetName + "'!A3";
                                }
                                break;
                            case "TCP Transactions":
                                if (link.TCPTransactionWorksheetName.Length > 0) {
                                    linkName = "#'" + link.TCPTransactionWorksheetName + "'!A3";
                                }
                                break;
                        }
                        var viewIcon = xlWorkSheet.Drawings.AddPicture(linkName + title, shape, new Uri(link.FileName + linkName, UriKind.Relative));
                        viewIcon.SetPosition(oRange.Start.Row - 1, 0, oRange.Start.Column - 1 , 0);
                    }
                }
                #endregion


                if (imageCellCount % 2 == 0) {
                    oRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    oRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }
            }
        }

        public static void InsertNavigate(ExcelWorksheet xlWorkSheet, string saveLocation, string previousSection, string previousWorksheet, string nextWorksheet, string nextSection, string worksheetToc, int pathwayCount = -1) {
            System.Drawing.Image logo = System.Drawing.Image.FromFile(saveLocation + @"\home.png");
            var RALogo = xlWorkSheet.Drawings.AddPicture("Go To Table of Contents", logo, new Uri("#'" + worksheetToc + "'!A3", UriKind.Relative));
            RALogo.SetPosition(1, 1);

            if (previousSection.Length > 0) {
                //Previous Section
                System.Drawing.Image shape1 = System.Drawing.Image.FromFile(saveLocation + @"\navigate_left2.png");
                var previousShape = xlWorkSheet.Drawings.AddPicture("previousSectionV1" + previousSection, shape1, new Uri("#'" + previousSection + "'!A3", UriKind.Relative));
                previousShape.SetPosition(2, 40);
                previousShape.SetSize(60);
            }

            if (previousWorksheet.Length > 0) {
                //Previous Worksheet
                System.Drawing.Image shape2 = System.Drawing.Image.FromFile(saveLocation + @"\navigate_left.png");
                var previousShape = xlWorkSheet.Drawings.AddPicture("previousWorksheetV1" + previousWorksheet, shape2, new Uri("#'" + previousWorksheet + "'!A3", UriKind.Relative));
                previousShape.SetPosition(2, 65);
                previousShape.SetSize(60);
            }

            if (nextWorksheet.Length > 0) {
                //Next Worksheet
                System.Drawing.Image shape3 = System.Drawing.Image.FromFile(saveLocation + @"\navigate_right.png");
                var previousShape = xlWorkSheet.Drawings.AddPicture("nextWorksheetV1" + nextWorksheet, shape3, new Uri("#'" + nextWorksheet + "'!A3", UriKind.Relative));
                previousShape.SetPosition(2, 90);
                previousShape.SetSize(60);
            }

            if (nextSection.Length > 0) {
                //Next Section
                System.Drawing.Image shape4 = System.Drawing.Image.FromFile(saveLocation + @"\navigate_right2.png");
                var previousShape = xlWorkSheet.Drawings.AddPicture("nextSectionV1" + nextSection, shape4, new Uri("#'" + nextSection + "'!A3", UriKind.Relative));
                previousShape.SetPosition(2, 115);
                previousShape.SetSize(60);
            }

            if (pathwayCount > 25) {
                xlWorkSheet.Cells["A1:" + ConvertNumberToCharStatic(pathwayCount + 1) + "1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                xlWorkSheet.Cells["A1:" + ConvertNumberToCharStatic(pathwayCount + 1) + "1"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGoldenrodYellow);
                xlWorkSheet.Cells["A1:" + ConvertNumberToCharStatic(pathwayCount + 1) + "1"].Merge = true;
            } else {
                xlWorkSheet.Cells["A1:z1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                xlWorkSheet.Cells["A1:z1"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGoldenrodYellow);
                xlWorkSheet.Cells["A1:Z1"].Merge = true;
            }
        }

        public static void InsertNavigateWorksheet(ExcelWorksheet xlWorkSheet, string saveLocation, string previousSection, string previousWorksheet, string nextWorksheet, string nextSection, string worksheetName) {
            //Home

            if (previousSection.Length > 0) {
                //Previous Section
                System.Drawing.Image shape1 = System.Drawing.Image.FromFile(saveLocation + @"\navigate_left2.png");
                var previousShape = xlWorkSheet.Drawings.AddPicture(worksheetName + "previousSectionV2", shape1, new Uri("#'" + previousSection + "'!A3", UriKind.Relative));
                previousShape.SetPosition(2, 40);
                previousShape.SetSize(60);
            }

            if (previousWorksheet.Length > 0) {
                //Previous Worksheet
                System.Drawing.Image shape2 = System.Drawing.Image.FromFile(saveLocation + @"\navigate_left.png");
                var previousShape = xlWorkSheet.Drawings.AddPicture(worksheetName + "previousWorksheetV2", shape2, new Uri("#'" + previousWorksheet + "'!A3", UriKind.Relative));
                previousShape.SetPosition(2, 65);
                previousShape.SetSize(60);
            }

            if (nextWorksheet.Length > 0) {
                //Next Worksheet
                System.Drawing.Image shape3 = System.Drawing.Image.FromFile(saveLocation + @"\navigate_right.png");
                var previousShape = xlWorkSheet.Drawings.AddPicture(worksheetName + "nextWorksheetV2", shape3, new Uri("#'" + nextWorksheet + "'!A3", UriKind.Relative));
                previousShape.SetPosition(2, 90);
                previousShape.SetSize(60);
            }

            if (nextSection.Length > 0) {
                //Next Section
                System.Drawing.Image shape4 = System.Drawing.Image.FromFile(saveLocation + @"\navigate_right2.png");
                var previousShape = xlWorkSheet.Drawings.AddPicture(worksheetName + "nextSectionV2", shape4, new Uri("#'" + nextSection + "'!A3", UriKind.Relative));
                previousShape.SetPosition(2, 115);
                previousShape.SetSize(60);
            }
        }

        public static void InsertErrorNavigate(ExcelWorksheet xlWorkSheet, string saveLocation, string worksheetToc) {
            var oRange = xlWorkSheet.Cells["A1"];

            System.Drawing.Image logo1 = System.Drawing.Image.FromFile(saveLocation + @"\home.png");
            var shape1 = xlWorkSheet.Drawings.AddPicture("", logo1, new Uri("#'" + worksheetToc + "'!A3", UriKind.Relative));
            shape1.SetPosition(1, 1);

            System.Drawing.Image shape2 = System.Drawing.Image.FromFile(saveLocation + @"\navigate_left.png");
            var previousShape = xlWorkSheet.Drawings.AddPicture("previousWorksheetV2ErrorPage", shape2, new Uri("#'" + xlWorkSheet.Workbook.Worksheets[xlWorkSheet.Workbook.Worksheets.Count - 1].Name.ToString() + "'!A3", UriKind.Relative));
            previousShape.SetPosition(2, 65);
            previousShape.SetSize(60);



            xlWorkSheet.Cells["A1:Z1"].Merge = true;
            xlWorkSheet.Cells["A1:Z1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            xlWorkSheet.Cells["A1:Z1"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGoldenrodYellow);
        }

        public static void InsertAlertHelpNavigate(ExcelWorksheet xlWorkSheet, string saveLocation, Enums.AlertReportTypes alertReportTypes, bool isLocalAnalyst) {
          
            var helpLink = "https://www.remoteanalyst.com/ranew/pathway-help/Pathway.html";
            if (isLocalAnalyst)
                helpLink = "https://www.remoteanalyst.com/pmc/pathway-help/Pathway.html";

            var page = "";
            System.Drawing.Image shape1 = System.Drawing.Image.FromFile(saveLocation + @"\help.gif");

            switch (alertReportTypes) {
                case Enums.AlertReportTypes.AlertCollection:
                    page = "Alerts";
                    break;
                case Enums.AlertReportTypes.QueueTCP:
                    page = "QueueTCP";
                    break;
                case Enums.AlertReportTypes.QueueLinkmon:
                    page = "QueueLinkmon";
                    break;
                case Enums.AlertReportTypes.UnusedClass:
                    page = "UnusedClass";
                    break;
                case Enums.AlertReportTypes.UnusedProcess:
                    page = "UnusedProcesses";
                    break;
                case Enums.AlertReportTypes.ErrorList:
                    page = "ErrorList";
                    break;
                case Enums.AlertReportTypes.QueueTCPSub:
                    page = "QueueTCPInterval";
                    break;
                case Enums.AlertReportTypes.QueueLinkmomSub:
                    page = "QueueLinkmonInterval";
                    break;
                case Enums.AlertReportTypes.ErrorListSub:
                    page = "ErrorListInterval";
                    break;
                case Enums.AlertReportTypes.HighMaxLinks:
                    page = "HighMaxLinks";
                    break;
                case Enums.AlertReportTypes.CheckDirectoryOn:
                    page = "CheckDirectoryOn";
                    break;
                case Enums.AlertReportTypes.HighDynamicServers:
                    page = "HighDynamicServers";
                    break;
            }

            var helpIcon = xlWorkSheet.Drawings.AddPicture(xlWorkSheet.Name, shape1, new Uri(helpLink + "#" + page, UriKind.Absolute));
            helpIcon.SetPosition(2, 150);
            xlWorkSheet.Cells["A1:Z1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            xlWorkSheet.Cells["A1:Z1"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGoldenrodYellow);
            xlWorkSheet.Cells["A1:Z1"].Merge = true;
        }

        public static void InsertPathwayHelpNavigate(ExcelWorksheet xlWorkSheet, string saveLocation, Enums.PathwayReportTypes pathwayReportTypes, bool isLocalAnalyst) {

            var helpLink = "https://www.remoteanalyst.com/ranew/pathwayfull-help/PathwayFull.html";
            if (isLocalAnalyst)
                helpLink = "https://www.remoteanalyst.com/pmc/pathwayfull-help/PathwayFull.html";

            var page = "";
            System.Drawing.Image shape1 = System.Drawing.Image.FromFile(saveLocation + @"\help.gif");

            switch (pathwayReportTypes) {
                case Enums.PathwayReportTypes.CpuBusy:
                    page = "CpuBusy";
                    break;
                case Enums.PathwayReportTypes.Transactions:
                    page = "Transactions";
                    break;
                case Enums.PathwayReportTypes.ServerCpuBusy:
                    page = "ServerCpuBusy";
                    break;
                case Enums.PathwayReportTypes.ServerTransactions:
                    page = "CpuServerTransactionsBusy";
                    break;
                case Enums.PathwayReportTypes.ServerQueuesTCP:
                    page = "ServerQueuesTCP";
                    break;
                case Enums.PathwayReportTypes.ServerQueuesLinkmon:
                    page = "ServerQueuesLinkmon";
                    break;
                case Enums.PathwayReportTypes.ServerUnusedClass:
                    page = "ServerUnusedClass";
                    break;
                case Enums.PathwayReportTypes.ServerUnusedProcesses:
                    page = "ServerUnusedProcesses";
                    break;
                case Enums.PathwayReportTypes.TCPTransactions:
                    page = "TCPTransactions";
                    break;
                case Enums.PathwayReportTypes.TCPQueues:
                    page = "TCPQueues";
                    break;
                case Enums.PathwayReportTypes.TCPUnused:
                    page = "TCPUnused";
                    break;
                case Enums.PathwayReportTypes.TermTransactions:
                    page = "TermTransactions";
                    break;
                case Enums.PathwayReportTypes.TermUnused:
                    page = "TermUnused";
                    break;
            }
            var helpIcon = xlWorkSheet.Drawings.AddPicture(xlWorkSheet.Name, shape1, new Uri(helpLink + "#" + page, UriKind.Absolute));
            helpIcon.SetPosition(2, 150);
            xlWorkSheet.Cells["A1:Z1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            xlWorkSheet.Cells["A1:Z1"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGoldenrodYellow);
            xlWorkSheet.Cells["A1:Z1"].Merge = true;
        }

        internal void InsertCollectionTOC(ExcelWorkbook xlWorkBook, int worksheetCount, List<string> pathways, Dictionary<String, Collection> days, string worksheetName, string saveLocation,
                                          Dictionary<string, List<ExcelFileInfo>> perPathwayFileInfo, bool isMultiDays) {
            //ExcelWorksheet xlWorkSheet = xlWorkBook.Worksheets[0];

            if (xlWorkBook.Worksheets.Count < worksheetCount)
                xlWorkBook.Worksheets.Add(worksheetName);

            ExcelWorksheet xlWorkSheet = (ExcelWorksheet)xlWorkBook.Worksheets[xlWorkBook.Worksheets.Count];

            var rowCount = 1;
            var cellCount = 3;

            //Set the cell size first. When resize the image at the end, some of the image extend.
            foreach (var pathway in pathways) {
                cellCount++;
                xlWorkSheet.Column(cellCount).Width = 9;
            }

            cellCount = 3;
            //Insert Pathways
            xlWorkSheet.Cells[rowCount, cellCount].Value = "All";
			//Only shows Pathway detail when request for one day report
			if (!isMultiDays) {
				foreach (string pathway in pathways) {
					cellCount++;
					xlWorkSheet.Cells[rowCount, cellCount].Value = pathway;

					//Add link to Pathway Collection Workbook.
					if (perPathwayFileInfo != null && perPathwayFileInfo.ContainsKey(pathway)) {
						var fineVal = perPathwayFileInfo[pathway].Find(x => x.ExcelReportDate == DateTime.MinValue);

                        if (fineVal != null) {
                            xlWorkSheet.Cells[rowCount, cellCount].Hyperlink = new Uri(fineVal.FileName, UriKind.Relative);
                        }
					}
				}
			}

            var header = xlWorkSheet.Cells["C1" + ":" + ConvertNumberToChar(cellCount) + "1"];
            AddHeaderNoFormat(header);
            AddBorder(header);

			if (!isMultiDays) {
				//Change background color.
				cellCount = 3;
				foreach (var pathway in pathways) {
					cellCount++;
					if (cellCount % 2 == 0) {
                        xlWorkSheet.Cells[ConvertNumberToChar(cellCount) + rowCount].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        xlWorkSheet.Cells[ConvertNumberToChar(cellCount) + rowCount].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
					}
				}
			}
            rowCount++;

            //var alertName = "Alerts";
            //var cpuSummary = "CPU Busy - Summary";
            var cpuDetail = "CPU Busy, Average";
            var serverTrans = "Transactions, Total";
            var hourlyPeakCPUBusyPerPathway = "Peak CPU Busy, Hourly";
            var hourlyAvgCPUBusyPerPathway = "Average CPU Busy, Hourly";
            var hourlyLinkmonTransactionCounts = "Linkmon Transaction Counts, Hourly";
            var hourlyTCPTransactionCounts = "TCP Transaction Counts, Hourly";
            var hourlyTransactionCounts = "Total Transaction Counts, Hourly";

            var tcpTrans = "TCP Transactions";

			//Set pathway to empty then table of content will not color cells
			if (isMultiDays) {
				pathways = new List<string>();
			}

            foreach (var day in days) {

                
                cellCount = 2;
                //Insert Image.
                var oRange = xlWorkSheet.Cells["A" + rowCount];
                var shape = System.Drawing.Image.FromFile(saveLocation + @"\collapse.gif");
                var helpIcon = xlWorkSheet.Drawings.AddPicture("collapse" + rowCount, shape);
                helpIcon.SetPosition(oRange.Start.Row - 1, 0, oRange.Start.Column - 1, 0);
                
                xlWorkSheet.Cells[rowCount, cellCount].Value = day.Key;
                var heading = xlWorkSheet.Cells[ConvertNumberToChar(cellCount) + rowCount];
                heading.Style.Font.Size = 10;
                heading.Style.Font.Bold = true;
                heading.Style.Font.Name = "Courier New";

                //Add Sub Category.
                rowCount++;
                cellCount++;
                int startRow = rowCount;


                xlWorkSheet.Cells[rowCount, cellCount].Value = cpuDetail;

                xlWorkSheet.Cells[rowCount, cellCount].Hyperlink = new Uri("#'" + day.Value.CPUDetail + "'!A3", UriKind.Relative);
                xlWorkSheet.Cells[rowCount, cellCount].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                xlWorkSheet.Cells[rowCount, cellCount].Style.Font.UnderLine = true;
                InsertImage(xlWorkSheet, cellCount, rowCount, pathways, saveLocation, day.Key, perPathwayFileInfo, cpuDetail);
                rowCount++;
                xlWorkSheet.Cells[rowCount, cellCount].Value = serverTrans;

                xlWorkSheet.Cells[rowCount, cellCount].Hyperlink = new Uri("#'" + day.Value.TransactionServer + "'!A3", UriKind.Relative);
                xlWorkSheet.Cells[rowCount, cellCount].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                xlWorkSheet.Cells[rowCount, cellCount].Style.Font.UnderLine = true;
                InsertImage(xlWorkSheet, cellCount, rowCount, pathways, saveLocation, day.Key, perPathwayFileInfo, serverTrans);
                rowCount++;
                if (isMultiDays) {
                    xlWorkSheet.Cells[rowCount, cellCount].Value = hourlyPeakCPUBusyPerPathway;
                    xlWorkSheet.Cells[rowCount, cellCount].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                    xlWorkSheet.Cells[rowCount, cellCount].Style.Font.UnderLine = true;
                    xlWorkSheet.Cells[rowCount, cellCount].Hyperlink = new Uri("#'" + day.Value.PeakCPUBusyHourly + "'!A3", UriKind.Relative);
                    rowCount++;

                    xlWorkSheet.Cells[rowCount, cellCount].Value = hourlyAvgCPUBusyPerPathway;
                    xlWorkSheet.Cells[rowCount, cellCount].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                    xlWorkSheet.Cells[rowCount, cellCount].Style.Font.UnderLine = true;
                    xlWorkSheet.Cells[rowCount, cellCount].Hyperlink = new Uri("#'" + day.Value.CPUBusyHourly + "'!A3", UriKind.Relative);
                    rowCount++;

                    xlWorkSheet.Cells[rowCount, cellCount].Value = hourlyLinkmonTransactionCounts;
                    xlWorkSheet.Cells[rowCount, cellCount].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                    xlWorkSheet.Cells[rowCount, cellCount].Style.Font.UnderLine = true;
                    xlWorkSheet.Cells[rowCount, cellCount].Hyperlink = new Uri("#'" + day.Value.TransactionLinkmon + "'!A3", UriKind.Relative);
                    rowCount++;

                    xlWorkSheet.Cells[rowCount, cellCount].Value = hourlyTCPTransactionCounts;
                    xlWorkSheet.Cells[rowCount, cellCount].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                    xlWorkSheet.Cells[rowCount, cellCount].Style.Font.UnderLine = true;
                    xlWorkSheet.Cells[rowCount, cellCount].Hyperlink = new Uri("#'" + day.Value.TransactionTCP + "'!A3", UriKind.Relative);
                    rowCount++;

                    xlWorkSheet.Cells[rowCount, cellCount].Value = hourlyTransactionCounts;
                    xlWorkSheet.Cells[rowCount, cellCount].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                    xlWorkSheet.Cells[rowCount, cellCount].Style.Font.UnderLine = true;
                    xlWorkSheet.Cells[rowCount, cellCount].Hyperlink = new Uri("#'" + day.Value.TransCountHourly + "'!A3", UriKind.Relative);
                }

                //InsertImage(xlWorkSheet, cellCount, rowCount, pathways, saveLocation, day.Key, perPathwayFileInfo, tcpTrans);

                //Border TERM
                var border = xlWorkSheet.Cells["C" + startRow + ":" + ConvertNumberToChar(pathways.Count + 3) + rowCount];
                AddBorder(border);
                rowCount++;
            }

            //Change Font.
            var sheetFontRange = xlWorkSheet.Cells["A1:C" + (rowCount)];
            ChangeFontAndWidth(sheetFontRange);

            sheetFontRange = xlWorkSheet.Cells["C2:Z" + (rowCount)];
            ChangeFont(sheetFontRange);

            //Khody's request to increate the font size and bold the sub category.

            xlWorkSheet.Column(1).Width = 2.86;
            xlWorkSheet.Column(2).Width = 30;
            xlWorkSheet.Column(3).Width = 36;

        }

        internal void InsertPathwayTOC(ExcelWorkbook xlWorkBook, int worksheetCount, string pathway, Dictionary<String, Collection> days, string worksheetName, string saveLocation, Dictionary<string, List<ExcelFileInfo>> perPathwayDailies = null) {
            if (xlWorkBook.Worksheets.Count < worksheetCount)
                xlWorkBook.Worksheets.Add(worksheetName);

            ExcelWorksheet xlWorkSheet = (ExcelWorksheet)xlWorkBook.Worksheets[xlWorkBook.Worksheets.Count];

            bool isInterval = false;
            if (perPathwayDailies == null)
                isInterval = true;

            var rowCount = 1;
            var loopCount = 0;
            foreach (var day in days) {
                var cellCount = 2;
                //Insert Image.
                var oRange = xlWorkSheet.Cells["A" + rowCount];

                System.Drawing.Image shape = System.Drawing.Image.FromFile(saveLocation + @"\collapse.gif");
                var collapseIcon = xlWorkSheet.Drawings.AddPicture("collapse" + rowCount, shape);
                collapseIcon.SetPosition(oRange.Start.Row - 1, 0, oRange.Start.Column - 1, 0);

                xlWorkSheet.Cells[rowCount, cellCount].Value = day.Key;
                if (!day.Key.Contains(" to ") && perPathwayDailies != null && perPathwayDailies.ContainsKey(pathway)) {
                    //Get date from the string.
                    string[] tempStrings = day.Key.Split('-');
                    var currentDate = Convert.ToDateTime(tempStrings[1] + "-" + tempStrings[2] + "-" + tempStrings[3]);
                    //Get file name base on key and date.
                    var selectedValue = perPathwayDailies[pathway].Find(x => x.ExcelReportDate.Date == currentDate.Date);

                    //Insert Hyperlink.
                    xlWorkSheet.Cells[rowCount, cellCount].Hyperlink = new Uri(selectedValue.FileName, UriKind.Relative);
                }
                var subTitle = xlWorkSheet.Cells["B" + rowCount]; //.Font.Bold = true;
                subTitle.Style.Font.Bold = true;
                subTitle.Style.Font.Name = "Courier New";
                subTitle.Style.Font.Size = 10;
                subTitle.AutoFitColumns();

                //Add Sub Category.
                rowCount++;
                cellCount++;
                cellCount++;

                int startRow = rowCount;
                if (loopCount == 0) {
                    xlWorkSheet.Cells[rowCount, cellCount].Value = "CPU Busy, Average";
                    if (CheckWorksheet(xlWorkBook, day.Value.CPUDetail)) {
                        xlWorkSheet.Cells[rowCount, cellCount].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                        xlWorkSheet.Cells[rowCount, cellCount].Style.Font.UnderLine = true;
                        xlWorkSheet.Cells[rowCount, cellCount].Hyperlink = new Uri("#'" + day.Value.CPUDetail + "'!A3", UriKind.Relative);
                    }
                    rowCount++;
                    xlWorkSheet.Cells[rowCount, cellCount].Value = "Transactions, Total";
                    if (CheckWorksheet(xlWorkBook, day.Value.TransactionServer)) {
                        xlWorkSheet.Cells[rowCount, cellCount].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                        xlWorkSheet.Cells[rowCount, cellCount].Style.Font.UnderLine = true;
                        xlWorkSheet.Cells[rowCount, cellCount].Hyperlink = new Uri("#'" + day.Value.TransactionServer + "'!A3", UriKind.Relative);
                    }
                    rowCount++;
                }
                else {
                    xlWorkSheet.Cells[rowCount, cellCount].Value = "CPU Busy, Average";
                    if (CheckWorksheet(xlWorkBook, day.Value.CPUDetail)) {
                        xlWorkSheet.Cells[rowCount, cellCount].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                        xlWorkSheet.Cells[rowCount, cellCount].Style.Font.UnderLine = true;
                        xlWorkSheet.Cells[rowCount, cellCount].Hyperlink = new Uri("#'" + day.Value.CPUDetail + "'!A3", UriKind.Relative);
                    }
                    rowCount++;
                }

                //SERVER
                oRange = xlWorkSheet.Cells["C" + rowCount];
                shape = System.Drawing.Image.FromFile(saveLocation + @"\collapse.gif");
                collapseIcon = xlWorkSheet.Drawings.AddPicture("collapse" + rowCount, shape);
                collapseIcon.SetPosition(oRange.Start.Row - 1, 0, oRange.Start.Column - 1, 0);
                collapseIcon.SetSize(0, 0);

                xlWorkSheet.Cells[rowCount, cellCount].Value = "SERVER";
                rowCount++;
                cellCount++;
            
                if (loopCount == 0) {
                    xlWorkSheet.Cells[rowCount, cellCount].Value = "CPU Busy, Average. Busiest Classes";
                    if (CheckWorksheet(xlWorkBook, day.Value.ServerCPUBusy)) {
                        xlWorkSheet.Cells[rowCount, cellCount].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                        xlWorkSheet.Cells[rowCount, cellCount].Style.Font.UnderLine = true;
                        xlWorkSheet.Cells[rowCount, cellCount].Hyperlink = new Uri("#'" + day.Value.ServerCPUBusy + "'!A3", UriKind.Relative);
                    }
                    rowCount++;
                    xlWorkSheet.Cells[rowCount, cellCount].Value = "Transactions, Total. Busiest Classes";
                    if (CheckWorksheet(xlWorkBook, day.Value.ServerTransactions)) {
                        xlWorkSheet.Cells[rowCount, cellCount].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                        xlWorkSheet.Cells[rowCount, cellCount].Style.Font.UnderLine = true;
                        xlWorkSheet.Cells[rowCount, cellCount].Hyperlink = new Uri("#'" + day.Value.ServerTransactions + "'!A3", UriKind.Relative);
                    }
                    rowCount++;
                    xlWorkSheet.Cells[rowCount, cellCount].Value = "Queues - TCP";
                    if (CheckWorksheet(xlWorkBook, day.Value.ServerQueuedTCP)) {
                        xlWorkSheet.Cells[rowCount, cellCount].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                        xlWorkSheet.Cells[rowCount, cellCount].Style.Font.UnderLine = true;
                        xlWorkSheet.Cells[rowCount, cellCount].Hyperlink = new Uri("#'" + day.Value.ServerQueuedTCP + "'!A3", UriKind.Relative);
                    }
                    rowCount++;
                    xlWorkSheet.Cells[rowCount, cellCount].Value = "Queues - Linkmon";
                    if (CheckWorksheet(xlWorkBook, day.Value.ServerQueuedLinkmon)) {
                        xlWorkSheet.Cells[rowCount, cellCount].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                        xlWorkSheet.Cells[rowCount, cellCount].Style.Font.UnderLine = true;
                        xlWorkSheet.Cells[rowCount, cellCount].Hyperlink = new Uri("#'" + day.Value.ServerQueuedLinkmon + "'!A3", UriKind.Relative);
                    }
                    rowCount++;
                    xlWorkSheet.Cells[rowCount, cellCount].Value = "Unused Classes";
                    if (CheckWorksheet(xlWorkBook, day.Value.ServerUnusedServerClasses)) {
                        xlWorkSheet.Cells[rowCount, cellCount].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                        xlWorkSheet.Cells[rowCount, cellCount].Style.Font.UnderLine = true;
                        xlWorkSheet.Cells[rowCount, cellCount].Hyperlink = new Uri("#'" + day.Value.ServerUnusedServerClasses + "'!A3", UriKind.Relative);
                    }
                    rowCount++;
                    xlWorkSheet.Cells[rowCount, cellCount].Value = "Unused Processes";
                    if (CheckWorksheet(xlWorkBook, day.Value.ServerUnusedServerProcesses)) {
                        xlWorkSheet.Cells[rowCount, cellCount].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                        xlWorkSheet.Cells[rowCount, cellCount].Style.Font.UnderLine = true;
                        xlWorkSheet.Cells[rowCount, cellCount].Hyperlink = new Uri("#'" + day.Value.ServerUnusedServerProcesses + "'!A3", UriKind.Relative);
                    }
                    rowCount++;
                } else {
                    xlWorkSheet.Cells[rowCount, cellCount].Value = "CPU Busy, Average. Busiest Classes";
                    if (CheckWorksheet(xlWorkBook, day.Value.ServerCPUBusy)) {
                        xlWorkSheet.Cells[rowCount, cellCount].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                        xlWorkSheet.Cells[rowCount, cellCount].Style.Font.UnderLine = true;
                        xlWorkSheet.Cells[rowCount, cellCount].Hyperlink = new Uri("#'" + day.Value.ServerCPUBusy + "'!A3", UriKind.Relative);
                    }
                    rowCount++;
                    xlWorkSheet.Cells[rowCount, cellCount].Value = "Transactions, Total. Busiest Classes";
                    if (CheckWorksheet(xlWorkBook, day.Value.ServerTransactions)) {
                        xlWorkSheet.Cells[rowCount, cellCount].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                        xlWorkSheet.Cells[rowCount, cellCount].Style.Font.UnderLine = true;
                        xlWorkSheet.Cells[rowCount, cellCount].Hyperlink = new Uri("#'" + day.Value.ServerTransactions + "'!A3", UriKind.Relative);
                    }
                    rowCount++;
                }

                //TCP
                cellCount--;
                oRange = xlWorkSheet.Cells["C" + rowCount];
                shape = System.Drawing.Image.FromFile(saveLocation + @"\collapse.gif");
                collapseIcon = xlWorkSheet.Drawings.AddPicture("collapse" + rowCount, shape);
                collapseIcon.SetPosition(oRange.Start.Row - 1, 0, oRange.Start.Column - 1, 0);
                collapseIcon.SetSize(0, 0);

                xlWorkSheet.Cells[rowCount, cellCount].Value = "TCP";
                rowCount++;
                cellCount++;
                xlWorkSheet.Cells[rowCount, cellCount].Value = "Transactions, Busiest";
                if (CheckWorksheet(xlWorkBook, day.Value.TCPTransactions)) {
                    xlWorkSheet.Cells[rowCount, cellCount].Hyperlink = new Uri("#'" + day.Value.TCPTransactions + "'!A3", UriKind.Relative);
                }
                rowCount++;
                if (loopCount == 0) {
                    xlWorkSheet.Cells[rowCount, cellCount].Value = "Queues";
                    if (CheckWorksheet(xlWorkBook, day.Value.TCPQueuedTransactions)) {
                        xlWorkSheet.Cells[rowCount, cellCount].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                        xlWorkSheet.Cells[rowCount, cellCount].Style.Font.UnderLine = true;
                        xlWorkSheet.Cells[rowCount, cellCount].Hyperlink = new Uri("#'" + day.Value.TCPQueuedTransactions + "'!A3", UriKind.Relative);
                    }
                    rowCount++;
                    xlWorkSheet.Cells[rowCount, cellCount].Value = "Unused";
                    if (CheckWorksheet(xlWorkBook, day.Value.TCPUnusedTCPs)) {
                        xlWorkSheet.Cells[rowCount, cellCount].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                        xlWorkSheet.Cells[rowCount, cellCount].Style.Font.UnderLine = true;
                        xlWorkSheet.Cells[rowCount, cellCount].Hyperlink = new Uri("#'" + day.Value.TCPUnusedTCPs + "'!A3", UriKind.Relative);
                    }
                    rowCount++;
                }

                if (loopCount == 0) {
                    //TERM
                    cellCount--;
                    oRange = xlWorkSheet.Cells["C" + rowCount];
                    shape = System.Drawing.Image.FromFile(saveLocation + @"\collapse.gif");
                    collapseIcon = xlWorkSheet.Drawings.AddPicture("collapse" + rowCount, shape);
                    collapseIcon.SetPosition(oRange.Start.Row - 1, 0, oRange.Start.Column - 1, 0);
                    collapseIcon.SetSize(0, 0);

                    xlWorkSheet.Cells[rowCount, cellCount].Value = "TERM";
                    rowCount++;
                    cellCount++;
                    xlWorkSheet.Cells[rowCount, cellCount].Value = "Transactions, Busiest";
                    if (CheckWorksheet(xlWorkBook, day.Value.TermTop20)) {
                        xlWorkSheet.Cells[rowCount, cellCount].Hyperlink = new Uri("#'" + day.Value.TermTop20 + "'!A3", UriKind.Relative);
                        xlWorkSheet.Cells[rowCount, cellCount].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                        xlWorkSheet.Cells[rowCount, cellCount].Style.Font.UnderLine = true;
                    }
                        rowCount++;
                    xlWorkSheet.Cells[rowCount, cellCount].Value = "Unused";
                    if (CheckWorksheet(xlWorkBook, day.Value.TermUnused)) {
                        xlWorkSheet.Cells[rowCount, cellCount].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                        xlWorkSheet.Cells[rowCount, cellCount].Style.Font.UnderLine = true;
                        xlWorkSheet.Cells[rowCount, cellCount].Hyperlink = new Uri("#'" + day.Value.TermUnused + "'!A3", UriKind.Relative);
                    }
                    rowCount++;
                }
                loopCount++;
            }
            //Change Font.
            var sheetFontRange = xlWorkSheet.Cells["D1:Z" + (rowCount)];
            ChangeFontAndWidth(sheetFontRange);

            xlWorkSheet.Column(1).Width = 2.86;
            xlWorkSheet.Column(3).Width = 2.86;

            xlWorkSheet.Name = worksheetName;
        }

        internal void GetWorksheetNavigation(ExcelWorkbook xlWorkBook, string saveLocation) {
            var navigateList = new Dictionary<string, NavigateView>();
            var currentKey = "";
            Regex dateReg = new Regex(@"\d{2}[-.]\d{1,2}[--.]\d{1,2}");
            var sectionList = new Dictionary<string, string>();

            #region Get Next, Previous WorkSheet.

            try {
                for (var x = 2; x <= xlWorkBook.Worksheets.Count; x++) {
                    var xlWorksheet = (ExcelWorksheet) xlWorkBook.Worksheets[x];
                    var navigate = new NavigateView();
                    //Check if current worksheet is part of collection.
                    string xlWorksheetName = xlWorksheet.Name;

                    if (xlWorksheetName.Contains("Col.") || xlWorksheetName.Contains("Ins.")){
                        #region Collection Section.

                        //First entry put in on SectionList.
                        if (!sectionList.ContainsKey("Col."))
                            sectionList.Add("Col.", xlWorksheetName);

                        //Check next and previus worksheet is also Col.
                        if (!x.Equals(2)) {
                            var xlWorksheetPreview = (ExcelWorksheet) xlWorkBook.Worksheets[x - 1];
                            if (xlWorksheetPreview.Name.Contains("Col.") || xlWorksheetPreview.Name.Contains("Ins."))
                                navigate.PreviousWorkSheet = xlWorksheetPreview.Name;
                            else
                                navigate.PreviousWorkSheet = "";
                        }
                        else
                            navigate.PreviousWorkSheet = "";

                        if (x < xlWorkBook.Worksheets.Count) {
                            var xlWorksheetNext = (ExcelWorksheet) xlWorkBook.Worksheets[x + 1];
                            if (xlWorksheetNext.Name.Contains("Col.") || xlWorksheetNext.Name.Contains("Ins"))
                                navigate.NextWorkSheet = xlWorksheetNext.Name;
                            else
                                navigate.NextWorkSheet = "";
                        }
                        else
                            navigate.NextWorkSheet = "";

                        if (!navigateList.ContainsKey(xlWorksheetName))
                            navigateList.Add(xlWorksheetName, navigate);

                        #endregion
                    }
                    else {
                        Match match = dateReg.Match(xlWorksheetName);

                        if (match.Success)
                        {
                            #region Multiday

                            var date = match.Value;
                            if (!sectionList.ContainsKey(date))
                                sectionList.Add(date, xlWorksheetName);
                            if (!x.Equals(2)) {//Set pre link
                                var xlWorksheetPreview = (ExcelWorksheet)xlWorkBook.Worksheets[x - 1];
                                if (xlWorksheetPreview.Name.Contains("Col.")){
                                    navigate.PreviousWorkSheet = xlWorksheetPreview.Name;
                                }
                                else{
                                    string xlWorksheetPreviewName = xlWorksheetPreview.Name;
                                    if (xlWorksheetPreviewName.Contains(date))
                                        navigate.PreviousWorkSheet = xlWorksheetPreviewName;
                                    else
                                        navigate.PreviousWorkSheet = "";
                                }
                            }
                            else {
                                navigate.PreviousWorkSheet = "";
                            }

                            if (x < xlWorkBook.Worksheets.Count) {//Set next link
                                var xlWorksheetNext = (ExcelWorksheet)xlWorkBook.Worksheets[x + 1];
                                string xlWorksheetNextName = xlWorksheetNext.Name;
                                if(xlWorksheetNextName.Contains(date))
                                    navigate.NextWorkSheet = xlWorksheetNextName;
                                else
                                    navigate.NextWorkSheet = "";
                            }
                            else {
                                navigate.NextWorkSheet = "";
                            }

                            if (!navigateList.ContainsKey(xlWorksheetName))
                                navigateList.Add(xlWorksheetName, navigate);
                            #endregion
                        }
                        else {
                            #region Hour

                            var names = xlWorksheetName.Split('-');
                            var time = names[1].Trim();
                            //put in on SectionList.
                            if (!sectionList.ContainsKey(time))
                                sectionList.Add(time, xlWorksheetName);

                            if (!x.Equals(2)){
                                var xlWorksheetPreview = (ExcelWorksheet)xlWorkBook.Worksheets[x - 1];
                                if (xlWorksheetPreview.Name.Contains("Col.")){
                                    navigate.PreviousWorkSheet = xlWorksheetPreview.Name;
                                }
                                else{
                                    if (xlWorksheetPreview.Name.Split('-')[1].Trim().Equals(time))
                                        navigate.PreviousWorkSheet = xlWorksheetPreview.Name;
                                    else
                                        navigate.PreviousWorkSheet = "";
                                }
                            }
                            else
                                navigate.PreviousWorkSheet = "";

                            if (x < xlWorkBook.Worksheets.Count){
                                var xlWorksheetNext = (ExcelWorksheet)xlWorkBook.Worksheets[x + 1];
                                if (xlWorksheetNext.Name.Split('-')[1].Trim().Equals(time))
                                    navigate.NextWorkSheet = xlWorksheetNext.Name;
                                else
                                    navigate.NextWorkSheet = "";
                            }
                            else
                                navigate.NextWorkSheet = "";

                            if (!navigateList.ContainsKey(xlWorksheetName))
                                navigateList.Add(xlWorksheetName, navigate);

                            #endregion
                        }
                    }
                }
            }
            catch (Exception ex) {
            }

            #endregion

            #region Next, Previous Section.
            foreach (var view in navigateList) {
                var name = view.Key;
                if (name.Contains("Col.") || name.Contains("Ins.")) {
                    view.Value.PreviousSection = "";
                    //find next section from sectionList.
                    for (var x = 0; x < sectionList.Count; x++) {
                        if (!sectionList.ElementAt(x).Key.Contains("Col.")) continue;
                        var nextIndex = x + 1;
                        if (nextIndex < sectionList.Count)
                            view.Value.NextSection = sectionList.ElementAt(nextIndex).Value;
                        break;
                    }
                }
                else {
                    Match match = dateReg.Match(name);
                    if (match.Success) {//Multi-day
                        var date = match.Value;
                        try {
                            for (var x = 0; x < sectionList.Count; x++) {
                                var tempName = sectionList.ElementAt(x).Key;
                                if (tempName.Contains("Col.")) continue;
                                if (!sectionList.ElementAt(x).Key.Equals(date)) continue;
                                var nextIndex = x + 1;
                                var previousIndex = x - 1;
                                view.Value.PreviousSection = sectionList.ElementAt(previousIndex).Value;

                                if (nextIndex < sectionList.Count)
                                    view.Value.NextSection = sectionList.ElementAt(nextIndex).Value;
                                else
                                    view.Value.NextSection = "";
                                break;
                            }
                        }
                        catch (Exception ex) {
                            throw new Exception(ex.Message);                                                      
                        }
                    }
                    else {
                        var time = name.Split('-')[1].Trim();
                        try {
                            for (var x = 0; x < sectionList.Count; x++) {
                                var tempName = sectionList.ElementAt(x).Key;
                                if (tempName.Contains("Col.")) continue;
                                if (!sectionList.ElementAt(x).Key.Equals(time)) continue;
                                var nextIndex = x + 1;
                                var previousIndex = x - 1;
                                view.Value.PreviousSection = sectionList.ElementAt(previousIndex).Value;

                                if (nextIndex < sectionList.Count)
                                    view.Value.NextSection = sectionList.ElementAt(nextIndex).Value;
                                else
                                    view.Value.NextSection = "";
                                break;
                            }
                        }
                        catch (Exception ex) {
                            throw new Exception(ex.Message);
                        }
                    }
                }
            }
            #endregion

            #region Insert Navigation to the worksheet.

            try {
                for (var x = 2; x <= xlWorkBook.Worksheets.Count; x++) {
                    var xlWorksheet = (ExcelWorksheet) xlWorkBook.Worksheets[x];
                    var view = navigateList[xlWorksheet.Name];
                    InsertNavigateWorksheet(xlWorksheet, saveLocation, view.PreviousSection, view.PreviousWorkSheet, view.NextWorkSheet, view.NextSection, xlWorksheet.Name);
                }
            }
            catch (Exception ex) { }

            #endregion
        }

        private bool CheckWorksheet(ExcelWorkbook xlWorkBook, string worksheetName) {
            bool worksheetExists = false;
            for (var x = 2; x <= xlWorkBook.Worksheets.Count; x++) {
                //var xlWorksheet = (Worksheet) xlWorkBook.Worksheets.Item[x];
                var xlWorksheet = (ExcelWorksheet)xlWorkBook.Worksheets[x];
                if (xlWorksheet.Name.Equals(worksheetName)) {
                    worksheetExists = true;
                    break;
                }
            }

            return worksheetExists;
        }

		private static string ConvertNumberToCharStatic(int columnNumber) {
			string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
			string returnValue = "";

			if (columnNumber > 0) {
				if ((columnNumber % 26) == 0) {
					returnValue = ConvertNumberToCharStatic((columnNumber / 26) - 1) + alphabet.Substring(25, 1);
				}
				else {
					returnValue = ConvertNumberToCharStatic((columnNumber / 26)) + alphabet.Substring((columnNumber % 26) - 1, 1);
				}
			}

			return returnValue;
		}
	}
}