using ModEndpoints.Core;
using ModResults;

namespace ModEndpoints.TestServer.Features.Books;

internal class FailureEndpointWithoutResponseModel
  : WebResultEndpointWithEmptyRequest
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    EndpointConfigurationContext configurationContext)
  {
    builder.MapPost("/failure/withoutResponseModel");
  }

  protected override async Task<WebResult> HandleAsync(CancellationToken ct)
  {
    return Result.NotFound("Item not found.");
  }
}
