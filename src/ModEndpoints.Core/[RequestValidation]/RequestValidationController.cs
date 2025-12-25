using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ModEndpoints.Core;

internal sealed class RequestValidationController : IRequestValidationController
{
  private readonly IOptions<RequestValidationOptions> _options;
  private static readonly RequestValidationMetadata _defaultMetadata = new();
  private const string ServiceNotRegistered =
    "Request validation service with name '{0}' is not registered.";

  public RequestValidationController(
    IOptions<RequestValidationOptions> options)
  {
    _options = options;
  }

  public async Task<RequestValidationResult?> ValidateAsync<TRequest>(
    TRequest req,
    HttpContext context,
    CancellationToken ct)
    where TRequest : notnull
  {
    if (_options.Value.IsEnabled)
    {
      var validationMetadata = context.GetEndpoint()?.Metadata.GetMetadata<RequestValidationMetadata>()
          ?? _defaultMetadata;
      if (validationMetadata.IsEnabled)
      {
        var validationServiceName = validationMetadata.ServiceName ?? _options.Value.DefaultServiceName;
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
    return null;
  }
}
