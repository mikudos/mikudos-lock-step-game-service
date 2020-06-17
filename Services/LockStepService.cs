using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using MikudosLockStepGameService.Types;
using MikudosLockStepGameService.Rx;
using Lockstep;

namespace MikudosLockStepGameService
{
    public class LockStepImpl : LockStepService.LockStepServiceBase
    {
        public Dictionary<string, IServerStreamWriter<HelloReply>> PlayerStreams;

        public CommonObservable<StepMessgae> requestO;

        public LockStepImpl()
        {
            PlayerStreams = new Dictionary<string, IServerStreamWriter<HelloReply>>();
            this.requestO = new CommonObservable<StepMessgae>();
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

            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;
            // Read incoming messages in a background task
            HelloRequest? lastMessageReceived = null;
            var readTask = Task.Run(async () =>
            {
                if (!PlayerStreams.ContainsKey(authToken))
                {
                    PlayerStreams.Add(authToken, responseStream);
                }
                while (!token.IsCancellationRequested && await requestStream.MoveNext(token))
                {
                    lastMessageReceived = requestStream.Current;
                    Console.WriteLine($"lastMessageReceived: {lastMessageReceived}");
                    await PlayerStreams[authToken].WriteAsync(new HelloReply { Message = "hello" + requestStream.Current.Name });
                    if (requestStream.Current.Name == "stop")
                    {
                        break;
                    }
                }
                PlayerStreams.Remove(authToken);
                Console.WriteLine($"close client connection: {authToken}");
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