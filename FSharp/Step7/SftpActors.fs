[<AutoOpen>]
module SftpActors
    open System
    open System.IO
    open Akka
    open Akka.FSharp
    open SftpClient
    open Utils

    type SftpCommand =
        | ListDirectory of Url
        | UploadFile of UncPath * Url
        | DownloadFile of UncPath * Url
        | Cancel of string

    type SftpCommandResult =
        | Completed
        | Cancelled
        | Error of string

    // Tip: sftpGetHash takes an SftpCommand as the input and returns hash value that should be the same for same remote files.

    let sftpGetHash (o : obj) =
        null

    let sftpActor (clientFactory : IClientFactory) (mailbox: Actor<_>) =
        let rec loop () =
            actor {
                let! msg = mailbox.Receive()
                return! loop ()
            }
        loop()
