using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;

using Shared;
using Messages;

namespace Actors
{
    // Tip: even though this step is probably the most complex one, you can quickly manage it with following technique:
    // Use ISftpClient.BeginUploadFile and ISftpClient.BeginDownloadFile instead of UploadFile and DownloadFile.

    // Tip: Write asyncCallback function that will take IAsyncResult, invoke EndUploadFile or EndDownloadFile 
    // and send Completed, Cancelled or Error message to the sender's mailbox.

    // Tip: In addition to "Connected" and "Disconnected" handlers you will need a handler "Tranfserring"
    // that will correspond to the state of the actor when file transfer is in progress

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
