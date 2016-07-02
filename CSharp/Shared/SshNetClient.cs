using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace Shared
{
	public class SftpConnectionDetails
	{
		public string Host { get; set; }
		public int Port { get; set; }
		public string UserName { get; set; }
		public string KeyFile { get; set; }
	}

	public class SshNetSftpAsyncResult : ISftpAsyncResult
	{
		private IAsyncResult _asyncResult;

		public SshNetSftpAsyncResult(IAsyncResult ar)
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
				return _asyncResult is SftpDownloadAsyncResult
					? (_asyncResult as SftpDownloadAsyncResult).IsDownloadCanceled
					: _asyncResult is SftpUploadAsyncResult
					? (_asyncResult as SftpUploadAsyncResult).IsUploadCanceled
					: false;
			}

			set
			{
				ColoredConsole.WriteLine(ConsoleColor.Cyan, "SSH.NET: Requesting cancel...");
				if (_asyncResult is SftpDownloadAsyncResult)
					(_asyncResult as SftpDownloadAsyncResult).IsDownloadCanceled = value;
				else if (_asyncResult is SftpUploadAsyncResult)
					(_asyncResult as SftpUploadAsyncResult).IsUploadCanceled = value;
				ColoredConsole.WriteLine(ConsoleColor.Green, "SSH.NET: Cancel requested.");
			}
		}
	}

	public class SshNetClient : ISftpClient, IDisposable
	{
		private SftpConnectionDetails _connectionDetails;
		private SftpClient _connection;
		private static object _locker = new object();
		private static Dictionary<string, PrivateKeyFile> _keyFiles = new Dictionary<string, PrivateKeyFile>();

		public SshNetClient(SftpConnectionDetails connectionDetails)
		{
			_connectionDetails = connectionDetails;
			_connection = CreateConnection();
		}

		public void Connect()
		{
			ColoredConsole.WriteLine(ConsoleColor.Cyan, "SSH.NET: Connecting...");
			_connection.Connect();
			ColoredConsole.WriteLine(ConsoleColor.Green, "SSH.NET: Connected.");
		}

		public void Disconnect()
		{
			ColoredConsole.WriteLine(ConsoleColor.Cyan, "SSH.NET: Disconnecting...");
			_connection.Disconnect();
			ColoredConsole.WriteLine(ConsoleColor.Green, "SSH.NET: Disconnected.");
		}

		public void DownloadFile(string path, Stream stream, Action<ulong> progress)
		{
			ColoredConsole.WriteLine(ConsoleColor.Cyan, "SSH.NET: Downloading file {0}...", path);
			_connection.DownloadFile(path, stream, progress);
			ColoredConsole.WriteLine(ConsoleColor.Green, "SSH.NET: File {0} is downloaded.", path);
		}

		public IAsyncResult BeginDownloadFile(string path, Stream stream, AsyncCallback callback, Action<ulong> progress)
		{
			ColoredConsole.WriteLine(ConsoleColor.Cyan, "SSH.NET: Beginning download of file {0}...", path);
			var ar = _connection.BeginDownloadFile(path, stream, callback, progress);
			ColoredConsole.WriteLine(ConsoleColor.Green, "SSH.NET: File {0} is being downloaded.", path);
			return ar;
		}

		public void EndDownloadFile(IAsyncResult ar)
		{
			ColoredConsole.WriteLine(ConsoleColor.Cyan, "SSH.NET: Ending download of file...");
			_connection.EndDownloadFile(ar);
			ColoredConsole.WriteLine(ConsoleColor.Green, "SSH.NET: Ended file downloaded.");
		}

		public void UploadFile(Stream stream, string path, Action<ulong> progress)
		{
			ColoredConsole.WriteLine(ConsoleColor.Cyan, "SSH.NET: Uploading file {0}...", path);
			_connection.UploadFile(stream, path, progress);
			ColoredConsole.WriteLine(ConsoleColor.Green, "SSH.NET: File {0} is uploaded.", path);
		}

		public IAsyncResult BeginUploadFile(Stream stream, string path, AsyncCallback callback, Action<ulong> progress)
		{
			ColoredConsole.WriteLine(ConsoleColor.Cyan, "SSH.NET: Beginning upload of file {0}...", path);
			var ar = _connection.BeginUploadFile(stream, path, callback, progress);
			ColoredConsole.WriteLine(ConsoleColor.Green, "SSH.NET: File {0} is being uploaded.", path);
			return ar;
		}

		public void EndUploadFile(IAsyncResult ar)
		{
			ColoredConsole.WriteLine(ConsoleColor.Cyan, "SSH.NET: Ending upload of file...");
			_connection.EndUploadFile(ar);
			ColoredConsole.WriteLine(ConsoleColor.Green, "SSH.NET: Ended file uploaded.");
		}

		public void RenameFile(string sourcePath, string destinationPath)
		{
			ColoredConsole.WriteLine(ConsoleColor.Cyan, "SSH.NET: Renaming file {0} to {1}...", sourcePath, destinationPath);
			_connection.RenameFile(sourcePath, destinationPath);
			ColoredConsole.WriteLine(ConsoleColor.Green, "SSH.NET: Renamed file {0} to {1}.", sourcePath, destinationPath);
		}

		public void DeleteFile(string path)
		{
			ColoredConsole.WriteLine(ConsoleColor.Cyan, "SSH.NET: Deleting file {0}...", path);
			_connection.DeleteFile(path);
			ColoredConsole.WriteLine(ConsoleColor.Green, "SSH.NET: Deleted file {0}.", path);
		}

		public void CreateDirectory(string path)
		{
			ColoredConsole.WriteLine(ConsoleColor.Cyan, "SSH.NET: Creating directory {0}...", path);
			_connection.CreateDirectory(path);
			ColoredConsole.WriteLine(ConsoleColor.Green, "SSH.NET: Created directory {0}.", path);
		}

		public void DeleteDirectory(string path)
		{
			ColoredConsole.WriteLine(ConsoleColor.Cyan, "SSH.NET: Deleting directory {0}...", path);
			_connection.DeleteDirectory(path);
			ColoredConsole.WriteLine(ConsoleColor.Green, "SSH.NET: Deleted directory {0}.", path);
		}

		public IEnumerable<SftpFileInfo> ListDirectory(string path, Action<int> progress = null)
		{
			ColoredConsole.WriteLine(ConsoleColor.Cyan, "SSH.NET: Listing directory {0}...", path);
			var result = _connection.ListDirectory(path, progress);
			ColoredConsole.WriteLine(ConsoleColor.Green, "SSH.NET: Listed directory {0}.", path);
			return result.Select(x => new SftpFileInfo()
				{
					Name = x.Name,
					Length = x.Length,
					IsDirectory = x.IsDirectory
				});
		}

		public bool DirectoryExists(string path)
		{
			ColoredConsole.WriteLine(ConsoleColor.Cyan, "SSH.NET: Checking if directory {0} exists...", path);
			var result = Directory.Exists(path);
			ColoredConsole.WriteLine(ConsoleColor.Green, "SSH.NET: Directory {0} {1} exists.", path, result ? "" : "doesn't");
			return result;
		}

		public void Dispose()
		{
			_connection.Dispose();
		}

		private SftpClient CreateConnection()
		{
			return new SftpClient(
				_connectionDetails.Host,
				_connectionDetails.Port,
				_connectionDetails.UserName,
				GetKeyFile());
		}

		private PrivateKeyFile GetKeyFile()
		{
			lock (_locker)
			{
				var path = _connectionDetails.KeyFile;
				PrivateKeyFile pk;
				if (!_keyFiles.TryGetValue(path, out pk))
				{
					pk = new PrivateKeyFile(path);
					_keyFiles.Add(path, pk);
				}
				return pk;
  			}
		}
	}
}

