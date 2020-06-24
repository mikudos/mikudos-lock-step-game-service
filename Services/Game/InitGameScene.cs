using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
namespace MikudosLockStepGameService.Services.Game
{
    public class InitGameScene
    {
        private static List<long> waitingPlayerIds;
        public InitGameScene()
        {
        }

        public static void AddWaitingPlayerId(long playerId)
        {
            waitingPlayerIds.Add(playerId);
        }

        public static void RemoveWaitingPlayerId(long playerId)
        {
            waitingPlayerIds.Remove(playerId);
        }

        // if waitingPlayers match to the config playerNumber in One Round, then Init Game
        public static bool CheckState(IConfiguration configuration)
        {
            if (waitingPlayerIds.Count >= configuration.GetValue<int>("scene_player_count", 2))
            {

                return true;
            } else
            {
                return false;
            }
        }
    }
}
