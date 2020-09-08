' ***********************************************************************
' Author           : Destroyer
' Last Modified On : 01-04-2020
' Discord          : Destroyer#8328
' ***********************************************************************
' <copyright file="MainAnalysis.vb" company="S4Lsalsoft">
'     Copyright (c) S4Lsalsoft. All rights reserved.
' </copyright>
' ***

Namespace Destroyer.AntiAnalysis
    Public Class MainAnalysis

        '// OllyDebug debugger
        '// Process Hacker
        '// Part of Sysinternals Suite
        '// Part of Sysinternals Suite
        '// Part of Sysinternals Suite
        '// Part of Sysinternals Suite
        '// Part of Sysinternals Suite
        '// Part of Sysinternals Suite
        '// Part of Sysinternals Suite
        '// IDA Pro Interactive Disassembler
        '// IDA Pro Interactive Disassembler
        '// ImmunityDebugger
        '// Wireshark packet sniffer
        '// Network traffic dump tool
        '// Find various types of runtime hooks
        '// Import Reconstructor
        '// PE Tool
        '// LordPE
        '// ESET SysInspector
        '// Part of SysAnalyzer iDefense
        '// Part of SysAnalyzer iDefense
        '// Part of SysAnalyzer iDefense
        '// Microsoft WinDbg
        '// Part of Joe Sandbox
        '// Part of Joe Sandbox
        '// Part of Joe Sandbox
        '// Resource Hacker
        '// x32dbg
        '// x64dbg
        '// Fiddler
        '// Http Debugger

#Region " Properties "

        Private Shared _LogResult As String = String.Empty
        Public Shared ReadOnly Property LogResult As String
            Get
                Return _LogResult
            End Get
        End Property

#End Region

#Region " Declare's "

        Private Shared szProcesses() As String = {"ollydbg.exe", _
                                      "ProcessHacker.exe", _
                                      "tcpview.exe", _
                                      "autoruns.exe", _
                                      "autorunsc.exe", _
                                      "filemon.exe", _
                                      "procmon.exe", _
                                      "regmon.exe", _
                                      "procexp.exe", _
                                      "idaq.exe", _
                                      "idaq64.exe", _
                                      "ImmunityDebugger.exe", _
                                      "Wireshark.exe", _
                                      "dumpcap.exe", _
                                      "HookExplorer.exe", _
                                      "ImportREC.exe", _
                                      "PETools.exe", _
                                      "LordPE.exe", _
                                      "SysInspector.exe", _
                                      "proc_analyzer.exe", _
                                      "sysAnalyzer.exe", _
                                      "sniff_hit.exe", _
                                      "windbg.exe", _
                                      "joeboxcontrol.exe", _
                                      "joeboxserver.exe", _
                                      "joeboxserver.exe", _
                                      "ResourceHacker.exe", _
                                      "x32dbg.exe", _
                                      "x64dbg.exe", _
                                      "Fiddler.exe", _
                                      "httpdebugger.exe", _
                                      "de4dot.exe", _
                                      "PEiD.exe", _
                                      "Universal_Fixer.exe", _
                                      "MegaDumper.exe", _
                                      "ida.exe", _
                                      "ida64.exe", _
                                      "cheatengine-i386.exe", _
                                      "cheatengine-x86_64.exe", _
                                      "Cheat Engine.exe", _
                                      "de4dot.exe", _
                                      "de4dot-x64.exe", _
                                      "dnSpy.exe", _
                                      "MegaDumper.exe"}

        Private Shared szProcessesTitle() As String = {"Olly", _
                                     "Process", _
                                     "Sysinternals", _
                                     "IDA", _
                                     "Disassembler", _
                                     "Debugger", _
                                     "Wireshark", _
                                     "packet", _
                                     "sniffer", _
                                     "dump", _
                                     "LordPE", _
                                     "PE Tool", _
                                     "ESET", _
                                     "WinDbg", _
                                     "Hook", _
                                     "Http Debugger", _
                                     "Fiddler", _
                                     "Resource Hacker", _
                                     "Dumper", _
                                     "The Interactive Disassembler", _
                                     "Cheat Engine", _
                                     "Winsock Packet Editor",
                                     "Winsock", _
                                     "artmoney", _
                                     "de4dot", _
                                     "dnSpy", _
                                     "MegaDumper"}



        Private Shared exclusion_processes() As String = {"chrome.exe", _
                                                          "Discord.exe", _
                                                          "MEGAsync.exe", _
                                                          "brave.exe", _
                                                          "Opera.exe", _
                                                          "Dropbox.exe", _
                                                          "explorer.exe", _
                                                          "taskmgr.exe"}

