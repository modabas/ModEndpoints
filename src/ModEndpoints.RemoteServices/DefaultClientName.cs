using ModEndpoints.RemoteServices.Core;

namespace ModEndpoints.RemoteServices;
internal class DefaultClientName
{
  public static string Resolve(IServiceRequestMarker request)
  {
    var requestType = request.GetType();
    return ResolveInternal(requestType);
  }

  public static string Resolve<TRequest>()
    where TRequest : IServiceRequestMarker
  {
    var requestType = typeof(TRequest);
    return ResolveInternal(requestType);
  }

  private static string ResolveInternal(Type requestType)
  {
    var requestName = requestType.AssemblyQualifiedName;
    if (string.IsNullOrWhiteSpace(requestName))
    {
      throw new ArgumentException("Request type should not be generic.", nameof(requestType));
    }
    return $"{requestName}.Client";
  }
}
