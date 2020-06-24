using System;
using Lockstep;

namespace MikudosLockStepGameService.Services.MessageHandlers
{
    public class PingHandler : IHandler
    {
        public PingHandler()
        {
        }

        public StepResponse Handle(long playerId, StepRequest message)
        {
            return new StepResponse { MsgType = EMessageType.Pong };
        }
    }
}
