[<AutoOpen>]
module SftpActors
    open System
    open Akka
    open Akka.FSharp
    open SftpClient

    type SftpCommand =
        | ListDirectory of Url

    // Tip: connect on demand, then use actor's context SetReceiveTimeout to trigger timeout when the actor is idle,
    // handle Actor.ReceiveTimeout message to release SFTP connection.

    let sftpActor (clientFactory : IClientFactory) (mailbox: Actor<_>) =
        let rec loop () =
            actor {
                let! msg = mailbox.Receive()
                return! loop ()
            }
        loop()
