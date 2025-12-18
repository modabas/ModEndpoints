using ModEndpoints.RemoteServices.Contracts;

namespace ModEndpoints.RemoteServices;

internal sealed class ServiceClientNameResolver
{
  private const string InvalidRequestType = "Request type should not be generic type parameter.";
  public static string GetDefaultName(IServiceRequestMarker request)
  {
    var requestType = request.GetType();
    return GetDefaultNameInternal(requestType);
  }

  public static string GetDefaultName<TRequest>()
    where TRequest : IServiceRequestMarker
  {
    var requestType = typeof(TRequest);
    return GetDefaultNameInternal(requestType);
  }

  private static string GetDefaultNameInternal(Type requestType)
  {
    var requestName = requestType.AssemblyQualifiedName;
    if (string.IsNullOrWhiteSpace(requestName))
    {
      throw new ArgumentException(InvalidRequestType, nameof(requestType));
    }
    return $"{requestName}.Client";
  }
}
