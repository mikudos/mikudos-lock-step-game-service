using System;
using Microsoft.Extensions.Configuration;
using MikudosLockStepGameService.Services.Game;
using Lockstep;

namespace MikudosLockStepGameService.Services.MessageHandlers
{
    public class BeginHandler: IHandler
    {
        private IConfiguration _configuration;
        private GameClass _game;
        public BeginHandler(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public MStepRes Handle(long playerId, MStepReq message)
        {
            InitGameScene.AddWaitingPlayerId(playerId);
            InitGameScene.CheckState(_configuration);
            return new MStepRes { MsgType = EResType.Pong };
        }
    }
}
