using System;

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
	}
}

