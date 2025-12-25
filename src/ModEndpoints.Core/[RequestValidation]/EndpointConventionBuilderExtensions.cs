using Microsoft.AspNetCore.Builder;

namespace ModEndpoints.Core;

public static partial class EndpointConventionBuilderExtensions
{
  extension<TBuilder>(TBuilder builder) where TBuilder : IEndpointConventionBuilder
  {
    /// <summary>
    /// Enables request validation for the endpoint using the specified validation service.
    /// </summary>
    /// <remarks>Use this method to require that incoming requests to the endpoint are validated according to
    /// the rules defined by the specified validation service. This can help ensure that requests meet expected criteria
    /// before further processing. Has no effect if request validation is globally turned off.</remarks>
    /// <param name="validationServiceName">The name of the validation service to use for request validation. If not specified, the default validation
    /// service is used.</param>
    /// <returns>The current builder instance with request validation enabled.</returns>
    public TBuilder ValidateRequestWithOptions(string? validationServiceName = null)
    {
      builder.WithMetadata(new RequestValidationMetadata(IsEnabled: true, ServiceName: validationServiceName));
      return builder;
    }
    /// <summary>
    /// Disables request validation for the endpoint being configured.
    /// </summary>
    /// <remarks>Request validation prevents potentially dangerous input from being processed by the endpoint.
    /// Disabling it may expose the application to security risks and should only be done when necessary, such as for
    /// endpoints that intentionally accept raw input.</remarks>
    /// <returns>The builder instance with request validation disabled, allowing further configuration.</returns>
    public TBuilder DisableRequestValidation()
    {
      builder.WithMetadata(new RequestValidationMetadata(IsEnabled: false));
      return builder;
    }
  }
}
