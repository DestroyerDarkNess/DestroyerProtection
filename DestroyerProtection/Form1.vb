Imports System.Runtime.InteropServices
Imports DestroyerProtection.Destroyer.Protect
Imports DestroyerProtection.Destroyer.Utils
Imports System.Environment
Imports System.Net
Imports System.IO
Imports System.Reflection


<Assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)> 
'<Assembly: DefaultDllImportSearchPaths(DllImportSearchPath.System32)> 'Optional

Public Class Form1


    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub


    Public Structure ResultScan
        Shared ProcessResult As String = String.Empty
        Shared DebuggersResult As String = String.Empty
        Shared VMResult As String = String.Empty
        Shared Finish As Boolean = False
    End Structure

#Region " Check Mode "

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ResultScan.ProcessResult = String.Empty
        ResultScan.DebuggersResult = String.Empty
        ResultScan.VMResult = String.Empty
        ResultScan.Finish = False
        TextBox1.Text = String.Empty
        DestroyerProtect.Start(Destroyer.AntiAnalysis.MainAnalysis.SearchType.FromNameandTitle, _
                              Destroyer.AntiDebug.MainDebug.DebugDetectionTypes.AllScanEgines, _
                              DestroyerProtect.VMScanType.By_Directory_Known)
        CheckTimer1.Enabled = True
    End Sub

    Private Sub CheckTimer1_Tick(sender As Object, e As EventArgs) Handles CheckTimer1.Tick
        Dim Finispscan As DestroyerProtect.ResultType = DestroyerProtect.Process_scanner
        Dim FinisDbugscan As DestroyerProtect.ResultType = DestroyerProtect.Debugers_scanner
        Dim FinisVMscan As DestroyerProtect.ResultType = DestroyerProtect.VM_scanner
        Dim Mdirs As New Destroyer.MainDirScan
        Dim MdirBool As Boolean = Mdirs.MaliciousDir()

        If Finispscan = DestroyerProtect.ResultType.Danger Then
            ResultScan.ProcessResult = "[DANGER] Malicious Process has been Detected"
        ElseIf Finispscan = DestroyerProtect.ResultType.Secure Then
            ResultScan.ProcessResult = "[SECURE] Malicious Process Not Detected"
        End If

        If FinisDbugscan = DestroyerProtect.ResultType.Danger Then
            ResultScan.DebuggersResult = "[DANGER] Debuggers Detected"
        ElseIf FinisDbugscan = DestroyerProtect.ResultType.Secure Then
            ResultScan.DebuggersResult = "[SECURE] Debuggers not Detected"
        End If

        If FinisVMscan = DestroyerProtect.ResultType.Danger Then
            ResultScan.VMResult = "[DANGER] Virutal Machines Have Been Detected"
        ElseIf FinisVMscan = DestroyerProtect.ResultType.Secure Then
            ResultScan.VMResult = "[SECURE] No Virtual Machines Found"
        End If

        If Not ResultScan.ProcessResult = String.Empty And Not ResultScan.DebuggersResult = String.Empty And Not ResultScan.VMResult = String.Empty Then
            If MdirBool = True Then : TextBox1.Text += "Detected DnSpy or De4dot in your system." & vbNewLine : End If
            TextBox1.Text += ResultScan.ProcessResult & vbNewLine & ResultScan.DebuggersResult & vbNewLine & ResultScan.VMResult & vbNewLine
            TextBox1.Text += DestroyerProtect.LogResult & vbNewLine
            TextBox2.Text += DestroyerProtect.DetectDescription & vbNewLine
            ResultScan.Finish = True
            Destroyer.AntiDump.MainDump.AntiDumpEnabled(Me)
            CheckTimer1.Enabled = False
        End If

    End Sub

#End Region

#Region " Monitor Mode "

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        ResultScan.ProcessResult = String.Empty
        ResultScan.DebuggersResult = String.Empty
        ResultScan.VMResult = String.Empty
        ResultScan.Finish = False
        TextBox1.Text = String.Empty
        DestroyerProtect.MonitorAsync(Destroyer.AntiAnalysis.MainAnalysis.SearchType.FromNameandTitle, _
                              Destroyer.AntiDebug.MainDebug.DebugDetectionTypes.AllScanEgines, _
                              DestroyerProtect.VMScanType.By_Directory_Known)
        MonitorTimer1.Enabled = True
    End Sub

    Private Sub MonitorTimer1_Tick(sender As Object, e As EventArgs) Handles MonitorTimer1.Tick
        Dim Finispscan As DestroyerProtect.ResultType = DestroyerProtect.Process_scanner
        Dim FinisDbugscan As DestroyerProtect.ResultType = DestroyerProtect.Debugers_scanner
        Dim VMscanner As DestroyerProtect.ResultType = DestroyerProtect.VM_scanner
        Dim Mdirs As New Destroyer.MainDirScan

        If Finispscan = DestroyerProtect.ResultType.Danger Then
            ResultScan.ProcessResult = "[DANGER] Malicious Process has been Detected"
        ElseIf Finispscan = DestroyerProtect.ResultType.Secure Then
            ResultScan.ProcessResult = "[SECURE] Malicious Process Not Detected"
        End If

        If FinisDbugscan = DestroyerProtect.ResultType.Danger Then
            ResultScan.DebuggersResult = "[DANGER] Debuggers Detected"
        ElseIf FinisDbugscan = DestroyerProtect.ResultType.Secure Then
            ResultScan.DebuggersResult = "[SECURE] Debuggers not Detected"
        End If

        If VMscanner = DestroyerProtect.ResultType.Danger Then
            ResultScan.VMResult = "[DANGER] Virutal Machines Have Been Detected"
        ElseIf Finispscan = DestroyerProtect.ResultType.Secure Then
            ResultScan.VMResult = "[SECURE] No Virtual Machines Found"
        End If

        If Not ResultScan.ProcessResult = String.Empty Then
            'DeleteSelfApplication() ' Auto Delete Function
            TextBox1.Text += ResultScan.ProcessResult & vbNewLine
            TextBox1.Text += DestroyerProtect.LogResult & vbNewLine
            TextBox2.Text += DestroyerProtect.DetectDescription & vbNewLine
            ResultScan.Finish = True
            MonitorTimer1.Enabled = False
        End If

        If Not ResultScan.DebuggersResult = String.Empty Then
            ' DeleteSelfApplication() ' Auto Delete Function
            TextBox1.Text += ResultScan.DebuggersResult & vbNewLine
            TextBox1.Text += DestroyerProtect.LogResult & vbNewLine
            TextBox2.Text += DestroyerProtect.DetectDescription & vbNewLine
            ResultScan.Finish = True
            MonitorTimer1.Enabled = False
        End If

        If Mdirs.MaliciousDir() = True Then  ' Detect DnSpy and De4dot.
            'DeleteSelfApplication() ' Auto Delete Function
            TextBox1.Text += "Detect DnSpy and De4dot." & vbNewLine
            TextBox2.Text += DestroyerProtect.DetectDescription & vbNewLine
            ResultScan.Finish = True
            MonitorTimer1.Enabled = False
        End If

        If Not ResultScan.VMResult = String.Empty Then ' Detect Virtaul Machine
            '  DeleteSelfApplication() ' Auto Delete Function
            TextBox1.Text += ResultScan.VMResult & vbNewLine
            TextBox2.Text += DestroyerProtect.DetectDescription & vbNewLine
            ResultScan.Finish = True
            MonitorTimer1.Enabled = False
        End If

    End Sub

#End Region

   
End Class
