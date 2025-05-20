using BenchmarkWebApi.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ModEndpoints.Core;
using ModResults;
using ModResults.MinimalApis;

namespace BenchmarkWebApi.Features.MinimalEndpoints;

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

internal class InProcessTest(IGetMeAStringService svc)
  : MinimalEndpoint<InProcessTestRequest, IResult>
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    ConfigurationContext<IEndpointConfigurationSettings> configurationContext)
  {
    builder.MapPost("/MinimalEndpoints/InProcessTest/{Id}")
      .Produces<InProcessTestResponse>();
  }

  protected override async Task<IResult> HandleAsync(
    InProcessTestRequest req,
    CancellationToken ct)
  {
    var result = await svc.GetMeAResultOfStringAsync(req.Body.Name, ct);
    return result.ToResult(x => new InProcessTestResponse(x)).ToResponse();
  }
}
