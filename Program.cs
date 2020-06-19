using System;
using System.Threading.Tasks;
using Grpc.Core;
using Lockstep;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace MikudosLockStepGameService
{
    class Program
    {
        public static void Main(string[] args)
        {
            var env = Environment.GetEnvironmentVariable("DOT_NET_ENV");
            var builder = new ConfigurationBuilder();
            builder
                .AddYamlFile("config/appsettings.yml", optional: true)
                .AddJsonFile($"config/appsettings.{env}.yml", optional: true);
            IConfiguration _conf = builder.Build();
            var lockStepService = new LockStepImpl(_conf);
            int Port = _conf.GetValue<int>("port", 50051);
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
