[<AutoOpen>]
module SftpActors
    open System
    open Akka
    open Akka.FSharp
    open SftpClient
    open Utils

    type SftpCommand =
        | ListDirectory of Url
        | UploadFile of UncPath * Url
        | DownloadFile of UncPath * Url

    // Tip: Use IClientFactory.CreateFileStreamProvider to obtain a local file stream for reading and writing.
    // ISftpClient.UploadFile and ISftpClient.DownloadFile are your friends to implement upload and download commands.

    // Tip: Use ensureParentDirectoryExists to create remote directories prior to uploading a file.

    // Tip: Use noProgressCallback helper function when calling API methods that require callback function.

    let sftpActor (clientFactory : IClientFactory) (mailbox: Actor<_>) =
        let rec loop () =
            actor {
                let! msg = mailbox.Receive()
                return! loop ()
            }
        loop()
