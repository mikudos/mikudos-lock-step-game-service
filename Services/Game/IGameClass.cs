using MikudosLockStepGameService.Types;

namespace MikudosLockStepGameService.Services.Game
{
    public interface IGameClass
    {
        int GameId { get; }
        int MapId { get; set; }
        int GameType { get; set; }
        string GameHash { get; set; }
        string Name { get; set; }
        int Tick { get; }
        int Seed { get; set; }
        PlayerModel[] Players { get; }
        int _tickSinceGameStart { get; }

        void DoDestroy();
        void DoStart(int gameType, int mapId, PlayerModel[] playerInfos, string gameHash);
        void DoUpdate(float deltaTime);
        ushort GetGameIdWithPlayerId(long playerId);
    }
}