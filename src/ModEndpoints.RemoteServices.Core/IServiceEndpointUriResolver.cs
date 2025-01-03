namespace ModEndpoints.RemoteServices.Core;
public interface IServiceEndpointUriResolver
{
  string? Resolve(IServiceRequestMarker req);
  string? Resolve<TRequest>()
    where TRequest : IServiceRequestMarker;
}
