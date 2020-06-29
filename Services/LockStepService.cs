using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using MikudosLockStepGameService.Services.Models;
using MikudosLockStepGameService.Rx;
using MikudosLockStepGameService.Services.Exceptions;
using Lockstep;

namespace MikudosLockStepGameService
{
    public class LockStepImpl : LockStepService.LockStepServiceBase, ILockStepImpl
    {
        public IConfiguration _configuration { get; private set; }
        public Dictionary<long, IServerStreamWriter<MStepRes>> PlayerStreams { get; private set; }

        public CommonObservable<StepMessageModel> requestO { get; private set; }

        public LockStepImpl(IConfiguration configuration)
        {
            _configuration = configuration;
            PlayerStreams = new Dictionary<long, IServerStreamWriter<MStepRes>>();
            this.requestO = new CommonObservable<StepMessageModel>();
        }
        // Server side handler of the SayHello RPC
        public override Task<MHelloReply> SayHello(MHelloReq request, ServerCallContext context)
        {
            return Task.FromResult(new MHelloReply { Message = "Hello " + request.Name });
        }

        public override async Task LockStepStream(IAsyncStreamReader<MStepReq> requestStream, IServerStreamWriter<MStepRes> responseStream, ServerCallContext context)
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
                throw new AuthenticationException();
            }
            else if (!PlayerStreams.ContainsKey(playerID))
            {
                PlayerStreams.Add(playerID, responseStream);
            }

            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;
            // Read incoming messages in a background task
            MStepReq? lastMessageReceived = null;
            var readTask = Task.Run(async () =>
            {
                while (!token.IsCancellationRequested && await requestStream.MoveNext(token))
                {
                    // Next the message to observable
                    this.requestO.Notify(new StepMessageModel(this) { PlayerId = playerID, Message = requestStream.Current });
                    lastMessageReceived = requestStream.Current;
                    Console.WriteLine($"lastMessageReceived: {lastMessageReceived}");
                    // if (requestStream.Current.Name == "stop")
                    // {
                    //     break;
                    // }
                }
                PlayerStreams.Remove(playerID);
                this.requestO.Notify(new StepMessageModel(this) { PlayerId = playerID, Message = new MStepReq() { MsgType = EMessageType.Leave } });
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