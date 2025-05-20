using Microsoft.AspNetCore.Builder;

namespace ModEndpoints.Core;

public interface IEndpointConfigurationSettings
{
  Dictionary<string, object?>? ConfigurationPropertyBag { get; set; }

  /// <summary>
  /// Endpoint configuration overrides. This executes after endpoint has been configured and the global endpoint configuration has completed.
  /// </summary>
  abstract Action<RouteHandlerBuilder, ConfigurationContext<IEndpointConfigurationSettings>>? ConfigurationOverrides { get; }
}
