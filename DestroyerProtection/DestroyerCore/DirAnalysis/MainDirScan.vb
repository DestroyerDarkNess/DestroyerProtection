Imports System.IO

Namespace Destroyer

    Public Class MainDirScan

#Region " Properties "

        Private Shared _LogResult As String = String.Empty
        Public Shared ReadOnly Property LogResult As String
            Get
                Return _LogResult
            End Get
        End Property

#End Region

#Region " Declare's "

        Dim MaliciusFolderName() As String = {"de4dot", _
                                               "dnSpy"}

        Public ReadOnly Property ResultDirScan As Boolean
            Get
                Return MaliciousDir()
            End Get
        End Property

#End Region

#Region " Methods "

        Public Function MaliciousDir() As Boolean
            Dim DirRoaming As List(Of String) = GetFolders(GetAppDataPath())

            For Each FolderPth As String In DirRoaming
                Dim FolderN As String = LCase(System.IO.Path.GetFileName(FolderPth))
                For Each FolderMname As String In MaliciusFolderName
                    If CheckString(FolderN, LCase(FolderMname)) = True Then
                        _LogResult = "Malicious path detected: " & FolderPth & vbNewLine
                        Return True
                    End If
                Next
            Next

            Return False
        End Function

        Private Function CheckString(ByVal Foldername As String, ByVal FolderIdent As String)
            Return Foldername.Contains(FolderIdent)
        End Function

        Private Function GetFolders(ByVal DirP As String) As List(Of String)
            Return FileDirSearcher.GetDirPaths(DirP, SearchOption.TopDirectoryOnly).ToList
        End Function

        Private Function GetAppDataPath() As String
            Return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
        End Function

#End Region

    End Class

End Namespace
