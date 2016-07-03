using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;

using Shared;
using Messages;

namespace Actors
{
    // Tip: after invoking SFTP connection ListDirectory method send the result to the sender.

    public class SftpActor : ReceiveActor
	{
		private IClientFactory _clientFactory;

		public SftpActor(IClientFactory clientFactory)
		{
            _clientFactory = clientFactory;
        }
    }
}
