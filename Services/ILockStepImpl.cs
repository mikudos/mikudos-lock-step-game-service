using System.Collections.Generic;
using Grpc.Core;
using Lockstep;
using Microsoft.Extensions.Configuration;
using MikudosLockStepGameService.Rx;
using MikudosLockStepGameService.Types;

namespace MikudosLockStepGameService
{
    public interface ILockStepImpl
    {
        IConfiguration _configuration { get; }
        Dictionary<long, IServerStreamWriter<HelloReply>> PlayerStreams { get; }
        CommonObservable<StepMessageModel> requestO { get; }
    }
}