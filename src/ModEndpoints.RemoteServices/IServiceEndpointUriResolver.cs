using ModEndpoints.RemoteServices.Core;

namespace ModEndpoints.RemoteServices;
public interface IServiceEndpointUriResolver
{
  string? Resolve(IServiceRequestMarker req);
  string? Resolve<TRequest>()
    where TRequest : IServiceRequestMarker;
}
