[<AutoOpen>]
module SftpClient

    open System
    open System.Collections.Generic
    open System.IO

    type SftpFileInfo = {
        Name : string
        Length : int64
        IsDirectory : bool
    }

    type ISftpAsyncResult =
        inherit IAsyncResult
        abstract member AsyncResult : IAsyncResult
        abstract member IsCanceled : bool with get, set

    type ISftpClient =
        abstract member Connect : unit -> unit
        abstract member Disconnect : unit -> unit
        abstract member DownloadFile : string * Stream * Action<uint64> -> unit
        abstract member BeginDownloadFile : string * Stream * AsyncCallback * obj * Action<uint64> -> IAsyncResult
        abstract member EndDownloadFile : IAsyncResult -> unit
        abstract member UploadFile : Stream * string * Action<uint64> -> unit
        abstract member BeginUploadFile : Stream * string * AsyncCallback * obj * Action<uint64> -> IAsyncResult
        abstract member EndUploadFile : IAsyncResult -> unit
        abstract member RenameFile : string * string -> unit
        abstract member DeleteFile : string -> unit
        abstract member CreateDirectory : string -> unit
        abstract member DeleteDirectory : string -> unit
        abstract member ListDirectory : string * Action<int> -> IEnumerable<SftpFileInfo>
        abstract member DirectoryExists : string -> bool
        abstract member Dispose : unit -> unit

    type ISshCommand =
        abstract member Execute : unit -> string
        abstract member BeginExecute : AsyncCallback -> IAsyncResult
        abstract member EndExecute : IAsyncResult -> string
        abstract member CancelAsync : unit -> unit

    type ISshClient =
        abstract member Connect : unit -> unit
        abstract member Disconnect : unit -> unit
        abstract member CreateCommand : string -> ISshCommand
        abstract member Dispose : unit -> unit

    type IFileStreamProvider =
        abstract member OpenRead : string -> Stream
        abstract member OpenWrite : string -> Stream

    type IClientFactory =
        abstract member CreateSftpClient : unit -> ISftpClient
        abstract member CreateSshClient : unit -> ISshClient
        abstract member CreateFileStreamProvider : unit -> IFileStreamProvider
        abstract member CreateSftpAsyncResult : IAsyncResult -> ISftpAsyncResult

    type FileStreamProvider() =
        interface IFileStreamProvider with
            member this.OpenRead(path : string) = File.OpenRead(path) :> Stream
            member this.OpenWrite(path : string) = File.OpenWrite(path) :> Stream
