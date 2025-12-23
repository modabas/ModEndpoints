using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ModEndpoints.Core;

public static class RequestValidationDefinitions
{
  public const string DefaultServiceName = "Default";

  internal static readonly RequestValidationMetadata DefaultMetadata = new();

  internal static readonly RequestValidationResult SuccessfulValidationResult = new(true);

  internal static async Task<RequestValidationResult> ValidateAsync<TRequest>(
    TRequest req,
    HttpContext context,
    CancellationToken ct)
    where TRequest : notnull
  {
    var validationController = context.RequestServices.GetService<IRequestValidationController>();
    if (validationController is not null)
    {
      var validationResult = await validationController.ValidateAsync(req, context, ct);
      return validationResult;
    }
    return RequestValidationDefinitions.SuccessfulValidationResult;
  }
}
