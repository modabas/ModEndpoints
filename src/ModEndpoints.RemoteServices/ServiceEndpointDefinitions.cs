using System.Text.Json.Serialization;
using System.Text.Json;

namespace ModEndpoints.RemoteServices;
public static class ServiceEndpointDefinitions
{
  public const string DefaultUriResolverName = "DefaultServiceEndpointUriResolver";
  public const string DefaultSerializerName = "DefaultServiceChannelSerializer";

  internal static readonly JsonSerializerOptions DefaultJsonDeserializationOptions = new()
  {
    PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    NumberHandling = JsonNumberHandling.AllowReadingFromString
  };
}
