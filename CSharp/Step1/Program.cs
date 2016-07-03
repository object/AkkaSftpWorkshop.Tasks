using System;
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
            Console.WriteLine("The step 1 of the workshop is simple: just teach your SFTP actor to connect to the FTP server.");
            Console.WriteLine("Once the actor is property implemented the program should display the following messages:");
            Console.WriteLine();
            ColoredConsole.WriteLine(ConsoleColor.Cyan, "SSH.NET: Connecting...");
            ColoredConsole.WriteLine(ConsoleColor.Green, "SSH.NET: Connected.");
            ColoredConsole.WriteLine(ConsoleColor.Cyan, "SSH.NET: Disconnecting...");
            ColoredConsole.WriteLine(ConsoleColor.Green, "SSH.NET: Disconnected.");
            Console.WriteLine();
        }

        private async static Task Run()
        {
            var clientFactory = ClientFactory.Create();
            var actorSystem = ActorSystem.Create("MyActorSystem");

            var sftpActor = actorSystem.ActorOf(
                Props.Create(() => new SftpActor(clientFactory)),
                "sftpActor");

            sftpActor.Tell(new Connect());
            sftpActor.Tell(new Disconnect());

            Console.ReadKey();

            await actorSystem.Terminate();
        }
    }
}
