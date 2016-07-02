using System;
using Akka.Actor;

using Shared;

namespace SftpActors
{
	public static class SftpCommand
	{
		public static string Connect = "Connect";
		public static string Disconnect = "Disconnect";
	}

	public class SftpActor : UntypedActor
	{
		private IClientFactory _clientFactory;

		public SftpActor(IClientFactory clientFactory)
		{
			_clientFactory = clientFactory;
		}

		protected override void OnReceive(object message)
		{
		}
	}
}
