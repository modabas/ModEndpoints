using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ModEndpoints.Core;
using ModEndpoints.RemoteServices;

namespace ModEndpoints;

public class DefaultUriResolverProvider :
  IUriResolverProvider
{
  public IServiceEndpointUriResolver GetResolver(
    IServiceProvider serviceProvider,
    IRouteGroupConfigurator? parentRouteGroup,
    ServiceEndpointConfigurator endpoint)
  {
    var resolverName = GetResolverName(endpoint);
    var resolver = serviceProvider.GetRequiredKeyedService<IServiceEndpointUriResolver>(resolverName);
    return resolver;
  }

  private string GetResolverName(ServiceEndpointConfigurator endpoint)
  {
    return endpoint.GetType().GetCustomAttribute<UriResolverAttribute>()?.Name ??
      RemoteServiceDefinitions.DefaultUriResolverName;
  }
}
