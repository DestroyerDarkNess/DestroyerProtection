Imports System.ComponentModel
Imports DestroyerProtectionLib.Destroyer.AntiDebug
Imports DestroyerProtectionLib.Destroyer.AntiVirtualMachine
Imports DestroyerProtectionLib.Destroyer.AntiAnalysis
Imports DestroyerProtectionLib.Destroyer.AntiDump
Imports System.Windows.Forms

Namespace Destroyer.Protect
    Public Class DestroyerProtect

#Region " Properties "

        Private Shared _Process_scanner As ResultType = ResultType.Indeterminate
        Public Shared ReadOnly Property Process_scanner As ResultType
            Get
                Return _Process_scanner
            End Get
        End Property

        Private Shared _Debugers_scanner As ResultType = ResultType.Indeterminate
        Public Shared ReadOnly Property Debugers_scanner As ResultType
            Get
                Return _Debugers_scanner
            End Get
        End Property

        Private Shared _VM_scanner As ResultType = ResultType.Indeterminate
        Public Shared ReadOnly Property VM_scanner As ResultType
            Get
                Return _VM_scanner
            End Get
        End Property

        Private Shared _LogResult As String = String.Empty
        Public Shared ReadOnly Property LogResult As String
            Get
                Return _LogResult
            End Get
        End Property

        Private Shared _DetectDescription As String = String.Empty
        Public Shared ReadOnly Property DetectDescription As String
            Get
                Return _DetectDescription
            End Get
        End Property

#End Region

#Region " Types "

        Public Enum ResultType
            Secure = 0
            Danger = 1
            Indeterminate = 2
            IError = 3
        End Enum

        Public Enum VMScanType
            By_Directory_Known = 0
            By_Generalized_Name = 1
            None = 2
        End Enum

#End Region

