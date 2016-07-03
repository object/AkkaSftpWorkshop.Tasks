using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;

using Shared;
using Messages;

namespace Actors
{
    // Tip: Use IClientFactory.CreateFileStreamProvider to obtain a local file stream for reading and writing.
    // ISftpClient.UploadFile and ISftpClient.DownloadFile are your friends to implement upload and download commands.

    // Tip: Use Utils.EnsureParentDirectoryExists to create remote directories prior to uploading a file.

    public class SftpActor : ReceiveActor, IWithUnboundedStash
	{
		private IClientFactory _clientFactory;

		public SftpActor(IClientFactory clientFactory)
		{
			_clientFactory = clientFactory;
		}

		public IStash Stash { get; set; }
	}
}
