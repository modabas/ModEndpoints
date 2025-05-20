using ModEndpoints;
using ModEndpoints.Core;
using ModResults;

namespace BenchmarkWebApi.Features.WebResultEndpoints;

internal class BasicTest
  : WebResultEndpointWithEmptyRequest<string>
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    ConfigurationContext<EndpointConfigurationParameters> configurationContext)
  {
    builder.MapGet("/WebResultEndpoints/BasicTest")
      .Produces<string>();
  }

  protected override Task<Result<string>> HandleAsync(
    CancellationToken ct)
  {
    return Task.FromResult(Result.Ok("Hello World"));
  }
}

