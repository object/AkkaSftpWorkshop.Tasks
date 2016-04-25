module ClientFactory

    open SftpClient
    open SshNetClient
    open LocalFileClient

    let createClientFactory () =
        LocalFileClientFactory("C:\\temp\\sftp", "", 0<s/MB>)

    let createClientFactoryWithTransferDelay (transferDelay) =
        LocalFileClientFactory("C:\\temp\\sftp", "", transferDelay)
