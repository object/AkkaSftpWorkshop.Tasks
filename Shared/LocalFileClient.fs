module LocalFileClient

    open System
    open System.Collections.Generic
    open System.IO

    open SftpClient

    type LocalAsyncResult() =
        interface IAsyncResult with
            member this.AsyncState = null
            member this.AsyncWaitHandle = null
            member this.CompletedSynchronously = true
            member this.IsCompleted = true

    type LocalSftpAsyncResult(ar : IAsyncResult) =
        interface IAsyncResult with
            member this.AsyncState = ar.AsyncState
            member this.AsyncWaitHandle = ar.AsyncWaitHandle
            member this.CompletedSynchronously = ar.CompletedSynchronously
            member this.IsCompleted = ar.IsCompleted
        interface ISftpAsyncResult with
            member this.AsyncResult = this :> IAsyncResult
            member this.IsCanceled 
                with get() = false 
                and set(value) =
                    cprintfn ConsoleColor.Cyan "LocalFile: Requesting cancel..."
                    cprintfn ConsoleColor.Green "LocalFile: Cancel requested."

    let private getAsyncResult(ac : AsyncCallback) =
        let result = LocalAsyncResult() :> IAsyncResult
        let asyncCallback = async {
            System.Threading.Thread.Sleep(100)
            ac.Invoke(result)
        }
        Async.Start(asyncCallback)
        result

    type LocalFileClient(localRoot : string, remoteRoot : string, transferDelay) =

        let getLocalPath (remotePath : string) =
            if not (remotePath.ToLower().StartsWith(remoteRoot.ToLower())) then
                raise <| ArgumentException(sprintf "Remote path must start with configured remote root %s" remoteRoot, "remoteRoot")
            Path.Combine(localRoot, remotePath.Substring(remoteRoot.Length).Replace("/", "\\").TrimStart('\\'))

        let delayTransfer (stream : Stream) =
            if transferDelay > 0<s/MB> then
                let sizeInMegabytes = (int)(stream.Length / (1024L*1024L)) * 1<MB>
                let delay = ((sizeInMegabytes + 1<MB>) * transferDelay) / 1<s>
                Async.Sleep (delay * 1000) |> Async.RunSynchronously

        interface ISftpClient with
            member this.Connect() =
                cprintfn ConsoleColor.Cyan "LocalFile: Connecting..."
                cprintfn ConsoleColor.Green "LocalFile: Connected."

            member this.Disconnect() =
                cprintfn ConsoleColor.Cyan "LocalFile: Disconnecting..."
                cprintfn ConsoleColor.Green "LocalFile: Disconnected."

            member this.DownloadFile(path, stream, progress) =
                cprintfn ConsoleColor.Cyan "LocalFile: Downloading file %s..." path
                delayTransfer stream
                use readStream = new FileStream(getLocalPath path, FileMode.Open)
                readStream.CopyTo(stream)
                cprintfn ConsoleColor.Green "LocalFile: File %s is downloaded." path

            member this.BeginDownloadFile(path, stream, ac, state, progress) = 
                cprintfn ConsoleColor.Cyan "LocalFile: Beginning download of file %s..." path
                delayTransfer stream
                use readStream = new FileStream(getLocalPath path, FileMode.Open)
                let result = getAsyncResult(ac)
                cprintfn ConsoleColor.Green "LocalFile: File %s is being downloaded." path
                result

            member this.EndDownloadFile(ar) =
                cprintfn ConsoleColor.Cyan "LocalFile: Ending download of file..."
                cprintfn ConsoleColor.Green "LocalFile: Ended file download."

            member this.UploadFile(stream, path, progress) =
                cprintfn ConsoleColor.Cyan "LocalFile: Uploading file %s..." path
                delayTransfer stream
                use writeStream = new FileStream(getLocalPath path, FileMode.Create)
                stream.CopyTo(writeStream)
                cprintfn ConsoleColor.Green "LocalFile: File %s is uploaded." path

            member this.BeginUploadFile(stream, path, ac, state, progress) = 
                cprintfn ConsoleColor.Cyan "LocalFile: Begining upload of file %s..." path
                delayTransfer stream
                let result = getAsyncResult(ac)
                cprintfn ConsoleColor.Green "LocalFile: File %s is being uploaded." path
                result

            member this.EndUploadFile(ar) = 
                cprintfn ConsoleColor.Cyan "LocalFile: Ending upload of file..."
                cprintfn ConsoleColor.Green "LocalFile: Ended file upload."

            member this.RenameFile(sourcePath, destinationPath) = 
                cprintfn ConsoleColor.Cyan "LocalFile: Renaming file %s to %s..." sourcePath destinationPath
                File.Move(getLocalPath sourcePath, getLocalPath destinationPath)
                cprintfn ConsoleColor.Green "LocalFile: Renamed file %s to %s..." sourcePath destinationPath

            member this.DeleteFile(path) =
                cprintfn ConsoleColor.Cyan "LocalFile: Deleting file %s..." path
                File.Delete(getLocalPath path)
                cprintfn ConsoleColor.Green "LocalFile: Deleted file %s." path

            member this.CreateDirectory(path) = 
                cprintfn ConsoleColor.Cyan "LocalFile: Creating directory %s..." path
                Directory.CreateDirectory(getLocalPath path) |> ignore
                cprintfn ConsoleColor.Green "LocalFile: Created directory %s." path

            member this.DeleteDirectory(path) = 
                cprintfn ConsoleColor.Cyan "LocalFile: Deleting directory %s..." path
                Directory.Delete(getLocalPath path)
                cprintfn ConsoleColor.Green "LocalFile: Deleted directory %s." path

            member this.ListDirectory(path, progress) = 
                cprintfn ConsoleColor.Cyan "LocalFile: Listing directory %s..." path
                let dirs = Directory.EnumerateDirectories(getLocalPath path)
                            |> Seq.map (fun x -> 
                            {
                                Name = Path.GetFileName(x)
                                Length = 0L
                                IsDirectory = true
                            })
                let files = Directory.EnumerateFiles(getLocalPath path)
                            |> Seq.map (fun x -> 
                            {
                                Name = Path.GetFileName(x)
                                Length = FileInfo(x).Length
                                IsDirectory = false
                            })
                cprintfn ConsoleColor.Green "LocalFile: Directory %s is listed." path
                Seq.append dirs files

            member this.DirectoryExists(path) =
                cprintfn ConsoleColor.Cyan "LocalFile: Checking if directory %s exists..." path
                let result = Directory.Exists(getLocalPath path)
                cprintfn ConsoleColor.Green "LocalFile: Directory %s %sexists." path (match result with | true -> "" | false -> "doesn't")
                result

            member this.Dispose() = ()

        interface System.IDisposable with 
            member this.Dispose() = ()

    type LocalSshCommand() =
        interface ISshCommand with
            member this.Execute() = null
            member this.BeginExecute(ac) = getAsyncResult(ac)
            member this.EndExecute(ar) = String.Empty
            member this.CancelAsync() = ()

    type LocalSshClient() =
        interface ISshClient with
            member this.Connect() = ()
            member this.Disconnect() = ()
            member this.CreateCommand(cmd) = LocalSshCommand() :> ISshCommand
            member this.Dispose() = ()
        interface System.IDisposable with 
            member this.Dispose() = ()

    type LocalFileStreamProvider() =
        interface IFileStreamProvider with
            member this.OpenRead(path : string) = new FileStream(path, FileMode.Open, FileAccess.Read) :> Stream
            member this.OpenWrite(path : string) = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write) :> Stream

    type LocalFileClientFactory(localRoot : string, remoteRoot : string, transferDelay) =
        interface IClientFactory with
            member this.CreateSftpClient() = new LocalFileClient(localRoot, remoteRoot, transferDelay) :> ISftpClient
            member this.CreateSshClient() = new LocalSshClient() :> ISshClient
            member this.CreateFileStreamProvider() = new LocalFileStreamProvider() :> IFileStreamProvider
            member this.CreateSftpAsyncResult(ar : IAsyncResult) = LocalSftpAsyncResult(ar) :> ISftpAsyncResult

