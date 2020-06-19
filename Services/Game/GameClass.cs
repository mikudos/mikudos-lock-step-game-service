using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using MikudosLockStepGameService.Services.Exceptions;
using MikudosLockStepGameService.Services.Logger;

namespace MikudosLockStepGameService.Services.Game
{
    public class GameClass : BaseLogger
    {
        private static Dictionary<int, GameClass> _games;

        public int GameId;
        public GameClass(IConfiguration configuration)
        {

        }

        public static GameClass GetGame(int key, IConfiguration configuration)
        {
            if (_games[key] == null && configuration == null)
            {
                throw new NullConfigurationException();
            }
            if (_games[key] == null)
            {
                _games[key] = new GameClass(configuration);
            }
            return _games[key];
        }
    }
}
