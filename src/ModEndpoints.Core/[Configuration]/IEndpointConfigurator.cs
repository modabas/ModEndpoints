using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace ModEndpoints.Core;

public interface IEndpointConfigurator : IEndpointConfiguration
{
  RouteHandlerBuilder? Configure(IEndpointRouteBuilder builder, ConfigurationContext<IEndpointConfiguration> configurationContext);
}
