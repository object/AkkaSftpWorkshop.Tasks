[<AutoOpen>]
module SftpActors
    open System
    open Akka.FSharp
    open SftpClient

    type SftpCommand =
        | Connect
        | Disconnect
        | ListDirectory of Url

    // Tip: after invoking SFTP connection ListDirectory method send the result to the sender.

    // Tip: Use noProgressCallback helper function when calling API methods that require callback function.

    let sftpActor (clientFactory : IClientFactory) (mailbox: Actor<_>) =
        let rec loop () =
            actor {
                let! msg = mailbox.Receive()
                return! loop ()
            }
        loop()
