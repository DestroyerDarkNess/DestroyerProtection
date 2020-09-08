using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Net;
using Microsoft.VisualBasic;
using System.Runtime.InteropServices;
using DestroyerProtectionLib;


namespace DestroyerProtectionTest
{

    // [assembly: Debuggable(System.Diagnostics.DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
    // <Assembly: DefaultDllImportSearchPaths(DllImportSearchPath.System32)> 'Optional
   

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        
        public struct ResultScan
        {
            public static string ProcessResult = string.Empty;
            public static string DebuggersResult = string.Empty;
            public static string VMResult = string.Empty;
            public static bool Finish = false;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            ResultScan.ProcessResult = string.Empty;
            ResultScan.DebuggersResult = string.Empty;
            ResultScan.VMResult = string.Empty;
            ResultScan.Finish = false;
            TextBox1.Text = string.Empty;
            DestroyerProtectionLib.Destroyer.Protect.DestroyerProtect.Start(DestroyerProtectionLib.Destroyer.AntiAnalysis.MainAnalysis.SearchType.FromNameandTitle, DestroyerProtectionLib.Destroyer.AntiDebug.MainDebug.DebugDetectionTypes.AllScanEgines, DestroyerProtectionLib.Destroyer.Protect.DestroyerProtect.VMScanType.By_Directory_Known);
            CheckTimer1.Enabled = true;
        }

