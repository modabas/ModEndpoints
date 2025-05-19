using ModEndpoints.Core;

namespace BenchmarkWebApi.Features.MinimalEndpoints;

internal class BasicTest
  : MinimalEndpoint<IResult>
{
  protected override void Configure(
    ConfigurationContext<IEndpointConfiguration> configurationContext)
  {
    MapGet("/MinimalEndpoints/BasicTest")
      .Produces<string>();
  }

  protected override Task<IResult> HandleAsync(
    CancellationToken ct)
  {
    return Task.FromResult(Results.Ok("Hello World"));
  }
}
