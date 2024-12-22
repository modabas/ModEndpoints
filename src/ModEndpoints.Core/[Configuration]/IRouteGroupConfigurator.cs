using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace ModEndpoints.Core;

public interface IRouteGroupConfigurator
{
  RouteGroupBuilder? Configure(IServiceProvider serviceProvider, IEndpointRouteBuilder builder, IRouteGroupConfigurator? parentRouteGroup);

  Dictionary<string, object?> PropertyBag { get; }

  /// <summary>
  /// Parameters: Endpoint's parent group (this group) and endpoint being configured 
  /// </summary>
  abstract Action<IServiceProvider, RouteHandlerBuilder, IRouteGroupConfigurator, IEndpointConfigurator>? EndpointConfigurationOverrides { get; }
  /// <summary>
  /// Parameters: Group's parent group (if any or null) and group being configured (this group)
  /// </summary>
  abstract Action<IServiceProvider, RouteGroupBuilder, IRouteGroupConfigurator?, IRouteGroupConfigurator>? ConfigurationOverrides { get; }
}
