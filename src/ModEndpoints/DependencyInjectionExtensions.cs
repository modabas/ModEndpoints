using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ModEndpoints.Core;
using ModEndpoints.RemoteServices;

namespace ModEndpoints;
public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddModEndpointsFromAssembly(
    this IServiceCollection services,
    Assembly assembly)
  {
    //WebResultEndpoint components
    services.TryAddKeyedSingleton<IResultToResponseMapper, DefaultResultToResponseMapper>(
      WebResultEndpointDefinitions.DefaultResultToResponseMapperName);
    services.TryAddScoped<ILocationStore, DefaultLocationStore>();
    services.TryAddSingleton<IResultToResponseMapProvider, DefaultResultToResponseMapProvider>();

    //ServiceEndpoint components
    services.TryAddKeyedSingleton<IServiceEndpointUriResolver, ServiceEndpointUriResolver>(
      ServiceEndpointDefinitions.DefaultUriResolverName);
    services.TryAddSingleton<IUriResolverProvider, DefaultUriResolverProvider>();

    services.AddHttpContextAccessor();
    return services.AddModEndpointsFromAssemblyCore(assembly);
  }

  public static WebApplication MapModEndpoints(
    this WebApplication app,
    Action<IServiceProvider, RouteHandlerBuilder, IRouteGroupConfigurator?, IEndpointConfigurator>? globalEndpointConfiguration = null,
    bool throwOnMissingConfiguration = false)
  {
    return app.MapModEndpointsCore(
      globalEndpointConfiguration,
      throwOnMissingConfiguration);
  }
}
