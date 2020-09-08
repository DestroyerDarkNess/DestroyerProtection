Imports System.Runtime.InteropServices

' ***********************************************************************
' Author           : Destroyer
' Last Modified On : 01-04-2020
' Discord          : Destroyer#8328
' ***********************************************************************
' <copyright file="MainVM.vb" company="S4Lsalsoft">
'     Copyright (c) S4Lsalsoft. All rights reserved.
' </copyright>
' ***

Namespace Destroyer.AntiVirtualMachine
    Public Class MainVM

#Region " Pinvoke's "

        <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True)> _
        Public Shared Function GetModuleHandle(ByVal lpModuleName As String) As IntPtr
        End Function

        <DllImport("kernel32.dll", SetLastError:=True, CharSet:=CharSet.Ansi, ExactSpelling:=True)> _
        Public Shared Function GetProcAddress(ByVal hModule As IntPtr, ByVal procName As String) As UIntPtr
        End Function

#End Region

#Region " Properties "

        Private Shared _LogResult As String = String.Empty
        Public Shared ReadOnly Property LogResult As String
            Get
                Return _LogResult
            End Get
        End Property

#End Region

#Region " Methods "

        Public Shared Function VirtualMachine_Check() As Boolean
            If VMWareCheck() = True Then
                Return True
            End If
            If WineCheck() = True Then
                Return True
            End If
            If VirtualPCCheck() = True Then
                Return True
            End If
            If VBoxCheck() = True Then
                Return True
            End If
            Return False
        End Function

#Region " VMware Check"

        Public Shared Function VMWareCheck() As Boolean
            If vmware_reg_key_value() = True Then
                Return True
            End If
            If vmware_reg_keys() = True Then
                Return True
            End If
            If vmware_files() = True Then
                Return True
            End If
            If Not vmware_processes() = 0 Then
                Return True
            End If
            Return False
        End Function

        Private Shared Function vmware_reg_key_value() As Boolean
            ' Array of strings of blacklisted registry key values 
            Dim szEntries()() As String = {
                New String() {"HARDWARE\DEVICEMAP\Scsi\Scsi Port 0\Scsi Bus 0\Target Id 0\Logical Unit Id 0", "Identifier", "VMWARE"},
                New String() {"HARDWARE\DEVICEMAP\Scsi\Scsi Port 1\Scsi Bus 0\Target Id 0\Logical Unit Id 0", "Identifier", "VMWARE"},
                New String() {"HARDWARE\DEVICEMAP\Scsi\Scsi Port 2\Scsi Bus 0\Target Id 0\Logical Unit Id 0", "Identifier", "VMWARE"},
                New String() {"SYSTEM\ControlSet001\Control\SystemInformation", "SystemManufacturer", "VMWARE"},
                New String() {"SYSTEM\ControlSet001\Control\SystemInformation", "SystemProductName", "VMWARE"}
            }

            Dim dwLength As UShort = szEntries.Length
            For i As Integer = 0 To dwLength - 1
                If Not Microsoft.Win32.Registry.LocalMachine.OpenSubKey(szEntries(i)(0)) Is Nothing Then
                    Dim regKey As String = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(szEntries(i)(0)).GetValue(szEntries(i)(1)).ToString
                    If regKey = szEntries(i)(2) Then
                        _LogResult = "Malicious Registry Value: " & regKey.ToString & vbNewLine
                        Return True
                    End If
                End If
            Next i
            Return False
        End Function

        Private Shared Function vmware_reg_keys() As Boolean
            ' Array of strings of blacklisted registry keys 
            Dim szKeys() As String = {"SOFTWARE\VMware", "Inc.\VMware Tools"}
            Dim dwlength As UShort = szKeys.Length

            ' Check one by one 
            For i As Integer = 0 To dwlength - 1
                If Not Microsoft.Win32.Registry.LocalMachine.OpenSubKey(szKeys(i)) Is Nothing Then
                    _LogResult = "Malicious Registry Value: " & "HKEY_LOCAL_MACHINE\" & szKeys(i).ToString & vbNewLine
                    Return True
                End If
            Next i
            Return False
        End Function


        Private Shared Function vmware_files() As Boolean
            Dim szPaths() As String = {"vmnet.sys", _
     "vmmouse.sys", _
     "vmusb.sys", _
     "vm3dmp.sys", _
     "vmci.sys", _
     "vmhgfs.sys", _
     "vmmemctl.sys", _
     "vmx86.sys", _
     "vmrawdsk.sys", _
     "vmusbmouse.sys", _
     "vmkdb.sys", _
     "vmnetuserif.sys", _
     "vmnetadapter.sys"}

            Dim winPath As String = Environment.GetEnvironmentVariable("windir") & "\System32\Drivers\"

            For Each DriverFile As String In szPaths
                If My.Computer.FileSystem.FileExists(winPath & DriverFile) Then
                    _LogResult = "Virtual Machine Detected. | " & "Detected Driver: " & winPath & DriverFile & vbNewLine
                    Return True
                End If
            Next
            Return False
        End Function

        Private Shared Function vmware_processes() As Integer
            Dim szProcesses() As String = {"vmtoolsd.exe", "vmwaretray.exe", "vmwareuser.exe", "VGAuthService.exe", "vmacthlp.exe"}
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

