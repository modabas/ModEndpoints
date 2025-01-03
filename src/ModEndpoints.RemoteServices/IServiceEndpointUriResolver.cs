using ModEndpoints.RemoteServices.Core;
using ModResults;

namespace ModEndpoints.RemoteServices;
public interface IServiceEndpointUriResolver
{
  Result<string> Resolve(IServiceRequestMarker req);
  Result<string> Resolve<TRequest>()
    where TRequest : IServiceRequestMarker;
}
