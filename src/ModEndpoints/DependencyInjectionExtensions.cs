using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ModEndpoints.Core;
using ModEndpoints.RemoteServices;
using ModEndpoints.RemoteServices.Core;

namespace ModEndpoints;
public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddModEndpointsFromAssembly(
    this IServiceCollection services,
    Assembly assembly)
  {
    services.TryAddKeyedSingleton<IResultToResponseMapper, DefaultResultToResponseMapper>(
      WebResultEndpointDefinitions.DefaultResultToResponseMapperName);
    services.TryAddScoped<ILocationStore, DefaultLocationStore>();
    services.TryAddSingleton<IResultToResponseMapProvider, DefaultResultToResponseMapProvider>();
    services.TryAddTransient<IServiceEndpointUriResolver, ServiceEndpointUriResolver>();
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
