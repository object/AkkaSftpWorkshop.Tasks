using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Shared
{
    public class LocalAsyncResult : IAsyncResult
    {
        public object AsyncState { get { return null; } }
        public WaitHandle AsyncWaitHandle { get { return null; } }
        public bool CompletedSynchronously { get { return true; } }
        public bool IsCompleted { get { return true; } }

        internal static IAsyncResult Create(AsyncCallback ac)
        {
            var result = new LocalAsyncResult();
            Task.Delay(100).Wait();
            ac.Invoke(result);
            return result;
        }
    }

    public class LocalSftpAsyncResult : ISftpAsyncResult
    {
        private readonly IAsyncResult _asyncResult;
        private bool _canceled;

        public LocalSftpAsyncResult(IAsyncResult ar)
        {
            _asyncResult = ar;
        }

        public object AsyncState { get { return _asyncResult.AsyncState; } }
        public WaitHandle AsyncWaitHandle { get { return _asyncResult.AsyncWaitHandle; } }
        public bool CompletedSynchronously { get { return _asyncResult.CompletedSynchronously; } }
        public bool IsCompleted { get { return _asyncResult.IsCompleted; } }
        public IAsyncResult AsyncResult { get { return _asyncResult; } }
        public bool IsCanceled
        {
            get
            {
                return _canceled;
            }

            set
            {
                ColoredConsole.WriteLine(ConsoleColor.Cyan, "LocalFile: Requesting cancel...");
                _canceled = value;
                ColoredConsole.WriteLine(ConsoleColor.Green, "LocalFile: Cancel requested.");
            }
        }
    }

    public class LocalFileClient : ISftpClient, IDisposable
    {
        readonly string _localRoot;
        readonly string _remoteRoot;
        readonly int _transferDelay;

        public LocalFileClient(string localRoot, string remoteRoot, int transferDelay)
        {
            _localRoot = localRoot;
            _remoteRoot = remoteRoot;
            _transferDelay = transferDelay;
        }

        public void Connect()
        {
            ColoredConsole.WriteLine(ConsoleColor.Cyan, "LocalFile: Connecting...");
            ColoredConsole.WriteLine(ConsoleColor.Green, "LocalFile: Connected.");
        }

        public void Disconnect()
        {
            ColoredConsole.WriteLine(ConsoleColor.Cyan, "LocalFile: Disconnecting...");
            ColoredConsole.WriteLine(ConsoleColor.Green, "LocalFile: Disconnected.");
        }

        public void DownloadFile(string path, Stream stream, Action<ulong> progress)
        {
            ColoredConsole.WriteLine(ConsoleColor.Cyan, "LocalFile: Downloading file {0}...", path);
            DelayTransfer(stream);
            using (var readStream = new FileStream(GetLocalPath(path), FileMode.Open))
            {
                readStream.CopyTo(stream);
            }
            ColoredConsole.WriteLine(ConsoleColor.Green, "LocalFile: File {0} is downloaded.", path);
        }

        public IAsyncResult BeginDownloadFile(string path, Stream stream, AsyncCallback callback, Action<ulong> progress)
        {
            ColoredConsole.WriteLine(ConsoleColor.Cyan, "LocalFile: Beginning download of file {0}...", path);
            DelayTransfer(stream);
            var result = LocalAsyncResult.Create(callback);
            ColoredConsole.WriteLine(ConsoleColor.Green, "LocalFile: File {0} is being downloaded.", path);
            return result;
        }

        public void EndDownloadFile(IAsyncResult ar)
        {
            ColoredConsole.WriteLine(ConsoleColor.Cyan, "LocalFile: Ending download of file...");
            ColoredConsole.WriteLine(ConsoleColor.Green, "LocalFile: Ended file download.");
        }

        public void UploadFile(Stream stream, string path, Action<ulong> progress)
        {
            ColoredConsole.WriteLine(ConsoleColor.Cyan, "LocalFile: Uploading file {0}...", path);
            DelayTransfer(stream);
            using (var writeStream = new FileStream(GetLocalPath(path), FileMode.Create))
            {
                stream.CopyTo(writeStream);
            }
            ColoredConsole.WriteLine(ConsoleColor.Green, "LocalFile: File {0} is uploaded.", path);
        }

        public IAsyncResult BeginUploadFile(Stream stream, string path, AsyncCallback callback, Action<ulong> progress)
        {
            ColoredConsole.WriteLine(ConsoleColor.Cyan, "LocalFile: Beginning upload of file {0}...", path);
            DelayTransfer(stream);
            var result = LocalAsyncResult.Create(callback);
            ColoredConsole.WriteLine(ConsoleColor.Green, "LocalFile: File {0} is being uploaded.", path);
            return result;
        }

        public void EndUploadFile(IAsyncResult ar)
        {
            ColoredConsole.WriteLine(ConsoleColor.Cyan, "LocalFile: Ending upload of file...");
            ColoredConsole.WriteLine(ConsoleColor.Green, "LocalFile: Ended file upload.");
        }

        public void RenameFile(string sourcePath, string destinationPath)
        {
            ColoredConsole.WriteLine(ConsoleColor.Cyan, "LocalFile: Renaming file {0} to {1}...", sourcePath, destinationPath);
            File.Move(GetLocalPath(sourcePath), GetLocalPath(destinationPath));
            ColoredConsole.WriteLine(ConsoleColor.Green, "LocalFile: Renamed file {0} to {1}...", sourcePath, destinationPath);
        }

        public void DeleteFile(string path)
        {
            ColoredConsole.WriteLine(ConsoleColor.Cyan, "LocalFile: Deleting file {0}...", path);
            File.Delete(GetLocalPath(path));
            ColoredConsole.WriteLine(ConsoleColor.Green, "LocalFile: Deleted file {0}.", path);
        }

        public void CreateDirectory(string path)
        {
            ColoredConsole.WriteLine(ConsoleColor.Cyan, "LocalFile: Creating directory {0}...", path);
            Directory.CreateDirectory(GetLocalPath(path));
            ColoredConsole.WriteLine(ConsoleColor.Green, "LocalFile: Created directory {0}.", path);
        }

        public void DeleteDirectory(string path)
        {
            ColoredConsole.WriteLine(ConsoleColor.Cyan, "LocalFile: Deleting directory {0}...", path);
            Directory.Delete(GetLocalPath(path));
            ColoredConsole.WriteLine(ConsoleColor.Green, "LocalFile: Deleted directory {0}.", path);
        }

        public IEnumerable<SftpFileInfo> ListDirectory(string path, Action<int> progress)
        {
            ColoredConsole.WriteLine(ConsoleColor.Cyan, "LocalFile: Listing directory {0}...", path);
            var dirs = Directory.EnumerateDirectories(GetLocalPath(path))
                                .Select(x => new SftpFileInfo()
                                {
                                    Name = Path.GetFileName(x),
                                    Length = 0L,
                                    IsDirectory = true
                                });
            var files = Directory.EnumerateFiles(GetLocalPath(path))
                                .Select(x => new SftpFileInfo()
                                {
                                    Name = Path.GetFileName(x),
                                    Length = new FileInfo(x).Length,
                                    IsDirectory = false
                                });
            ColoredConsole.WriteLine(ConsoleColor.Green, "LocalFile: Directory {0} is listed.", path);
            return dirs.Concat(files);
        }

        public bool DirectoryExists(string path)
        {
            ColoredConsole.WriteLine(ConsoleColor.Cyan, "LocalFile: Checking if directory {0} exists...", path);
            var result = Directory.Exists(GetLocalPath(path));
            ColoredConsole.WriteLine(ConsoleColor.Green, "LocalFile: Directory {0} {1} exists.", path, result ? "" : "doesn't");
            return result;
        }

        public void Dispose()
        {
        }

        private void DelayTransfer(Stream stream)
        {
            if (_transferDelay > 0)
            {
                var sizeInMegabytes = (int)(stream.Length / (1024L * 1024L));
                var delay = ((sizeInMegabytes + 1) * _transferDelay);
                Task.Delay(delay * 1000).Wait();
            }
        }

        private string GetLocalPath(string remotePath)
        {
            if (!(remotePath.ToLower().StartsWith(_remoteRoot.ToLower(), StringComparison.InvariantCultureIgnoreCase)))
                throw new ArgumentException(
                    string.Format("Remote path must start with configured remote root {0}", _remoteRoot), "remoteRoot");

            return Path.Combine(_localRoot,
                 remotePath.Substring(_remoteRoot.Length)
                        .Replace('/', Path.DirectorySeparatorChar)
                        .TrimStart(Path.DirectorySeparatorChar));

        }
    }

    public class LocalSshCommand : ISshCommand
    {
        public IAsyncResult BeginExecute(AsyncCallback callback)
        {
            return LocalAsyncResult.Create(callback);
        }

        public void CancelAsync()
        {
        }

        public string EndExecute(IAsyncResult result)
        {
            return string.Empty;
        }

        public string Execute()
        {
            return null;
        }
    }

    public class LocalSshClient : ISshClient
    {
        public void Connect()
        {
        }

        public ISshCommand CreateCommand(string command)
        {
            return new LocalSshCommand();
        }

        public void Disconnect()
        {
        }

        public void Dispose()
        {
        }
    }

    public class LocalFileStreamProvider : IFileStreamProvider
    {
        public Stream OpenRead(string path)
        {
            return new FileStream(path, FileMode.Open, FileAccess.Read);
        }

        public Stream OpenWrite(string path)
        {
            return new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
        }
    }

    public class LocalFileClientFactory : IClientFactory
    {
        readonly string _localRoot;
        readonly string _remoteRoot;
        readonly int _transferDelay;

        public LocalFileClientFactory(string localRoot, string remoteRoot, int transferDelay)
        {
            _localRoot = localRoot;
            _remoteRoot = remoteRoot;
            _transferDelay = transferDelay;
        }

        public IFileStreamProvider CreateFileStreamProvider()
        {
            return new LocalFileStreamProvider();
        }

        public ISftpAsyncResult CreateSftpAsyncResult(IAsyncResult result)
        {
            return new LocalSftpAsyncResult(result);
        }

        public ISftpClient CreateSftpClient()
        {
            return new LocalFileClient(_localRoot, _remoteRoot, _transferDelay);
        }

        public ISshClient CreateSshClient()
        {
            return new LocalSshClient();
        }
    }
}

