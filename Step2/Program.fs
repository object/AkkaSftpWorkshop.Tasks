module Application

    open System
    open Akka
    open Akka.FSharp
    open ClientFactory

    let printInstuctions () =
        printfn "In the step 2 you teach your actor not only execute commands but also send information back to the caller."
        printfn "Write implementation for ListDirectory command so the actor will send back a list of files in the given directory."
        printfn "Once the actor is property implemented the program should display the following messages:"
        printfn ""
        cprintfn ConsoleColor.Cyan "SSH.NET: Connecting..."
        cprintfn ConsoleColor.Green "SSH.NET: Connected."
        cprintfn ConsoleColor.Cyan "SSH.NET: Listing directory <directory name>..."
        cprintfn ConsoleColor.Green "SSH.NET: Directory <directory name> is listed."
        printfn "    Directory listing results"
        cprintfn ConsoleColor.Cyan "SSH.NET: Disconnecting..."
        cprintfn ConsoleColor.Green "SSH.NET: Disconnected."
        printfn ""

    let run () =
        let clientFactory = createClientFactory()
        let system = System.create "system" <| Configuration.load ()
        let sftp = spawn system "sftp" <| sftpActor clientFactory

        sftp <! Connect
        let remoteUrl = Url "/"
        let result : SftpFileInfo list option = (sftp <? ListDirectory remoteUrl |> Async.RunSynchronously)
        printfn ""
        match result with
        | Some x -> x |> Seq.iter (fun y -> 
            printfn "%s: %s" (y.IsDirectory |> function | true -> "Directory" | false -> "File") y.Name)
        | None -> printfn "The remote directory is empty"
        sftp <! Disconnect

    [<EntryPoint>]
    let main argv = 

        printInstuctions ()
        waitForInput "Press any key to start the actor system and validate the implementation."

        run ()

        waitForInput ""
        0

