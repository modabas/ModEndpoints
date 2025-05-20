using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace ModEndpoints.Core;

public interface IEndpointConfigurator : IEndpointConfigurationSettings
{
  RouteHandlerBuilder? Configure(IEndpointRouteBuilder builder, ConfigurationContext<IEndpointConfigurationSettings> configurationContext);

  /// <summary>
  /// Endpoint configuration overrides. This executes after endpoint has been configured and the global endpoint configuration has completed.
  /// </summary>
  abstract Action<RouteHandlerBuilder, ConfigurationContext<IEndpointConfigurationSettings>>? ConfigurationOverrides { get; }
}
