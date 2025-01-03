using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using ModEndpoints.RemoteServices.Core;

namespace ModEndpoints.RemoteServices;
public class ServiceChannelRegistry
{
  private readonly ConcurrentDictionary<Type, string> _registry;

  private static Lazy<ServiceChannelRegistry> _instance =
    new Lazy<ServiceChannelRegistry>(
      () => new ServiceChannelRegistry(),
      LazyThreadSafetyMode.ExecutionAndPublication);

  public static ServiceChannelRegistry Instance => _instance.Value;

  private ServiceChannelRegistry()
  {
    _registry = new();
  }

  public bool IsRegistered<TRequest>()
    where TRequest : IServiceRequestMarker
  {
    return _registry.ContainsKey(typeof(TRequest));
  }

  public bool IsRegistered(Type requestType)
  {
    return _registry.ContainsKey(requestType);
  }

  public bool IsRegistered(string clientName)
  {
    return _registry.Values.Any(c => c.Equals(clientName, StringComparison.OrdinalIgnoreCase));
  }

  internal bool Register<TRequest>(string clientName)
    where TRequest : IServiceRequestMarker
  {
    return _registry.TryAdd(typeof(TRequest), clientName);
  }

  internal bool Register(Type requestType, string clientName)
  {
    return _registry.TryAdd(requestType, clientName);
  }

  public bool IsRegistered<TRequest>([NotNullWhen(true)] out string? clientName)
    where TRequest : IServiceRequestMarker
  {
    return _registry.TryGetValue(typeof(TRequest), out clientName);
  }
}