#End Region

#Region " WineVM Check"

        Public Shared Function WineCheck() As Boolean
            If wine_exports() = True Then
                Return True
            End If
            If wine_reg_keys() = True Then
                Return True
            End If
            Return False
        End Function

        Private Shared Function wine_exports() As Boolean
            Dim hKernel32 As IntPtr

            hKernel32 = GetModuleHandle("kernel32.dll")
            If hKernel32 = Nothing Then
                Return False
            End If

            ' Check if wine_get_unix_file_name is exported by this dll 
            If GetProcAddress(hKernel32, "wine_get_unix_file_name") = Nothing Then
                _LogResult = "Running on WineVM detected." & vbNewLine
                Return False
            Else
                Return True
            End If
        End Function

        Private Shared Function wine_reg_keys() As Boolean
            If Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\Wine") Is Nothing Then
                Return False
            Else
                _LogResult = "Malicious Registry Value: " & "HKEY_CURRENT_USER\SOFTWARE\Wine" & vbNewLine
                Return True
            End If
        End Function

#End Region

#Region " xen "

        Public Shared Function XenCheck() As Boolean
            If Not Xen_processes() = 0 Then
                Return True
            End If
            Return False
        End Function


        Private Shared Function Xen_processes() As Integer
            Dim szProcesses() As String = {"xenservice.exe"}
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

#End Region

#Region " VirtualPC "

        Public Shared Function VirtualPCCheck() As Boolean
            If VirtualPC_reg_keys() = True Then
                Return True
            End If
            If Not VirtualPC_processes() = 0 Then
                Return True
            End If
            Return False
        End Function

        Private Shared Function VirtualPC_reg_keys() As Boolean
            If Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Virtual Machine\\Guest\\Parameters") Is Nothing Then
                Return False
            Else
                _LogResult = "Malicious Registry Value: " & "HKEY_CURRENT_USER\SOFTWARE\Microsoft\Virtual Machine\Guest\Parameters" & vbNewLine
                Return True
            End If
        End Function

        Private Shared Function VirtualPC_processes() As Integer
            Dim szProcesses() As String = {"VMSrvc.exe", "VMUSrvc.exe"}
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

#End Region

