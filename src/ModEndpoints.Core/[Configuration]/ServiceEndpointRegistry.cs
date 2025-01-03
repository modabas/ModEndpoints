using System.Collections.Concurrent;

namespace ModEndpoints.Core;
public class ServiceEndpointRegistry
{
  private readonly ConcurrentDictionary<Type, Type> _registry;

  private static readonly Lazy<ServiceEndpointRegistry> _instance =
    new Lazy<ServiceEndpointRegistry>(
      () => new ServiceEndpointRegistry(),
      LazyThreadSafetyMode.ExecutionAndPublication);

  public static ServiceEndpointRegistry Instance => _instance.Value;

  private ServiceEndpointRegistry()
  {
    _registry = new();
  }

  public bool IsRegistered(Type requestType)
  {
    return _registry.ContainsKey(requestType);
  }

  internal bool Register(Type requestType, Type endpointType)
  {
    return _registry.TryAdd(requestType, endpointType);
  }
}
