using ModEndpoints.Core;
using ModResults;

namespace ModEndpoints.TestServer.Features.Books;

internal class FailureEndpointWithoutResponseModel
  : WebResultEndpointWithEmptyRequest
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    ConfigurationContext<EndpointConfigurationParameters> configurationContext)
  {
    builder.MapPost("/failure/withoutResponseModel");
  }

  protected override Task<Result> HandleAsync(CancellationToken ct)
  {
    return Task.FromResult(Result.NotFound("Item not found."));
  }
}
