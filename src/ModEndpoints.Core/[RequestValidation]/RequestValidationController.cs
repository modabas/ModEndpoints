using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ModEndpoints.Core;

internal sealed class RequestValidationController : IRequestValidationController
{
  private readonly RequestValidationOptions _options;
  private const string ServiceNotRegistered =
    "Request validation service with name '{0}' is not registered.";
  private readonly RequestValidationEndpointMetadata _enabledEndpointMetadata = new(IsEnabled: true);
  private readonly RequestValidationEndpointMetadata _disabledEndpointMetadata = new(IsEnabled: false);

  public RequestValidationController(
    IOptions<RequestValidationOptions> options)
  {
    _options = options.Value;
  }

  public Task<RequestValidationResult?> ValidateAsync<TRequest>(
    TRequest req,
    HttpContext context,
    CancellationToken ct)
    where TRequest : notnull
  {
    if (_options.IsEnabled || _options.IsPerEndpointCustomizationEnabled)
    {
      string? validationServiceName;
      if (_options.IsPerEndpointCustomizationEnabled)
      {
        //Get endpoint metadata
        var metadata = context.GetEndpoint()?.Metadata.GetMetadata<RequestValidationEndpointMetadata>() ??
          (_options.IsEnabled ? _enabledEndpointMetadata : _disabledEndpointMetadata);

        if (metadata.IsEnabled)
        {
          validationServiceName = metadata.ServiceName ?? _options.ServiceName;
        }
        else
        {
          return Task.FromResult<RequestValidationResult?>(null);
        }
      }
      else
      {
        validationServiceName = _options.ServiceName;
      }
      var validationService = context.RequestServices.GetKeyedService<IRequestValidationService>(validationServiceName);
      if (validationService is null)
      {
        throw new RequestValidationException(string.Format(
          ServiceNotRegistered, validationServiceName));
      }
      return validationService.ValidateAsync(req, context, ct);
    }
    return Task.FromResult<RequestValidationResult?>(null);
  }
}
