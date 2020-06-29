using Lockstep;
using Microsoft.Extensions.Configuration;
using MikudosLockStepGameService.Services.Models;

namespace MikudosLockStepGameService.Services.Game
{
    public interface IGameClass
    {
        ushort GameId { get; }
        int MapId { get; set; }
        int GameType { get; set; }
        string GameHash { get; set; }
        string Name { get; set; }
        int Tick { get; }
        int Seed { get; set; }
        EGameState State { get; }
        PlayerModel[] Players { get; }
        int _tickSinceGameStart { get; }
        int CurPlayerCount { get; }

        IGameClass DoCreate(IConfiguration configuration);
        void DoDestroy();
        void DoStart(int gameType, int mapId, PlayerModel[] playerInfos, string gameHash);
        void DoUpdate(float deltaTime);
        void HandlePlayerInput(Msg_PlayerInput input);
        void OnPlayerDisconnect(PlayerModel player);
        void OnPlayerLeave(long userId);
        void OnPlayerLeave(PlayerModel player);
        void OnPlayerReconnect(PlayerModel player);
    }
}