using System.Text.Json;
using System.Text.Json.Serialization;

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

  internal static readonly JsonSerializerOptions DefaultJsonDeserializationOptionsForStreamingResponse = new()
  {
    PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    NumberHandling = JsonNumberHandling.AllowReadingFromString,
    DefaultBufferSize = 128,
  };
}
