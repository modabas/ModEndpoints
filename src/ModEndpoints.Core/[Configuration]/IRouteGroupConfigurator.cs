using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace ModEndpoints.Core;

public interface IRouteGroupConfigurator : IRouteGroupConfiguration
{
  RouteGroupBuilder? Configure(IEndpointRouteBuilder builder, ConfigurationContext<IRouteGroupConfiguration> configurationContext);

}
