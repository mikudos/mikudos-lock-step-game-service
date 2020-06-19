using System;
using Lockstep;
using Microsoft.Extensions.Configuration;
using MikudosLockStepGameService.Services.Game;

namespace MikudosLockStepGameService.Services.MessageHandlers
{
    public class InputHandler: IHandler
    {
        public InputHandler(IConfiguration configuration)
        {
        }

        public HelloReply Handle(long playerId, HelloRequest message)
        {
            PlayerInput input = new PlayerInput { mousePos = FixVector2.Zero, inputUV = FixVector2.Zero, isInputFire = false, isSpeedUp = false, skillId = -1};
            GameClass game = GameClass.GetGame(1, null);

            return new HelloReply { MsgType = MessageType.Pong, Message = "hello" + message.Name };
        }
    }
}
