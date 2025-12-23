using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ModEndpoints.Core;

internal sealed class RequestValidationController : IRequestValidationController
{
  public async Task<RequestValidationResult> ValidateAsync<TRequest>(
    TRequest req,
    HttpContext context,
    CancellationToken ct)
    where TRequest : notnull
  {
    var validationMetadata = 
      context.GetEndpoint()?.Metadata.GetMetadata<RequestValidationMetadata>()
      ?? RequestValidationDefinitions.DefaultMetadata;
    if (validationMetadata.IsEnabled)
    {
      var validationServiceName = validationMetadata.ServiceName;
      var validationService = context.RequestServices.GetRequiredKeyedService<IRequestValidationService>(validationServiceName);
      return await validationService.ValidateAsync(req, context, ct);
    }
    return RequestValidationDefinitions.SuccessfulValidationResult;
  }
}
