using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;

using Shared;
using Messages;

namespace Actors
{
    // Tip: connect on demand, then use actor's context SetReceiveTimeout to trigger timeout when the actor is idle,
    // handle Actor.ReceiveTimeout message to release SFTP connection.

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
