using System;
using Lockstep;
using Microsoft.Extensions.Configuration;
using MikudosLockStepGameService.Services.Game;

namespace MikudosLockStepGameService.Services.MessageHandlers
{
    public class InputHandler : IHandler
    {
        public InputHandler(IConfiguration configuration)
        {
        }

        public StepResponse Handle(long playerId, StepRequest message)
        {
            PlayerInput input = new PlayerInput { mousePos = FixVector2.Zero, inputUV = FixVector2.Zero, isInputFire = false, isSpeedUp = false, skillId = -1 };
            IGameClass game = GameClass.GetGame(1);

            return new StepResponse { MsgType = EMessageType.Pong };
        }
    }
}
