using System.Collections.Concurrent;

namespace ModEndpoints.Core;

internal class ComponentRegistry
{
  private readonly ConcurrentDictionary<Type, Type> _routeGroups;
  private readonly ConcurrentDictionary<Type, Type> _endpoints;

  private static Lazy<ComponentRegistry> _instance =
    new Lazy<ComponentRegistry>(
      () => new ComponentRegistry(),
      LazyThreadSafetyMode.ExecutionAndPublication);

  public static ComponentRegistry Instance => _instance.Value;

  private ComponentRegistry()
  {
    _routeGroups = new();
    _endpoints = new();
  }

  public void TryAddRouteGroups(IEnumerable<Type> implementationTypes)
  {
    foreach (var type in implementationTypes)
    {
      _routeGroups[type] = type;
    }
  }

  public void TryAddEndpoint(Type key, Type implementationType)
  {
    _endpoints[key] = implementationType;
  }

  public ICollection<Type> GetRouteGroups()
  {
    return _routeGroups.Values;
  }

  public ICollection<Type> GetEndpoints()
  {
    return _endpoints.Values;
  }
}
