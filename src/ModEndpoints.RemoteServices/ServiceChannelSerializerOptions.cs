using System.Text.Json;

namespace ModEndpoints.RemoteServices;
public class ServiceChannelSerializerOptions
{
  public JsonSerializerOptions? SerializationOptions { get; set; }
  public JsonSerializerOptions? DeserializationOptions { get; set; }
}
