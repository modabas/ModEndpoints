using ModEndpoints.Core;
using ModEndpoints.TestServer.Features.Books.Configuration;
using ModResults;

namespace ModEndpoints.TestServer.Features.Books;

[MapToGroup<BooksRouteGroup>()]
internal class ResultEndpointWithEmptyRequest
  : WebResultEndpointWithEmptyRequest
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    EndpointConfigurationContext configurationContext)
  {
    builder.MapDelete("/").Produces(StatusCodes.Status204NoContent);
  }

  protected override async Task<WebResult> HandleAsync(
    CancellationToken ct)
  {
    await Task.CompletedTask; // Simulate async work
    return Result.Ok();
  }
}
