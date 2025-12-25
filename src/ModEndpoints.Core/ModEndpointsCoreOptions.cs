using Microsoft.Extensions.DependencyInjection;

namespace ModEndpoints.Core;

public class ModEndpointsCoreOptions
{
  public ServiceLifetime EndpointLifetime { get; set; } = ServiceLifetime.Transient;
  public ServiceLifetime RouteGroupConfiguratorLifetime { get; set; } = ServiceLifetime.Transient;
  public bool EnableRequestValidation { get; set; } = true;
  public string DefaultRequestValidationServiceName { get; set; } = RequestValidationDefinitions.DefaultServiceName;
  public bool ThrowOnDuplicateUseOfServiceEndpointRequest { get; set; } = true;
}
