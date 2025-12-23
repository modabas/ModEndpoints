using Microsoft.AspNetCore.Http;

namespace ModEndpoints.Core;

internal interface IRequestValidationController
{
  Task<RequestValidationResult> ValidateAsync<TRequest>(
    TRequest req,
    HttpContext context,
    CancellationToken ct)
    where TRequest : notnull;
}
