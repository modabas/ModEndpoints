using Microsoft.AspNetCore.Builder;

namespace ModEndpoints.Core;

public static partial class EndpointConventionBuilderExtensions
{
  extension<TBuilder>(TBuilder builder) where TBuilder : IEndpointConventionBuilder
  {
    internal TBuilder AddConfigurationMetadata()
    {
      builder.Add(b => b.Metadata.Add(
        new EndpointConfigurationMetadata(
          EndpointUniqueName: b.DisplayName)));
      return builder;
    }
  }
}
