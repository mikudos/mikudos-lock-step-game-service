using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using MikudosLockStepGameService.Types;
using MikudosLockStepGameService.Rx;
using Lockstep;

namespace MikudosLockStepGameService
{
    public class LockStepImpl : LockStepService.LockStepServiceBase
    {
        public IConfiguration _configuration;
        public Dictionary<long, IServerStreamWriter<HelloReply>> PlayerStreams;

        public CommonObservable<StepMessageModel> requestO;

        public LockStepImpl(IConfiguration configuration)
        {
            _configuration = configuration;
            PlayerStreams = new Dictionary<long, IServerStreamWriter<HelloReply>>();
            this.requestO = new CommonObservable<StepMessageModel>();
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
            long playerID = 0L;
            if (authToken != "")
            {
                playerID = 24L;
            }
            // TODO: check if authToken is validate
            if (playerID == 0L)
            {
                await responseStream.WriteAsync(new HelloReply { Message = "Authentication Error" });
                return;
            }
            else if (!PlayerStreams.ContainsKey(playerID))
            {
                PlayerStreams.Add(playerID, responseStream);
            }

            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;
            // Read incoming messages in a background task
            HelloRequest? lastMessageReceived = null;
            var readTask = Task.Run(async () =>
            {
                while (!token.IsCancellationRequested && await requestStream.MoveNext(token))
                {
                    // Next the message to observable
                    this.requestO.Notify(new StepMessageModel { PlayerId = playerID, Message = requestStream.Current });
                    lastMessageReceived = requestStream.Current;
                    Console.WriteLine($"lastMessageReceived: {lastMessageReceived}");
                    // if (requestStream.Current.Name == "stop")
                    // {
                    //     break;
                    // }
                }
                PlayerStreams.Remove(playerID);
                Console.WriteLine($"close client connection: {playerID}");
                context.CancellationToken.ThrowIfCancellationRequested();
                return "";
            }, token);

            context.CancellationToken.Register(() =>
            {
                Console.WriteLine("request closed, close the read task");
                tokenSource.Cancel();
                readTask.Dispose();
            });
            await readTask;
        }
    }
}