using ModEndpoints.RemoteServices.Core;

namespace ModEndpoints.RemoteServices;
public class ServiceEndpointUriResolver : IServiceEndpointUriResolver
{
  public string? Resolve(IServiceRequestMarker req)
  {
    var requestType = req.GetType();
    return ResolveInternal(requestType);
  }

  public string? Resolve<TRequest>() where TRequest : IServiceRequestMarker
  {
    var requestType = typeof(TRequest);
    return ResolveInternal(requestType);
  }

  private string? ResolveInternal(Type requestType)
  {
    var requestName = requestType.FullName;
    if (string.IsNullOrWhiteSpace(requestName))
    {
      return null;
    }
    return $"{requestName}.Endpoint";
  }
}
