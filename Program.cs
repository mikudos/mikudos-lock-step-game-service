using System;
using System.Threading.Tasks;
using Grpc.Core;
using Lockstep;
using Microsoft.Extensions.Logging;

namespace mikudos_lock_step_game_service
{
    class Program
    {
        const int Port = 50051;

        public static void Main(string[] args)
        {
            Server server = new Server
            {
                Services = { LockStepService.BindService(new LockStepImpl()) },
                Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
            };
            server.Start();

            Console.WriteLine("Greeter server listening on port " + Port);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }
    }
}
