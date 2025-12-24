using Microsoft.AspNetCore.Http;

namespace ModEndpoints.Core;

public interface IRequestValidationService
{
  Task<RequestValidationResult?> ValidateAsync<TRequest>(
    TRequest req,
    HttpContext context,
    CancellationToken ct)
    where TRequest : notnull;
}
