using System;
using Akka.Actor;

using Shared;
using Messages;

namespace Actors
{
    // Tip: you will be implementing a stateful actor (switching between disconnected and connected states) using Become method.
    // The actor is typed so you can use generic Receive<Y> method to specify message types the actor expects.

    public class SftpActor : ReceiveActor
	{
		private IClientFactory _clientFactory;

		public SftpActor(IClientFactory clientFactory)
		{
            _clientFactory = clientFactory;
		}
	}
}
