using Microsoft.Extensions.DependencyInjection;

namespace ModEndpoints.Core;

public class ModEndpointsCoreOptions
{
  public ServiceLifetime EndpointLifetime { get; set; } = ServiceLifetime.Transient;
  public ServiceLifetime RouteGroupConfiguratorLifetime { get; set; } = ServiceLifetime.Transient;

  /// <summary>
  /// Enables/disables request validation performed by ModEndpoints infrastructure.
  /// Disabling request validation allows requests with potentially unsafe content to reach the
  /// endpoint. Use this method only when you trust the input source or have implemented alternative validation
  /// mechanisms.
  /// </summary>
  public bool EnableRequestValidation { get; set; } = true;

  /// <summary>
  /// Enables/disables the capacity to customize request validation on endpoint basis via configuration extension methods.
  /// </summary>
  public bool EnablePerEndpointRequestValidationCustomization { get; set; } = false;

  /// <summary>
  /// Sets service name to be used for request validation. This is the DI key used during service registry.
  /// </summary>
  public string RequestValidationServiceName { get; set; } = RequestValidationDefinitions.DefaultServiceName;
  public bool ThrowOnDuplicateUseOfServiceEndpointRequest { get; set; } = true;
}
