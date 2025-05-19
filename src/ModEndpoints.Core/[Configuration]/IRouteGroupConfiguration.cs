using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace ModEndpoints.Core;

public interface IRouteGroupConfiguration
{
  IRouteGroupConfiguration? ParentRouteGroup { get; }

  Dictionary<string, object?> ConfigurationPropertyBag { get; }

  /// <summary>
  /// Endpoint configuration overrides. This executes after all child endpoints have been configured, the global endpoint configuration has completed, and any endpoint configuration overrides have run.
  /// </summary>
  abstract Action<RouteHandlerBuilder, ConfigurationContext<IEndpointConfiguration>>? EndpointConfigurationOverrides { get; }
  /// <summary>
  /// Group configuration overrides, runs after all child groups and endpoints have been fully configured.
  /// </summary>
  abstract Action<RouteGroupBuilder, ConfigurationContext<IRouteGroupConfiguration>>? ConfigurationOverrides { get; }
}
