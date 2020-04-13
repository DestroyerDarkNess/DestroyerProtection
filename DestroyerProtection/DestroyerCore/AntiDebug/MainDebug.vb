Imports System.Runtime.InteropServices
Imports System.Management
Imports System.Security.Principal

' ***********************************************************************
' Author           : Destroyer
' Last Modified On : 01-04-2020
' Discord          : Destroyer#8328
' ***********************************************************************
' <copyright file="MainDebug.vb" company="S4Lsalsoft">
'     Copyright (c) S4Lsalsoft. All rights reserved.
' </copyright>
' ***

Namespace Destroyer.AntiDebug
    Public Class MainDebug

#Region " Pinvoke "

        <DllImport("Kernel32.dll", SetLastError:=False)>
        Friend Shared Function IsDebuggerPresent(
    ) As <MarshalAs(UnmanagedType.Bool)> Boolean
        End Function

        <DllImport("ntdll.dll", SetLastError:=True)>
        Public Shared Function CsrGetProcessId() As Integer
        End Function

        <DllImport("kernel32.dll", SetLastError:=True)>
        Public Shared Function OpenProcess(
                                      ByVal dwDesiredAccess As Integer,
                                      ByVal bInheritHandle As Boolean,
                                      ByVal dwProcessId As Integer) As IntPtr
        End Function



#End Region

#Region " Types "

        Public Enum DebugDetectionTypes
            IsDebuggerPresent = 0
            IsDbgCsrss = 1
            Parent_Process = 2
            IsDebuggerAttached = 3
            IsDebuggerLogging = 4
            AllScanEgines = 5
            None = 6
        End Enum

#End Region

#Region " Public Methods "

        Public Shared Function Debugger_Check(ByVal DebugTypeFunction As DebugDetectionTypes) As Boolean
            Select Case DebugTypeFunction
                Case DebugDetectionTypes.IsDebuggerPresent : Return IsDebuggerPresent()
                Case DebugDetectionTypes.IsDebuggerAttached : Return IsDebuggerAttached()
                Case DebugDetectionTypes.IsDebuggerLogging : Return IsDebuggerLogging()
                Case DebugDetectionTypes.IsDbgCsrss : Return False 'IsDbgCsrss()
                Case DebugDetectionTypes.Parent_Process : Return Parent_Process()
                Case DebugDetectionTypes.AllScanEgines : Return AllScan()
            End Select
            Return False
        End Function

        Private Shared Function AllScan() As Boolean
            If IsDebuggerPresent() = True Then
                Return True
            End If
            'If IsDbgCsrss() = True Then
            'Return True
            'End If
            If IsDebuggerAttached() = True Then
                Return True
            End If
            If IsDebuggerLogging() = True Then
                Return True
            End If
            If Parent_Process() = True Then
                Return True
            End If
            Return False
        End Function

        Public Shared Function IsDebuggerAttached() As Boolean
            Return Debugger.IsAttached
        End Function

        Public Shared Function IsDebuggerLogging() As Boolean
            Return Debugger.IsLogging
        End Function

        Public Shared Sub DebuggerBreak()
            Debugger.Break()
        End Sub

        '////////////////////////////////////////////////////////////////////////
        'Administrator privileges are required for this Function.
        Const PROCESS_ALL_ACCESS = &H1F0FFF
        Public Shared Function IsDbgCsrss() As Boolean
            If IsAdministrator = True Then
                Return CBool(OpenProcess(PROCESS_ALL_ACCESS, 0, CsrGetProcessId))
            End If
            Return False
        End Function
        'Administrator privileges are required for this Function.
        '////////////////////////////////////////////////////////////////////////

        Public Shared Function Parent_Process(Optional ByVal ProcessName As String = "explorer.exe") As Boolean
            If ProcessName.ToLower.EndsWith(".exe") Then ProcessName = ProcessName.Substring(0, ProcessName.Length - 4)
            Dim myId = Process.GetCurrentProcess().Id
            Dim query = String.Format("SELECT ParentProcessId FROM Win32_Process WHERE ProcessId = {0}", myId)
            Dim search = New ManagementObjectSearcher("root\CIMV2", query)
            Dim results = search.[Get]().GetEnumerator()
            results.MoveNext()
            Dim queryObj = results.Current
            Dim parentId = CUInt(queryObj("ParentProcessId"))
            Dim parent = Process.GetProcessById(CInt(parentId))
            Dim ParentProcessName As String = parent.ProcessName
            If LCase(ParentProcessName) = LCase(ProcessName) Then
                Return False
            Else
                Return True
            End If
        End Function

        Public Shared ReadOnly Property IsAdministrator As Boolean
            Get
                Return New WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator)
            End Get
        End Property

#End Region

    End Class
End Namespace

