' ***********************************************************************
' Author   : Elektro
' Modified : 14-February-2015
' ***********************************************************************
' <copyright file="FileDirSearcher.vb" company="Elektro Studios">
'     Copyright (c) Elektro Studios. All rights reserved.
' </copyright>
' ***********************************************************************

#Region " Usage Examples "

'Dim files As List(Of FileInfo) = FileDirSearcher.GetFiles("C:\Windows\System32", SearchOption.AllDirectories).ToList
'Dim files As List(Of String) = FileDirSearcher.GetFilePaths("C:\Windows\System32", SearchOption.AllDirectories).ToList

'Dim dirs As List(Of DirectoryInfo) = FileDirSearcher.GetDirs("C:\Windows\System32", SearchOption.AllDirectories).ToList
'Dim dirs As List(Of String) = FileDirSearcher.GetDirPaths("C:\Windows\System32", SearchOption.AllDirectories).ToList

'Dim files As IEnumerable(Of FileInfo) = FileDirSearcher.GetFiles(dirPath:="C:\Windows\System32",
'                                                                 searchOption:=SearchOption.TopDirectoryOnly,
'                                                                 fileNamePatterns:={"*"},
'                                                                 fileExtPatterns:={"*.dll", "*.exe"},
'                                                                 ignoreCase:=True,
'                                                                 throwOnError:=True)

'Dim dirs As IEnumerable(Of DirectoryInfo) = FileDirSearcher.GetDirs(dirPath:="C:\Windows\System32",
'                                                                    searchOption:=SearchOption.TopDirectoryOnly,
'                                                                    dirPathPatterns:={"*"},
'                                                                    dirNamePatterns:={"*Microsoft*"},
'                                                                    ignoreCase:=True,
'                                                                    throwOnError:=True)

#End Region

#Region " Option Statements "

Option Explicit On
Option Strict On
Option Infer Off

#End Region

#Region " Imports "

Imports System.IO
Imports System.Collections.Concurrent
Imports System.Threading.Tasks

#End Region

#Region " File Dir Searcher "

