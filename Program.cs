using System;
using System.Threading.Tasks;
using Grpc.Core;
using Lockstep;
using Microsoft.Extensions.Logging;

namespace MikudosLockStepGameService
{
    class Program
    {
        const int Port = 50051;

        public static void Main(string[] args)
        {
            var lockStepService = new LockStepImpl();
            Server server = new Server
            {
                Services = { LockStepService.BindService(lockStepService) },
                Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
            };
            StepService stepService = new StepService(lockStepService);
            server.Start();

            Console.WriteLine("Greeter server listening on port " + Port);
            Console.WriteLine("Press any key to stop the server...");
            var res = stepService.Start();
            // res.Wait();
            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }
    }
}
