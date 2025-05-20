using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace ModEndpoints.Core;

public interface IEndpointConfigurator : IEndpointConfigurationSettings
{
  RouteHandlerBuilder? Configure(IEndpointRouteBuilder builder, ConfigurationContext<IEndpointConfigurationSettings> configurationContext);
}
