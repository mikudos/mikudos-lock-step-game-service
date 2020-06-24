using System;
using Lockstep;

namespace MikudosLockStepGameService.Services.MessageHandlers
{
    public class PingHandler : IHandler
    {
        public PingHandler()
        {
        }

        public MStepRes Handle(long playerId, MStepReq message)
        {
            return new MStepRes { MsgType = EMessageType.Pong };
        }
    }
}
