using ModEndpoints.Core;
using ModEndpoints.TestServer.Features.Books.Configuration;
using ModResults;

namespace ModEndpoints.TestServer.Features.Books;

[MapToGroup<BooksRouteGroup>()]
internal class ResultEndpointWithEmptyRequest
  : WebResultEndpointWithEmptyRequest
{
  protected override void Configure(
    ConfigurationContext<IEndpointConfiguration> configurationContext)
  {
    MapDelete("/").Produces(StatusCodes.Status204NoContent);
  }

  protected override async Task<Result> HandleAsync(
    CancellationToken ct)
  {
    await Task.CompletedTask; // Simulate async work
    return Result.Ok();
  }
}
