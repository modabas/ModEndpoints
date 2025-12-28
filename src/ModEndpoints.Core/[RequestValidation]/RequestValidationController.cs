using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ModEndpoints.Core;

internal sealed class RequestValidationController : IRequestValidationController
{
  private readonly RequestValidationOptions _options;
  private const string ServiceNotRegistered =
    "Request validation service with name '{0}' is not registered.";
  private static readonly RequestValidationEndpointMetadata _defaultEndpointMetadata = new(IsEnabled: true);

  public RequestValidationController(
    IOptions<RequestValidationOptions> options)
  {
    _options = options.Value;
  }

  private RequestValidationEndpointMetadata GetEndpointMetadata(HttpContext context)
  {
    return context.GetEndpoint()?.Metadata.GetMetadata<RequestValidationEndpointMetadata>() ??
      _defaultEndpointMetadata;
  }

  public async Task<RequestValidationResult?> ValidateAsync<TRequest>(
    TRequest req,
    HttpContext context,
    CancellationToken ct)
    where TRequest : notnull
  {
    if (_options.IsEnabled)
    {
      string validationServiceName = _options.ServiceName;
      if (_options.IsPerEndpointCustomizationEnabled)
      {
        var metadata = GetEndpointMetadata(context);
        if (!metadata.IsEnabled)
        {
          return null;
        }
        validationServiceName = metadata.ServiceName ?? _options.ServiceName;
      }
      var validationService = context.RequestServices.GetKeyedService<IRequestValidationService>(validationServiceName);
      if (validationService is null)
      {
        throw new RequestValidationException(string.Format(
          ServiceNotRegistered, validationServiceName));
      }
      return await validationService.ValidateAsync(req, context, ct);
    }
    return null;
  }
}
