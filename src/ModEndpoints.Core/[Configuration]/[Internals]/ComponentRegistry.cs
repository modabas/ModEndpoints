using System.Diagnostics.CodeAnalysis;

namespace ModEndpoints.Core;

internal sealed class ComponentRegistry
{
  private readonly Dictionary<Type, Type> _routeGroups = new();
  private readonly Dictionary<Type, Type> _endpoints = new();
  private RequestValidationOptions? _validationOptions;
  private bool _requestValidationOptionsConflict = false;

  public bool TrySetValidationOptions(RequestValidationOptions validationOptions)
  {
    if (_validationOptions == null)
    {
      _validationOptions = validationOptions;
      return true;
    }
    if (!validationOptions.Equals(_validationOptions))
    {
      _requestValidationOptionsConflict = true;
    }
    return false;
  }

  public bool GetRequestValitionOptionsConflict([NotNullWhen(true)] out RequestValidationOptions? configuredOptions)
  {
    configuredOptions = _validationOptions;
    return _requestValidationOptionsConflict;
  }

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
