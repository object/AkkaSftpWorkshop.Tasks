using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Routing;

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

            Console.Write("Press any key to start the system with a single actor and no transfer delay. 1 file will be transferred. Wait until the actor disconnects.");
            Console.ReadKey();
            Run(1, 1, 1, 0).Wait();

            Console.Write("Press any key to start the system with a single actor and no transfer delay. 1 file will be transferred. Wait until the actor disconnects.");
            Console.ReadKey();
            Run(2, 10, 1, 2).Wait();

            Console.Write("Press any key to start the system with a single actor and no transfer delay. 1 file will be transferred. Wait until the actor disconnects.");
            Console.ReadKey();
            Run(3, 10, 10, 2).Wait();
        }

        private static void PrintInstructions()
        {
            Console.WriteLine("Now that we have file transfer commands in place, it's time to check how they perform.");
            Console.WriteLine("In the step 5 you're not going to write any new code, just sit back and watch the program running.");
            Console.WriteLine("The only thing that's changed is that instead of a single SFTP actor there will be a pool of 10 actors transferring files.");
            Console.WriteLine();
        }

        private static async Task Run(int roundNumber, int fileCount, int poolSize, int transferDelay)
        {
            var clientFactory = ClientFactory.Create();
            var actorSystem = ActorSystem.Create("MyActorSystem" + roundNumber);

            var sftpActor = actorSystem.ActorOf(
                Props.Create(() => new SftpActor(clientFactory)).WithRouter(new SmallestMailboxPool(poolSize)),
                "sftpActor");

            for (int fileNumber = 0; fileNumber < fileCount; fileNumber++)
            {
                var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                var remotePath = "/test/12345" + "-" + roundNumber + "-" + fileNumber + ".dll";
                sftpActor.Tell(new UploadFile(Path.Combine(baseDir, "Wire.dll"), remotePath));
                Console.WriteLine();
            }

            Console.ReadKey();
            await actorSystem.Terminate();
        }
    }
}
