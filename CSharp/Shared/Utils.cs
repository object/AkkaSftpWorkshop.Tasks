using System;
using System.Collections.Generic;
using System.Linq;

namespace System
{
    public static class ColoredConsole
    {
        private static void Execute(ConsoleColor color, Action<string, string[]> action, string format, string[] args)
        {
            var currentColor = Console.ForegroundColor;
            try
            {
                Console.ForegroundColor = color;
                action(format, args);
            }
            finally
            {
                Console.ForegroundColor = currentColor;
            }
        }

        public static void Write(ConsoleColor color, string format, params string[] args)
        {
            Execute(color, Console.Write, format, args);
        }

        public static void WriteLine(ConsoleColor color, string format, params string[] args)
        {
            Execute(color, Console.WriteLine, format, args);
        }
    }
}

namespace Shared
{
    public static class Utils
    {
        public static void CreateDirectoryTree(ISftpClient connection, string parentPath, IEnumerable<string> dirs)
        {
            if (dirs.Any())
            {
                var dir = string.Join("/", parentPath, dirs.First());
                connection.CreateDirectory(dir);
                CreateDirectoryTree(connection, dir, dirs.Skip(1));
            }
        }

        public static void EnsureParentDirectoryExists(ISftpClient connection, string remotePath)
        {
            var pos = remotePath.LastIndexOf('/');
            if (pos > 0)
            {
                var dir = remotePath.Substring(0, pos);
                if (!connection.DirectoryExists(dir))
                    CreateDirectoryTree(connection, "",
                        dir.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries));
            }
        }
    }
}

