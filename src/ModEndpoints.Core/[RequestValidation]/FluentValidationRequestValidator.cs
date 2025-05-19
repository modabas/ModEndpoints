using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ModEndpoints.Core;

public class FluentValidationRequestValidator : IRequestValidator
{
  public async Task<RequestValidationResult> ValidateAsync<TRequest>(
    TRequest req,
    HttpContext context,
    CancellationToken ct)
    where TRequest : notnull
  {
    var validator = context.RequestServices.GetService<IValidator<TRequest>>();
    if (validator is not null)
    {
      var validationResult = await validator.ValidateAsync(req, ct);
      if (!validationResult.IsValid)
      {
        return new RequestValidationResult
        {
          IsFailed = true,
          Errors = validationResult.Errors.Select(e => new RequestValidationFailure
          {
            PropertyName = e.PropertyName,
            ErrorMessage = e.ErrorMessage,
            ErrorCode = e.ErrorCode,
            AttemptedValue = e.AttemptedValue
          }).ToList()
        };
      }
    }
    return new RequestValidationResult
    {
      IsFailed = false
    };
  }
}
