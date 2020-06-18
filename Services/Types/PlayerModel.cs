using MikudosLockStepGameService.Util;
using Lockstep;

namespace MikudosLockStepGameService.Types
{
    public class PlayerModel : BaseRecyclable
    {
        public long UserId;
        public string Account;
        public string LoginHash;
        public byte LocalId;
        public Game Game;
        public GameData GameData;
        public int GameId => Game?.GameId ?? -1;

        public void OnLeave()
        {
        }

        public override void OnRecycle()
        {
        }
    }
}