using Microsoft.Extensions.DependencyInjection;

namespace ModEndpoints.Core;
public class ModEndpointsCoreOptions
{
  public ServiceLifetime EndpointLifetime { get; set; } = ServiceLifetime.Transient;
  public ServiceLifetime RouteGroupConfiguratorLifetime { get; set; } = ServiceLifetime.Transient;
  public bool UseDefaultRequestValidation { get; set; } = true;
  public bool ThrowOnDuplicateServiceEndpointRequest { get; set; } = true;
  public bool ThrowOnServiceEndpointRegistrationFailure { get; set; } = true;
}