#Region " VirtualBox "

        Public Shared Function VBoxCheck() As Boolean
            If vbox_reg_key_value() = True Then
                Return True
            End If
            If vbox_reg_keys() = True Then
                Return True
            End If
            If vbox_files() = True Then
                Return True
            End If
            If Not VBox_processes() = 0 Then
                Return True
            End If
            Return False
        End Function

        Private Shared Function vbox_reg_key_value() As Boolean
            ' Array of strings of blacklisted registry key values 
            Dim szEntries()() As String = {
     New String() {"HARDWARE\DEVICEMAP\Scsi\Scsi Port 0\Scsi Bus 0\Target Id 0\Logical Unit Id 0", "Identifier", "VBOX"},
     New String() {"HARDWARE\Description\System", "SystemBiosVersion", "VBOX"},
     New String() {"HARDWARE\Description\System", "VideoBiosVersion", "VIRTUALBOX"},
     New String() {"HARDWARE\Description\System", "SystemBiosDate", "06/23/99"}
 }
            Dim dwLength As UShort = szEntries.Length
            For i As Integer = 0 To dwLength - 1
                If Not Microsoft.Win32.Registry.LocalMachine.OpenSubKey(szEntries(i)(0)) Is Nothing Then
                    Dim regKey As String = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(szEntries(i)(0)).GetValue(szEntries(i)(1)).ToString
                    If regKey = szEntries(i)(2) Then
                        _LogResult = "Malicious Registry Value: " & regKey.ToString & vbNewLine
                        Return True
                    End If
                End If
            Next i
            Return False
        End Function

        Private Shared Function vbox_reg_keys() As Boolean
            ' Array of strings of blacklisted registry keys 
            Dim szKeys() As String = {"HARDWARE\ACPI\DSDT\VBOX__", _
                                      "HARDWARE\ACPI\FADT\VBOX__", _
                                      "HARDWARE\ACPI\RSDT\VBOX__", _
                                      "SOFTWARE\Oracle\VirtualBox Guest Additions", _
                                      "SYSTEM\ControlSet001\Services\VBoxGuest", _
                                      "SYSTEM\ControlSet001\Services\VBoxMouse", _
                                      "SYSTEM\ControlSet001\Services\VBoxService", _
                                      "SYSTEM\ControlSet001\Services\VBoxSF", _
                                      "SYSTEM\ControlSet001\Services\VBoxVideo"}

            Dim dwlength As UShort = szKeys.Length
            For i As Integer = 0 To dwlength - 1
                If Not Microsoft.Win32.Registry.LocalMachine.OpenSubKey(szKeys(i)) Is Nothing Then
                    _LogResult = "Malicious Registry Value: " & "HKEY_LOCAL_MACHINE\" & szKeys(i).ToString & vbNewLine
                    Return True
                End If
            Next i
            Return False
        End Function

        Private Shared Function vbox_files() As Boolean
            Dim szPaths() As String = {"System32\drivers\VBoxMouse.sys", _
                                       "System32\drivers\VBoxGuest.sys", _
                                       "System32\drivers\VBoxSF.sys", _
                                       "System32\drivers\VBoxVideo.sys", _
                                       "System32\vboxdisp.dll", _
                                       "System32\vboxhook.dll", _
                                       "System32\vboxmrxnp.dll", _
                                       "System32\vboxogl.dll", _
                                       "System32\vboxoglarrayspu.dll", _
                                       "System32\vboxoglcrutil.dll", _
                                       "System32\vboxoglerrorspu.dll", _
                                       "System32\vboxoglfeedbackspu.dll", _
                                       "System32\vboxoglpackspu.dll", _
                                       "System32\vboxoglpassthroughspu.dll", _
                                       "System32\vboxservice.exe", _
                                       "System32\vboxtray.exe", _
                                       "System32\VBoxControl.exe"}

            Dim winPath As String = Environment.GetEnvironmentVariable("windir") & "\"

            For Each DriverFile As String In szPaths
                If My.Computer.FileSystem.FileExists(winPath & DriverFile) Then
                    _LogResult = "VBox Detected. | " & "Detected Driver: " & winPath & DriverFile & vbNewLine
                    Return True
                End If
            Next
            Return False
        End Function

        Private Shared Function VBox_processes() As Integer
            Dim szProcesses() As String = {"vboxservice.exe", "vboxtray.exe"}
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

#End Region

#End Region

    End Class
End Namespace

