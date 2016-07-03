using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;

using Shared;
using Messages;

namespace Actors
{
    // Tip: GetHashMap takes an ISftpCommand as the input and returns hash value that should be the same for same remote files.

    public class SftpActor : ReceiveActor, IWithUnboundedStash
	{
		private IClientFactory _clientFactory;

		public SftpActor(IClientFactory clientFactory)
		{
			_clientFactory = clientFactory;
		}

		public IStash Stash { get; set; }

        public static object GetHashMap(object o)
        {
            throw new NotImplementedException();
        }
    }
}
