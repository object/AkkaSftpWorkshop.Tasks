using System;
using System.IO;

namespace Shared
{
    public static class ClientFactory
    {
        private const string FtpRoot = "sftp";

        public static IClientFactory Create(int transferDelay = 0)
        {
            var rootDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FtpRoot));
            if (!Directory.Exists(rootDir)) Directory.CreateDirectory(rootDir);
            return new LocalFileClientFactory(rootDir, "", transferDelay);
        }
    }
}

