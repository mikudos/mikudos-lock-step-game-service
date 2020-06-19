using Lockstep;

namespace MikudosLockStepGameService.Services.MessageHandlers
{
    public interface IHandler
    {
        HelloReply Handle(long playerId, HelloRequest message);
    }
}