#Region " Methods "

        Private Shared Pscan As MainAnalysis.SearchType
        Private Shared Dscan As MainDebug.DebugDetectionTypes
        Private Shared VMscan As VMScanType
        Private Shared tmrFormMonitor As System.Windows.Forms.Timer = New System.Windows.Forms.Timer()

        Public Shared Sub Start(ByVal ProcessScan As MainAnalysis.SearchType, ByVal DebugScan As MainDebug.DebugDetectionTypes, _
                                ByVal VMScanType As VMScanType, Optional EnableAntiDump As Boolean = False, Optional MainForm As Form = Nothing)
            Execution_Start()
            _Process_scanner = ResultType.Indeterminate
            _Debugers_scanner = ResultType.Indeterminate
            _VM_scanner = ResultType.Indeterminate
            Pscan = ProcessScan
            Dscan = DebugScan
            VMscan = VMScanType
            If EnableAntiDump = True Then
                MainDump.AntiDumpEnabled(MainForm)
                _LogResult += MainDump.LogResult & vbNewLine
            End If
            AddHandler tmrFormMonitor.Tick, New System.EventHandler(AddressOf tmrFormMonitor_Tick)
            tmrFormMonitor.Interval = 100
            tmrFormMonitor.Enabled = True
            Dim t = New System.Threading.Thread(New System.Threading.ThreadStart(AddressOf StartAnalisys))
            t.Start()
        End Sub

        Private Shared Sub tmrFormMonitor_Tick(ByVal sender As Object, ByVal e As EventArgs)
            If VMscan = VMScanType.By_Generalized_Name Then
                Dim VMDetection As OldAntiVM.ResultType = OldAntiVM.IsVirtualMachinePresent
                Dim VMDescription As String = OldAntiVM.DescriptcionVM

                If VMDetection = OldAntiVM.ResultType.Danger Then
                    _VM_scanner = ResultType.Danger
                    _DetectDescription += "Virtual Machine Detected : " & VMDescription & vbNewLine
                    Execution_End()
                    tmrFormMonitor.Enabled = False
                ElseIf VMDetection = OldAntiVM.ResultType.Secure Then
                    _VM_scanner = ResultType.Secure
                    Execution_End()
                    tmrFormMonitor.Enabled = False
                End If
            End If
        End Sub

        Private Shared Sub StartAnalisys()

            If Not Pscan = MainAnalysis.SearchType.None Then
                If MainAnalysis.Malicious_Processes_Found(Pscan) = True Then
                    _Process_scanner = ResultType.Danger
                    _DetectDescription += MainAnalysis.LogResult & vbNewLine
                Else
                    _Process_scanner = ResultType.Secure
                End If
            End If

            If Not Dscan = MainDebug.DebugDetectionTypes.None Then

                If MainDebug.Debugger_Check(Dscan) = True Then
                    _Debugers_scanner = ResultType.Danger
                    _DetectDescription += MainDebug.LogResult & vbNewLine
                Else
                    _Debugers_scanner = ResultType.Secure
                End If

            End If

            If Not VMscan = VMScanType.None Then
                If VMscan = VMScanType.By_Directory_Known Then
                    If MainVM.VirtualMachine_Check() = True Then
                        _VM_scanner = ResultType.Danger
                        _DetectDescription += MainVM.LogResult & vbNewLine
                    Else
                        _VM_scanner = ResultType.Secure
                    End If
                    Execution_End()
                ElseIf VMscan = VMScanType.By_Generalized_Name Then
                    OldAntiVM.VM_Start()
                End If
            End If

        End Sub

        Private Shared PscanMonitor As MainAnalysis.SearchType
        Private Shared DscanMonitor As MainDebug.DebugDetectionTypes

        Public Shared Sub MonitorAsync(ByVal ProcessMonitoreScan As MainAnalysis.SearchType, ByVal DebugMonitoreScan As MainDebug.DebugDetectionTypes, Optional VmScanAsyncro As VMScanType = VMScanType.None)
            _Process_scanner = ResultType.Indeterminate
            _Debugers_scanner = ResultType.Indeterminate
            PscanMonitor = ProcessMonitoreScan
            DscanMonitor = DebugMonitoreScan
            If Not VmScanAsyncro = VMScanType.None Then
                VMscan = VmScanAsyncro
                AddHandler tmrFormMonitor.Tick, New System.EventHandler(AddressOf tmrFormMonitor_Tick)
                tmrFormMonitor.Interval = 100
                tmrFormMonitor.Enabled = True
            End If
            Dim EngineThread As New Task(ProcessAsyncMonitoring, TaskCreationOptions.LongRunning)
            EngineThread.Start()
            Execution_Start()
        End Sub

        Private Shared ProcessAsyncMonitoring As New Action(
 Sub()
     Do While True

         If Not PscanMonitor = MainAnalysis.SearchType.None Then
             If MainAnalysis.Malicious_Processes_Found(PscanMonitor) = True Then
                 _DetectDescription += MainAnalysis.LogResult & vbNewLine
                 _Process_scanner = ResultType.Danger
                 Execution_End()
             End If
         End If

         If Not DscanMonitor = MainDebug.DebugDetectionTypes.None Then
             If MainDebug.Debugger_Check(DscanMonitor) = True Then
                 _DetectDescription += MainDebug.LogResult & vbNewLine
                 _Debugers_scanner = ResultType.Danger
                 Execution_End()
             End If
         End If

         If Not VMscan = VMScanType.None Then
             If VMscan = VMScanType.By_Directory_Known Then
                 If MainVM.VirtualMachine_Check() = True Then
                     _DetectDescription += MainVM.LogResult & vbNewLine
                     _VM_scanner = ResultType.Danger
                 Else
                     _VM_scanner = ResultType.Secure
                 End If
                 Execution_End()
             ElseIf VMscan = VMScanType.By_Generalized_Name Then
                 OldAntiVM.VM_Start()
             End If
         End If

     Loop
 End Sub)

#End Region

#Region " Execution Time "

        ' [ Code Execution Time ]
        '
        ' Examples :
        ' Execution_Start() : Threading.Thread.Sleep(500) : Execution_End()

        Private Shared Execution_Watcher As New Stopwatch

        Private Shared Sub Execution_Start()
            If Execution_Watcher.IsRunning Then Execution_Watcher.Restart()
            Execution_Watcher.Start()
        End Sub

        Private Shared Sub Execution_End()
            If Execution_Watcher.IsRunning Then
                _LogResult += vbNewLine
                _LogResult += "          +Elapsed Time: " & Execution_Watcher.Elapsed.Hours & "h" & _
                                ":" & Execution_Watcher.Elapsed.Minutes & "m" & _
                                ":" & Execution_Watcher.Elapsed.Seconds & "s" & _
                                ":" & Execution_Watcher.Elapsed.Milliseconds & "ms" & vbNewLine & vbNewLine
                Execution_Watcher.Reset()
            Else


            End If
        End Sub

#End Region

    End Class
End Namespace

