using Microsoft.AspNetCore.Builder;

namespace ModEndpoints.Core;

public interface IEndpointConfiguration
{
  Dictionary<string, object?>? ConfigurationPropertyBag { get; }

  /// <summary>
  /// Endpoint configuration overrides. This executes after endpoint has been configured and the global endpoint configuration has completed.
  /// </summary>
  abstract Action<RouteHandlerBuilder, ConfigurationContext<IEndpointConfiguration>>? ConfigurationOverrides { get; }
}
