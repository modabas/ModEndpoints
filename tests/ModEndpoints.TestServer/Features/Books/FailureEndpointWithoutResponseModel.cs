using ModEndpoints.Core;
using ModResults;

namespace ModEndpoints.TestServer.Features.Books;

internal class FailureEndpointWithoutResponseModel
  : WebResultEndpointWithEmptyRequest
{
  protected override void Configure(IServiceProvider serviceProvider, IRouteGroupConfigurator? parentRouteGroup)
  {
    MapPost("/failure/withoutResponseModel");
  }

  protected override Task<Result> HandleAsync(CancellationToken ct)
  {
    return Task.FromResult(Result.NotFound("Item not found."));
  }
}
