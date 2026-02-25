using Microsoft.AspNetCore.Http;

namespace ModEndpoints;

internal interface IEndpointConfigurationResolver
{
  string? GetUniqueName(Endpoint endpoint);
}
