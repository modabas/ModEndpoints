using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ModEndpoints.Core;
using ModEndpoints.RemoteServices;

namespace ModEndpoints;
public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddModEndpointsFromAssemblyContaining<T>(
    this IServiceCollection services,
    ServiceLifetime lifetime = ServiceLifetime.Transient)
  {
    return services.AddModEndpointsFromAssembly(typeof(T).Assembly, lifetime);
  }

  public static IServiceCollection AddModEndpointsFromAssembly(
    this IServiceCollection services,
    Assembly assembly,
    ServiceLifetime lifetime = ServiceLifetime.Transient)
  {
    //WebResultEndpoint components
    services.TryAddKeyedSingleton<IResultToResponseMapper, DefaultResultToResponseMapper>(
      WebResultEndpointDefinitions.DefaultResultToResponseMapperName);
    services.TryAddKeyedSingleton<IPreferredSuccessStatusCodeCache, DefaultPreferredSuccessStatusCodeCacheForResult>(
      WebResultEndpointDefinitions.DefaultPreferredSuccessStatusCodeCacheNameForResult);
    services.TryAddKeyedSingleton<IPreferredSuccessStatusCodeCache, DefaultPreferredSuccessStatusCodeCacheForResultOfT>(
      WebResultEndpointDefinitions.DefaultPreferredSuccessStatusCodeCacheNameForResultOfT);
    services.TryAddScoped<ILocationStore, DefaultLocationStore>();
    services.TryAddSingleton<IResultToResponseMapProvider, DefaultResultToResponseMapProvider>();

    //ServiceEndpoint components
    services.TryAddKeyedSingleton<IServiceEndpointUriResolver, DefaultServiceEndpointUriResolver>(
      ServiceEndpointDefinitions.DefaultUriResolverName);
    services.TryAddSingleton<IUriResolverProvider, DefaultUriResolverProvider>();

    services.AddHttpContextAccessor();
    return services.AddModEndpointsCoreFromAssembly(assembly, lifetime);
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
