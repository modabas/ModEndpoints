using System.Text.Json;
using System.Text.Json.Serialization;

namespace ModEndpoints.RemoteServices;

public static class RemoteServiceDefinitions
{
  public const string DefaultUriResolverName = "Default";
  public const string DefaultSerializerName = "Default";

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
