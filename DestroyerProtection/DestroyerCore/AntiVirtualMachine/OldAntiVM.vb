Imports System.Runtime.InteropServices
Imports System.Management

' ***********************************************************************
' Author           : Destroyer
' Last Modified On : 09-05-2019
' Discord          : Destroyer#8328
' ***********************************************************************
' <copyright file="OldAntiVM.vb" company="S4Lsalsoft">
'     Copyright (c) S4Lsalsoft. All rights reserved.
' </copyright>
' ***

Namespace Destroyer.AntiVirtualMachine
    Public Class OldAntiVM

#Region "PInvokes"

        <DllImport("kernel32.dll", BestFitMapping:=True, CallingConvention:=CallingConvention.Winapi, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Shared Function LoadLibrary(<MarshalAs(UnmanagedType.LPStr)> ByVal moduleName As String) As IntPtr
        End Function

#End Region

#Region " Declarations "

        Private Shared DetectVM As ResultType = ResultType.Indeterminate
        Private Shared DesCriptVM As String = String.Empty

#End Region

#Region "Propiertes"

        Public Shared Function IsVirtualMachinePresent() As Boolean
            Return DetectVM
        End Function

        Public Shared Function DescriptcionVM() As String
            Return DesCriptVM
        End Function

#End Region

#Region " Types "

        Public Enum ResultType
            Secure = 0
            Danger = 1
            Indeterminate = 2
        End Enum

#End Region

#Region " Public Methods  "

        Public Shared Sub VM_Start()

            Dim ScanAsyncEngine As New Task(StarScan, TaskCreationOptions.LongRunning)

            ScanAsyncEngine.Start()

        End Sub

        Private Shared StarScan As New Action(
     Sub()
         StartProcessScan()
     End Sub)

        Private Shared Sub StartProcessScan()

            If LoadLibrary("SbieDll.dll") = True Then
                DesCriptVM = "Sandboxie Detected"
                DetectVM = ResultType.Danger
            End If

            If Detect_Virtual_Machine() = True Then
                DesCriptVM = "VM Detected"
                DetectVM = ResultType.Danger
            End If

            If Not AntiVM() = "False" Then
                DesCriptVM = AntiVM()
                DetectVM = ResultType.Danger
            End If

            DetectVM = ResultType.Secure
        End Sub

#End Region

#Region "Funcs"

        Private Shared Function Detect_Virtual_Machine() As Boolean
            Dim ModelName As String = Nothing
            Dim SearchQuery = New ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive WHERE BytesPerSector > 0")

            For Each ManagementSystem In SearchQuery.Get

                ModelName = ManagementSystem("Model").ToString.Split(" ").First.ToLower

                If ModelName = "virtual" Or _
                   ModelName = "vmware" Or _
                   ModelName = "vbox" Or _
                   ModelName = "qemu" _
                Then
                    Return True
                End If

            Next

            Return False
        End Function

        Private Shared Function AntiVM() As String
            Dim oItem
            With GetObject("winmgmts:\\.\root\cimv2")
                For Each oItem In .ExecQuery("Select * from Win32_ComputerSystem")
                    Debug.Print(oItem.Model, CBool(InStr(1, oItem.Model, "Virtual", vbTextCompare)))
                    If CBool(InStr(1, oItem.Model, "Virtual", vbTextCompare)) = True Then
                        Return "Win32_ComputerSystem"
                    End If

                Next

                For Each oItem In .ExecQuery("Select * from Win32_VideoController")
                    If CBool(InStr(1, oItem.Caption, "VMWare", vbTextCompare)) = True Then
                        Return "Win32_VideoController"
                    End If
                Next

                For Each Nic In .ExecQuery("SELECT * FROM Win32_NetworkAdapterConfiguration")

                    If Not IsDBNull(Nic.MACAddress) And Not IsDBNull(Nic.Description) Then
                        Dim MacAddress = LCase(CStr(Nic.MACAddress))
                        Dim Description = LCase(CStr(Nic.Description))
                        If InStr(1, MacAddress, "08:00:27:") = 1 And InStr(1, Description, "virtualbox") = 0 Then
                            Return "NetworkAdapterConfiguration"
                        End If
                    End If

                Next


                For Each SysDrv In .ExecQuery("SELECT * FROM Win32_SystemDriver")
                    Dim DescSysDrv = SysDrv.Description
                    Dim DispSysDrv = SysDrv.DisplayName
                    Dim NameSysDrv = SysDrv.Name
                    Dim PathSysDrv = SysDrv.PathName
                    If Not IsDBNull(DescSysDrv) Then
                        If DescSysDrv = "VirtualBox Guest Driver" Or DescSysDrv = "VirtualBox Guest Mouse Service" Or DescSysDrv = "VirtualBox Shared Folders" Or DescSysDrv = "VBoxVideo" Then
                            'test 
                            Return "Win32_SystemDriver ==> SysDrv.Description ==> " & DescSysDrv
                        End If
                    End If

                    If Not IsDBNull(DispSysDrv) Then
                        If DispSysDrv = "VirtualBox Guest Driver" Or DispSysDrv = "VirtualBox Guest Mouse Service" Or DispSysDrv = "VirtualBox Shared Folders" Or DispSysDrv = "VBoxVideo" Then
                            'test
                            Return "Win32_SystemDriver ==> SysDrv.DisplayName ==> " & DispSysDrv
                        End If
                    End If

                    If Not IsDBNull(NameSysDrv) Then
                        If NameSysDrv = "VBoxGuest" Or NameSysDrv = "VBoxMouse" Or NameSysDrv = "VBoxSF" Or NameSysDrv = "VBoxVideo" Then
                            'test 
                            Return "Win32_SystemDriver ==> SysDrv.Name ==> " & NameSysDrv
                        End If
                    End If

                    If Not IsDBNull(PathSysDrv) Then
                        Dim PathSysDrv_l = LCase(PathSysDrv)
                        If InStr(1, PathSysDrv_l, "vboxguest.sys") > 0 Or InStr(1, PathSysDrv_l, "vboxmouse.sys") > 0 Or InStr(1, PathSysDrv_l, "vboxsf.sys") > 0 Or InStr(1, PathSysDrv_l, "vboxvideo.sys") > 0 Then
                            'test
                            Return "Win32_SystemDriver ==> SysDrv.PathName ==> " & PathSysDrv
                        End If
                    End If
                Next

                For Each EvtLogX In .ExecQuery("SELECT * FROM Win32_NTEventlogFile")
                    If Not IsDBNull(EvtLogX) Then
                        Dim FileNameEvtX = CStr(EvtLogX.FileName)
                        Dim FileNameEvtX_l = LCase(FileNameEvtX)
                        If FileNameEvtX_l = "sysevent" Or FileNameEvtX_l = "system" Then
                            Dim SourcesEvtX = EvtLogX.Sources
                            For Each SourceEvtX In SourcesEvtX
                                Dim SourceEvtX_l = LCase(CStr(SourceEvtX))
                                If SourceEvtX_l = "vboxvideo" Then
                                    'test 
                                    Return "Win32_NTEventlogFile ==> EvtLogX.Sources ==> " & SourceEvtX
                                End If
                            Next
                        End If
                    End If
                Next


                For Each Bios In .ExecQuery("SELECT * FROM Win32_BIOS")
                    If Not IsDBNull(Bios) Then
                        If Not IsDBNull(Bios.Manufacturer) Then
                            Dim ManufacturerBios = LCase(CStr(Bios.Manufacturer))
                            If InStr(1, ManufacturerBios, "innotek gmbh") > 0 Then
                                'test 
                                Return "Win32_BIOS ==> Bios.Manufacturer ==> " & Bios.Manufacturer
                            End If
                        End If
                        If Not IsDBNull(Bios.SMBIOSBIOSVersion) Then
                            Dim SMBIOSBIOSVersionBios = LCase(CStr(Bios.SMBIOSBIOSVersion))
                            If InStr(1, SMBIOSBIOSVersionBios, "virtualbox") > 0 Then

                                Return "Win32_BIOS ==> Bios.SMBIOSBIOSVersion ==> " & Bios.SMBIOSBIOSVersion
                            End If
                        End If
                        If Not IsDBNull(Bios.Version) Then
                            Dim VersionBios = LCase(CStr(Bios.Version))
                            If InStr(1, VersionBios, "vbox   - 1") > 0 Then
                                'test 
                                Return "Win32_BIOS ==> Bios.Version ==> " & Bios.Version
                            End If
                        End If
                    End If
                Next

                For Each DiskDrive In .ExecQuery("SELECT * FROM Win32_DiskDrive")
                    If Not IsDBNull(DiskDrive) Then
                        If Not IsDBNull(DiskDrive.Model) Then
                            Dim ModelDskDrv = LCase(DiskDrive.Model)
                            If ModelDskDrv = "vbox harddisk" Then
                                'test 
                                Return "Win32_DiskDrive ==> DiskDrive.Model ==> " & DiskDrive.Model
                            End If
                        End If
                        If Not IsDBNull(DiskDrive.PNPDeviceID) Then
                            Dim PNPDeviceIDDskDrv = LCase(DiskDrive.PNPDeviceID)
                            If InStr(1, PNPDeviceIDDskDrv, "diskvbox") > 0 Then
                                'test 
                                Return "Win32_DiskDrive ==> DiskDrive.PNPDeviceID ==> " & DiskDrive.PNPDeviceID
                            End If
                        End If
                    End If
                Next

                For Each Startup In .ExecQuery("SELECT * FROM Win32_StartupCommand")
                    If Not IsDBNull(Startup) Then
                        If Not IsDBNull(Startup.Caption) Then
                            Dim CaptionStartup = LCase(CStr(Startup.Caption))
                            If CaptionStartup = "vboxtray" Then
                                'test 
                                Return "Win32_StartupCommand ==> Startup.Caption ==> " & Startup.Caption
                            End If
                        End If
                        If Not IsDBNull(Startup.Command) Then
                            Dim CommandStartup = LCase(CStr(Startup.Command))
                            If InStr(1, CommandStartup, "vboxtray.exe") > 0 Then
                                'test 
                                Return "Win32_StartupCommand ==> Startup.Command ==> " & Startup.Command
                            End If
                        End If
                        If Not IsDBNull(Startup.Description) Then
                            Dim DescStartup = LCase(CStr(Startup.Description))
                            If DescStartup = "vboxtray" Then

                                Return "Win32_StartupCommand ==> Startup.Description ==> " & Startup.Description
                            End If
                        End If
                    End If
                Next

                For Each ComputerSystem In .ExecQuery("SELECT * FROM Win32_ComputerSystem")
                    If Not IsDBNull(ComputerSystem) Then
                        If Not IsDBNull(ComputerSystem.Manufacturer) Then
                            Dim ManufacturerComputerSystem = LCase(CStr(ComputerSystem.Manufacturer))
                            If ManufacturerComputerSystem = "innotek gmbh" Then
                                'test 
                                Return "Win32_ComputerSystem ==> ComputerSystem.Manufacturer ==> " & ComputerSystem.Manufacturer
                            End If
                        End If
                        If Not IsDBNull(ComputerSystem.Model) Then
                            Dim ModelComputerSystem = LCase(CStr(ComputerSystem.Model))
                            If ModelComputerSystem = "virtualbox" Then

                                Return "Win32_ComputerSystem ==> ComputerSystem.Model ==> " & ComputerSystem.Model
                            End If
                        End If
                        If Not IsDBNull(ComputerSystem.OEMStringArray) Then
                            Dim OEMStringArrayComputerSystem = ComputerSystem.OEMStringArray
                            For Each OEM In OEMStringArrayComputerSystem
                                Dim OEM_l = LCase(OEM)
                                If InStr(1, OEM_l, "vboxver_") > 0 Or InStr(1, OEM_l, "vboxrev_") > 0 Then
                                    'test
                                    Return "Win32_ComputerSystem ==> ComputerSystem.OEMStringArray ==> " & OEM
                                End If
                            Next
                        End If
                    End If
                Next


                For Each Service In .ExecQuery("SELECT * FROM Win32_Service")
                    If Not IsDBNull(Service) Then
                        If Not IsDBNull(Service.Caption) Then
                            Dim CaptionService = LCase(CStr(Service.Caption))
                            If CaptionService = "virtualbox guest additions service" Then
                                'test 
                                Return "Win32_Service ==> Service.Caption ==> " & Service.Caption
                            End If
                        End If
                        If Not IsDBNull(Service.DisplayName) Then
                            Dim DisplayNameService = LCase(CStr(Service.DisplayName))
                            If DisplayNameService = "virtualbox guest additions service" Then
                                'test 
                                Return "Win32_Service ==> Service.DisplayName ==> " & Service.DisplayName
                            End If
                        End If
                        If Not IsDBNull(Service.Name) Then
                            Dim NameService = LCase(CStr(Service.Name))
                            If NameService = "vboxservice" Then
                                'test 
                                Return "Win32_Service ==> Service.Name ==> " & Service.Name
                            End If
                        End If
                        If Not IsDBNull(Service.PathName) Then
                            Dim PathNameService = LCase(CStr(Service.PathName))
                            If InStr(1, PathNameService, "vboxservice.exe") > 0 Then
                                'test 
                                Return "Win32_Service ==> Service.PathName ==> " & Service.PathName
                            End If
                        End If
                    End If
                Next


                For Each LogicalDisk In .ExecQuery("SELECT * FROM Win32_LogicalDisk")
                    If Not IsDBNull(LogicalDisk) Then
                        If Not IsDBNull(LogicalDisk.DriveType) Then
                            If LogicalDisk.DriveType = 3 Then
                                If Not IsDBNull(LogicalDisk.VolumeSerialNumber) Then
                                    Dim VolumeSerialNumberLogicalDisk = LCase(LogicalDisk.VolumeSerialNumber)
                                    If VolumeSerialNumberLogicalDisk = "fceae0a3" Then
                                        'test 
                                        Return "Win32_LogicalDisk ==> LogicalDisk.VolumeSerialNumber ==> " & LogicalDisk.VolumeSerialNumber
                                    End If
                                End If
                            ElseIf LogicalDisk.DriveType = 5 Then
                                If Not IsDBNull(LogicalDisk.VolumeName) Then
                                    Dim VolumeNameLogicalDisk = LCase(LogicalDisk.VolumeName)
                                    'Volume name should be "VBOXADDITIONS_4."
                                    If InStr(1, VolumeNameLogicalDisk, "vboxadditions") > 0 Then

                                        Return "Win32_LogicalDisk ==> LogicalDisk.VolumeName ==> " & LogicalDisk.VolumeName
                                    End If
                                End If
                            End If
                        End If
                    End If
                Next

                '//////////////////////////////////////////////////////////////////////////


                For Each LocalProgramGroup In .ExecQuery("SELECT * FROM Win32_LogicalProgramGroup")
                    If Not IsDBNull(LocalProgramGroup) Then
                        Dim NameLocalProgramGroup = LCase(LocalProgramGroup.Name)
                        If InStr(1, NameLocalProgramGroup, "oracle vm virtualbox guest additions") > 0 Then
                            'test 
                            Return "Win32_LogicalProgramGroup ==> LocalProgramGroup.Name ==> " & LocalProgramGroup.Name
                        End If
                    End If
                Next



                'Win32_NetworkAdapter aka NIC
                For Each NIC_x In .ExecQuery("SELECT * FROM Win32_NetworkAdapter")
                    If Not IsDBNull(NIC_x.MACAddress) And Not IsDBNull(NIC_x.Description) Then
                        Dim MacAddress_x = LCase(CStr(NIC_x.MACAddress))
                        Dim Description_x = LCase(CStr(NIC_x.Description))
                        'We want to detect the VirtualBox guest, not the host
                        If InStr(1, MacAddress_x, "08:00:27:") = 1 And InStr(1, Description_x, "virtualbox") = 0 Then
                            'test 
                            Return "Wow: Win32_NetworkAdapter ==> NIC.MacAddress: " & NIC_x.MACAddress
                        End If
                    End If
                Next


                'Win32_Process aka process
                For Each Process In .ExecQuery("SELECT * FROM Win32_Process")
                    If Not IsDBNull(Process) Then
                        If Not IsDBNull(Process.Description) Then
                            Dim DescProcess = LCase(Process.Description)
                            If DescProcess = "vboxservice.exe" Or DescProcess = "vboxtray.exe" Then
                                'test 
                                Return "Win32_Process ==> Process.Description ==> " & Process.Description
                            End If
                        End If
                        If Not IsDBNull(Process.Name) Then
                            Dim NameProcess = LCase(Process.Name)
                            If NameProcess = "vboxservice.exe" Or NameProcess = "vboxtray.exe" Then
                                ''test "Win32_Process ==> Process.Name ==> " & Process.Name
                                Return True
                            End If
                        End If
                        If Not IsDBNull(Process.CommandLine) Then
                            Dim CmdProcess = LCase(Process.CommandLine)
                            If InStr(1, CmdProcess, "vboxservice.exe") > 0 Or InStr(1, CmdProcess, "vboxtray.exe") > 0 Then
                                'test 
                                Return "Win32_Service ==> Process.CommandLine ==> " & Process.CommandLine
                            End If
                        End If
                        If Not IsDBNull(Process.ExecutablePath) Then
                            Dim ExePathProcess = LCase(Process.ExecutablePath)
                            If InStr(1, ExePathProcess, "vboxservice.exe") > 0 Or InStr(1, ExePathProcess, "vboxtray.exe") > 0 Then
                                ''test 
                                Return "Win32_Service ==> Process.ExecutablePath ==> " & Process.ExecutablePath
                            End If
                        End If
                    End If
                Next

                'Win32_BaseBoard aka BaseBoard

                For Each BaseBoard In .ExecQuery("SELECT * FROM Win32_BaseBoard")
                    If Not IsDBNull(BaseBoard) Then
                        If Not IsDBNull(BaseBoard.Manufacturer) Then
                            Dim ManufacturerBaseBoard = LCase(BaseBoard.Manufacturer)
                            If ManufacturerBaseBoard = "oracle corporation" Then
                                ''test 
                                Return "Win32_BaseBoard ==> BaseBoard.Manufacturer ==> " & BaseBoard.Manufacturer
                            End If
                        End If
                        If Not IsDBNull(BaseBoard.Product) Then
                            Dim ProductBaseBoard = LCase(BaseBoard.Product)
                            If ProductBaseBoard = "virtualbox" Then
                                ''test 
                                Return "Win32_BaseBoard ==> BaseBoard.Product ==> " & BaseBoard.Product
                            End If
                        End If
                    End If
                Next

                'Win32_SystemEnclosure aka SystemEnclosure

                For Each SystemEnclosure In .ExecQuery("SELECT * FROM Win32_SystemEnclosure")
                    If Not IsDBNull(SystemEnclosure) Then
                        If Not IsDBNull(SystemEnclosure.Manufacturer) Then
                            Dim ManufacturerSystemEnclosure = LCase(SystemEnclosure.Manufacturer)
                            If ManufacturerSystemEnclosure = "oracle corporation" Then
                                'test 
                                Return "Win32_SystemEnclosure ==> SystemEnclosure.Manufacturer ==> " & SystemEnclosure.Manufacturer
                            End If
                        End If
                    End If
                Next

                'Win32_CDROMDrive aka cdrom
                For Each CDRom In .ExecQuery("SELECT * FROM Win32_CDROMDrive")
                    If Not IsDBNull(CDRom) Then
                        If Not IsDBNull(CDRom.Name) Then
                            Dim NameCDRom = LCase(CDRom.Name)
                            If NameCDRom = "vbox cd-rom" Then
                                'test 
                                Return "Win32_CDROMDrive ==> CDRom.Name ==> " & CDRom.Name
                            End If
                        End If
                        If Not IsDBNull(CDRom.VolumeName) Then
                            Dim VolumeNameCDRom = LCase(CDRom.VolumeName)
                            'Volume name should be "VBOXADDITIONS_4."
                            If InStr(1, VolumeNameCDRom, "vboxadditions") > 0 Then
                                'test 
                                Return "Win32_CDROMDrive ==> CDRom.VolumeName ==> " & CDRom.VolumeName
                            End If
                        End If
                        If Not IsDBNull(CDRom.DeviceID) Then
                            Dim DeviceIDCDRom = LCase(CDRom.DeviceID)
                            If InStr(1, DeviceIDCDRom, "cdromvbox") > 0 Then
                                'test 
                                Return "Win32_CDROMDrive ==> CDRom.DeviceID ==> " & CDRom.DeviceID
                            End If
                        End If
                        If Not IsDBNull(CDRom.PNPDeviceID) Then
                            Dim PNPDeviceIDCDRom = LCase(CDRom.PNPDeviceID)
                            If InStr(1, PNPDeviceIDCDRom, "cdromvbox") > 0 Then

                                Return "Win32_CDROMDrive ==> CDRom.PNPDeviceID ==> " & CDRom.PNPDeviceID
                            End If
                        End If
                    End If
                Next


                'WIN32_NetworkClient aka netclient
                For Each NetClient In .ExecQuery("SELECT * FROM WIN32_NetworkClient")
                    If Not IsDBNull(NetClient) Then
                        If Not IsDBNull(NetClient.Description) Then
                            Dim DescNetClient = LCase(NetClient.Description)
                            If DescNetClient = "vboxsf" Then
                                'test 
                                Return "WIN32_NetworkClient ==> NetClient.Description ==> " & NetClient.Description
                            End If
                        End If
                        If Not IsDBNull(NetClient.Manufacturer) Then
                            Dim ManufacturerNetClient = LCase(NetClient.Manufacturer)
                            If ManufacturerNetClient = "oracle corporation" Then
                                'test 
                                Return "WIN32_NetworkClient ==> NetClient.Manufacturer ==> " & NetClient.Manufacturer
                            End If
                        End If
                        If Not IsDBNull(NetClient.Name) Then
                            Dim NameNetClient = LCase(NetClient.Name)
                            If NameNetClient = "virtualbox shared folders" Then
                                'test 
                                Return "WIN32_NetworkClient ==> NetClient.Name ==> " & NetClient.Name
                            End If
                        End If
                    End If
                Next

                'Win32_ComputerSystemProduct aka csproduct

                For Each CSProduct In .ExecQuery("SELECT * FROM Win32_ComputerSystemProduct")
                    If Not IsDBNull(CSProduct) Then
                        If Not IsDBNull(CSProduct.Name) Then
                            Dim NameCSProduct = LCase(CSProduct.Name)
                            If NameCSProduct = "virtualbox" Then
                                'test 
                                Return "Win32_ComputerSystemProduct ==> CSProduct.Name ==> " & CSProduct.Name
                            End If
                        End If
                        If Not IsDBNull(CSProduct.Vendor) Then
                            Dim VendorCSProduct = LCase(CSProduct.Vendor)
                            If VendorCSProduct = "innotek gmbh" Then
                                'test 
                                Return "Win32_ComputerSystemProduct ==> CSProduct.Vendor ==> " & CSProduct.Vendor
                            End If
                        End If
                    End If
                Next

                'Win32_VideoController

                For Each VideoController In .ExecQuery("SELECT * FROM Win32_VideoController")
                    If Not IsDBNull(VideoController) Then
                        If Not IsDBNull(VideoController.Name) Then
                            Dim NameVideoController = LCase(VideoController.Name)
                            If NameVideoController = "virtualbox graphics adapter" Then
                                'test 
                                Return "Win32_VideoController ==> VideoController.Name ==> " & VideoController.Name
                            End If
                        End If
                        If Not IsDBNull(VideoController.Description) Then
                            Dim DescVideoController = LCase(VideoController.Description)
                            If DescVideoController = "virtualbox graphics adapter" Then
                                'test 
                                Return "Win32_VideoController ==> VideoController.Description ==> " & VideoController.Description
                            End If
                        End If
                        If Not IsDBNull(VideoController.Caption) Then
                            Dim CaptionVideoController = LCase(VideoController.Caption)
                            If CaptionVideoController = "virtualbox graphics adapter" Then
                                'test 
                                Return "Win32_VideoController ==> VideoController.Caption ==> " & VideoController.Caption
                            End If
                        End If
                        If Not IsDBNull(VideoController.VideoProcessor) Then
                            Dim VideoProcessorVideoController = LCase(VideoController.VideoProcessor)
                            If VideoProcessorVideoController = "vbox" Then

                                Return "Win32_VideoController ==> VideoController.VideoProcessor ==> " & VideoController.VideoProcessor
                            End If
                        End If
                        If Not IsDBNull(VideoController.InstalledDisplayDrivers) Then
                            Dim InstalledDisplayDriversVideoController = LCase(VideoController.InstalledDisplayDrivers)
                            If InstalledDisplayDriversVideoController = "vboxdisp.sys" Then
                                'test 
                                Return "Win32_VideoController ==> VideoController.InstalledDisplayDrivers ==> " & VideoController.InstalledDisplayDrivers
                            End If
                        End If
                        If Not IsDBNull(VideoController.InfSection) Then
                            Dim InfSectionVideoController = LCase(VideoController.InfSection)
                            If InfSectionVideoController = "vboxvideo" Then
                                'test 
                                Return "Win32_VideoController ==> VideoController.InfSection ==> " & VideoController.InfSection
                            End If
                        End If
                        If Not IsDBNull(VideoController.AdapterCompatibility) Then
                            Dim AdapterCompatibilityVideoController = LCase(VideoController.AdapterCompatibility)
                            If AdapterCompatibilityVideoController = "oracle corporation" Then
                                'test 
                                Return "Win32_VideoController ==> VideoController.AdapterCompatibility ==> " & VideoController.AdapterCompatibility
                            End If
                        End If
                    End If
                Next


                'Win32_PnPEntity

                For Each PnPEntity In .ExecQuery("SELECT * FROM Win32_PnPEntity")
                    If Not IsDBNull(PnPEntity) Then
                        If Not IsDBNull(PnPEntity.Name) Then
                            Dim NamePnPEntity = LCase(PnPEntity.Name)
                            If NamePnPEntity = "virtualbox device" Or NamePnPEntity = "vbox harddisk" Or NamePnPEntity = "vbox cd-rom" Or NamePnPEntity = "virtualbox graphics adapter" Then
                                'test 
                                Return "Win32_PnPEntity ==> PnPEntity.Name ==> " & PnPEntity.Name
                            End If
                        End If
                        If Not IsDBNull(PnPEntity.Caption) Then
                            Dim CaptionPnPEntity = LCase(PnPEntity.Caption)
                            If CaptionPnPEntity = "virtualbox device" Or CaptionPnPEntity = "vbox harddisk" Or CaptionPnPEntity = "vbox cd-rom" Or CaptionPnPEntity = "virtualbox graphics adapter" Then
                                'test 
                                Return "Win32_PnPEntity ==> PnPEntity.Caption ==> " & PnPEntity.Caption
                            End If
                        End If
                        If Not IsDBNull(PnPEntity.Description) Then
                            Dim DescPnPEntity = LCase(PnPEntity.Description)
                            If DescPnPEntity = "virtualbox device" Or DescPnPEntity = "virtualbox graphics adapter" Then

                                Return "Win32_PnPEntity ==> PnPEntity.Description ==> " & PnPEntity.Description
                            End If
                        End If
                        If Not IsDBNull(PnPEntity.Service) Then
                            Dim SrvPnPEntity = LCase(PnPEntity.Service)
                            If SrvPnPEntity = "vboxguest" Or SrvPnPEntity = "vboxvideo" Then

                                Return "Win32_PnPEntity ==> PnPEntity.Service ==> " & PnPEntity.Service
                            End If
                        End If
                        If Not IsDBNull(PnPEntity.DeviceID) Then
                            Dim DeviceIDPnPEntity = LCase(PnPEntity.DeviceID)
                            If InStr(1, DeviceIDPnPEntity, "diskvbox_") > 0 Or InStr(1, DeviceIDPnPEntity, "cdromvbox_") > 0 Then
                                'test 
                                Return "Win32_PnPEntity ==> PnPEntity.DeviceID ==> " & PnPEntity.DeviceID
                            End If
                        End If
                        If Not IsDBNull(PnPEntity.PNPDeviceID) Then
                            Dim PNPDeviceIDPnPEntity = LCase(PnPEntity.PNPDeviceID)
                            If InStr(1, PNPDeviceIDPnPEntity, "diskvbox_") > 0 Or InStr(1, PNPDeviceIDPnPEntity, "cdromvbox_") > 0 Then
                                'test 
                                Return "Win32_PnPEntity ==> PnPEntity.PNPDeviceID ==> " & PnPEntity.PNPDeviceID
                            End If
                        End If
                    End If
                Next

                'Win32_NetworkConnection aka NetUse

                For Each NetUse In .ExecQuery("SELECT * FROM Win32_NetworkConnection")
                    If Not IsDBNull(NetUse) Then
                        If Not IsDBNull(NetUse.Name) Then
                            Dim NameNetUse = LCase(NetUse.Name)
                            If InStr(1, NameNetUse, "vboxsvr") > 0 Then
                                'test
                                Return "Win32_NetworkConnection ==> NetUse.Name ==> " & NetUse.Name
                            End If
                        End If
                        If Not IsDBNull(NetUse.Description) Then
                            Dim DescNetUse = LCase(NetUse.Description)
                            If InStr(1, DescNetUse, "virtualbox shared folders") > 0 Then
                                'test 
                                Return "Win32_NetworkConnection ==> NetUse.Description ==> " & NetUse.Description
                            End If
                        End If
                        If Not IsDBNull(NetUse.ProviderName) Then
                            Dim PrvNameNetUse = LCase(NetUse.ProviderName)
                            If PrvNameNetUse = "virtualbox shared folders" Then
                                'test 
                                Return "Win32_NetworkConnection ==> NetUse.ProviderName ==> " & NetUse.ProviderName
                            End If
                        End If

                        If Not IsDBNull(NetUse.RemoteName) Then
                            Dim RemoteNameNetUse = LCase(NetUse.RemoteName)
                            If InStr(1, RemoteNameNetUse, "vboxsvr") > 0 Then
                                'test 
                                Return "Win32_NetworkConnection ==> NetUse.RemoteName ==> " & NetUse.RemoteName
                            End If
                        End If
                        If Not IsDBNull(NetUse.RemotePath) Then
                            Dim RemotePathNetUse = LCase(NetUse.RemotePath)
                            If InStr(1, RemotePathNetUse, "vboxsvr") > 0 Then
                                'test  
                                Return "Win32_NetworkConnection ==> NetUse.RemotePath ==> " & NetUse.RemotePath
                            End If
                        End If
                    End If
                Next

            End With
            Return "False"
        End Function

#End Region

    End Class
End Namespace

