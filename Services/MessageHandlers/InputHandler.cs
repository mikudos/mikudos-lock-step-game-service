using System;
using Lockstep;
using Microsoft.Extensions.Configuration;
using MikudosLockStepGameService.Services.Models;
using MikudosLockStepGameService.Services.Game;

namespace MikudosLockStepGameService.Services.MessageHandlers
{
    public class InputHandler : IHandler
    {
        public InputHandler(IConfiguration configuration)
        {
        }

        public MStepRes Handle(long playerId, MStepReq message)
        {
            PlayerInput input = new PlayerInput(message.GameInput);
            IGameClass game = GameClass.GetGame(1);

            return new MStepRes { MsgType = EResType.Pong };
        }
    }
}
