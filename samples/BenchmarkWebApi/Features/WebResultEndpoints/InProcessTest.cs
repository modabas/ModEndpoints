using BenchmarkWebApi.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ModEndpoints;
using ModEndpoints.Core;
using ModResults;

namespace BenchmarkWebApi.Features.WebResultEndpoints;

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
  : WebResultEndpoint<InProcessTestRequest, InProcessTestResponse>
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    ConfigurationContext<IEndpointConfigurationSettings> configurationContext)
  {
    builder.MapPost("/WebResultEndpoints/InProcessTest/{Id}")
      .Produces<InProcessTestResponse>();
  }

  protected override async Task<Result<InProcessTestResponse>> HandleAsync(
    InProcessTestRequest req,
    CancellationToken ct)
  {
    var result = await svc.GetMeAResultOfStringAsync(req.Body.Name, ct);
    return result.ToResult(x => new InProcessTestResponse(x));
  }
}
