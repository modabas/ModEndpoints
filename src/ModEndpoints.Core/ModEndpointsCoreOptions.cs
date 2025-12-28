using Microsoft.Extensions.DependencyInjection;

namespace ModEndpoints.Core;

public class ModEndpointsCoreOptions
{
  public ServiceLifetime EndpointLifetime { get; set; } = ServiceLifetime.Transient;
  public ServiceLifetime RouteGroupConfiguratorLifetime { get; set; } = ServiceLifetime.Transient;
  public bool EnableRequestValidation { get; set; } = true;
  public bool EnablePerEndpointRequestValidationCustomization { get; set; } = false;
  public string RequestValidationServiceName { get; set; } = RequestValidationDefinitions.DefaultServiceName;
  public bool ThrowOnDuplicateUseOfServiceEndpointRequest { get; set; } = true;
}
