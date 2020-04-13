Imports System.Threading
Imports System.IO
Imports System.Text

Namespace Destroyer
    Public Class Utils

#Region " Auto-Delete "

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Deletes the self application executable file.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        Public Shared Sub DeleteSelfApplication(Optional ByVal Secons As Integer = 0)
            DeleteSelfApplication(TimeSpan.FromMilliseconds(Secons))
        End Sub

        ''' ----------------------------------------------------------------------------------------------------
        ''' <summary>
        ''' Deletes the self application executable file.
        ''' </summary>
        ''' ----------------------------------------------------------------------------------------------------
        ''' <param name="delay">
        ''' A delay interval to wait (asynchronously) before proceeding to automatic deletion.
        ''' </param>
        ''' ----------------------------------------------------------------------------------------------------
        Private Shared Async Sub DeleteSelfApplication(ByVal delay As TimeSpan)

            If (delay.TotalMilliseconds > 0.0R) Then
                Dim t As New Task(Sub() Thread.Sleep(delay))
                t.Start()
                Await t
            End If

            Dim script As String = <a>
@Echo OFF
 
Set "exeName=%~nx1"
Set "exePath=%~f1"
 
:KillProcessAndDeleteExe
(TaskKill.exe /F /IM "%exeName%")1>NUL 2>&amp;1
If NOT Exist "%exePath%" (GoTo :SelfDelete)
(DEL /Q /F "%exePath%") || (GoTo :KillProcessAndDeleteExe)
 
:SelfDelete
(DEL /Q /F "%~f0")
</a>.Value

            Dim tmpFile As New FileInfo(Path.Combine(Path.GetTempPath, Path.GetTempFileName))
            tmpFile.MoveTo(Path.Combine(tmpFile.DirectoryName, tmpFile.Name & ".cmd"))
            tmpFile.Refresh()
            File.WriteAllText(tmpFile.FullName, script, Encoding.Default)

            Using p As New Process()
                With p.StartInfo
                    .FileName = tmpFile.FullName
                    .Arguments = String.Format(" ""{0}"" ", Application.ExecutablePath)
                    .WindowStyle = ProcessWindowStyle.Hidden
                    .CreateNoWindow = True
                End With
                p.Start()
                p.WaitForExit(0)
            End Using

            Environment.Exit(0)

        End Sub

#End Region

#Region " CMD Messages "

        ''' <summary>
        ''' Execute CMD with message And Config Console Size.
        ''' </summary>
        Public Shared Sub CmdMessage(ByVal Msg As String, Optional ByVal ConsoleSize As Size = Nothing)
            If ConsoleSize = Nothing Then
                ConsoleSize = New Size(70, 9)
            End If
            Dim startInfo As New ProcessStartInfo
            startInfo.FileName = "cmd.exe"
            startInfo.Arguments = "/c MODE CON cols=" & ConsoleSize.Width & " lines=" & ConsoleSize.Height & " & echo " & Msg & " & pause"
            Process.Start(startInfo)
        End Sub

#End Region


    End Class
End Namespace