        private void CheckTimer1_Tick(object sender, EventArgs e)
        {
            DestroyerProtectionLib.Destroyer.Protect.DestroyerProtect.ResultType Finispscan = DestroyerProtectionLib.Destroyer.Protect.DestroyerProtect.Process_scanner;
            DestroyerProtectionLib.Destroyer.Protect.DestroyerProtect.ResultType FinisDbugscan = DestroyerProtectionLib.Destroyer.Protect.DestroyerProtect.Debugers_scanner;
            DestroyerProtectionLib.Destroyer.Protect.DestroyerProtect.ResultType FinisVMscan = DestroyerProtectionLib.Destroyer.Protect.DestroyerProtect.VM_scanner;
            DestroyerProtectionLib.Destroyer.MainDirScan Mdirs = new DestroyerProtectionLib.Destroyer.MainDirScan();
            bool MdirBool = Mdirs.MaliciousDir();

            if (Finispscan == DestroyerProtectionLib.Destroyer.Protect.DestroyerProtect.ResultType.Danger)
                ResultScan.ProcessResult = "[DANGER] Malicious Process has been Detected";
            else if (Finispscan == DestroyerProtectionLib.Destroyer.Protect.DestroyerProtect.ResultType.Secure)
                ResultScan.ProcessResult = "[SECURE] Malicious Process Not Detected";

            if (FinisDbugscan == DestroyerProtectionLib.Destroyer.Protect.DestroyerProtect.ResultType.Danger)
                ResultScan.DebuggersResult = "[DANGER] Debuggers Detected";
            else if (FinisDbugscan == DestroyerProtectionLib.Destroyer.Protect.DestroyerProtect.ResultType.Secure)
                ResultScan.DebuggersResult = "[SECURE] Debuggers not Detected";

            if (FinisVMscan == DestroyerProtectionLib.Destroyer.Protect.DestroyerProtect.ResultType.Danger)
                ResultScan.VMResult = "[DANGER] Virutal Machines Have Been Detected";
            else if (FinisVMscan == DestroyerProtectionLib.Destroyer.Protect.DestroyerProtect.ResultType.Secure)
                ResultScan.VMResult = "[SECURE] No Virtual Machines Found";

            if (!(ResultScan.ProcessResult == string.Empty) & !(ResultScan.DebuggersResult == string.Empty) & !(ResultScan.VMResult == string.Empty))
            {
                if (MdirBool == true)
                    TextBox1.Text += "Detected DnSpy or De4dot in your system." + Environment.NewLine;
                TextBox1.Text += ResultScan.ProcessResult + Environment.NewLine + ResultScan.DebuggersResult + Environment.NewLine + ResultScan.VMResult + Environment.NewLine;
                TextBox1.Text += DestroyerProtectionLib.Destroyer.Protect.DestroyerProtect.LogResult + Environment.NewLine;
                TextBox2.Text += DestroyerProtectionLib.Destroyer.Protect.DestroyerProtect.DetectDescription + Environment.NewLine;
                ResultScan.Finish = true;
                DestroyerProtectionLib.Destroyer.AntiDump.MainDump.AntiDumpEnabled(this);
                CheckTimer1.Enabled = false;
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            ResultScan.ProcessResult = string.Empty;
            ResultScan.DebuggersResult = string.Empty;
            ResultScan.VMResult = string.Empty;
            ResultScan.Finish = false;
            TextBox1.Text = string.Empty;
            DestroyerProtectionLib.Destroyer.Protect.DestroyerProtect.MonitorAsync(DestroyerProtectionLib.Destroyer.AntiAnalysis.MainAnalysis.SearchType.FromNameandTitle, DestroyerProtectionLib.Destroyer.AntiDebug.MainDebug.DebugDetectionTypes.AllScanEgines, DestroyerProtectionLib.Destroyer.Protect.DestroyerProtect.VMScanType.By_Directory_Known);
            MonitorTimer1.Enabled = true;
        }

        private void MonitorTimer1_Tick(object sender, EventArgs e)
        {
            DestroyerProtectionLib.Destroyer.Protect.DestroyerProtect.ResultType Finispscan = DestroyerProtectionLib.Destroyer.Protect.DestroyerProtect.Process_scanner;
            DestroyerProtectionLib.Destroyer.Protect.DestroyerProtect.ResultType FinisDbugscan = DestroyerProtectionLib.Destroyer.Protect.DestroyerProtect.Debugers_scanner;
            DestroyerProtectionLib.Destroyer.Protect.DestroyerProtect.ResultType VMscanner = DestroyerProtectionLib.Destroyer.Protect.DestroyerProtect.VM_scanner;
            DestroyerProtectionLib.Destroyer.MainDirScan Mdirs = new DestroyerProtectionLib.Destroyer.MainDirScan();

            if (Finispscan == DestroyerProtectionLib.Destroyer.Protect.DestroyerProtect.ResultType.Danger)
                ResultScan.ProcessResult = "[DANGER] Malicious Process has been Detected";
            else if (Finispscan == DestroyerProtectionLib.Destroyer.Protect.DestroyerProtect.ResultType.Secure)
                ResultScan.ProcessResult = "[SECURE] Malicious Process Not Detected";

            if (FinisDbugscan == DestroyerProtectionLib.Destroyer.Protect.DestroyerProtect.ResultType.Danger)
                ResultScan.DebuggersResult = "[DANGER] Debuggers Detected";
            else if (FinisDbugscan == DestroyerProtectionLib.Destroyer.Protect.DestroyerProtect.ResultType.Secure)
                ResultScan.DebuggersResult = "[SECURE] Debuggers not Detected";

            if (VMscanner == DestroyerProtectionLib.Destroyer.Protect.DestroyerProtect.ResultType.Danger)
                ResultScan.VMResult = "[DANGER] Virutal Machines Have Been Detected";
            else if (Finispscan == DestroyerProtectionLib.Destroyer.Protect.DestroyerProtect.ResultType.Secure)
                ResultScan.VMResult = "[SECURE] No Virtual Machines Found";

            if (!(ResultScan.ProcessResult == string.Empty))
            {
                // DeleteSelfApplication() ' Auto Delete Function
                TextBox1.Text += ResultScan.ProcessResult + Environment.NewLine;
                TextBox1.Text += DestroyerProtectionLib.Destroyer.Protect.DestroyerProtect.LogResult + Environment.NewLine;
                TextBox2.Text += DestroyerProtectionLib.Destroyer.Protect.DestroyerProtect.DetectDescription + Environment.NewLine;
                ResultScan.Finish = true;
                MonitorTimer1.Enabled = false;
            }

            if (!(ResultScan.DebuggersResult == string.Empty))
            {
                // DeleteSelfApplication() ' Auto Delete Function
                TextBox1.Text += ResultScan.DebuggersResult + Environment.NewLine;
                TextBox1.Text += DestroyerProtectionLib.Destroyer.Protect.DestroyerProtect.LogResult + Environment.NewLine;
                TextBox2.Text += DestroyerProtectionLib.Destroyer.Protect.DestroyerProtect.DetectDescription + Environment.NewLine;
                ResultScan.Finish = true;
                MonitorTimer1.Enabled = false;
            }

            if (Mdirs.MaliciousDir() == true)
            {
                // DeleteSelfApplication() ' Auto Delete Function
                TextBox1.Text += "Detect DnSpy and De4dot." + Environment.NewLine;
                TextBox2.Text += DestroyerProtectionLib.Destroyer.Protect.DestroyerProtect.DetectDescription + Environment.NewLine;
                ResultScan.Finish = true;
                MonitorTimer1.Enabled = false;
            }

            if (!(ResultScan.VMResult == string.Empty))
            {
                // DeleteSelfApplication() ' Auto Delete Function
                TextBox1.Text += ResultScan.VMResult + Environment.NewLine;
                TextBox2.Text += DestroyerProtectionLib.Destroyer.Protect.DestroyerProtect.DetectDescription + Environment.NewLine;
                ResultScan.Finish = true;
                MonitorTimer1.Enabled = false;
            }
        }




    }
}
