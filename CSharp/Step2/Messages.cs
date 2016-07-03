using System;

namespace Messages
{

    public class Connect { }
    public class Disconnect { }
    public class ListDirectory
    {
        public ListDirectory(string remotePath)
        {
            this.RemotePath = remotePath;
        }

        public string RemotePath { get; private set; }
    }
}

