using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;

using Shared;
using Messages;

namespace Actors
{
    // Tip: just paste the implementation from the previous step! The magic happens in the way how the actor is spawned.

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
