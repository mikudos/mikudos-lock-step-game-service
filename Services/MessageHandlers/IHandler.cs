using Lockstep;

namespace MikudosLockStepGameService.Services.MessageHandlers
{
    public interface IHandler
    {
        MStepRes Handle(long playerId, MStepReq message);
    }
}