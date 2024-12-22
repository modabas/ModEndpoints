using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace ModEndpoints.Core;

public interface IEndpointConfigurator
{
  RouteHandlerBuilder? Configure(IServiceProvider serviceProvider, IEndpointRouteBuilder builder, IRouteGroupConfigurator? parentRouteGroup);

  Dictionary<string, object?>? PropertyBag { get; set; }

  /// <summary>
  /// Parameters: Endpoint's parent group (if any or null) and endpoint being configured (this endpoint)
  /// </summary>
  abstract Action<IServiceProvider, RouteHandlerBuilder, IRouteGroupConfigurator?, IEndpointConfigurator>? ConfigurationOverrides { get; }
}
