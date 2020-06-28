using System;
using System.Collections.Generic;
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
            var playerInputs = new List<PlayerInput>();
            foreach (var item in message.GameInput.Commands)
            {
                playerInputs.Add(new PlayerInput(item));
            }
            var input = new Msg_PlayerInput(message.GameInput.Tick, (byte)message.GameInput.ActorId, playerInputs);
#if DEBUG_SHOW_INPUT
            if (input.Commands != null && input.Commands?.Length > 0) {
                var cmd = input.Commands[0];
                var playerInput = new Deserializer(cmd.content).Parse<Lockstep.Game.PlayerInput>();
                if (playerInput.inputUV != LVector2.zero) {
                    Debug.Log(
                        $"curTick{Tick} isOutdate{input.Tick < Tick} RecvInput actorID:{input.ActorId}  inputTick:{input.Tick}  move:{playerInput.inputUV}");
                }
            }
#endif
            var gameId = GameClass.GetGameIdWithPlayerId(playerId);
            IGameClass game = GameClass.GetGame(gameId);
            game.HandlePlayerInput(input);

            return new MStepRes { MsgType = EResType.Pong };
        }
    }
}
