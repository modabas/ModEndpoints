using Microsoft.AspNetCore.Http;

namespace ModEndpoints;

internal interface IEndpointNameResolver
{
  string? GetName(Endpoint endpoint);
}
