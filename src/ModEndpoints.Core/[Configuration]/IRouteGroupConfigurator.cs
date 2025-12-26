using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace ModEndpoints.Core;

public interface IRouteGroupConfiguratorMarker;

public interface IRouteGroupConfigurator : IRouteGroupConfiguratorMarker
{
  RouteGroupBuilder[] Configure(IEndpointRouteBuilder builder, RouteGroupConfigurationContext configurationContext);

  /// <summary>
  /// Endpoint post configuration executes for each endpoint directly under this route group, after endpoint has been configured, the global endpoint configuration has completed, and endpoint's own post configuration have run.
  /// </summary>
  void EndpointPostConfigure(RouteHandlerBuilder builder, EndpointConfigurationContext configurationContext);

  /// <summary>
  /// Group post configuration runs after all child groups and endpoints have been fully configured.
  /// </summary>
  void PostConfigure(RouteGroupBuilder builder, RouteGroupConfigurationContext configurationContext);

}
