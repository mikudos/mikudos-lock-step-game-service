using MikudosLockStepGameService.Util;
using MikudosLockStepGameService.Services.Game;
using Lockstep;

namespace MikudosLockStepGameService.Services.Models
{
    public class PlayerModel : BaseRecyclable
    {
        public long UserId;
        public string Account;
        public string LoginHash;
        public byte LocalId;
        public GameClass Game;
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