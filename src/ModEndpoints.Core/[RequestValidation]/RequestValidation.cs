using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ModEndpoints.Core;

public static class RequestValidation
{
  public const string DefaultServiceName = "Default";

  private static readonly RequestValidationMetadata DefaultMetadata = new();

  internal static readonly RequestValidationResult SuccessfulValidationResult = new(true, null);

  private const string ServiceNotRegistered =
    "Request validation service with name '{0}' is not registered.";

  internal static async Task<RequestValidationResult?> ValidateAsync<TRequest>(
    TRequest req,
    HttpContext context,
    CancellationToken ct)
    where TRequest : notnull
  {
    var options = context.RequestServices.GetRequiredService<IOptions<RequestValidationOptions>>();
    if (options.Value.IsEnabled)
    {
      var validationMetadata =
        context.GetEndpoint()?.Metadata.GetMetadata<RequestValidationMetadata>()
        ?? RequestValidation.DefaultMetadata;
      if (validationMetadata.IsEnabled)
      {
        var validationServiceName = validationMetadata.ServiceName ?? options.Value.DefaultServiceName;
        var validationService = context.RequestServices.GetKeyedService<IRequestValidationService>(validationServiceName);
        if (validationService is null)
        {
          throw new RequestValidationException(string.Format(
            RequestValidation.ServiceNotRegistered, validationServiceName));
        }
        return await validationService.ValidateAsync(req, context, ct);
      }
      return null;
    }
    return null;
  }
}
