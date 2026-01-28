using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using ModEndpoints;
using ModEndpoints.Core;
using ShowcaseWebApi.Data;
using ShowcaseWebApi.Features.Books.Configuration;

namespace ShowcaseWebApi.Features.Books;

[MapToGroup<BooksV1RouteGroup>()]
internal class ListBooksWithStreamingResponse(ServiceDbContext db)
  : WebResultEndpointWithEmptyRequest<IAsyncEnumerable<ListBooksResponseItem>>
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    EndpointConfigurationContext configurationContext)
  {
    builder.MapGet("/listWithStreamingResponse/");
  }

  protected override async Task<WebResult<IAsyncEnumerable<ListBooksResponseItem>>> HandleAsync(
    CancellationToken ct)
  {
    return WebResults.WithStreamingResponse(GetBooks(ct));

    async IAsyncEnumerable<ListBooksResponseItem> GetBooks(
      [EnumeratorCancellation] CancellationToken ct)
    {
      await foreach (var book in db.Books
        .Select(b => new ListBooksResponseItem(
          b.Id,
          b.Title,
          b.Author,
          b.Price))
        .AsAsyncEnumerable()
        .WithCancellation(ct))
      {
        await Task.Delay(1000, ct);
        yield return book;
      }
    }
  }
}
