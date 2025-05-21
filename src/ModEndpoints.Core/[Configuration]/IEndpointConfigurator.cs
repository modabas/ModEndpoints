using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace ModEndpoints.Core;

public interface IEndpointConfiguratorMarker;

public interface IEndpointConfigurator : IEndpointConfiguratorMarker
{
  RouteHandlerBuilder[] Configure(IEndpointRouteBuilder builder, ConfigurationContext<EndpointConfigurationParameters> configurationContext);

  void PostConfigure(RouteHandlerBuilder builder, ConfigurationContext<EndpointConfigurationParameters> configurationContext);
}
