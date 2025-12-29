using Microsoft.AspNetCore.Builder;

namespace ModEndpoints.Core;

public static partial class EndpointConventionBuilderExtensions
{
  extension<TBuilder>(TBuilder builder) where TBuilder : IEndpointConventionBuilder
  {
    /// <summary>
    /// Disables request validation for a specific endpoint or all endpoints within a route group.
    /// </summary>
    /// <remarks>Disabling request validation allows requests with potentially unsafe content to reach the
    /// endpoint. Use this method only when you trust the input source or have implemented alternative validation
    /// mechanisms.
    /// <br/>Requires `EnablePerEndpointRequestValidationCustomization` option to be set to true.
    /// </remarks>
    /// <returns>The current <see cref="IEndpointConventionBuilder"/> instance for method chaining.</returns>
    public TBuilder DisableRequestValidation()
    {
      builder.WithMetadata(new RequestValidationEndpointMetadata(IsEnabled: false));
      return builder;
    }

    /// <summary>
    /// Enables request validation for a specific endpoint or all endpoints within a route group, and optionally specifies the name of the request validation service
    /// to use.
    /// </summary>
    /// <remarks>Call this method to add request validation metadata to the endpoint. This allows ModEndpoints infrastructure
    /// to perform validation using the specified service before the endpoint is executed.
    /// <br/>Requires `EnablePerEndpointRequestValidationCustomization` option to be set to true.
    /// </remarks>
    /// <param name="serviceName">The name of the request validation service to associate with the endpoint. If null, the default validation
    /// service is used.</param>
    /// <returns>The current <see cref="IEndpointConventionBuilder"/> instance for method chaining.</returns>
    public TBuilder EnableRequestValidation(string? serviceName = null)
    {
      builder.WithMetadata(new RequestValidationEndpointMetadata(IsEnabled: true, ServiceName: serviceName));
      return builder;
    }
  }

}
