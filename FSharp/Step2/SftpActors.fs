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

    let sftpActor (clientFactory : IClientFactory) (mailbox: Actor<_>) =
        let rec loop () =
            actor {
                let! msg = mailbox.Receive()
                return! loop ()
            }
        loop()
