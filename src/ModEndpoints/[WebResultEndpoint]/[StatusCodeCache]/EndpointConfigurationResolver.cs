using Microsoft.AspNetCore.Http;
using ModEndpoints.Core;

namespace ModEndpoints;

internal sealed class EndpointConfigurationResolver : IEndpointConfigurationResolver
{
  public string? GetUniqueName(Endpoint endpoint)
  {
    //FirstOrDefault to get the first added metadata (in case of multiple)
    var list = endpoint.Metadata.GetOrderedMetadata<EndpointConfigurationMetadata>();
    if (list.Count > 0)
    {
      return list[0].EndpointUniqueName;
    }
    return null;
  }
}
