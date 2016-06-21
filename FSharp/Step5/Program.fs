module Application

    open System
    open Akka
    open Akka.FSharp
    open ClientFactory

    let printInstuctions () =
        printfn "Now that we have file transfer commands in place, it's time to check how they perform."
        printfn "In the step 5 you're not going to write any new code, just sit back and watch the program running."
        printfn "The only thing that's changed is that instead of a single SFTP actor there will be a pool of 10 actors transferring files."
        printfn ""

    let run (roundNumber : int, fileCount : int, poolSize : int, transferDelay) =
        let clientFactory = createClientFactoryWithTransferDelay(transferDelay)
        let system = System.create <| sprintf "system%d"  roundNumber <| Configuration.load ()
        let sftp = spawnOpt system "sftp" 
                    <| sftpActor clientFactory 
                    <| [SpawnOption.Router(Routing.SmallestMailboxPool(poolSize))]

        for fileNumber in 1..10 do
            let localPath = UncPath uploadFileName
            let remotePath = Url <| sprintf "/152818/no/open/test/12345-%d-%d.txt" roundNumber fileNumber
            sftp <! UploadFile (localPath, remotePath)

    [<EntryPoint>]
    let main argv = 

        printInstuctions ()

        waitForInput "Press any key to start the system with a single actor and no transfer delay. 1 file will be transferred. Wait until the actor disconnects."
        run (1, 1, 1, 0<s/MB>)

        waitForInput "Press any key to start the system with a single actor and 2 s/MB transfer delay. 10 file will be transferred. Wait until the actor disconnects."
        run (2, 10, 1, 2<s/MB>)

        waitForInput "Press any key to start the system with a pool of 10 actors and 2 s/MB transfer delay. 10 file will be transferred. Wait until the actor disconnects."
        run (3, 10, 10, 2<s/MB>)

        waitForInput ""
        0

