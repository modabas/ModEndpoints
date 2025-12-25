using Microsoft.AspNetCore.Http;

namespace ModEndpoints.Core;

internal sealed class DefaultEndpointNameResolver : IEndpointNameResolver
{
  public string? GetName(Endpoint endpoint)
  {
    string? endpointName = null;
    //FirstOrDefault to get the first added metadata (in case of multiple)
    var list = endpoint.Metadata.GetOrderedMetadata<EndpointConfigurationMetadata>();
    if (list is not null && list.Count > 0)
    {
      endpointName = list[0].EndpointUniqueName;
    }
    if (string.IsNullOrWhiteSpace(endpointName))
    {
      return endpoint.ToString();
    }
    return endpointName;
  }
}
