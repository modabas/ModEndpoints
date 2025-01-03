using ModEndpoints.RemoteServices.Core;
using ModResults;

namespace ModEndpoints.RemoteServices;
public class ServiceEndpointUriResolver : IServiceEndpointUriResolver
{
  public Result<string> Resolve(IServiceRequestMarker req)
  {
    var requestType = req.GetType();
    return ResolveInternal(requestType);
  }

  public Result<string> Resolve<TRequest>() where TRequest : IServiceRequestMarker
  {
    var requestType = typeof(TRequest);
    return ResolveInternal(requestType);
  }

  private Result<string> ResolveInternal(Type requestType)
  {
    var requestName = requestType.FullName;
    if (string.IsNullOrWhiteSpace(requestName))
    {
      return Result<string>.CriticalError("Cannot resolve request uri for service endpoint.");
    }
    return $"{requestName}.Endpoint";
  }
}
