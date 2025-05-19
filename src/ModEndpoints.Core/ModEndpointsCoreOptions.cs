using Microsoft.Extensions.DependencyInjection;

namespace ModEndpoints.Core;
public class ModEndpointsCoreOptions
{
  public ServiceLifetime ServiceLifetime { get; set; } = ServiceLifetime.Transient;
  public bool UseDefaultRequestValidation { get; set; } = true;
}
