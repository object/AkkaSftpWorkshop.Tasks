[<AutoOpen>]
module Utils

open System

let cprintf c fmt = 
    Printf.kprintf
        (fun s ->
            let old = System.Console.ForegroundColor
            try
              System.Console.ForegroundColor <- c;
              System.Console.Write s
            finally
              System.Console.ForegroundColor <- old)
        fmt

let cprintfn c fmt =
    let result = cprintf c fmt
    printfn ""
    result

let waitForInput message =
    printfn message
    Console.ReadKey() |> ignore

let noProgressCallback<'a> = Action<'a>(fun x -> ())

let rec createDirectoryTree (connection : ISftpClient) parentPath (dirs : string list) =
    match dirs with
    | [] -> ()
    | x :: xs ->
        let dir = sprintf "%s/%s" parentPath x
        connection.CreateDirectory(dir)
        createDirectoryTree connection dir xs
        
let ensureParentDirectoryExists (connection : ISftpClient) (remotePath : string) =
    match remotePath.LastIndexOf('/') with
    | pos when pos > 0 ->
        let dir = remotePath.Substring(0, pos)
        if not (connection.DirectoryExists(dir)) then
            createDirectoryTree connection "" (dir.Split([|'/'|], StringSplitOptions.RemoveEmptyEntries) |> List.ofArray)
    | _ -> ()
