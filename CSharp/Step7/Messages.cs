using System;

namespace Messages
{
	public interface ISftpCommand { }

	public class ListDirectory : ISftpCommand
	{
		public ListDirectory(string remotePath)
		{
			this.RemotePath = remotePath;
		}

		public string RemotePath { get; private set; }
	}

	public class UploadFile : ISftpCommand
	{
		public UploadFile(string localPath, string remotePath)
		{
			this.LocalPath = localPath;
			this.RemotePath = remotePath;
		}

		public string LocalPath { get; private set; }
		public string RemotePath { get; private set; }
	}

	public class DownloadFile : ISftpCommand
	{
		public DownloadFile(string localPath, string remotePath)
		{
			this.LocalPath = localPath;
			this.RemotePath = remotePath;
		}

		public string LocalPath { get; private set; }
		public string RemotePath { get; private set; }
	}

	public class Cancel : ISftpCommand
	{
		public Cancel(string target)
		{
			this.Target = target;
		}

		public string Target { get; private set; }
	}

	public interface ISftpCommandResult { }

	public class Completed : ISftpCommandResult { }

	public class Cancelled : ISftpCommandResult { }

	public class Error : ISftpCommandResult
	{
		public Error(string message)
		{
			this.Message = message;
		}

		public string Message { get; private set; }
	}
}

