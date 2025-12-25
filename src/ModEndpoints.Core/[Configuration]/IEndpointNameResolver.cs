using Microsoft.AspNetCore.Http;

namespace ModEndpoints.Core;

public interface IEndpointNameResolver
{
  string? GetName(Endpoint endpoint);
}
