using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Lockstep;

namespace mikudos_lock_step_game_service
{
    public class LockStepImpl : LockStepService.LockStepServiceBase
    {
        public Dictionary<string, IServerStreamWriter<HelloReply>> PlayerStreams;

        public LockStepImpl()
        {
            PlayerStreams = new Dictionary<string, IServerStreamWriter<HelloReply>>();
        }
        // Server side handler of the SayHello RPC
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply { Message = "Hello " + request.Name });
        }

        public override async Task SayStream(IAsyncStreamReader<HelloRequest> requestStream, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
        {
            var authToken = context.RequestHeaders.Single(h => h.Key == "authentication").Value;
            Console.WriteLine($"authToken:{authToken}");

            // Read incoming messages in a background task
            HelloRequest? lastMessageReceived = null;
            var readTask = Task.Run(async () =>
            {
                if (!PlayerStreams.ContainsKey(authToken))
                {
                    PlayerStreams.Add(authToken, responseStream);
                }
                await foreach (var message in requestStream.ReadAllAsync())
                {
                    lastMessageReceived = message;
                    Console.WriteLine($"lastMessageReceived: {lastMessageReceived}");
                    await PlayerStreams[authToken].WriteAsync(new HelloReply { Message = "hello" + message.Name });
                    if (message.Name == "stop")
                    {
                        break;
                    }
                }
                PlayerStreams.Remove(authToken);
                return "";
            });

            await readTask;
        }
    }
}