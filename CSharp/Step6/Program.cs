using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;

using Shared;
using Messages;
using Actors;

namespace Application
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            PrintInstructions();

            Console.Write("Press any key to start the actor system and validate the implementation.");
            Console.ReadKey();

            Run().Wait();
        }

        private static void PrintInstructions()
        {
            Console.WriteLine("Time to write mode code! In step 6 you will implement replace synchronous implementation file transfer commands with asynchronous,");
            Console.WriteLine("so it should be possible to cancel pending transfer. Then you implement Cancel command.");
            Console.WriteLine("Once the actor is property implemented the program should display the following messages:");
            Console.WriteLine();
            ColoredConsole.WriteLine(ConsoleColor.Cyan, "SSH.NET: Connecting...");
            ColoredConsole.WriteLine(ConsoleColor.Green, "SSH.NET: Connected.");
            ColoredConsole.WriteLine(ConsoleColor.Cyan, "SSH.NET: Checking if directory <directory name> exists...");
            ColoredConsole.WriteLine(ConsoleColor.Green, "SSH.NET: Directory <directory name> exists.");
            ColoredConsole.WriteLine(ConsoleColor.Cyan, "SSH.NET: Beginning Upload of file <file name>...");
            ColoredConsole.WriteLine(ConsoleColor.Green, "SSH.NET: Ending upload of file <file name>.");
            ColoredConsole.WriteLine(ConsoleColor.Cyan, "SSH.NET: Requesting cancel...");
            ColoredConsole.WriteLine(ConsoleColor.Green, "SSH.NET: Cancel requested.");
            Console.WriteLine("    pause for about 10 seconds");
            ColoredConsole.WriteLine(ConsoleColor.Cyan, "SSH.NET: Disconnecting...");
            ColoredConsole.WriteLine(ConsoleColor.Green, "SSH.NET: Disconnected.");
            Console.WriteLine();
        }

        private static async Task Run()
        {
            var clientFactory = ClientFactory.Create(10);
            var actorSystem = ActorSystem.Create("MyActorSystem");

            var sftpActor = actorSystem.ActorOf(
                Props.Create(() => new SftpActor(clientFactory)),
                "sftpActor");

            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var remotePath = "/test/12345.dll";
            sftpActor.Tell(new UploadFile(Path.Combine(baseDir, "Wire.dll"), remotePath));
            await Task.Delay(2000);
            sftpActor.Tell(new Cancel(remotePath));
            Console.WriteLine();

            Console.ReadKey();

            await actorSystem.Terminate();
        }
    }
}
