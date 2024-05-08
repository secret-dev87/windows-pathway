using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.ReportGenerator.Excel {
    class Macros {
        internal string GenerateThisWorkbookCollection() {
            string header = @"Private Sub Workbook_Open()
                                Call initializeShape_Collection_ClickEvents
                              End Sub";

            return header;
        }

        internal string GenerateThisWorkbookPathway() {
            string header = @"Private Sub Workbook_Open()
                                Call initializeShape_PerPathway_ClickEvents
                                End Sub";

            return header;
        }

        internal string GenerateThisWorkbookPreviousWorksheet() {
            var header = new StringBuilder();
            header.Append(@"Private Sub Workbook_Open()
                            mySheetName = ActiveSheet.Name
                         End Sub

                         Private Sub Workbook_SheetActivate(ByVal Sh As Object)
                             OldSheetName = mySheetName
                             mySheetName = Sh.Name
                         End Sub");

            return header.ToString();
        }

        internal string GenerateCollectionMacro(string worksheetName) {
            var macro = new StringBuilder();
            macro.Append("Dim strSheetName As String\n");

            #region Help
            macro.Append("Sub TerminateProcess()\n");
            macro.Append("	Dim strTerminateThis As String 'The variable to hold the process to terminate\n");
            macro.Append("	Dim objWMIcimv2 As Object\n");
            macro.Append("	Dim objProcess As Object\n");
            macro.Append("	Dim objList As Object\n");
            macro.Append("	Dim intError As Integer\n");
            macro.Append("	'HTML Help component hh.exe\n");
            macro.Append("	strTerminateThis = \"hh.exe\"\n");
            macro.Append("	Set objWMIcimv2 = GetObject(\"winmgmts:{impersonationLevel=impersonate}!\\.\\root\\cimv2\") 'Connect to CIMV2 Namespace\n");
            macro.Append("	Set objList = objWMIcimv2.ExecQuery(\"select * from win32_process where name='\" & strTerminateThis & \"'\")  'Find the process to terminate\n");
            macro.Append("	If objList.count = 0 Then 'If 0 then process isn't running\n");
            macro.Append("		Set objWMIcimv2 = Nothing\n");
            macro.Append("		Set objList = Nothing\n");
            macro.Append("		Set objProcess = Nothing\n");
            macro.Append("		Exit Sub\n");
            macro.Append("	Else\n");
            macro.Append("		For Each objProcess In objList\n");
            macro.Append("			intError = objProcess.Terminate 'Terminates a process and all of its threads.\n");
            macro.Append("			If intError <> 0 Then\n");
            macro.Append("				Exit Sub\n");
            macro.Append("			End If\n");
            macro.Append("		Next\n");
            macro.Append("		Set objWMIcimv2 = Nothing\n");
            macro.Append("		Set objList = Nothing\n");
            macro.Append("		Set objProcess = Nothing\n");
            macro.Append("		Exit Sub\n");
            macro.Append("	End If\n");
            macro.Append("End Sub\n");

            macro.Append("Public Function FileExistsFunctiom(Path As String) As Boolean\n");
            macro.Append("	Dim objFSO As Object\n");
            macro.Append("	Set objFSO = CreateObject(\"Scripting.FileSystemObject\")\n");
            macro.Append("	Select Case objFSO.FileExists(Path)\n");
            macro.Append("	Case True\n");
            macro.Append("		FileExistsFunctiom = True\n");
            macro.Append("	Case False\n");
            macro.Append("		FileExistsFunctiom = False\n");
            macro.Append("	End Select\n");
            macro.Append("	Set objFSO = Nothing\n");
            macro.Append("End Function\n");

            macro.Append("Sub OpenHelpCpuBusy()\n");
            macro.Append("	On Error Resume Next\n");
            macro.Append("	TerminateProcess\n");
            macro.Append("	If FileExistsFunctiom(ThisWorkbook.Path + \"\\Pathway.txt\") = True Then\n");
            macro.Append("		FileCopy ThisWorkbook.Path + \"\\Pathway.txt\", ThisWorkbook.Path + \"\\Pathway.chm\"\n");
            macro.Append("		Kill ThisWorkbook.Path + \"\\Pathway.txt\"\n");
            macro.Append("	End If\n");
            macro.Append("		Application.Help ThisWorkbook.Path + \"\\Pathway.chm\", 2001\n");
            macro.Append("End Sub\n");

            macro.Append("Sub OpenHelpTransactions()\n");
            macro.Append("	On Error Resume Next\n");
            macro.Append("	TerminateProcess\n");
            macro.Append("	If FileExistsFunctiom(ThisWorkbook.Path + \"\\Pathway.txt\") = True Then\n");
            macro.Append("		FileCopy ThisWorkbook.Path + \"\\Pathway.txt\", ThisWorkbook.Path + \"\\Pathway.chm\"\n");
            macro.Append("		Kill ThisWorkbook.Path + \"\\Pathway.txt\"\n");
            macro.Append("	End If\n");
            macro.Append("		Application.Help ThisWorkbook.Path + \"\\Pathway.chm\", 2002\n");
            macro.Append("End Sub\n");
            #endregion

            macro.Append("Sub initializeShape_Collection_ClickEvents()\n");

            macro.Append("    On Error GoTo ErrMsg\n    'Sheet2.Select'\n");
            macro.Append("    'Sheet1.Select'\n");
            macro.Append("    strSheetName = \"" + worksheetName + "\"\n");
            macro.Append("    Dim theShape As Shape, theSheet As Worksheet\n");
            macro.Append("    For Each theShape In Worksheets(strSheetName).Shapes\n");
            macro.Append("        If theShape.TopLeftCell.Column = 1 Then\n");
            macro.Append("            If theShape.TopLeftCell.Row <> 1 Then\n");
            macro.Append("                theShape.OnAction = \"'TOC_EventClicked \"\"\" & theShape.Name & \"\"\", \"\"\" & theShape.TopLeftCell.Row & \"\"\", \"\"\" & theShape.TopLeftCell.Column & \"\"\"'\"\n");
            macro.Append("                If theShape.TopLeftCell.Row <> 2 Then\n");
            macro.Append("                    Call TOC_EventClicked(theShape.Name, theShape.TopLeftCell.Row, theShape.TopLeftCell.Column)\n");
            macro.Append("                End If\n");
            macro.Append("            End If\n");
            macro.Append("        End If\n");
            macro.Append("    Next theShape\n");
            macro.Append("    Range(\"B1\").Select\n");
            macro.Append("    ErrMsg:\n");
            macro.Append("    If Err.Number <> 0 Then\n");
            macro.Append("      msg = \"The report was opened in the protected or read only mode.\" & vbCrLf & _\n");
            macro.Append("      \"     Cause: The file may have been opened within the zip file\" & vbCrLf & _\n");
            macro.Append("      \"     Effect: Macros/functionality may not work correctly.\" & vbCrLf & _\n");
            macro.Append("      \"     Recovery: Extract the report(s) from the archive (zip file) and open the report again.\"\n");
            macro.Append("      MsgBox msg\n");
            macro.Append("    End If\n");
            macro.Append("End Sub\n");

            macro.Append("Public Function TOC_EventClicked(ByVal theShapeName As String, ByVal theCellRow As Integer, ByVal theCellColumn As Integer)\n");
            macro.Append("    'Worksheets(strSheetName).Shapes(theShapeName).Delete\n");
            macro.Append("    'Worksheets(strSheetName).Cells(theCellRow, theCellColumn).Select\n");
            macro.Append("    Set newPic = Worksheets(strSheetName).Pictures.Insert(ActiveWorkbook.Path & \"\\expand.gif\")\n");

            macro.Append("    newPic.Left = Cells(theCellRow, theCellColumn).Left + 1\n");
            macro.Append("    newPic.Top = Cells(theCellRow, theCellColumn).Top + 2\n");

            macro.Append("    'Hide picture when Hidden\n");
            macro.Append("    newPic.Placement = xlMoveAndSize\n");
            macro.Append("    Dim startRow As Integer\n");
            macro.Append("    Dim endRow As Integer\n");
            macro.Append("    startRow = (theCellRow + 1)\n");
            macro.Append("    endRow = startRow + 6\n");

            macro.Append("    'Hide Rows.\n");
            macro.Append("    Rows(startRow & \":\" & endRow).Select\n");
            macro.Append("    Selection.EntireRow.Hidden = True\n");

            macro.Append("    newPic.OnAction = \"'TOC_ExpandClicked \"\"\" & newPic.Name & \"\"\", \"\"\" & theCellRow & \"\"\", \"\"\" & theCellColumn & \"\"\"'\"\n");
            macro.Append("End Function\n");

            macro.Append("Public Function TOC_ExpandClicked(ByVal theShapeName As String, ByVal theCellRow As Integer, ByVal theCellColumn As Integer)\n");
            macro.Append("    'Worksheets(strSheetName).Shapes(theShapeName).Delete\n");
            macro.Append("    'Worksheets(strSheetName).Cells(theCellRow, theCellColumn).Select\n");
            macro.Append("    Set newPic = Worksheets(strSheetName).Pictures.Insert(ActiveWorkbook.Path & \"\\collapse.gif\")\n");
            macro.Append("    newPic.Left = Cells(theCellRow, theCellColumn).Left + 1\n");
            macro.Append("    newPic.Top = Cells(theCellRow, theCellColumn).Top + 2\n");

            macro.Append("    'Hide picture when Hidden\n");
            macro.Append("    newPic.Placement = xlMoveAndSize\n");
            macro.Append("    Dim startRow As Integer\n");
            macro.Append("    Dim endRow As Integer\n");
            macro.Append("    startRow = (theCellRow + 1)\n");
            macro.Append("    endRow = startRow + 6\n");

            macro.Append("    'Show Rows.\n");
            macro.Append("    Rows(startRow & \":\" & endRow).Select\n");
            macro.Append("    Selection.EntireRow.Hidden = False\n");

            macro.Append("    newPic.OnAction = \"'TOC_EventClicked \"\"\" & newPic.Name & \"\"\", \"\"\" & theCellRow & \"\"\", \"\"\" & theCellColumn & \"\"\"'\"\n");
            macro.Append("End Function\n");

            return macro.ToString();
        }

        internal string GeneratePathwayMacroInterval(string worksheetName) {
            var macro = new StringBuilder();
            macro.Append("Dim strSheetName As String\n");

            #region Help
            macro.Append("Sub TerminateProcess()\n");
            macro.Append("	Dim strTerminateThis As String 'The variable to hold the process to terminate\n");
            macro.Append("	Dim objWMIcimv2 As Object\n");
            macro.Append("	Dim objProcess As Object\n");
            macro.Append("	Dim objList As Object\n");
            macro.Append("	Dim intError As Integer\n");
            macro.Append("	'HTML Help component hh.exe\n");
            macro.Append("	strTerminateThis = \"hh.exe\"\n");
            macro.Append("	Set objWMIcimv2 = GetObject(\"winmgmts:{impersonationLevel=impersonate}!\\.\\root\\cimv2\") 'Connect to CIMV2 Namespace\n");
            macro.Append("	Set objList = objWMIcimv2.ExecQuery(\"select * from win32_process where name='\" & strTerminateThis & \"'\")  'Find the process to terminate\n");
            macro.Append("	If objList.count = 0 Then 'If 0 then process isn't running\n");
            macro.Append("		Set objWMIcimv2 = Nothing\n");
            macro.Append("		Set objList = Nothing\n");
            macro.Append("		Set objProcess = Nothing\n");
            macro.Append("		Exit Sub\n");
            macro.Append("	Else\n");
            macro.Append("		For Each objProcess In objList\n");
            macro.Append("			intError = objProcess.Terminate 'Terminates a process and all of its threads.\n");
            macro.Append("			If intError <> 0 Then\n");
            macro.Append("				Exit Sub\n");
            macro.Append("			End If\n");
            macro.Append("		Next\n");
            macro.Append("		Set objWMIcimv2 = Nothing\n");
            macro.Append("		Set objList = Nothing\n");
            macro.Append("		Set objProcess = Nothing\n");
            macro.Append("		Exit Sub\n");
            macro.Append("	End If\n");
            macro.Append("End Sub\n");

            macro.Append("Public Function FileExistsFunctiom(Path As String) As Boolean\n");
            macro.Append("	Dim objFSO As Object\n");
            macro.Append("	Set objFSO = CreateObject(\"Scripting.FileSystemObject\")\n");
            macro.Append("	Select Case objFSO.FileExists(Path)\n");
            macro.Append("	Case True\n");
            macro.Append("		FileExistsFunctiom = True\n");
            macro.Append("	Case False\n");
            macro.Append("		FileExistsFunctiom = False\n");
            macro.Append("	End Select\n");
            macro.Append("	Set objFSO = Nothing\n");
            macro.Append("End Function\n");

            macro.Append("Sub OpenHelpCpuBusy()\n");
            macro.Append("	On Error Resume Next\n");
            macro.Append("	TerminateProcess\n");
            macro.Append("	If FileExistsFunctiom(ThisWorkbook.Path + \"\\Pathway.txt\") = True Then\n");
            macro.Append("		FileCopy ThisWorkbook.Path + \"\\Pathway.txt\", ThisWorkbook.Path + \"\\Pathway.chm\"\n");
            macro.Append("		Kill ThisWorkbook.Path + \"\\Pathway.txt\"\n");
            macro.Append("	End If\n");
            macro.Append("		Application.Help ThisWorkbook.Path + \"\\Pathway.chm\", 2001\n");
            macro.Append("End Sub\n");

            macro.Append("Sub OpenHelpTransactions()\n");
            macro.Append("	On Error Resume Next\n");
            macro.Append("	TerminateProcess\n");
            macro.Append("	If FileExistsFunctiom(ThisWorkbook.Path + \"\\Pathway.txt\") = True Then\n");
            macro.Append("		FileCopy ThisWorkbook.Path + \"\\Pathway.txt\", ThisWorkbook.Path + \"\\Pathway.chm\"\n");
            macro.Append("		Kill ThisWorkbook.Path + \"\\Pathway.txt\"\n");
            macro.Append("	End If\n");
            macro.Append("		Application.Help ThisWorkbook.Path + \"\\Pathway.chm\", 2002\n");
            macro.Append("End Sub\n");

            macro.Append("Sub OpenHelpServerCpuBusyn()\n");
            macro.Append("	On Error Resume Next\n");
            macro.Append("	TerminateProcess\n");
            macro.Append("	If FileExistsFunctiom(ThisWorkbook.Path + \"\\Pathway.txt\") = True Then\n");
            macro.Append("		FileCopy ThisWorkbook.Path + \"\\Pathway.txt\", ThisWorkbook.Path + \"\\Pathway.chm\"\n");
            macro.Append("		Kill ThisWorkbook.Path + \"\\Pathway.txt\"\n");
            macro.Append("	End If\n");
            macro.Append("		Application.Help ThisWorkbook.Path + \"\\Pathway.chm\", 2003\n");
            macro.Append("End Sub\n");

            macro.Append("Sub OpenHelpServerTransactions()\n");
            macro.Append("	On Error Resume Next\n");
            macro.Append("	TerminateProcess\n");
            macro.Append("	If FileExistsFunctiom(ThisWorkbook.Path + \"\\Pathway.txt\") = True Then\n");
            macro.Append("		FileCopy ThisWorkbook.Path + \"\\Pathway.txt\", ThisWorkbook.Path + \"\\Pathway.chm\"\n");
            macro.Append("		Kill ThisWorkbook.Path + \"\\Pathway.txt\"\n");
            macro.Append("	End If\n");
            macro.Append("		Application.Help ThisWorkbook.Path + \"\\Pathway.chm\", 2004\n");
            macro.Append("End Sub\n");

            macro.Append("Sub OpenHelpServerQueuesTCP()\n");
            macro.Append("	On Error Resume Next\n");
            macro.Append("	TerminateProcess\n");
            macro.Append("	If FileExistsFunctiom(ThisWorkbook.Path + \"\\Pathway.txt\") = True Then\n");
            macro.Append("		FileCopy ThisWorkbook.Path + \"\\Pathway.txt\", ThisWorkbook.Path + \"\\Pathway.chm\"\n");
            macro.Append("		Kill ThisWorkbook.Path + \"\\Pathway.txt\"\n");
            macro.Append("	End If\n");
            macro.Append("		Application.Help ThisWorkbook.Path + \"\\Pathway.chm\", 2005\n");
            macro.Append("End Sub\n");

            macro.Append("Sub OpenHelpServerQueuesLinkmon()\n");
            macro.Append("	On Error Resume Next\n");
            macro.Append("	TerminateProcess\n");
            macro.Append("	If FileExistsFunctiom(ThisWorkbook.Path + \"\\Pathway.txt\") = True Then\n");
            macro.Append("		FileCopy ThisWorkbook.Path + \"\\Pathway.txt\", ThisWorkbook.Path + \"\\Pathway.chm\"\n");
            macro.Append("		Kill ThisWorkbook.Path + \"\\Pathway.txt\"\n");
            macro.Append("	End If\n");
            macro.Append("		Application.Help ThisWorkbook.Path + \"\\Pathway.chm\", 2006\n");
            macro.Append("End Sub\n");

            macro.Append("Sub OpenHelpServerUnusedClass()\n");
            macro.Append("	On Error Resume Next\n");
            macro.Append("	TerminateProcess\n");
            macro.Append("	If FileExistsFunctiom(ThisWorkbook.Path + \"\\Pathway.txt\") = True Then\n");
            macro.Append("		FileCopy ThisWorkbook.Path + \"\\Pathway.txt\", ThisWorkbook.Path + \"\\Pathway.chm\"\n");
            macro.Append("		Kill ThisWorkbook.Path + \"\\Pathway.txt\"\n");
            macro.Append("	End If\n");
            macro.Append("		Application.Help ThisWorkbook.Path + \"\\Pathway.chm\", 2007\n");
            macro.Append("End Sub\n");

            macro.Append("Sub OpenHelpServerUnusedProcesses()\n");
            macro.Append("	On Error Resume Next\n");
            macro.Append("	TerminateProcess\n");
            macro.Append("	If FileExistsFunctiom(ThisWorkbook.Path + \"\\Pathway.txt\") = True Then\n");
            macro.Append("		FileCopy ThisWorkbook.Path + \"\\Pathway.txt\", ThisWorkbook.Path + \"\\Pathway.chm\"\n");
            macro.Append("		Kill ThisWorkbook.Path + \"\\Pathway.txt\"\n");
            macro.Append("	End If\n");
            macro.Append("		Application.Help ThisWorkbook.Path + \"\\Pathway.chm\", 2008\n");
            macro.Append("End Sub\n");

            macro.Append("Sub OpenHelpTCPTransactions()\n");
            macro.Append("	On Error Resume Next\n");
            macro.Append("	TerminateProcess\n");
            macro.Append("	If FileExistsFunctiom(ThisWorkbook.Path + \"\\Pathway.txt\") = True Then\n");
            macro.Append("		FileCopy ThisWorkbook.Path + \"\\Pathway.txt\", ThisWorkbook.Path + \"\\Pathway.chm\"\n");
            macro.Append("		Kill ThisWorkbook.Path + \"\\Pathway.txt\"\n");
            macro.Append("	End If\n");
            macro.Append("		Application.Help ThisWorkbook.Path + \"\\Pathway.chm\", 2009\n");
            macro.Append("End Sub\n");

            macro.Append("Sub OpenHelpTCPQueues()\n");
            macro.Append("	On Error Resume Next\n");
            macro.Append("	TerminateProcess\n");
            macro.Append("	If FileExistsFunctiom(ThisWorkbook.Path + \"\\Pathway.txt\") = True Then\n");
            macro.Append("		FileCopy ThisWorkbook.Path + \"\\Pathway.txt\", ThisWorkbook.Path + \"\\Pathway.chm\"\n");
            macro.Append("		Kill ThisWorkbook.Path + \"\\Pathway.txt\"\n");
            macro.Append("	End If\n");
            macro.Append("		Application.Help ThisWorkbook.Path + \"\\Pathway.chm\", 2010\n");
            macro.Append("End Sub\n");

            macro.Append("Sub OpenHelpTCPUnused()\n");
            macro.Append("	On Error Resume Next\n");
            macro.Append("	TerminateProcess\n");
            macro.Append("	If FileExistsFunctiom(ThisWorkbook.Path + \"\\Pathway.txt\") = True Then\n");
            macro.Append("		FileCopy ThisWorkbook.Path + \"\\Pathway.txt\", ThisWorkbook.Path + \"\\Pathway.chm\"\n");
            macro.Append("		Kill ThisWorkbook.Path + \"\\Pathway.txt\"\n");
            macro.Append("	End If\n");
            macro.Append("		Application.Help ThisWorkbook.Path + \"\\Pathway.chm\", 2011\n");
            macro.Append("End Sub\n");

            macro.Append("Sub OpenHelpTermTransactions()\n");
            macro.Append("	On Error Resume Next\n");
            macro.Append("	TerminateProcess\n");
            macro.Append("	If FileExistsFunctiom(ThisWorkbook.Path + \"\\Pathway.txt\") = True Then\n");
            macro.Append("		FileCopy ThisWorkbook.Path + \"\\Pathway.txt\", ThisWorkbook.Path + \"\\Pathway.chm\"\n");
            macro.Append("		Kill ThisWorkbook.Path + \"\\Pathway.txt\"\n");
            macro.Append("	End If\n");
            macro.Append("		Application.Help ThisWorkbook.Path + \"\\Pathway.chm\", 2012\n");
            macro.Append("End Sub\n");

            macro.Append("Sub OpenHelpTermUnused()\n");
            macro.Append("	On Error Resume Next\n");
            macro.Append("	TerminateProcess\n");
            macro.Append("	If FileExistsFunctiom(ThisWorkbook.Path + \"\\Pathway.txt\") = True Then\n");
            macro.Append("		FileCopy ThisWorkbook.Path + \"\\Pathway.txt\", ThisWorkbook.Path + \"\\Pathway.chm\"\n");
            macro.Append("		Kill ThisWorkbook.Path + \"\\Pathway.txt\"\n");
            macro.Append("	End If\n");
            macro.Append("		Application.Help ThisWorkbook.Path + \"\\Pathway.chm\", 2013\n");
            macro.Append("End Sub\n");
            #endregion

            macro.Append("Public Sub initializeShape_PerPathway_ClickEvents()\n");
            macro.Append("    On Error GoTo ErrMsg\n   'Sheet2.Select'\n");
            macro.Append("    'Sheet1.Select'\n");
            macro.Append("    strSheetName = \"" + worksheetName + "\"\n");
            macro.Append("    'INITIALIZE THE OnAction FOR EACH SHAPE TO TRAP CLICKS WITH THE SHAPE NAMES\n");
            macro.Append("    Dim theShape As Shape, theSheet As Worksheet\n");
            macro.Append("    For Each theShape In Worksheets(strSheetName).Shapes\n");
            macro.Append("        If theShape.TopLeftCell.Column = 1 Then\n");
            macro.Append("            theShape.OnAction = \"'EventIntervalClicked \"\"\" & theShape.Name & \"\"\", \"\"\" & theShape.TopLeftCell.Row & \"\"\", \"\"\" & theShape.TopLeftCell.Column & \"\"\"'\"\n");
            macro.Append("            If theShape.TopLeftCell.Row <> 1 Then\n");
            macro.Append("                Call EventIntervalClicked(theShape.Name, theShape.TopLeftCell.Row, theShape.TopLeftCell.Column)\n");
            macro.Append("            End If\n");
            macro.Append("        End If\n");
            macro.Append("    Next theShape\n");
            macro.Append("    For Each theShape In Worksheets(strSheetName).Shapes\n");
            macro.Append("        If theShape.TopLeftCell.Column = 1 Then\n");
            macro.Append("            If theShape.TopLeftCell.Row = 1 Then\n");
            macro.Append("                Call ExpandClicked(theShape.Name, theShape.TopLeftCell.Row, theShape.TopLeftCell.Column)\n");
            macro.Append("            End If\n");
            macro.Append("        End If\n");
            macro.Append("    Next theShape\n");
            macro.Append("    Range(\"B1\").Select\n");
            macro.Append("    ErrMsg:\n");
            macro.Append("    If Err.Number <> 0 Then\n");
            macro.Append("      msg = \"The report was opened in the protected or read only mode.\" & vbCrLf & _\n");
            macro.Append("      \"     Cause: The file may have been opened within the zip file\" & vbCrLf & _\n");
            macro.Append("      \"     Effect: Macros/functionality may not work correctly.\" & vbCrLf & _\n");
            macro.Append("      \"     Recovery: Extract the report(s) from the archive (zip file) and open the report again.\"\n");
            macro.Append("      MsgBox msg\n");
            macro.Append("    End If\n");
            macro.Append("End Sub\n");


            macro.Append("Public Function EventIntervalClicked(ByVal theShapeName As String, ByVal theCellRow As Integer, ByVal theCellColumn As Integer)\n");
            macro.Append("    Worksheets(strSheetName).Shapes(theShapeName).Delete\n");
            macro.Append("    Set newPic = Worksheets(strSheetName).Pictures.Insert(ActiveWorkbook.Path & \"\\expand.gif\")\n");
            macro.Append("    newPic.Left = Cells(theCellRow, theCellColumn).Left + 1\n");
            macro.Append("    newPic.Top = Cells(theCellRow, theCellColumn).Top + 2\n");
            macro.Append("    'Hide picture when Hidden\n");
            macro.Append("    newPic.Placement = xlMoveAndSize\n");
            macro.Append("    Dim startRow As Integer\n");
            macro.Append("    Dim endRow As Integer\n");
            macro.Append("    startRow = (theCellRow + 1)\n");
            macro.Append("    endRow = startRow + 5\n");
            macro.Append("    'Hide Rows.\n");
            macro.Append("    Rows(startRow & \":\" & endRow).Select\n");
            macro.Append("    Selection.EntireRow.Hidden = True\n");
            macro.Append("    newPic.OnAction = \"'ExpandIntervalClicked \"\"\" & newPic.Name & \"\"\", \"\"\" & theCellRow & \"\"\", \"\"\" & theCellColumn & \"\"\"'\"\n");
            macro.Append("End Function\n");



            macro.Append("Public Function ExpandIntervalClicked(ByVal theShapeName As String, ByVal theCellRow As Integer, ByVal theCellColumn As Integer)\n");
            macro.Append("    Worksheets(strSheetName).Shapes(theShapeName).Delete\n");
            macro.Append("    Set newPic = Worksheets(strSheetName).Pictures.Insert(ActiveWorkbook.Path & \"\\collapse.gif\")\n");
            macro.Append("    newPic.Left = Cells(theCellRow, theCellColumn).Left + 1\n");
            macro.Append("    newPic.Top = Cells(theCellRow, theCellColumn).Top + 2\n");
            macro.Append("    'Hide picture when Hidden\n");
            macro.Append("    newPic.Placement = xlMoveAndSize\n");
            macro.Append("    Dim startRow As Integer\n");
            macro.Append("    Dim endRow As Integer\n");
            macro.Append("    startRow = (theCellRow + 1)\n");
            macro.Append("    endRow = startRow + 5\n");
            macro.Append("    'Show Rows.\n");
            macro.Append("    Rows(startRow & \":\" & endRow).Select\n");
            macro.Append("    Selection.EntireRow.Hidden = False\n");
            macro.Append("    Call CollapesIntervalSubInterval(theCellRow)\n");
            macro.Append("    newPic.OnAction = \"'EventIntervalClicked \"\"\" & newPic.Name & \"\"\", \"\"\" & theCellRow & \"\"\", \"\"\" & theCellColumn & \"\"\"'\"\n");
            macro.Append("End Function\n");



            macro.Append("Public Function CollapesIntervalSubInterval(ByVal theCellRow As Integer)\n");
            macro.Append("    Dim serverStartRow As Integer\n");
            macro.Append("    Dim tcpStartRow As Integer\n");
            macro.Append("    serverStartRow = theCellRow + 3\n");
            macro.Append("    tcpStartRow = theCellRow + 6\n");
            macro.Append("    Dim serverEndRow As Integer\n");
            macro.Append("    Dim termEndRow As Integer\n");
            macro.Append("    Dim tcpEndRow As Integer\n");
            macro.Append("    serverEndRow = serverStartRow + 1\n");
            macro.Append("    tcpEndRow = tcpStartRow\n");
            macro.Append("    'Hide Rows.\n");
            macro.Append("    Rows(serverStartRow & \":\" & serverEndRow).Select\n");
            macro.Append("    Selection.EntireRow.Hidden = True\n");
            macro.Append("    Rows(tcpStartRow & \":\" & tcpEndRow).Select\n");
            macro.Append("    Selection.EntireRow.Hidden = True\n");
            macro.Append("    'Change the picture to Expand.\n");
            macro.Append("    Dim serverIconRow As Integer\n");
            macro.Append("    Dim tcpIconRow As Integer\n");
            macro.Append("    serverIconRow = theCellRow + 2\n");
            macro.Append("    tcpIconRow = theCellRow + 5\n");
            macro.Append("    For Each theShape In Worksheets(strSheetName).Shapes\n");
            macro.Append("        If theShape.TopLeftCell.Row = serverIconRow And theShape.TopLeftCell.Column = 3 Then\n");
            macro.Append("            Call SetPictureExpandInterval(theShape.Name, theShape.TopLeftCell.Row, theShape.TopLeftCell.Column, \"SERVER\")\n");
            macro.Append("        ElseIf theShape.TopLeftCell.Row = tcpIconRow And theShape.TopLeftCell.Column = 3 Then\n");
            macro.Append("            Call SetPictureExpandInterval(theShape.Name, theShape.TopLeftCell.Row, theShape.TopLeftCell.Column, \"TCP\")\n");
            macro.Append("        End If\n");
            macro.Append("    Next theShape\n");
            macro.Append("End Function\n");



            macro.Append("Public Function EventClicked(ByVal theShapeName As String, ByVal theCellRow As Integer, ByVal theCellColumn As Integer)\n");
            macro.Append("    Worksheets(strSheetName).Shapes(theShapeName).Delete\n");
            macro.Append("    Set newPic = Worksheets(strSheetName).Pictures.Insert(ActiveWorkbook.Path & \"\\expand.gif\")\n");
            macro.Append("    newPic.Left = Cells(theCellRow, theCellColumn).Left + 1\n");
            macro.Append("    newPic.Top = Cells(theCellRow, theCellColumn).Top + 2\n");
            macro.Append("    'Hide picture when Hidden\n");
            macro.Append("    newPic.Placement = xlMoveAndSize\n");
            macro.Append("    Dim startRow As Integer\n");
            macro.Append("    Dim endRow As Integer\n");
            macro.Append("    startRow = (theCellRow + 1)\n");
            macro.Append("    endRow = startRow + 15\n");
            macro.Append("    'Hide Rows.\n");
            macro.Append("    Rows(startRow & \":\" & endRow).Select\n");
            macro.Append("    Selection.EntireRow.Hidden = True\n");
            macro.Append("    newPic.OnAction = \"'ExpandClicked \"\"\" & newPic.Name & \"\"\", \"\"\" & theCellRow & \"\"\", \"\"\" & theCellColumn & \"\"\"'\"\n");
            macro.Append("End Function\n");



            macro.Append("Public Function ExpandClicked(ByVal theShapeName As String, ByVal theCellRow As Integer, ByVal theCellColumn As Integer)\n");
            macro.Append("    Worksheets(strSheetName).Shapes(theShapeName).Delete\n");
            macro.Append("    Set newPic = Worksheets(strSheetName).Pictures.Insert(ActiveWorkbook.Path & \"\\collapse.gif\")\n");
            macro.Append("    newPic.Left = Cells(theCellRow, theCellColumn).Left + 1\n");
            macro.Append("    newPic.Top = Cells(theCellRow, theCellColumn).Top + 2\n");
            macro.Append("    'Hide picture when Hidden\n");
            macro.Append("    newPic.Placement = xlMoveAndSize\n");
            macro.Append("    Dim startRow As Integer\n");
            macro.Append("    Dim endRow As Integer\n");
            macro.Append("    startRow = (theCellRow + 1)\n");
            macro.Append("    endRow = startRow + 15\n");
            macro.Append("    'Show Rows.\n");
            macro.Append("    Rows(startRow & \":\" & endRow).Select\n");
            macro.Append("    Selection.EntireRow.Hidden = False\n");
            macro.Append("    Dim serverIconRow As Integer\n");
            macro.Append("    Dim termIconRow As Integer\n");
            macro.Append("    Dim tcpIconRow As Integer\n");
            macro.Append("    serverIconRow = theCellRow + 3\n");
            macro.Append("    tcpIconRow = theCellRow + 10\n");
            macro.Append("    termIconRow = theCellRow + 14\n");
            macro.Append("    For Each theShape In Worksheets(strSheetName).Shapes\n");
            macro.Append("        If theShape.TopLeftCell.Row = serverIconRow And theShape.TopLeftCell.Column = 3 Then\n");
            macro.Append("            Call SetPictureCollapes(theShape.Name, theShape.TopLeftCell.Row, theShape.TopLeftCell.Column, \"SERVER\")\n");
            macro.Append("        ElseIf theShape.TopLeftCell.Row = tcpIconRow And theShape.TopLeftCell.Column = 3 Then\n");
            macro.Append("            Call SetPictureCollapes(theShape.Name, theShape.TopLeftCell.Row, theShape.TopLeftCell.Column, \"TCP\")\n");
            macro.Append("        ElseIf theShape.TopLeftCell.Row = termIconRow And theShape.TopLeftCell.Column = 3 Then\n");
            macro.Append("            Call SetPictureCollapes(theShape.Name, theShape.TopLeftCell.Row, theShape.TopLeftCell.Column, \"TERM\")\n");
            macro.Append("        End If\n");
            macro.Append("    Next theShape\n");
            macro.Append("    newPic.OnAction = \"'EventClicked \"\"\" & newPic.Name & \"\"\", \"\"\" & theCellRow & \"\"\", \"\"\" & theCellColumn & \"\"\"'\"\n");
            macro.Append("End Function\n");



            macro.Append("Public Function CollapesSubInterval(ByVal theCellRow As Integer)\n");
            macro.Append("    Dim serverStartRow As Integer\n");
            macro.Append("    Dim tcpStartRow As Integer\n");
            macro.Append("    serverStartRow = theCellRow + 4\n");
            macro.Append("    tcpStartRow = theCellRow + 7\n");
            macro.Append("    Dim serverEndRow As Integer\n");
            macro.Append("    Dim tcpEndRow As Integer\n");
            macro.Append("    serverEndRow = serverStartRow + 1\n");
            macro.Append("    tcpEndRow = tcpStartRow\n");
            macro.Append("    'Hide Rows.\n");
            macro.Append("    Rows(serverStartRow & \":\" & serverEndRow).Select\n");
            macro.Append("    Selection.EntireRow.Hidden = True\n");
            macro.Append("    Rows(tcpStartRow & \":\" & tcpEndRow).Select\n");
            macro.Append("    Selection.EntireRow.Hidden = True\n");
            macro.Append("    'Change the picture to Expand.\n");
            macro.Append("    Dim serverIconRow As Integer\n");
            macro.Append("    Dim tcpIconRow As Integer\n");
            macro.Append("    serverIconRow = theCellRow + 3\n");
            macro.Append("    tcpIconRow = theCellRow + 6\n");
            macro.Append("    For Each theShape In Worksheets(strSheetName).Shapes\n");
            macro.Append("        If theShape.TopLeftCell.Row = serverIconRow And theShape.TopLeftCell.Column = 3 Then\n");
            macro.Append("            Call SetPictureExpandInterval(theShape.Name, theShape.TopLeftCell.Row, theShape.TopLeftCell.Column, \"SERVER\")\n");
            macro.Append("        ElseIf theShape.TopLeftCell.Row = tcpIconRow And theShape.TopLeftCell.Column = 3 Then\n");
            macro.Append("            Call SetPictureExpandInterval(theShape.Name, theShape.TopLeftCell.Row, theShape.TopLeftCell.Column, \"TCP\")\n");
            macro.Append("        End If\n");
            macro.Append("    Next theShape\n");
            macro.Append("End Function\n");



            macro.Append("Public Function ExpandSub(ByVal theShapeName As String, ByVal theCellRow As Integer, ByVal theCellColumn As Integer, ByVal key As String)\n");
            macro.Append("    If key = \"SERVER\" Then\n");
            macro.Append("        Call SetPictureCollapes(theShapeName, theCellRow, theCellColumn, key)\n");
            macro.Append("        Dim serverStartRow As Integer\n");
            macro.Append("        Dim serverEndRow As Integer\n");
            macro.Append("        serverStartRow = theCellRow + 1\n");
            macro.Append("        serverEndRow = serverStartRow + 5\n");
            macro.Append("        Rows(serverStartRow & \":\" & serverEndRow).Select\n");
            macro.Append("        Selection.EntireRow.Hidden = False\n");
            macro.Append("    ElseIf key = \"TERM\" Then\n");
            macro.Append("        Call SetPictureCollapes(theShapeName, theCellRow, theCellColumn, key)\n");
            macro.Append("        Dim termStartRow As Integer\n");
            macro.Append("        Dim termEndRow As Integer\n");
            macro.Append("        termStartRow = theCellRow + 1\n");
            macro.Append("        termEndRow = termStartRow + 1\n");
            macro.Append("        Rows(termStartRow & \":\" & termEndRow).Select\n");
            macro.Append("        Selection.EntireRow.Hidden = False\n");
            macro.Append("    ElseIf key = \"TCP\" Then\n");
            macro.Append("        Call SetPictureCollapes(theShapeName, theCellRow, theCellColumn, key)\n");
            macro.Append("        Dim tcpStartRow As Integer\n");
            macro.Append("        Dim tcpEndRow As Integer\n");
            macro.Append("        tcpStartRow = theCellRow + 1\n");
            macro.Append("        tcpEndRow = tcpStartRow + 2\n");
            macro.Append("        Rows(tcpStartRow & \":\" & tcpEndRow).Select\n");
            macro.Append("        Selection.EntireRow.Hidden = False\n");
            macro.Append("    End If\n");
            macro.Append("End Function\n");



            macro.Append("Public Function CollapesSub(ByVal theShapeName As String, ByVal theCellRow As Integer, ByVal theCellColumn As Integer, ByVal key As String)\n");
            macro.Append("    If key = \"SERVER\" Then\n");
            macro.Append("        Call SetPictureExpand(theShapeName, theCellRow, theCellColumn, key)\n");
            macro.Append("        Dim serverStartRow As Integer\n");
            macro.Append("        Dim serverEndRow As Integer\n");
            macro.Append("        serverStartRow = theCellRow + 1\n");
            macro.Append("        serverEndRow = serverStartRow + 5\n");
            macro.Append("        Rows(serverStartRow & \":\" & serverEndRow).Select\n");
            macro.Append("        Selection.EntireRow.Hidden = True\n");
            macro.Append("    ElseIf key = \"TERM\" Then\n");
            macro.Append("        Call SetPictureExpand(theShapeName, theCellRow, theCellColumn, key)\n");
            macro.Append("        Dim termStartRow As Integer\n");
            macro.Append("        Dim termEndRow As Integer\n");
            macro.Append("        termStartRow = theCellRow + 1\n");
            macro.Append("        termEndRow = termStartRow + 1\n");
            macro.Append("        Rows(termStartRow & \":\" & termEndRow).Select\n");
            macro.Append("        Selection.EntireRow.Hidden = True\n");
            macro.Append("    ElseIf key = \"TCP\" Then\n");
            macro.Append("        Call SetPictureExpand(theShapeName, theCellRow, theCellColumn, key)\n");
            macro.Append("        Dim tcpStartRow As Integer\n");
            macro.Append("        Dim tcpEndRow As Integer\n");
            macro.Append("        tcpStartRow = theCellRow + 1\n");
            macro.Append("        tcpEndRow = tcpStartRow + 2\n");
            macro.Append("        Rows(tcpStartRow & \":\" & tcpEndRow).Select\n");
            macro.Append("        Selection.EntireRow.Hidden = True\n");
            macro.Append("    End If\n");
            macro.Append("End Function\n");



            macro.Append("Public Function SetPictureExpand(ByVal theShapeName As String, ByVal theCellRow As Integer, ByVal theCellColumn As Integer, ByVal key As String)\n");
            macro.Append("    Worksheets(strSheetName).Shapes(theShapeName).Select\n");
            macro.Append("        Set newPic = Worksheets(strSheetName).Pictures.Insert(ActiveWorkbook.Path & \"\\expand.gif\")\n");
            macro.Append("        newPic.Left = Cells(theCellRow, theCellColumn).Left + 1\n");
            macro.Append("        newPic.Top = Cells(theCellRow, theCellColumn).Top + 2\n");
            macro.Append("        'Hide picture when Hidden\n");
            macro.Append("        newPic.Placement = xlMoveAndSize\n");
            macro.Append("        newPic.OnAction = \"'ExpandSub \"\"\" & newPic.Name & \"\"\", \"\"\" & theCellRow & \"\"\", \"\"\" & theCellColumn & \"\"\", \"\"\" & key & \"\"\"'\"\n");
            macro.Append("End Function\n");



            macro.Append("Public Function SetPictureCollapes(ByVal theShapeName As String, ByVal theCellRow As Integer, ByVal theCellColumn As Integer, ByVal key As String)\n");
            macro.Append("    'Worksheets(strSheetName).Shapes(theShapeName).Select\n");
            macro.Append("        Set newPic = Worksheets(strSheetName).Pictures.Insert(ActiveWorkbook.Path & \"\\collapse.gif\")\n");
            macro.Append("        newPic.Left = Cells(theCellRow, theCellColumn).Left + 1\n");
            macro.Append("        newPic.Top = Cells(theCellRow, theCellColumn).Top + 2\n");
            macro.Append("        'Hide picture when Hidden\n");
            macro.Append("        newPic.Placement = xlMoveAndSize\n");
            macro.Append("        newPic.OnAction = \"'CollapesSub \"\"\" & newPic.Name & \"\"\", \"\"\" & theCellRow & \"\"\", \"\"\" & theCellColumn & \"\"\", \"\"\" & key & \"\"\"'\"\n");
            macro.Append("End Function\n");



            macro.Append("Public Function SetPictureExpandInterval(ByVal theShapeName As String, ByVal theCellRow As Integer, ByVal theCellColumn As Integer, ByVal key As String)\n");
            macro.Append("    Worksheets(strSheetName).Shapes(theShapeName).Select\n");
            macro.Append("        Set newPic = Worksheets(strSheetName).Pictures.Insert(ActiveWorkbook.Path & \"\\expand.gif\")\n");
            macro.Append("        newPic.Left = Cells(theCellRow, theCellColumn).Left + 1\n");
            macro.Append("        newPic.Top = Cells(theCellRow, theCellColumn).Top + 2\n");
            macro.Append("        'Hide picture when Hidden\n");
            macro.Append("        newPic.Placement = xlMoveAndSize\n");
            macro.Append("        newPic.OnAction = \"'ExpandSubInterval \"\"\" & newPic.Name & \"\"\", \"\"\" & theCellRow & \"\"\", \"\"\" & theCellColumn & \"\"\", \"\"\" & key & \"\"\"'\"\n");
            macro.Append("End Function\n");



            macro.Append("Public Function SetPictureCollapesInterval(ByVal theShapeName As String, ByVal theCellRow As Integer, ByVal theCellColumn As Integer, ByVal key As String)\n");
            macro.Append("    'Worksheets(strSheetName).Shapes(theShapeName).Select\n");
            macro.Append("        Set newPic = Worksheets(strSheetName).Pictures.Insert(ActiveWorkbook.Path & \"\\collapse.gif\")\n");
            macro.Append("        newPic.Left = Cells(theCellRow, theCellColumn).Left + 1\n");
            macro.Append("        newPic.Top = Cells(theCellRow, theCellColumn).Top + 2\n");
            macro.Append("        'Hide picture when Hidden\n");
            macro.Append("        newPic.Placement = xlMoveAndSize\n");
            macro.Append("        newPic.OnAction = \"'CollapesSubIntervalAction \"\"\" & newPic.Name & \"\"\", \"\"\" & theCellRow & \"\"\", \"\"\" & theCellColumn & \"\"\", \"\"\" & key & \"\"\"'\"\n");
            macro.Append("End Function\n");



            macro.Append("Public Function ExpandSubInterval(ByVal theShapeName As String, ByVal theCellRow As Integer, ByVal theCellColumn As Integer, ByVal key As String)\n");
            macro.Append("    If key = \"SERVER\" Then\n");
            macro.Append("        Call SetPictureCollapesInterval(theShapeName, theCellRow, theCellColumn, key)\n");
            macro.Append("        Dim serverStartRow As Integer\n");
            macro.Append("        Dim serverEndRow As Integer\n");
            macro.Append("        serverStartRow = theCellRow + 1\n");
            macro.Append("        serverEndRow = serverStartRow + 1\n");
            macro.Append("        Rows(serverStartRow & \":\" & serverEndRow).Select\n");
            macro.Append("        Selection.EntireRow.Hidden = False\n");
            macro.Append("    ElseIf key = \"TCP\" Then\n");
            macro.Append("        Call SetPictureCollapesInterval(theShapeName, theCellRow, theCellColumn, key)\n");
            macro.Append("        Dim tcpStartRow As Integer\n");
            macro.Append("        Dim tcpEndRow As Integer\n");
            macro.Append("        tcpStartRow = theCellRow + 1\n");
            macro.Append("        tcpEndRow = tcpStartRow\n");
            macro.Append("        Rows(tcpStartRow & \":\" & tcpEndRow).Select\n");
            macro.Append("        Selection.EntireRow.Hidden = False\n");
            macro.Append("    End If\n");
            macro.Append("End Function\n");



            macro.Append("Public Function CollapesSubIntervalAction(ByVal theShapeName As String, ByVal theCellRow As Integer, ByVal theCellColumn As Integer, ByVal key As String)\n");
            macro.Append("    If key = \"SERVER\" Then\n");
            macro.Append("        Call SetPictureExpandInterval(theShapeName, theCellRow, theCellColumn, key)\n");
            macro.Append("        Dim serverStartRow As Integer\n");
            macro.Append("        Dim serverEndRow As Integer\n");
            macro.Append("        serverStartRow = theCellRow + 1\n");
            macro.Append("        serverEndRow = serverStartRow + 1\n");
            macro.Append("        Rows(serverStartRow & \":\" & serverEndRow).Select\n");
            macro.Append("        Selection.EntireRow.Hidden = True\n");
            macro.Append("    ElseIf key = \"TCP\" Then\n");
            macro.Append("        Call SetPictureExpandInterval(theShapeName, theCellRow, theCellColumn, key)\n");
            macro.Append("        Dim tcpStartRow As Integer\n");
            macro.Append("        Dim tcpEndRow As Integer\n");
            macro.Append("        tcpStartRow = theCellRow + 1\n");
            macro.Append("        tcpEndRow = tcpStartRow\n");
            macro.Append("        Rows(tcpStartRow & \":\" & tcpEndRow).Select\n");
            macro.Append("        Selection.EntireRow.Hidden = True\n");
            macro.Append("    End If\n");
            macro.Append("End Function\n");


            return macro.ToString();
        }

        internal string GeneratePreviousWorksheetMacro() {
            var macro = new StringBuilder();
            macro.Append(@"Public OldSheetName As String
                            Public mySheetName As String
                            Sub ReturnToLastSheet()
                              Worksheets(OldSheetName).Activate
                            End Sub
    
                            Sub TerminateProcess()
                                Dim strTerminateThis As String 'The variable to hold the process to terminate
                                Dim objWMIcimv2 As Object
                                Dim objProcess As Object
                                Dim objList As Object
                                Dim intError As Integer
 
                                'HTML Help component hh.exe
                                strTerminateThis = ""hh.exe""
                                Set objWMIcimv2 = GetObject(""winmgmts:{impersonationLevel=impersonate}!\\.\root\cimv2"") 'Connect to CIMV2 Namespace
                                Set objList = objWMIcimv2.ExecQuery(""select * from win32_process where name='"" & strTerminateThis & ""'"")  'Find the process to terminate
 
                                If objList.count = 0 Then 'If 0 then process isn't running
                                    Set objWMIcimv2 = Nothing
                                    Set objList = Nothing
                                    Set objProcess = Nothing
                                    Exit Sub
                                Else
                                    For Each objProcess In objList
                                        intError = objProcess.Terminate 'Terminates a process and all of its threads.
                                        If intError <> 0 Then
                                            Exit Sub
                                        End If
                                    Next
 
                                    Set objWMIcimv2 = Nothing
                                    Set objList = Nothing
                                    Set objProcess = Nothing
                                    Exit Sub
                                End If
                            End Sub

                            Public Function FileExistsFunctiom(Path As String) As Boolean
                                Dim objFSO As Object
                                Set objFSO = CreateObject(""Scripting.FileSystemObject"")
 
                                Select Case objFSO.FileExists(Path)
                                Case True
                                    FileExistsFunctiom = True
                                Case False
                                    FileExistsFunctiom = False
                                End Select
 
                                Set objFSO = Nothing
                            End Function

                            Sub OpenHelpAlerts()
                                On Error Resume Next
                                TerminateProcess
                                If FileExistsFunctiom(ThisWorkbook.Path + ""\Pathway.txt"") = True Then
                                    FileCopy ThisWorkbook.Path + ""\Pathway.txt"", ThisWorkbook.Path + ""\Pathway.chm""
                                    Kill ThisWorkbook.Path + ""\Pathway.txt""
                                End If
                                
                                Application.Help ThisWorkbook.Path + ""\Pathway.chm"", 8001
                            End Sub

                            Sub OpenHelpQueueTCP()
                                On Error Resume Next
                                TerminateProcess
                                If FileExistsFunctiom(ThisWorkbook.Path + ""\Pathway.txt"") = True Then
                                    FileCopy ThisWorkbook.Path + ""\Pathway.txt"", ThisWorkbook.Path + ""\Pathway.chm""
                                    Kill ThisWorkbook.Path + ""\Pathway.txt""
                                End If
                                
                                Application.Help ThisWorkbook.Path + ""\Pathway.chm"", 8002
                            End Sub

                            Sub OpenHelpQueueLinkmon()
                                On Error Resume Next
                                TerminateProcess
                                If FileExistsFunctiom(ThisWorkbook.Path + ""\Pathway.txt"") = True Then
                                    FileCopy ThisWorkbook.Path + ""\Pathway.txt"", ThisWorkbook.Path + ""\Pathway.chm""
                                    Kill ThisWorkbook.Path + ""\Pathway.txt""
                                End If
                                
                                Application.Help ThisWorkbook.Path + ""\Pathway.chm"", 8003
                            End Sub

                            Sub OpenHelpUnusedClass()
                                On Error Resume Next
                                TerminateProcess
                                If FileExistsFunctiom(ThisWorkbook.Path + ""\Pathway.txt"") = True Then
                                    FileCopy ThisWorkbook.Path + ""\Pathway.txt"", ThisWorkbook.Path + ""\Pathway.chm""
                                    Kill ThisWorkbook.Path + ""\Pathway.txt""
                                End If
                                
                                Application.Help ThisWorkbook.Path + ""\Pathway.chm"", 8004
                            End Sub

                            Sub OpenHelpServerUnusedClass()
                                'WAS NECESSARY TO CLONE OpenHelpUnusedClass() FUNCTION IN ORDER FOR REPORT NOT TO ABORT
                                On Error Resume Next
                                TerminateProcess
                                If FileExistsFunctiom(ThisWorkbook.Path + ""\Pathway.txt"") = True Then
                                    FileCopy ThisWorkbook.Path + ""\Pathway.txt"", ThisWorkbook.Path + ""\Pathway.chm""
                                    Kill ThisWorkbook.Path + ""\Pathway.txt""
                                End If
                                
                                Application.Help ThisWorkbook.Path + ""\Pathway.chm"", 8004
                            End Sub

                            Sub OpenHelpUnusedProcesses()
                                On Error Resume Next
                                TerminateProcess
                                If FileExistsFunctiom(ThisWorkbook.Path + ""\Pathway.txt"") = True Then
                                    FileCopy ThisWorkbook.Path + ""\Pathway.txt"", ThisWorkbook.Path + ""\Pathway.chm""
                                    Kill ThisWorkbook.Path + ""\Pathway.txt""
                                End If
                                
                                Application.Help ThisWorkbook.Path + ""\Pathway.chm"", 8005
                            End Sub

                            Sub OpenHelpQueueTCPInterval()
                                On Error Resume Next
                                TerminateProcess
                                If FileExistsFunctiom(ThisWorkbook.Path + ""\Pathway.txt"") = True Then
                                    FileCopy ThisWorkbook.Path + ""\Pathway.txt"", ThisWorkbook.Path + ""\Pathway.chm""
                                    Kill ThisWorkbook.Path + ""\Pathway.txt""
                                End If
                                
                                Application.Help ThisWorkbook.Path + ""\Pathway.chm"", 8006
                            End Sub

                            Sub OpenHelpQueueLinkmonInterval()
                                On Error Resume Next
                                TerminateProcess
                                If FileExistsFunctiom(ThisWorkbook.Path + ""\Pathway.txt"") = True Then
                                    FileCopy ThisWorkbook.Path + ""\Pathway.txt"", ThisWorkbook.Path + ""\Pathway.chm""
                                    Kill ThisWorkbook.Path + ""\Pathway.txt""
                                End If
                                
                                Application.Help ThisWorkbook.Path + ""\Pathway.chm"", 8007
                            End Sub

                            Sub OpenHelpErrorList()
                                On Error Resume Next
                                TerminateProcess
                                If FileExistsFunctiom(ThisWorkbook.Path + ""\Pathway.txt"") = True Then
                                    FileCopy ThisWorkbook.Path + ""\Pathway.txt"", ThisWorkbook.Path + ""\Pathway.chm""
                                    Kill ThisWorkbook.Path + ""\Pathway.txt""
                                End If
                                
                                Application.Help ThisWorkbook.Path + ""\Pathway.chm"", 8008
                            End Sub

                            Sub OpenHelpErrorListInterval()
                                On Error Resume Next
                                TerminateProcess
                                If FileExistsFunctiom(ThisWorkbook.Path + ""\Pathway.txt"") = True Then
                                    FileCopy ThisWorkbook.Path + ""\Pathway.txt"", ThisWorkbook.Path + ""\Pathway.chm""
                                    Kill ThisWorkbook.Path + ""\Pathway.txt""
                                End If
                                
                                Application.Help ThisWorkbook.Path + ""\Pathway.chm"", 8009
                            End Sub

                            Sub OpenHelpHighMaxLinks()
                                On Error Resume Next
                                TerminateProcess
                                If FileExistsFunctiom(ThisWorkbook.Path + ""\Pathway.txt"") = True Then
                                    FileCopy ThisWorkbook.Path + ""\Pathway.txt"", ThisWorkbook.Path + ""\Pathway.chm""
                                    Kill ThisWorkbook.Path + ""\Pathway.txt""
                                End If
                                
                                Application.Help ThisWorkbook.Path + ""\Pathway.chm"", 8010
                            End Sub

                            Sub OpenHelpCheckDirectoryOn()
                                On Error Resume Next
                                TerminateProcess
                                If FileExistsFunctiom(ThisWorkbook.Path + ""\Pathway.txt"") = True Then
                                    FileCopy ThisWorkbook.Path + ""\Pathway.txt"", ThisWorkbook.Path + ""\Pathway.chm""
                                    Kill ThisWorkbook.Path + ""\Pathway.txt""
                                End If
                                
                                Application.Help ThisWorkbook.Path + ""\Pathway.chm"", 8011
                            End Sub

                            Sub OpenHelpHighDynamicServers()
                                On Error Resume Next
                                TerminateProcess
                                If FileExistsFunctiom(ThisWorkbook.Path + ""\Pathway.txt"") = True Then
                                    FileCopy ThisWorkbook.Path + ""\Pathway.txt"", ThisWorkbook.Path + ""\Pathway.chm""
                                    Kill ThisWorkbook.Path + ""\Pathway.txt""
                                End If
                                
                                Application.Help ThisWorkbook.Path + ""\Pathway.chm"", 8012
                            End Sub
                            ");

            return macro.ToString();
        }
    }
}
