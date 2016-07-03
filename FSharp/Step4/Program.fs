module Application

    open System
    open System.IO
    open Akka
    open Akka.FSharp
    open ClientFactory

    let printInstuctions () =
        printfn "We are ready for real things! In step 4 you will implement UploadFile and DownloadFile commands."
        printfn "Once the actor is property implemented the program should display the following messages:"
        printfn ""
        cprintfn ConsoleColor.Cyan "SSH.NET: Connecting..."
        cprintfn ConsoleColor.Green "SSH.NET: Connected."
        cprintfn ConsoleColor.Cyan "SSH.NET: Checking if directory <directory name> exists..."
        cprintfn ConsoleColor.Green "SSH.NET: Directory <directory name> exists."
        cprintfn ConsoleColor.Cyan "SSH.NET: Uploading file <file name>..."
        cprintfn ConsoleColor.Green "SSH.NET: File <file name> is uploaded."
        cprintfn ConsoleColor.Cyan "SSH.NET: Downloading file <file name>..."
        cprintfn ConsoleColor.Green "SSH.NET: File <file name> is downloaded."
        printfn "    pause for about 10 seconds"
        cprintfn ConsoleColor.Cyan "SSH.NET: Disconnecting..."
        cprintfn ConsoleColor.Green "SSH.NET: Disconnected."
        printfn ""

    let run () =
        let clientFactory = createClientFactory()
        let system = System.create "system" <| Configuration.load ()
        let sftp = spawn system "sftp" <| sftpActor clientFactory

        let baseDir = AppDomain.CurrentDomain.BaseDirectory
        let localPath = UncPath <| Path.Combine(baseDir, @"Wire.dll")
        let remotePath = Url "/test/12345.dll"
        sftp <! UploadFile (localPath, remotePath)
        let localPath = UncPath <| Path.Combine(baseDir, @"Wire.bak")
        sftp <! DownloadFile (localPath, remotePath)
        printfn ""

    [<EntryPoint>]
    let main argv = 

        printInstuctions ()
        waitForInput "Press any key to start the actor system and validate the implementation."

        run ()

        waitForInput ""
        0

