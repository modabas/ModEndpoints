using System.Collections.Concurrent;

namespace ModEndpoints.Core;

internal sealed class ComponentRegistry
{
  private readonly ConcurrentDictionary<Type, Type> _routeGroups = new();
  private readonly ConcurrentDictionary<Type, Type> _endpoints = new();

  public void TryAddRouteGroups(IEnumerable<Type> implementationTypes)
  {
    foreach (var type in implementationTypes)
    {
      TryAddRouteGroup(type);
    }
  }

  private void TryAddRouteGroup(Type implementationType)
  {
    _routeGroups.TryAdd(implementationType, implementationType);
  }

  public void AddEndpoint(Type key, Type implementationType)
  {
    _endpoints[key] = implementationType;
  }

  public void TryAddEndpoint(Type key, Type implementationType)
  {
    _endpoints.TryAdd(key, implementationType);
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