#End Region

#Region " Types "

        Public Enum SearchType
            FromName = 0
            FromTitle = 1
            FromNameandTitle = 2
            None = 3
        End Enum

#End Region

#Region " Public Methods "

        Public Shared Function Malicious_Processes_Found(ByVal Search_Level As SearchType) As Boolean
            Dim Nproc As Integer = 0

            Select Case Search_Level
                Case 0 : Nproc = ProcessMonitorFromName()
                Case 1 : Nproc = ProcessMonitorFromTitle()
                Case 2 : Nproc = (ProcessMonitorFromName() + ProcessMonitorFromTitle())
                Case 3 : Nproc = 0
            End Select

            If Nproc = 0 Then
                Return False
            Else
                Return True
            End If
        End Function

        Public Shared Function ProcessMonitorFromName() As Integer
            Dim ProcessesFound As Integer = 0
            For Each ProcessName As String In szProcesses
                If ProcessName.ToLower.EndsWith(".exe") Then ProcessName = ProcessName.Substring(0, ProcessName.Length - 4)
                For Each process As Process In process.GetProcessesByName(ProcessName)
                    _LogResult = "Malicious Process Detected: " & ProcessName & ".exe" & vbNewLine
                    ProcessesFound += 1
                Next
            Next
            Return ProcessesFound
        End Function

        Public Shared Function ProcessMonitorFromTitle() As Integer
            Dim ProcessesFound As Integer = 0

            Dim poc() As Process = Process.GetProcesses()

            For Each ProcessTitle As String In szProcessesTitle
                For i As Integer = 0 To poc.Length - 1
                    Dim NameProc As String = poc(i).ProcessName
                    Dim WinTitleProc As String = poc(i).MainWindowTitle
                    If ExclusionClases(exclusion_processes.Count, NameProc) = False Then
                        If InStr(1, LCase(WinTitleProc), LCase(ProcessTitle)) > 0 Then
                            _LogResult = "Malicious identification detected: " & WinTitleProc & " In process :" & NameProc & ".exe" & " | Corresponding to : " & ProcessTitle & vbNewLine
                            ProcessesFound += 1
                        End If
                    End If
                Next
            Next

            Return ProcessesFound
        End Function

#End Region

#Region " Private Methods "

        Private Shared Function ExclusionClases(ByVal CountClass As Integer, ByVal Clase As String) As Boolean
            If Clase.ToLower.EndsWith(".exe") Then Clase = Clase.Substring(0, Clase.Length - 4)
            Dim MaxValue As Integer = CountClass
            Dim DetectionCount As Integer = 0
            Dim ProcessClass As Integer = 0
            For Each Processnames As String In exclusion_processes
                If Processnames.ToLower.EndsWith(".exe") Then Processnames = Processnames.Substring(0, Processnames.Length - 4)

                If LCase(Processnames) = LCase(Clase) Then
                    DetectionCount += 1
                End If
                ProcessClass += 1
            Next
            If MaxValue = ProcessClass Then
                If DetectionCount > 0 Then
                    Return True
                End If
            End If
            Return False
        End Function

        Private Shared Function Get_Process_Window_Title(ByVal ProcessName As String) As String
            If ProcessName.ToLower.EndsWith(".exe") Then ProcessName = ProcessName.Substring(0, ProcessName.Length - 4)
            Dim ProcessArray = Process.GetProcessesByName(ProcessName)
            If ProcessArray.Length = 0 Then Return Nothing Else Return ProcessArray(0).MainWindowTitle
        End Function

#End Region

    End Class
End Namespace

