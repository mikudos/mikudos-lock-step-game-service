using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Health.V1;

namespace MikudosLockStepGameService
{
    public class HealthImpl : Health.HealthBase
    {
        public override Task<HealthCheckResponse> Check(HealthCheckRequest request, ServerCallContext context)
        {
            var servcieName = request.Service;
            return Task.FromResult(new HealthCheckResponse { Status = HealthCheckResponse.Types.ServingStatus.Serving });
        }

        public override async Task Watch(HealthCheckRequest request, IServerStreamWriter<HealthCheckResponse> responseStream, ServerCallContext context)
        {
            var servcieName = request.Service;
            await responseStream.WriteAsync((new HealthCheckResponse { Status = HealthCheckResponse.Types.ServingStatus.Serving }));
        }
    }
}