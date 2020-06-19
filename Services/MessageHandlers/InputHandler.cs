using System;
using Lockstep;
using Microsoft.Extensions.Configuration;
namespace MikudosLockStepGameService.Services.MessageHandlers
{
    public class InputHandler: IHandler
    {
        public InputHandler(IConfiguration configuration)
        {
        }

        public HelloReply Handle(long playerId, HelloRequest message)
        {
            return new HelloReply { MsgType = MessageType.Pong, Message = "hello" + message.Name };
        }
    }
}
