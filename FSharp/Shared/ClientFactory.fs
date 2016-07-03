module ClientFactory

    open System
    open System.IO
    open SftpClient
    open SshNetClient
    open LocalFileClient

    let private ftpRoot = "sftp"

    let createClientFactory () =
        let rootDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ftpRoot)
        if not (Directory.Exists(rootDir)) then Directory.CreateDirectory(rootDir) |> ignore
        LocalFileClientFactory(rootDir, "", 0<s/MB>)

    let createClientFactoryWithTransferDelay (transferDelay) =
        let rootDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ftpRoot)
        if not (Directory.Exists(rootDir)) then Directory.CreateDirectory(rootDir) |> ignore
        LocalFileClientFactory(rootDir, "", transferDelay)
