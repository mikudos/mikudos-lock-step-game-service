using Lockstep;

namespace MikudosLockStepGameService.Services.MessageHandlers
{
    public interface IHandler
    {
        StepResponse Handle(long playerId, StepRequest message);
    }
}