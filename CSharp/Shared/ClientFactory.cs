using System;

namespace Shared
{
	public static class ClientFactory
	{
		public static IClientFactory Create(int transferDelay = 0)
		{
			return new LocalFileClientFactory("C:\\temp\\sftp", "", transferDelay);
		}
	}
}

