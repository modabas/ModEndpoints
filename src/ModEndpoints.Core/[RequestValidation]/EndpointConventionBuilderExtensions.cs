using Microsoft.AspNetCore.Builder;

namespace ModEndpoints.Core;

public static partial class EndpointConventionBuilderExtensions
{
  extension<TBuilder>(TBuilder builder) where TBuilder : IEndpointConventionBuilder
  {
    /// <summary>
    /// Disables request validation for the associated route handler.
    /// </summary>
    /// <remarks>Disabling request validation allows requests with potentially unsafe content to reach the
    /// endpoint. Use this method only when you trust the input source or have implemented alternative validation
    /// mechanisms.</remarks>
    /// <returns>The current <see cref="IEndpointConventionBuilder"/> instance for method chaining.</returns>
    public TBuilder DisableRequestValidation()
    {
      builder.WithMetadata(new RequestValidationEndpointMetadata(IsEnabled: false));
      return builder;
    }

    /// <summary>
    /// Enables request validation for the endpoint and optionally specifies the name of the request validation service
    /// to use.
    /// </summary>
    /// <remarks>Call this method to add request validation metadata to the endpoint. This allows ModEndpoints infrastructure
    /// to perform validation using the specified service before the endpoint is executed.</remarks>
    /// <param name="requestValidationServiceName">The name of the request validation service to associate with the endpoint. If null, the default validation
    /// service is used.</param>
    /// <returns>The current <see cref="IEndpointConventionBuilder"/> instance for method chaining.</returns>
    public TBuilder EnableRequestValidation(string? requestValidationServiceName = null)
    {
      builder.WithMetadata(new RequestValidationEndpointMetadata(IsEnabled: true, ServiceName: requestValidationServiceName));
      return builder;
    }
  }

}
