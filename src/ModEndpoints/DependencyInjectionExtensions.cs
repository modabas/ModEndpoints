using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ModEndpoints.Core;
using ModEndpoints.RemoteServices;

namespace ModEndpoints;

public static class DependencyInjectionExtensions
{
  extension(IServiceCollection services)
  {
    /// <summary>
    /// Registers route groups and endpoints from the assembly containing the specified type into the dependency injection container. 
    /// </summary>
    /// <remarks>This method scans the assembly containing the specified type for components and registers them with the
    /// dependency injection container.</remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="configure">An optional delegate to configure <see cref="ModEndpointsCoreOptions"/> for customizing the behavior of the
    /// registration process.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
    public IServiceCollection AddModEndpointsFromAssemblyContaining<T>(
      Action<ModEndpointsOptions>? configure = null)
    {
      return services.AddModEndpointsFromAssembly(typeof(T).Assembly, configure);
    }

    /// <summary>
    /// Registers route groups and endpoints from the specified assembly into the dependency injection container.
    /// </summary>
    /// <remarks>This method scans the specified assembly for components and registers them with the
    /// dependency injection container.</remarks>
    /// <param name="assembly">The assembly containing components to be registered.</param>
    /// <param name="configure">An optional delegate to configure <see cref="ModEndpointsCoreOptions"/> for customizing the behavior of the
    /// registration process.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
    public IServiceCollection AddModEndpointsFromAssembly(
      Assembly assembly,
      Action<ModEndpointsOptions>? configure = null)
    {
      ModEndpointsOptions options = new();
      configure?.Invoke(options);

      //WebResultEndpoint components
      services.TryAddSingleton<IEndpointConfigurationResolver, EndpointConfigurationResolver>();
      services.TryAddKeyedSingleton<IPreferredSuccessStatusCodeCache, DefaultPreferredSuccessStatusCodeCacheForResult>(
        WebResultEndpointDefinitions.DefaultPreferredSuccessStatusCodeCacheNameForResult);
      services.TryAddKeyedSingleton<IPreferredSuccessStatusCodeCache, DefaultPreferredSuccessStatusCodeCacheForResultOfT>(
        WebResultEndpointDefinitions.DefaultPreferredSuccessStatusCodeCacheNameForResultOfT);

      //ServiceEndpoint components
      services.TryAddKeyedSingleton<IServiceEndpointUriResolver, DefaultServiceEndpointUriResolver>(
        RemoteServiceDefinitions.DefaultUriResolverName);
      services.TryAddSingleton<IUriResolverProvider, DefaultUriResolverProvider>();

      return services.AddModEndpointsCoreFromAssembly(
        assembly,
        conf =>
        {
          conf.EndpointLifetime = options.CoreOptions.EndpointLifetime;
          conf.RouteGroupConfiguratorLifetime = options.CoreOptions.RouteGroupConfiguratorLifetime;
          conf.EnableRequestValidation = options.CoreOptions.EnableRequestValidation;
          conf.EnablePerEndpointRequestValidationCustomization = options.CoreOptions.EnablePerEndpointRequestValidationCustomization;
          conf.RequestValidationServiceName = options.CoreOptions.RequestValidationServiceName;
          conf.ThrowOnDuplicateUseOfServiceEndpointRequest = options.CoreOptions.ThrowOnDuplicateUseOfServiceEndpointRequest;
        });
    }
  }

  extension(IEndpointRouteBuilder app)
  {
    /// <summary>
    /// Maps and configures route groups and endpoints. Configuration processing order:
    /// Group's Configuration -> Endpoint's Configuration -> Global Endpoint Configuration -> Endpoint's PostConfigure -> Group's EndpointPostConfigure -> Group's PostConfigure.
    /// Global Endpoint Configuration apply to all endpoints.
    /// Group's EndpointPostConfigure apply to endpoints directly under that group (not if they are under a child group of current group).
    /// </summary>
    /// <param name="globalEndpointConfiguration">Endpoint configuration to be applied to all endpoints.</param>
    /// <param name="throwOnMissingConfiguration"></param>
    /// <returns></returns>
    public IEndpointRouteBuilder MapModEndpoints(
      Action<RouteHandlerBuilder, EndpointConfigurationContext>? globalEndpointConfiguration = null,
      bool throwOnMissingConfiguration = false)
    {
      return app.MapModEndpointsCore(
        globalEndpointConfiguration,
        throwOnMissingConfiguration);
    }
  }
}
