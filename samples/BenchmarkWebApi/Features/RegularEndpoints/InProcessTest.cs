using BenchmarkWebApi.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ModEndpoints.Core;
using ModResults;
using ModResults.MinimalApis;

namespace BenchmarkWebApi.Features.RegularEndpoints;

public record InProcessTestRequest(Guid Id, [FromBody] InProcessTestRequestBody Body);

public record InProcessTestRequestBody(string Name);

public record InProcessTestResponse(string Reply);

internal class InProcessTestRequestValidator : AbstractValidator<InProcessTestRequest>
{
  public InProcessTestRequestValidator()
  {
    RuleFor(x => x.Id).NotEmpty();
    RuleFor(x => x.Body.Name).NotEmpty();
  }
}

internal static class InProcessTest
{
  public static RouteHandlerBuilder MapMinimalApiForInProcessTest(this IEndpointRouteBuilder builder)
  {
    return builder.MapPost("RegularEndpoints/InProcessTest/{Id}",
      async Task<IResult> (
        [AsParameters] InProcessTestRequest req,
        [FromServices] IValidator<InProcessTestRequest> validator,
        [FromServices] IGetMeAStringService svc,
        CancellationToken cancellationToken) =>
    {
      var validationResult = await validator.ValidateAsync(req, cancellationToken);
      if (!validationResult.IsValid)
      {
        return validationResult.ToMinimalApiResult();
      }

      var result = await svc.GetMeAResultOfStringAsync(req.Body.Name, cancellationToken);
      return result.ToResult(x => new InProcessTestResponse(x)).ToResponse();
    }).Produces<InProcessTestResponse>();
  }
}
