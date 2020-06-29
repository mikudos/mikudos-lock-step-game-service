using System;
using Lockstep;
using Microsoft.Extensions.Configuration;
using MikudosLockStepGameService.Services.Game;
namespace MikudosLockStepGameService.Services.MessageHandlers
{
    public class LeaveHandler : IHandler
    {
        public LeaveHandler(IConfiguration configuration)
        {
        }

        public MStepRes Handle(long playerId, MStepReq message)
        {
            InitGameScene.RemoveWaitingPlayerId(playerId);
            var gameId = GameClass.GetGameIdWithPlayerId(playerId);
            IGameClass game = GameClass.GetGame(gameId);
            game.OnPlayerLeave(playerId);
            return new MStepRes { ID = message.ID, MsgType = EResType.Pong };
        }
    }
}
