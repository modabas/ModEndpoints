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
    Action<ModEndpointsOptions>? configure = null)
  {
    return services.AddModEndpointsFromAssembly(typeof(T).Assembly, configure);
  }

  public static IServiceCollection AddModEndpointsFromAssembly(
    this IServiceCollection services,
    Assembly assembly,
    Action<ModEndpointsOptions>? configure = null)
  {
    ModEndpointsOptions options = new();
    configure?.Invoke(options);

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
      RemoteServiceDefinitions.DefaultUriResolverName);
    services.TryAddSingleton<IUriResolverProvider, DefaultUriResolverProvider>();

    services.AddHttpContextAccessor();
    return services.AddModEndpointsCoreFromAssembly(
      assembly,
      conf =>
      {
        conf.EndpointLifetime = options.CoreOptions.EndpointLifetime;
        conf.RouteGroupConfiguratorLifetime = options.CoreOptions.RouteGroupConfiguratorLifetime;
        conf.UseDefaultRequestValidation = options.CoreOptions.UseDefaultRequestValidation;
      });
  }

  public static WebApplication MapModEndpoints(
    this WebApplication app,
    Action<RouteHandlerBuilder, ConfigurationContext<EndpointConfigurationParameters>>? globalEndpointConfiguration = null,
    bool throwOnMissingConfiguration = false)
  {
    return app.MapModEndpointsCore(
      globalEndpointConfiguration,
      throwOnMissingConfiguration);
  }
}
