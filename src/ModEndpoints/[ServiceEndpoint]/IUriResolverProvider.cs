using ModEndpoints.Core;
using ModEndpoints.RemoteServices;

namespace ModEndpoints;

/// <summary>
/// Helper methods for service endpoint uri resolvers.
/// </summary>
public interface IUriResolverProvider
{
  /// <summary>
  /// Gets the uri resolver for a particular <see cref="ServiceEndpointConfigurator"/> implementation,
  /// that will be used to determine the uri, endpoint will be mapped to. 
  /// </summary>
  /// <param name="serviceProvider"></param>
  /// <param name="endpoint"></param>
  /// <returns></returns>
  IServiceEndpointUriResolver GetResolver(IServiceProvider serviceProvider, ServiceEndpointConfigurator endpoint);
}
