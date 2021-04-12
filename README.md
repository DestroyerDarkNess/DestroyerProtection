# DestroyerProtection

![Preview](https://i.imgur.com/tN8zOfd.png) ![Preview](https://i.ibb.co/nQybzxG/S2.png)

![Preview](https://i.ibb.co/cwFHJ1z/Dnspytest.png)

## Introduction
Several classes that give you Protection for your .NET Application.
Remember to leave your Star to the Project! Thank you!

If you like my work and want to support it, then please consider to deposit a donation through Paypal by clicking on the next link: [www.paypal.me/SalvadorKrilewski](https://www.paypal.me/SalvadorKrilewski)

## Possible uses

- You are making an anti-debug plugin and you want to check its effectiveness.
- You want to ensure that your sandbox solution is hidden enough.
- Or you want to make sure your application has at least a minimum of Protection.
- It also has functions which you can adapt to your packer.

## Features
### Anti-debugging attacks
- IsDebuggerPresent
- CheckRemoteDebuggerPresent
- Parent Process.

### Anti-Dumping
- Erase PE header from memory (Fix For Winform App and Console App)

### Anti-Virtualization / Full-System Emulation
- **Registry key value artifacts**
  - HARDWARE\\DEVICEMAP\\Scsi\\Scsi Port 0\\Scsi Bus 0\\Target Id 0\\Logical Unit Id 0 (Identifier) (VBOX)
  - HARDWARE\\DEVICEMAP\\Scsi\\Scsi Port 0\\Scsi Bus 0\\Target Id 0\\Logical Unit Id 0 (Identifier) (QEMU)
  - HARDWARE\\Description\\System (SystemBiosVersion) (VBOX)
  - HARDWARE\\Description\\System (SystemBiosVersion) (QEMU)
  - HARDWARE\\Description\\System (VideoBiosVersion) (VIRTUALBOX)
  - HARDWARE\\Description\\System (SystemBiosDate) (06/23/99)
  - HARDWARE\\DEVICEMAP\\Scsi\\Scsi Port 0\\Scsi Bus 0\\Target Id 0\\Logical Unit Id 0 (Identifier) (VMWARE)
  - HARDWARE\\DEVICEMAP\\Scsi\\Scsi Port 1\\Scsi Bus 0\\Target Id 0\\Logical Unit Id 0 (Identifier) (VMWARE)
  - HARDWARE\\DEVICEMAP\\Scsi\\Scsi Port 2\\Scsi Bus 0\\Target Id 0\\Logical Unit Id 0 (Identifier) (VMWARE)
  - SYSTEM\\ControlSet001\\Control\\SystemInformation (SystemManufacturer) (VMWARE)
  - SYSTEM\\ControlSet001\\Control\\SystemInformation (SystemProductName) (VMWARE)
- **Registry Keys artifacts**
  - HARDWARE\\ACPI\\DSDT\\VBOX__ (VBOX)
  - HARDWARE\\ACPI\\FADT\\VBOX__ (VBOX)
  - HARDWARE\\ACPI\\RSDT\\VBOX__ (VBOX)
  - SOFTWARE\\Oracle\\VirtualBox Guest Additions (VBOX)
  - SYSTEM\\ControlSet001\\Services\\VBoxGuest (VBOX)
  - SYSTEM\\ControlSet001\\Services\\VBoxMouse (VBOX)
  - SYSTEM\\ControlSet001\\Services\\VBoxService (VBOX)
  - SYSTEM\\ControlSet001\\Services\\VBoxSF (VBOX)
  - SYSTEM\\ControlSet001\\Services\\VBoxVideo (VBOX)
  - SOFTWARE\\VMware, Inc.\\VMware Tools (VMWARE)
  - SOFTWARE\\Wine (WINE)
  - SOFTWARE\Microsoft\Virtual Machine\Guest\Parameters (HYPER-V)
- **File system artifacts**
  - "system32\\drivers\\VBoxMouse.sys"
  - "system32\\drivers\\VBoxGuest.sys"
  - "system32\\drivers\\VBoxSF.sys"
  - "system32\\drivers\\VBoxVideo.sys"
  - "system32\\vboxdisp.dll"
  - "system32\\vboxhook.dll"
  - "system32\\vboxmrxnp.dll"
  - "system32\\vboxogl.dll"
  - "system32\\vboxoglarrayspu.dll"
  - "system32\\vboxoglcrutil.dll"
  - "system32\\vboxoglerrorspu.dll"
  - "system32\\vboxoglfeedbackspu.dll"
  - "system32\\vboxoglpackspu.dll"
  - "system32\\vboxoglpassthroughspu.dll"
  - "system32\\vboxservice.exe"
  - "system32\\vboxtray.exe"
  - "system32\\VBoxControl.exe"
  - "system32\\drivers\\vmmouse.sys"
  - "system32\\drivers\\vmhgfs.sys"
  - "system32\\drivers\\vm3dmp.sys"
  - "system32\\drivers\\vmci.sys"
  - "system32\\drivers\\vmhgfs.sys"
  - "system32\\drivers\\vmmemctl.sys"
  - "system32\\drivers\\vmmouse.sys"
  - "system32\\drivers\\vmrawdsk.sys"
  - "system32\\drivers\\vmusbmouse.sys"

### Anti-Analysis
- **Processes**
  - OllyDBG / ImmunityDebugger / WinDbg / IDA Pro
  - SysInternals Suite Tools (Process Explorer / Process Monitor / Regmon / Filemon, TCPView, Autoruns)
  - Wireshark / Dumpcap
  - ProcessHacker / SysAnalyzer / HookExplorer / SysInspector
  - ImportREC / PETools / LordPE
  - JoeBox Sandbox
  - MegaDumper.
  
  ### Additional features
  - DnSpy Detection.
  - De4dot Detection.
  - Auto-Delete Function.
  - CMD Message.
  
  ### Exploits
  - Debug Message Flood (In testing)
  
  ## Contributors
- Destroyer : Creator and Developer.  / Discord : Destroyer#8328 
- Harold : It helped the development. / Discord : H a r o l d#7626


  ## Special thanks :
- [ElektroStudios](https://github.com/ElektroStudios): For its Snippet FileDirSearcher and other Functions.
   - Which are part of its [DevCase Framework](https://codecanyon.net/item/elektrokit-class-library-for-net/19260282)
- [LordNoteworthy](https://github.com/LordNoteworthy): For your Project al-khaser

  
