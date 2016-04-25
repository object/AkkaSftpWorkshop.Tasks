module Application

    open System
    open Akka
    open Akka.FSharp
    open ClientFactory

    let printInstuctions () =
        printfn "The step 1 of the workshop is simple: just teach your SFTP actor to connect to the FTP server."
        printfn "Once the actor is property implemented the program should display the following messages:"
        printfn ""
        cprintfn ConsoleColor.Cyan "SSH.NET: Connecting..."
        cprintfn ConsoleColor.Green "SSH.NET: Connected."
        cprintfn ConsoleColor.Cyan "SSH.NET: Disconnecting..."
        cprintfn ConsoleColor.Green "SSH.NET: Disconnected."
        printfn ""

    let run () =
        let clientFactory = createClientFactory()
        let system = System.create "system" <| Configuration.load ()
        let sftp = spawn system "sftp" <| sftpActor clientFactory

        sftp <! Connect
        sftp <! Disconnect

    [<EntryPoint>]
    let main argv = 

        printInstuctions ()
        waitForInput "Press any key to start the actor system and validate the implementation."

        run ()

        waitForInput ""
        0

