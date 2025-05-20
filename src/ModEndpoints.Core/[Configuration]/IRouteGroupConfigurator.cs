using Microsoft.AspNetCore.Routing;

namespace ModEndpoints.Core;

public interface IRouteGroupConfigurator : IRouteGroupConfigurationSettings
{
  RouteGroupBuilder? Configure(IEndpointRouteBuilder builder, ConfigurationContext<IRouteGroupConfigurationSettings> configurationContext);

}
