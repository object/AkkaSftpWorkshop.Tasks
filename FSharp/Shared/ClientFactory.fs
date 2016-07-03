module ClientFactory

    open System.IO
    open SftpClient
    open SshNetClient
    open LocalFileClient

    let private ftpRoot = "sftp"

    let createClientFactory () =
        if not (Directory.Exists(ftpRoot)) then Directory.CreateDirectory(ftpRoot) |> ignore
        LocalFileClientFactory(ftpRoot, "", 0<s/MB>)

    let createClientFactoryWithTransferDelay (transferDelay) =
        LocalFileClientFactory(ftpRoot, "", transferDelay)
