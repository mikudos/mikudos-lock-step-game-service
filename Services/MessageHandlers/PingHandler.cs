using System;
using Lockstep;

namespace MikudosLockStepGameService.Services.MessageHandlers
{
    public class PingHandler : IHandler
    {
        public PingHandler()
        {
        }

        public HelloReply Handle(long playerId, HelloRequest message)
        {
            return new HelloReply { MsgType = EMessageType.Pong, Message = "hello" + message.Name };
        }
    }
}
