using Microsoft.AspNetCore.Http;

namespace ModEndpoints.Core;

public interface IRequestValidator
{
  Task<RequestValidationResult> ValidateAsync<TRequest>(
    TRequest req,
    HttpContext context,
    CancellationToken ct)
    where TRequest : notnull;
}
