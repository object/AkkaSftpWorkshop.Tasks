module SshNetClient

    open System
    open System.Collections.Generic
    open System.IO
    open Renci.SshNet
    open Renci.SshNet.Sftp

    open SftpClient

    type SftpConnectionDetails = {
        Host : string
        Port : int
        UserName : string
        KeyFile : string
    }

    type SshNetSftpAsyncResult(ar : IAsyncResult) =

        interface IAsyncResult with
            member this.AsyncState = ar.AsyncState
            member this.AsyncWaitHandle = ar.AsyncWaitHandle
            member this.CompletedSynchronously = ar.CompletedSynchronously
            member this.IsCompleted = ar.IsCompleted

        interface ISftpAsyncResult with
            member this.AsyncResult = ar
            member this.IsCanceled 
                with get() = 
                    match ar with
                    | :? Renci.SshNet.Sftp.SftpDownloadAsyncResult as ar -> ar.IsDownloadCanceled
                    | :? Renci.SshNet.Sftp.SftpUploadAsyncResult as ar -> ar.IsUploadCanceled
                and set(value) = 
                    cprintfn ConsoleColor.Cyan "SSH.NET: Requesting cancel..."
                    match ar with
                    | :? Renci.SshNet.Sftp.SftpDownloadAsyncResult as ar -> ar.IsDownloadCanceled <- value
                    | :? Renci.SshNet.Sftp.SftpUploadAsyncResult as ar -> ar.IsUploadCanceled <- value
                    cprintfn ConsoleColor.Green "SSH.NET: Cancel requested."

    type SshNetSftpClient(connectionDetails : SftpConnectionDetails) =

        static let locker = obj()
        static let keyFiles = Dictionary<string, PrivateKeyFile>()

        let keyFile = lock locker (fun _ -> 
            let path = connectionDetails.KeyFile
            match keyFiles.ContainsKey(path) with
            | true -> keyFiles.[path]
            | false -> 
                let pk = new PrivateKeyFile(path); 
                keyFiles.Add(path, pk); 
                pk)
        let connection = new SftpClient(connectionDetails.Host, connectionDetails.Port, connectionDetails.UserName, keyFile)

        interface ISftpClient with

            member this.Connect() = 
                cprintfn ConsoleColor.Cyan "SSH.NET: Connecting..."
                connection.Connect()
                cprintfn ConsoleColor.Green "SSH.NET: Connected."

            member this.Disconnect() = 
                cprintfn ConsoleColor.Cyan "SSH.NET: Disconnecting..."
                connection.Disconnect()
                cprintfn ConsoleColor.Green "SSH.NET: Disconnected."

            member this.DownloadFile(path, stream, progress) =
                cprintfn ConsoleColor.Cyan "SSH.NET: Downloading file %s..." path
                connection.DownloadFile(path, stream, progress)
                cprintfn ConsoleColor.Green "SSH.NET: File %s is downloaded." path

            member this.BeginDownloadFile(path, stream, ac, state, progress) =
                cprintfn ConsoleColor.Cyan "SSH.NET: Beginning download of file %s..." path
                let result = connection.BeginDownloadFile(path, stream, ac, state, progress)
                cprintfn ConsoleColor.Green "SSH.NET: File %s is being downloaded." path
                result

            member this.EndDownloadFile(ar) =
                cprintfn ConsoleColor.Cyan "SSH.NET: Ending download of file..."
                connection.EndDownloadFile(ar)
                cprintfn ConsoleColor.Green "SSH.NET: Ended file download."

            member this.UploadFile(stream, path, progress) =
                cprintfn ConsoleColor.Cyan "SSH.NET: Uploading file %s..." path
                connection.UploadFile(stream, path, progress)
                cprintfn ConsoleColor.Green "SSH.NET: File %s is uploaded." path

            member this.BeginUploadFile(stream, path, ac, state, progress) =
                cprintfn ConsoleColor.Cyan "SSH.NET: Begining upload of file %s..." path
                let result = connection.BeginUploadFile(stream, path, ac, state, progress)
                cprintfn ConsoleColor.Green "SSH.NET: File %s is being uploaded." path
                result

            member this.EndUploadFile(ar) =
                cprintfn ConsoleColor.Cyan "SSH.NET: Ending upload of file..."
                connection.EndUploadFile(ar)
                cprintfn ConsoleColor.Green "SSH.NET: Ended file upload."

            member this.RenameFile(sourcePath, destinationPath) =
                cprintfn ConsoleColor.Cyan "SSH.NET: Renaming file %s to %s..." sourcePath destinationPath
                connection.RenameFile(sourcePath, destinationPath)
                cprintfn ConsoleColor.Green "SSH.NET: Renamed file %s to %s..." sourcePath destinationPath

            member this.DeleteFile(path) =
                cprintfn ConsoleColor.Cyan "SSH.NET: Deleting file %s..." path
                connection.DeleteFile(path)
                cprintfn ConsoleColor.Green "SSH.NET: Deleted file %s." path

            member this.CreateDirectory(path) =
                cprintfn ConsoleColor.Cyan "SSH.NET: Creating directory %s..." path
                connection.CreateDirectory(path)
                cprintfn ConsoleColor.Green "SSH.NET: Created directory %s." path

            member this.DeleteDirectory(path) =
                cprintfn ConsoleColor.Cyan "SSH.NET: Deleting directory %s..." path
                connection.DeleteDirectory(path)
                cprintfn ConsoleColor.Green "SSH.NET: Deleted directory %s." path

            member this.ListDirectory(path, progress) =
                cprintfn ConsoleColor.Cyan "SSH.NET: Listing directory %s..." path
                let result = connection.ListDirectory(path, progress) 
                            |> Seq.map (fun x -> 
                            {
                                Name = x.Name
                                Length = x.Length
                                IsDirectory = x.IsDirectory
                            })
                cprintfn ConsoleColor.Green "SSH.NET: Directory %s is listed." path
                result

            member this.DirectoryExists(path) =
                cprintfn ConsoleColor.Cyan "SSH.NET: Checking if directory %s exists..." path
                let result = 
                    try
                        connection.ListDirectory (path, noProgressCallback) |> ignore
                        true
                    with 
                    | ex -> false
                cprintfn ConsoleColor.Green "SSH.NET: Directory %s %sexists." path (match result with | true -> "" | false -> "doesn't")
                result

            member this.Dispose() = 
                connection.Dispose()

        interface System.IDisposable with 
            member this.Dispose() = connection.Dispose()

    type SshNetSshCommand(command : SshCommand) =

        interface ISshCommand with
            member this.Execute() = command.Execute()
            member this.BeginExecute(ac) = command.BeginExecute()
            member this.EndExecute(ar) = command.EndExecute(ar)
            member this.CancelAsync() = command.CancelAsync()

    type SshNetSshClient(connectionDetails : SftpConnectionDetails) =

        let keyFile = new PrivateKeyFile(connectionDetails.KeyFile)
        let connection = new SshClient(connectionDetails.Host, connectionDetails.Port, connectionDetails.UserName, keyFile)

        interface ISshClient with
            member this.Connect() = connection.Connect()
            member this.Disconnect() = connection.Disconnect()
            member this.CreateCommand(cmd) = SshNetSshCommand(connection.CreateCommand(cmd)) :> ISshCommand
            member this.Dispose() = connection.Dispose()

        interface System.IDisposable with 
            member this.Dispose() = connection.Dispose()

    type SshNetClientFactory(connectionDetails : SftpConnectionDetails) =
        interface IClientFactory with
            member this.CreateSftpClient() = new SshNetSftpClient(connectionDetails) :> ISftpClient
            member this.CreateSshClient() = new SshNetSshClient(connectionDetails) :> ISshClient
            member this.CreateFileStreamProvider() = new FileStreamProvider() :> IFileStreamProvider
            member this.CreateSftpAsyncResult (ar : IAsyncResult) = SshNetSftpAsyncResult(ar) :> ISftpAsyncResult