''' <summary>
''' Searchs for files and directories.
''' </summary>
Public NotInheritable Class FileDirSearcher

#Region " Public Methods "

    ''' <summary>
    ''' Gets the files those matches the criteria inside the specified directory and/or sub-directories.
    ''' </summary>
    ''' <param name="dirPath">The root directory path to search for files.</param>
    ''' <param name="searchOption">The searching mode.</param>
    ''' <param name="fileNamePatterns">The file name pattern(s) to match.</param>
    ''' <param name="fileExtPatterns">The file extension pattern(s) to match.</param>
    ''' <param name="ignoreCase">If <c>True</c>, ignores the comparing case of <paramref name="fileNamePatterns"/> and <paramref name="fileExtPatterns"/> patterns.</param>
    ''' <param name="throwOnError">Determines whether exceptions will be thrown, like access denied to file or directory.</param>
    ''' <returns>An <see cref="IEnumerable(Of FileInfo)"/> instance containing the files information.</returns>
    ''' <exception cref="System.ArgumentException">dirPath or searchOption</exception>
    Public Shared Function GetFiles(ByVal dirPath As String,
                                    ByVal searchOption As SearchOption,
                                    Optional ByVal fileNamePatterns As IEnumerable(Of String) = Nothing,
                                    Optional ByVal fileExtPatterns As IEnumerable(Of String) = Nothing,
                                    Optional ByVal ignoreCase As Boolean = True,
                                    Optional ByVal throwOnError As Boolean = False) As IEnumerable(Of FileInfo)

        ' Analyze and fix path problems.
        ' eg. 'C:' -> 'C:\'
        AnalyzePath(dirPath)

        If Not Directory.Exists(dirPath) Then
            Throw New ArgumentException(String.Format("Directory doesn't exists: '{0}'", dirPath), "dirPath")

        ElseIf (searchOption <> searchOption.TopDirectoryOnly) AndAlso (searchOption <> searchOption.AllDirectories) Then
            Throw New ArgumentException(String.Format("Value of '{0}' is not valid enumeration value.", CStr(searchOption)), "searchOption")

        Else ' Get and return the files.
            Dim queue As New ConcurrentQueue(Of FileInfo)
            CollectFiles(queue, dirPath, searchOption, fileNamePatterns, fileExtPatterns, ignoreCase, throwOnError)
            Return queue.AsEnumerable

        End If

    End Function

    ''' <summary>
    ''' Gets the filepaths those matches the criteria inside the specified directory and/or sub-directories.
    ''' </summary>
    ''' <param name="dirPath">The root directory path to search for files.</param>
    ''' <param name="searchOption">The searching mode.</param>
    ''' <param name="fileNamePatterns">The file name pattern(s) to match.</param>
    ''' <param name="fileExtPatterns">The file extension pattern(s) to match.</param>
    ''' <param name="ignoreCase">If <c>True</c>, ignores the comparing case of <paramref name="fileNamePatterns"/> and <paramref name="fileExtPatterns"/> patterns.</param>
    ''' <param name="throwOnError">Determines whether exceptions will be thrown, like access denied to file or directory.</param>
    ''' <returns>An <see cref="IEnumerable(Of String)"/> instance containing the filepaths.</returns>
    ''' <exception cref="System.ArgumentException">dirPath or searchOption</exception>
    Public Shared Function GetFilePaths(ByVal dirPath As String,
                                        ByVal searchOption As SearchOption,
                                        Optional ByVal fileNamePatterns As IEnumerable(Of String) = Nothing,
                                        Optional ByVal fileExtPatterns As IEnumerable(Of String) = Nothing,
                                        Optional ByVal ignoreCase As Boolean = True,
                                        Optional ByVal throwOnError As Boolean = False) As IEnumerable(Of String)

        ' Analyze and fix path problems.
        ' eg. 'C:' -> 'C:\'
        AnalyzePath(dirPath)

        If Not Directory.Exists(dirPath) Then
            Throw New ArgumentException(String.Format("Directory doesn't exists: '{0}'", dirPath), "dirPath")

        ElseIf (searchOption <> searchOption.TopDirectoryOnly) AndAlso (searchOption <> searchOption.AllDirectories) Then
            Throw New ArgumentException(String.Format("Value of '{0}' is not valid enumeration value.", CStr(searchOption)), "searchOption")

        Else ' Get and return the filepaths.
            Dim queue As New ConcurrentQueue(Of String)
            CollectFilePaths(queue, dirPath, searchOption, fileNamePatterns, fileExtPatterns, ignoreCase, throwOnError)
            Return queue.AsEnumerable

        End If

    End Function

    ''' <summary>
    ''' Gets the directories those matches the criteria inside the specified directory and/or sub-directories.
    ''' </summary>
    ''' <param name="dirPath">The root directory path to search for directories.</param>
    ''' <param name="searchOption">The searching mode.</param>
    ''' <param name="dirPathPatterns">The directory path pattern(s) to match.</param>
    ''' <param name="dirNamePatterns">The directory name pattern(s) to match.</param>
    ''' <param name="ignoreCase">If <c>True</c>, ignores the comparing case of <paramref name="dirPathPatterns"/> and <paramref name="dirNamePatterns"/> patterns.</param>
    ''' <param name="throwOnError">Determines whether exceptions will be thrown, like access denied to directory.</param>
    ''' <returns>An <see cref="IEnumerable(Of DirectoryInfo)"/> instance containing the dirrectories information.</returns>
    ''' <exception cref="System.ArgumentException">dirPath or searchOption</exception>
    Public Shared Function GetDirs(ByVal dirPath As String,
                                   ByVal searchOption As SearchOption,
                                   Optional ByVal dirPathPatterns As IEnumerable(Of String) = Nothing,
                                   Optional ByVal dirNamePatterns As IEnumerable(Of String) = Nothing,
                                   Optional ByVal ignoreCase As Boolean = True,
                                   Optional ByVal throwOnError As Boolean = False) As IEnumerable(Of DirectoryInfo)

        ' Analyze and fix path problems.
        ' eg. 'C:' -> 'C:\'
        AnalyzePath(dirPath)

        If Not Directory.Exists(dirPath) Then
            Throw New ArgumentException(String.Format("Directory doesn't exists: '{0}'", dirPath), "dirPath")

        ElseIf (searchOption <> searchOption.TopDirectoryOnly) AndAlso (searchOption <> searchOption.AllDirectories) Then
            Throw New ArgumentException(String.Format("Value of '{0}' is not valid enumeration value.", CStr(searchOption)), "searchOption")

        Else ' Get and return the files.
            Dim queue As New ConcurrentQueue(Of DirectoryInfo)
            CollectDirs(queue, dirPath, searchOption, dirPathPatterns, dirNamePatterns, ignoreCase, throwOnError)
            Return queue.AsEnumerable

        End If

    End Function

    ''' <summary>
    ''' Gets the filepaths those matches the criteria inside the specified directory and/or sub-directories.
    ''' </summary>
    ''' <param name="dirPath">The root directory path to search for directories.</param>
    ''' <param name="searchOption">The searching mode.</param>
    ''' <param name="dirPathPatterns">The directory path pattern(s) to match.</param>
    ''' <param name="dirNamePatterns">The directory name pattern(s) to match.</param>
    ''' <param name="ignoreCase">If <c>True</c>, ignores the comparing case of <paramref name="dirPathPatterns"/> and <paramref name="dirNamePatterns"/> patterns.</param>
    ''' <param name="throwOnError">Determines whether exceptions will be thrown, like access denied to directory.</param>
    ''' <returns>An <see cref="IEnumerable(Of String)"/> instance containing the directory paths.</returns>
    ''' <exception cref="System.ArgumentException">dirPath or searchOption</exception>
    Public Shared Function GetDirPaths(ByVal dirPath As String,
                                       ByVal searchOption As SearchOption,
                                       Optional ByVal dirPathPatterns As IEnumerable(Of String) = Nothing,
                                       Optional ByVal dirNamePatterns As IEnumerable(Of String) = Nothing,
                                       Optional ByVal ignoreCase As Boolean = True,
                                       Optional ByVal throwOnError As Boolean = False) As IEnumerable(Of String)

        ' Analyze and fix path problems.
        ' eg. 'C:' -> 'C:\'
        AnalyzePath(dirPath)

        If Not Directory.Exists(dirPath) Then
            Throw New ArgumentException(String.Format("Directory doesn't exists: '{0}'", dirPath), "dirPath")

        ElseIf (searchOption <> searchOption.TopDirectoryOnly) AndAlso (searchOption <> searchOption.AllDirectories) Then
            Throw New ArgumentException(String.Format("Value of '{0}' is not valid enumeration value.", CStr(searchOption)), "searchOption")

        Else ' Get and return the filepaths.
            Dim queue As New ConcurrentQueue(Of String)
            CollectDirPaths(queue, dirPath, searchOption, dirPathPatterns, dirNamePatterns, ignoreCase, throwOnError)
            Return queue.AsEnumerable

        End If

    End Function

#End Region

#Region " Private Methods "

    ''' <summary>
    ''' Analyzes a directory path and perform specific changes on it.
    ''' </summary>
    ''' <param name="dirPath">The directory path.</param>
    ''' <exception cref="System.ArgumentNullException">dirPath;Value is null, empty, or white-spaced.</exception>
    Private Shared Sub AnalyzePath(ByRef dirPath As String)

        If String.IsNullOrEmpty(dirPath) OrElse String.IsNullOrWhiteSpace(dirPath) Then
            Throw New ArgumentNullException("dirPath", "Value is null, empty, or white-spaced.")

        Else
            ' Trim unwanted characters.
            dirPath = dirPath.TrimStart({" "c}).TrimEnd({" "c})

            If Path.IsPathRooted(dirPath) Then
                ' The root paths contained on the returned FileInfo objects will start with the same string-case as this root path.
                ' So just for a little visual improvement, I'll treat this root path as a Drive-Letter and I convert it to UpperCase.
                dirPath = Char.ToUpper(dirPath.First) & dirPath.Substring(1)
            End If

            If Not dirPath.EndsWith("\"c) Then
                ' Possibly its a drive letter without backslash ('C:') or else just a normal path without backslash ('C\Dir').
                ' In any case, fix the ending backslash.
                dirPath = dirPath.Insert(dirPath.Length, "\"c)
            End If

        End If

    End Sub

    ''' <summary>
    ''' Collects the files those matches the criteria inside the specified directory and/or sub-directories.
    ''' </summary>
    ''' <param name="queue">The <see cref="ConcurrentQueue(Of FileInfo)"/> instance to enqueue new files.</param>
    ''' <param name="dirPath">The root directory path to search for files.</param>
    ''' <param name="searchOption">The searching mode.</param>
    ''' <param name="fileNamePatterns">The file name pattern(s) to match.</param>
    ''' <param name="fileExtPatterns">The file extension pattern(s) to match.</param>
    ''' <param name="ignoreCase">If <c>True</c>, ignores the comparing case of <paramref name="fileNamePatterns"/> and <paramref name="fileExtPatterns"/> patterns.</param>
    ''' <param name="throwOnError">Determines whether exceptions will be thrown, like access denied to file or directory.</param>
    Private Shared Sub CollectFiles(ByVal queue As ConcurrentQueue(Of FileInfo),
                                    ByVal dirPath As String,
                                    ByVal searchOption As SearchOption,
                                    ByVal fileNamePatterns As IEnumerable(Of String),
                                    ByVal fileExtPatterns As IEnumerable(Of String),
                                    ByVal ignoreCase As Boolean,
                                    ByVal throwOnError As Boolean)

        ' Initialize a DirectoryInfo.
        Dim dirInfo As DirectoryInfo = Nothing

        ' Initialize a FileInfo collection.
        Dim fileInfoCol As IEnumerable(Of FileInfo) = Nothing

        Try ' Set up a new DirectoryInfo instance using the passed directory path.
            dirInfo = New DirectoryInfo(dirPath)

        Catch ex As ArgumentNullException
            If throwOnError Then
                Throw
            End If

        Catch ex As ArgumentException
            If throwOnError Then
                Throw
            End If

        Catch ex As Security.SecurityException
            If throwOnError Then
                Throw
            End If

        Catch ex As PathTooLongException
            If throwOnError Then
                Throw
            End If

        End Try

        Try ' Get the top files of the current directory.

            If fileExtPatterns IsNot Nothing Then
                ' Decrease time execution by searching for files that has extension.
                fileInfoCol = dirInfo.GetFiles("*.*", searchOption.TopDirectoryOnly)
            Else
                ' Search for all files.
                fileInfoCol = dirInfo.GetFiles("*", searchOption.TopDirectoryOnly)
            End If

        Catch ex As UnauthorizedAccessException
            If throwOnError Then
                Throw
            End If

        Catch ex As DirectoryNotFoundException
            If throwOnError Then
                Throw
            End If

        Catch ex As Exception
            If throwOnError Then
                Throw
            End If

        End Try

        ' If the fileInfoCol collection is not empty then...
        If fileInfoCol IsNot Nothing Then

            ' Iterate the files.
            For Each fInfo As FileInfo In fileInfoCol

                ' Flag to determine whether a filename pattern is matched.
                Dim flagNamePattern As Boolean = False

                ' Flag to determine whether a file extension pattern is matched.
                Dim flagExtPattern As Boolean = False

                ' If filename patterns collection is not empty then...
                If fileNamePatterns IsNot Nothing Then

                    ' Iterate the filename pattern(s).
                    For Each fileNamePattern As String In fileNamePatterns

                        ' Match the filename pattern on the current filename.

                        ' Supress consecuent conditionals if pattern its an asterisk.
                        If fileNamePattern.Equals("*", StringComparison.OrdinalIgnoreCase) Then
                            flagNamePattern = True ' Activate the filename flag.
                            Exit For

                        ElseIf ignoreCase Then ' Compare filename with ignoring case rules.
                            If fInfo.Name.ToLower Like fileNamePattern.ToLower Then
                                flagNamePattern = True ' Activate the filename flag.
                                Exit For
                            End If

                        Else ' Compare filename without ignoring case rules.
                            If fInfo.Name Like fileNamePattern Then
                                flagNamePattern = True ' Activate the filename flag.
                                Exit For
                            End If

                        End If ' ignoreCase

                    Next fileNamePattern

                Else ' filename patterns collection is empty.
                    flagNamePattern = True ' Activate the filename flag.

                End If ' fileNamePatterns IsNot Nothing

                ' If file extension patterns collection is not empty then...
                If fileExtPatterns IsNot Nothing Then

                    ' Iterate the file extension pattern(s).
                    For Each fileExtPattern As String In fileExtPatterns

                        ' Match the file extension pattern on the current file extension.

                        ' Supress consecuent conditionals if pattern its an asterisk.
                        If fileExtPattern.Equals("*", StringComparison.OrdinalIgnoreCase) Then
                            flagExtPattern = True ' Activate the file extension flag.
                            Exit For

                        ElseIf ignoreCase Then ' Compare filename with ignoring case rules.
                            If fInfo.Extension.ToLower Like fileExtPattern.ToLower Then
                                flagExtPattern = True ' Activate the file extension flag.
                                Exit For
                            End If

                        Else ' Compare filename without ignoring case rules.
                            If fInfo.Extension Like fileExtPattern Then
                                flagExtPattern = True ' Activate the file extension flag.
                                Exit For
                            End If

                        End If

                    Next fileExtPattern

                Else  ' file extension patterns collection is empty.
                    flagExtPattern = True ' Activate the file extension flag.

                End If ' fileExtPatterns IsNot Nothing

                ' If fileName and also fileExtension patterns are matched then...
                If flagNamePattern AndAlso flagExtPattern Then
                    queue.Enqueue(fInfo) ' Enqueue this FileInfo object.
                End If

            Next fInfo

        End If ' fileInfoCol IsNot Nothing

        ' If searchOption is recursive then...
        If searchOption = searchOption.AllDirectories Then

            Try
                ' Try next iterations.
                Task.WaitAll(dirInfo.GetDirectories.
                             Select(Function(dir As DirectoryInfo)
                                        Return Task.Factory.StartNew(Sub()
                                                                         CollectFiles(queue,
                                                                                      dir.FullName, searchOption.AllDirectories,
                                                                                      fileNamePatterns, fileExtPatterns,
                                                                                      ignoreCase, throwOnError)
                                                                     End Sub)
                                    End Function).ToArray)

            Catch ex As UnauthorizedAccessException
                If throwOnError Then
                    Throw
                End If

            Catch ex As DirectoryNotFoundException
                If throwOnError Then
                    Throw
                End If

            Catch ex As Exception
                If throwOnError Then
                    Throw
                End If

            End Try

        End If ' searchOption = searchOption.AllDirectories

    End Sub

    ''' <summary>
    ''' Collects the filepaths those matches the criteria inside the specified directory and/or sub-directories.
    ''' </summary>
    ''' <param name="queue">The <see cref="ConcurrentQueue(Of String)"/> instance to enqueue new filepaths.</param>
    ''' <param name="dirPath">The root directory path to search for files.</param>
    ''' <param name="searchOption">The searching mode.</param>
    ''' <param name="fileNamePatterns">The file name pattern(s) to match.</param>
    ''' <param name="fileExtPatterns">The file extension pattern(s) to match.</param>
    ''' <param name="ignoreCase">If <c>True</c>, ignores the comparing case of <paramref name="fileNamePatterns"/> and <paramref name="fileExtPatterns"/> patterns.</param>
    ''' <param name="throwOnError">Determines whether exceptions will be thrown, like access denied to file or directory.</param>
    Private Shared Sub CollectFilePaths(ByVal queue As ConcurrentQueue(Of String),
                                        ByVal dirPath As String,
                                        ByVal searchOption As SearchOption,
                                        ByVal fileNamePatterns As IEnumerable(Of String),
                                        ByVal fileExtPatterns As IEnumerable(Of String),
                                        ByVal ignoreCase As Boolean,
                                        ByVal throwOnError As Boolean)

        ' Initialize a filepath collection.
        Dim filePathCol As IEnumerable(Of String) = Nothing

        Try ' Get the top files of the current directory.

            If fileExtPatterns IsNot Nothing Then
                ' Decrease time execution by searching for files that has extension.
                filePathCol = Directory.GetFiles(dirPath, "*.*", searchOption.TopDirectoryOnly)
            Else
                ' Search for all files.
                filePathCol = Directory.GetFiles(dirPath, "*", searchOption.TopDirectoryOnly)
            End If

        Catch ex As UnauthorizedAccessException
            If throwOnError Then
                Throw
            End If

        Catch ex As DirectoryNotFoundException
            If throwOnError Then
                Throw
            End If

        Catch ex As Exception
            If throwOnError Then
                Throw
            End If

        End Try

        ' If the filepath collection is not empty then...
        If filePathCol IsNot Nothing Then

            ' Iterate the filepaths.
            For Each filePath As String In filePathCol

                ' Flag to determine whether a filename pattern is matched.
                Dim flagNamePattern As Boolean = False

                ' Flag to determine whether a file extension pattern is matched.
                Dim flagExtPattern As Boolean = False

                ' If filename patterns collection is not empty then...
                If fileNamePatterns IsNot Nothing Then

                    ' Iterate the filename pattern(s).
                    For Each fileNamePattern As String In fileNamePatterns

                        ' Match the filename pattern on the current filename.

                        ' Supress consecuent conditionals if pattern its an asterisk.
                        If fileNamePattern.Equals("*", StringComparison.OrdinalIgnoreCase) Then
                            flagNamePattern = True ' Activate the filename flag.
                            Exit For

                        ElseIf ignoreCase Then ' Compare filename with ignoring case rules.
                            If Path.GetFileNameWithoutExtension(filePath).ToLower Like fileNamePattern.ToLower Then
                                flagNamePattern = True ' Activate the filename flag.
                                Exit For
                            End If

                        Else ' Compare filename without ignoring case rules.
                            If Path.GetFileNameWithoutExtension(filePath) Like fileNamePattern Then
                                flagNamePattern = True ' Activate the filename flag.
                                Exit For
                            End If

                        End If ' ignoreCase

                    Next fileNamePattern

                Else ' filename patterns collection is empty.
                    flagNamePattern = True ' Activate the filename flag.

                End If ' fileNamePatterns IsNot Nothing

                ' If file extension patterns collection is not empty then...
                If fileExtPatterns IsNot Nothing Then

                    ' Iterate the file extension pattern(s).
                    For Each fileExtPattern As String In fileExtPatterns

                        ' Match the file extension pattern on the current file extension.

                        ' Supress consecuent conditionals if pattern its an asterisk.
                        If fileExtPattern.Equals("*", StringComparison.OrdinalIgnoreCase) Then
                            flagExtPattern = True ' Activate the file extension flag.
                            Exit For

                        ElseIf ignoreCase Then ' Compare filename with ignoring case rules.
                            If Path.GetExtension(filePath).ToLower Like fileExtPattern.ToLower Then
                                flagExtPattern = True ' Activate the file extension flag.
                                Exit For
                            End If

                        Else ' Compare filename without ignoring case rules.
                            If Path.GetExtension(filePath) Like fileExtPattern Then
                                flagExtPattern = True ' Activate the file extension flag.
                                Exit For
                            End If

                        End If

                    Next fileExtPattern

                Else  ' file extension patterns collection is empty.
                    flagExtPattern = True ' Activate the file extension flag.

                End If ' fileExtPatterns IsNot Nothing

                ' If fileName and also fileExtension patterns are matched then...
                If flagNamePattern AndAlso flagExtPattern Then
                    queue.Enqueue(filePath) ' Enqueue this filepath.
                End If

            Next filePath

        End If ' filePathCol IsNot Nothing

        ' If searchOption is recursive then...
        If searchOption = searchOption.AllDirectories Then

            Try
                ' Try next iterations.
                Task.WaitAll(New DirectoryInfo(dirPath).GetDirectories.
                                 Select(Function(dir As DirectoryInfo)
                                            Return Task.Factory.StartNew(Sub()
                                                                             CollectFilePaths(queue,
                                                                                              dir.FullName, searchOption.AllDirectories,
                                                                                              fileNamePatterns, fileExtPatterns,
                                                                                              ignoreCase, throwOnError)
                                                                         End Sub)
                                        End Function).ToArray)

            Catch ex As UnauthorizedAccessException
                If throwOnError Then
                    Throw
                End If

            Catch ex As DirectoryNotFoundException
                If throwOnError Then
                    Throw
                End If

            Catch ex As Exception
                If throwOnError Then
                    Throw
                End If

            End Try

        End If ' searchOption = searchOption.AllDirectories

    End Sub

    ''' <summary>
    ''' Collects the directories those matches the criteria inside the specified directory and/or sub-directories.
    ''' </summary>
    ''' <param name="queue">The <see cref="ConcurrentQueue(Of DirectoryInfo)"/> instance to enqueue new directories.</param>
    ''' <param name="dirPath">The root directory path to search for directories.</param>
    ''' <param name="searchOption">The searching mode.</param>
    ''' <param name="dirPathPatterns">The directory path pattern(s) to match.</param>
    ''' <param name="dirNamePatterns">The directory name pattern(s) to match.</param>
    ''' <param name="ignoreCase">If <c>True</c>, ignores the comparing case of <paramref name="dirPathPatterns"/> and <paramref name="dirNamePatterns"/> patterns.</param>
    ''' <param name="throwOnError">Determines whether exceptions will be thrown, like access denied to directory.</param>
    Private Shared Sub CollectDirs(ByVal queue As ConcurrentQueue(Of DirectoryInfo),
                                   ByVal dirPath As String,
                                   ByVal searchOption As SearchOption,
                                   ByVal dirPathPatterns As IEnumerable(Of String),
                                   ByVal dirNamePatterns As IEnumerable(Of String),
                                   ByVal ignoreCase As Boolean,
                                   ByVal throwOnError As Boolean)

        ' Initialize a DirectoryInfo.
        Dim dirInfo As DirectoryInfo = Nothing

        ' Initialize a DirectoryInfo collection.
        Dim dirInfoCol As IEnumerable(Of DirectoryInfo) = Nothing

        Try ' Set up a new DirectoryInfo instance using the passed directory path.
            dirInfo = New DirectoryInfo(dirPath)

        Catch ex As ArgumentNullException
            If throwOnError Then
                Throw
            End If

        Catch ex As ArgumentException
            If throwOnError Then
                Throw
            End If

        Catch ex As Security.SecurityException
            If throwOnError Then
                Throw
            End If

        Catch ex As PathTooLongException
            If throwOnError Then
                Throw
            End If

        End Try

        Try ' Get the top directories of the current directory.
            dirInfoCol = dirInfo.GetDirectories("*", searchOption.TopDirectoryOnly)

        Catch ex As UnauthorizedAccessException
            If throwOnError Then
                Throw
            End If

        Catch ex As DirectoryNotFoundException
            If throwOnError Then
                Throw
            End If

        Catch ex As Exception
            If throwOnError Then
                Throw
            End If

        End Try

        ' If the fileInfoCol collection is not empty then...
        If dirInfoCol IsNot Nothing Then

            ' Iterate the files.
            For Each dir As DirectoryInfo In dirInfoCol

                ' Flag to determine whether a directory path pattern is matched.
                Dim flagPathPattern As Boolean = False

                ' Flag to determine whether a directory name pattern is matched.
                Dim flagNamePattern As Boolean = False

                ' If directory path patterns collection is not empty then...
                If dirPathPatterns IsNot Nothing Then

                    ' Iterate the directory path pattern(s).
                    For Each dirpathPattern As String In dirPathPatterns

                        ' Match the directory path pattern on the current filename.

                        ' Supress consecuent conditionals if pattern its an asterisk.
                        If dirpathPattern.Equals("*", StringComparison.OrdinalIgnoreCase) Then
                            flagPathPattern = True ' Activate the directory path flag.
                            Exit For

                        ElseIf ignoreCase Then ' Compare directory path with ignoring case rules.
                            If dir.FullName.ToLower Like dirpathPattern.ToLower Then
                                flagPathPattern = True ' Activate the directory path flag.
                                Exit For
                            End If

                        Else ' Compare directory path without ignoring case rules.
                            If dir.FullName Like dirpathPattern Then
                                flagPathPattern = True ' Activate the directory path flag.
                                Exit For
                            End If

                        End If ' ignoreCase

                    Next dirpathPattern

                Else ' directory path patterns collection is empty.
                    flagPathPattern = True ' Activate the directory path flag.

                End If ' dirpathPatterns IsNot Nothing

                ' If directory name patterns collection is not empty then...
                If dirNamePatterns IsNot Nothing Then

                    ' Iterate the directory name pattern(s).
                    For Each dirNamePattern As String In dirNamePatterns

                        ' Match the directory name pattern on the current filename.

                        ' Supress consecuent conditionals if pattern its an asterisk.
                        If dirNamePattern.Equals("*", StringComparison.OrdinalIgnoreCase) Then
                            flagNamePattern = True ' Activate the directory name flag.
                            Exit For

                        ElseIf ignoreCase Then ' Compare directory name with ignoring case rules.
                            If dir.Name.ToLower Like dirNamePattern.ToLower Then
                                flagNamePattern = True ' Activate the directory name flag.
                                Exit For
                            End If

                        Else ' Compare directory name without ignoring case rules.
                            If dir.Name Like dirNamePattern Then
                                flagNamePattern = True ' Activate the directory name flag.
                                Exit For
                            End If

                        End If ' ignoreCase

                    Next dirNamePattern

                Else ' directory name patterns collection is empty.
                    flagNamePattern = True ' Activate the filename flag.

                End If ' dirNamePatterns IsNot Nothing

                ' If directory path and also directory name patterns are matched then...
                If flagPathPattern AndAlso flagNamePattern Then
                    queue.Enqueue(dir) ' Enqueue this DirectoryInfo object.
                End If

            Next dir

        End If ' dirInfoCol IsNot Nothing

        ' If searchOption is recursive then...
        If searchOption = searchOption.AllDirectories Then

            Try
                ' Try next iterations.
                Task.WaitAll(dirInfo.GetDirectories.
                             Select(Function(dir As DirectoryInfo)
                                        Return Task.Factory.StartNew(Sub()
                                                                         CollectDirs(queue,
                                                                                     dir.FullName, searchOption.AllDirectories,
                                                                                     dirPathPatterns, dirNamePatterns,
                                                                                     ignoreCase, throwOnError)
                                                                     End Sub)
                                    End Function).ToArray)

            Catch ex As UnauthorizedAccessException
                If throwOnError Then
                    Throw
                End If

            Catch ex As DirectoryNotFoundException
                If throwOnError Then
                    Throw
                End If

            Catch ex As Exception
                If throwOnError Then
                    Throw
                End If

            End Try

        End If ' searchOption = searchOption.AllDirectories

    End Sub

    ''' <summary>
    ''' Collects the directory paths those matches the criteria inside the specified directory and/or sub-directories.
    ''' </summary>
    ''' <param name="queue">The <see cref="ConcurrentQueue(Of String)"/> instance to enqueue new directory paths.</param>
    ''' <param name="dirPath">The root directory path to search for directories.</param>
    ''' <param name="searchOption">The searching mode.</param>
    ''' <param name="dirPathPatterns">The directory path pattern(s) to match.</param>
    ''' <param name="dirNamePatterns">The directory name pattern(s) to match.</param>
    ''' <param name="ignoreCase">If <c>True</c>, ignores the comparing case of <paramref name="dirPathPatterns"/> and <paramref name="dirNamePatterns"/> patterns.</param>
    ''' <param name="throwOnError">Determines whether exceptions will be thrown, like access denied to directory.</param>
    Private Shared Sub CollectDirPaths(ByVal queue As ConcurrentQueue(Of String),
                                       ByVal dirPath As String,
                                       ByVal searchOption As SearchOption,
                                       ByVal dirPathPatterns As IEnumerable(Of String),
                                       ByVal dirNamePatterns As IEnumerable(Of String),
                                       ByVal ignoreCase As Boolean,
                                       ByVal throwOnError As Boolean)

        ' Initialize a directory paths collection.
        Dim dirPathCol As IEnumerable(Of String) = Nothing

        Try ' Get the top directories of the current directory.
            dirPathCol = Directory.GetDirectories(dirPath, "*", searchOption.TopDirectoryOnly)

        Catch ex As UnauthorizedAccessException
            If throwOnError Then
                Throw
            End If

        Catch ex As DirectoryNotFoundException
            If throwOnError Then
                Throw
            End If

        Catch ex As Exception
            If throwOnError Then
                Throw
            End If

        End Try

        ' If the fileInfoCol collection is not empty then...
        If dirPathCol IsNot Nothing Then

            ' Iterate the files.
            For Each dir As String In dirPathCol

                ' Flag to determine whether a directory path pattern is matched.
                Dim flagPathPattern As Boolean = False

                ' Flag to determine whether a directory name pattern is matched.
                Dim flagNamePattern As Boolean = False

                ' If directory path patterns collection is not empty then...
                If dirPathPatterns IsNot Nothing Then

                    ' Iterate the directory path pattern(s).
                    For Each dirpathPattern As String In dirPathPatterns

                        ' Match the directory path pattern on the current filename.

                        ' Supress consecuent conditionals if pattern its an asterisk.
                        If dirpathPattern.Equals("*", StringComparison.OrdinalIgnoreCase) Then
                            flagPathPattern = True ' Activate the directory path flag.
                            Exit For

                        ElseIf ignoreCase Then ' Compare directory path with ignoring case rules.
                            If dir.ToLower Like dirpathPattern.ToLower Then
                                flagPathPattern = True ' Activate the directory path flag.
                                Exit For
                            End If

                        Else ' Compare directory path without ignoring case rules.
                            If dir Like dirpathPattern Then
                                flagPathPattern = True ' Activate the directory path flag.
                                Exit For
                            End If

                        End If ' ignoreCase

                    Next dirpathPattern

                Else ' directory path patterns collection is empty.
                    flagPathPattern = True ' Activate the directory path flag.

                End If ' dirpathPatterns IsNot Nothing

                ' If directory name patterns collection is not empty then...
                If dirNamePatterns IsNot Nothing Then

                    ' Iterate the directory name pattern(s).
                    For Each dirNamePattern As String In dirNamePatterns

                        ' Match the directory name pattern on the current filename.

                        ' Supress consecuent conditionals if pattern its an asterisk.
                        If dirNamePattern.Equals("*", StringComparison.OrdinalIgnoreCase) Then
                            flagNamePattern = True ' Activate the directory name flag.
                            Exit For

                        ElseIf ignoreCase Then ' Compare directory name with ignoring case rules.
                            If Path.GetFileName(dir).ToLower Like dirNamePattern.ToLower Then
                                flagNamePattern = True ' Activate the directory name flag.
                                Exit For
                            End If

                        Else ' Compare directory name without ignoring case rules.
                            If Path.GetFileName(dir) Like dirNamePattern Then
                                flagNamePattern = True ' Activate the directory name flag.
                                Exit For
                            End If

                        End If ' ignoreCase

                    Next dirNamePattern

                Else ' directory name patterns collection is empty.
                    flagNamePattern = True ' Activate the filename flag.

                End If ' dirNamePatterns IsNot Nothing

                ' If directory path and also directory name patterns are matched then...
                If flagPathPattern AndAlso flagNamePattern Then
                    queue.Enqueue(dir) ' Enqueue this directory path.
                End If

            Next dir

        End If ' dirPathCol IsNot Nothing

        ' If searchOption is recursive then...
        If searchOption = searchOption.AllDirectories Then

            Try
                ' Try next iterations.
                Task.WaitAll(New DirectoryInfo(dirPath).GetDirectories.
                                 Select(Function(dir As DirectoryInfo)
                                            Return Task.Factory.StartNew(Sub()
                                                                             CollectDirPaths(queue,
                                                                                         dir.FullName, searchOption.AllDirectories,
                                                                                         dirPathPatterns, dirNamePatterns,
                                                                                         ignoreCase, throwOnError)
                                                                         End Sub)
                                        End Function).ToArray)

            Catch ex As UnauthorizedAccessException
                If throwOnError Then
                    Throw
                End If

            Catch ex As DirectoryNotFoundException
                If throwOnError Then
                    Throw
                End If

            Catch ex As Exception
                If throwOnError Then
                    Throw
                End If

            End Try

        End If ' searchOption = searchOption.AllDirectories

    End Sub

#End Region

End Class

#End Region