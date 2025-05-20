using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace ModEndpoints.Core;

public interface IRouteGroupConfiguratorMarker;

public interface IRouteGroupConfigurator : IRouteGroupConfiguratorMarker
{
  RouteGroupBuilder? Configure(IEndpointRouteBuilder builder, ConfigurationContext<RouteGroupConfigurationParameters> configurationContext);

  /// <summary>
  /// Endpoint configuration overrides. This executes for each endpoint directly under this route group, after endpoint has been configured, the global endpoint configuration has completed, and endpoint's own configuration overrides have run.
  /// </summary>
  abstract Action<RouteHandlerBuilder, ConfigurationContext<EndpointConfigurationParameters>>? EndpointConfigurationOverrides { get; }
  /// <summary>
  /// Group configuration overrides, runs after all child groups and endpoints have been fully configured.
  /// </summary>
  abstract Action<RouteGroupBuilder, ConfigurationContext<RouteGroupConfigurationParameters>>? ConfigurationOverrides { get; }
}
