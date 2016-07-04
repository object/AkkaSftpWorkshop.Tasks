using System;
using System.Collections.Generic;
using System.IO;

namespace Shared
{
    public class SftpFileInfo
    {
        public string Name { get; set; }
        public long Length { get; set; }
        public bool IsDirectory { get; set; }
    }

    public interface ISftpAsyncResult : IAsyncResult
    {
        IAsyncResult AsyncResult { get; }
        bool IsCanceled { get; set; }
    }

    public interface ISftpClient
    {
        void Connect();
        void Disconnect();
        void DownloadFile(string path, Stream stream, Action<ulong> progress);
        IAsyncResult BeginDownloadFile(string path, Stream stream, AsyncCallback callback, Action<ulong> progress);
        void EndDownloadFile(IAsyncResult ar);
        void UploadFile(Stream stream, string path, Action<ulong> progress);
        IAsyncResult BeginUploadFile(Stream stream, string path, AsyncCallback callback, Action<ulong> progress);
        void EndUploadFile(IAsyncResult ar);
        void RenameFile(string sourcePath, string destinationPath);
        void DeleteFile(string path);
        void CreateDirectory(string path);
        void DeleteDirectory(string path);
        IEnumerable<SftpFileInfo> ListDirectory(string path, Action<int> progress);
        bool DirectoryExists(string path);
        void Dispose();
    }

    public interface ISshCommand
    {
        string Execute();
        IAsyncResult BeginExecute(AsyncCallback callback);
        string EndExecute(IAsyncResult result);
        void CancelAsync();
    }

    public interface ISshClient
    {
        void Connect();
        void Disconnect();
        ISshCommand CreateCommand(string command);
        void Dispose();
    }

    public interface IFileStreamProvider
    {
        Stream OpenRead(string path);
        Stream OpenWrite(string path);
    }

    public interface IClientFactory
    {
        ISftpClient CreateSftpClient();
        ISshClient CreateSshClient();
        IFileStreamProvider CreateFileStreamProvider();
        ISftpAsyncResult CreateSftpAsyncResult(IAsyncResult result);
    }

    public class FileStreamProvider : IFileStreamProvider
    {
        public Stream OpenRead(string path)
        {
            return File.OpenRead(path);
        }

        public Stream OpenWrite(string path)
        {
            return File.OpenWrite(path);
        }
    }
}
