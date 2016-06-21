[<AutoOpen>]
module SftpActors
    open System
    open Akka.FSharp
    open SftpClient

    type SftpCommand =
        | Connect
        | Disconnect

    // Tip: you will be implementing a stateful actor (switching between disconnected and connected states)
    // using means of stateless functional programming. The state in FP is usually passed in function arguments.
    // How should you change "loop" placeholder to represent actor's state?

    let sftpActor (clientFactory : IClientFactory) (mailbox: Actor<_>) =
        let rec loop () =
            actor {
                let! msg = mailbox.Receive()
                return! loop ()
            }
        loop()
