using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using MikudosLockStepGameService.Services.Exceptions;
using MikudosLockStepGameService.Services.Logger;

namespace MikudosLockStepGameService.Services.Game
{
    public class GameClass : BaseLogger
    {
        private static Dictionary<string, GameClass> _games;
        public GameClass()
        {
        }

        public static GameClass GetGame(string key, IConfiguration configuration)
        {
            if (_games[key]== null && configuration== null)
            {
                throw new NullConfigurationException();
            }
            if (_games[key] == null)
            {
                _games[key] = new GameClass();
            }
            return _games[key];
        }
    }
}
