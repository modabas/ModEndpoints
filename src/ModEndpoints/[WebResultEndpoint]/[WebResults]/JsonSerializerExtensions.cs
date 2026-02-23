using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization.Metadata;

namespace ModEndpoints;

internal static class JsonSerializerExtensions
{
  extension(JsonTypeInfo jsonTypeInfo)
  {
    public bool HasKnownPolymorphism()
   => jsonTypeInfo.Type.IsSealed || jsonTypeInfo.Type.IsValueType || jsonTypeInfo.PolymorphismOptions is not null;

    public bool ShouldUseWith([NotNullWhen(false)] Type? runtimeType)
     => runtimeType is null || jsonTypeInfo.Type == runtimeType || jsonTypeInfo.HasKnownPolymorphism();
  }
}
