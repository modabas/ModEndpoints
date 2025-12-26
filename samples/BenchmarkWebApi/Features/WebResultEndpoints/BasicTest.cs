using ModEndpoints;
using ModEndpoints.Core;
using ModResults;

namespace BenchmarkWebApi.Features.WebResultEndpoints;

internal class BasicTest
  : WebResultEndpointWithEmptyRequest<string>
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    EndpointConfigurationContext configurationContext)
  {
    builder.MapGet("/WebResultEndpoints/BasicTest")
      .Produces<string>();
  }

  protected override async Task<WebResult<string>> HandleAsync(
    CancellationToken ct)
  {
    return Result.Ok("Hello World");
  }
}

