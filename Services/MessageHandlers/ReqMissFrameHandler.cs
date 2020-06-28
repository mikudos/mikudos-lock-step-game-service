using System;
using Lockstep;
using Microsoft.Extensions.Configuration;
using MikudosLockStepGameService.Services.Models;

namespace MikudosLockStepGameService.Services.MessageHandlers
{
    public class ReqMissFrameHandler : IHandler
    {
        public ReqMissFrameHandler(IConfiguration configuration)
        {
        }

        public MStepRes Handle(long playerId, MStepReq message)
        {
            return new MStepRes { MsgType = EResType.Pong };
        }
    }
}
