module Application

    open System
    open Akka
    open Akka.FSharp
    open ClientFactory

    let printInstuctions () =
        printfn "The final step 7 will focus on improving actor routing. The smallest mailbox strategy no longer fits our implementation,"
        printfn "so you will have to add support for consistent hashing strategy by filling in missing sftpGetHash implementation."
        printfn ""

    let run (roundNumber : int, fileCount : int, poolSize : int, transferDelay) =
        let clientFactory = createClientFactoryWithTransferDelay(transferDelay)
        let system = System.create <| sprintf "system%d"  roundNumber <| Configuration.load ()
        let sftp = spawnOpt system "sftp" 
                <| sftpActor clientFactory 
                <| [SpawnOption.Router(Routing.ConsistentHashingPool(10).WithHashMapping(Routing.ConsistentHashMapping sftpGetHash))]


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

