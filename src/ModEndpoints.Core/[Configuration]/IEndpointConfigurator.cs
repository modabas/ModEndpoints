using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace ModEndpoints.Core;

public interface IEndpointConfiguratorMarker;

public interface IEndpointConfigurator : IEndpointConfiguratorMarker
{
  RouteHandlerBuilder? Configure(IEndpointRouteBuilder builder, ConfigurationContext<EndpointConfigurationParameters> configurationContext);

  /// <summary>
  /// Endpoint configuration overrides. This executes after endpoint has been configured and the global endpoint configuration has completed.
  /// </summary>
  abstract Action<RouteHandlerBuilder, ConfigurationContext<EndpointConfigurationParameters>>? ConfigurationOverrides { get; }
}
