using System;
using Lockstep;
using Microsoft.Extensions.Configuration;
namespace MikudosLockStepGameService.Services.MessageHandlers
{
    public class LoadingProgressHandler : IHandler
    {
        public LoadingProgressHandler(IConfiguration configuration)
        {
        }

        public MStepRes Handle(long playerId, MStepReq message)
        {
            return new MStepRes { ID = message.ID, MsgType = EResType.Pong };
        }
    }
}
