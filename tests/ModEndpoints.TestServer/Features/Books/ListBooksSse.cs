#if NET10_0_OR_GREATER
using System.Net.ServerSentEvents;
using System.Runtime.CompilerServices;
using ModEndpoints.Core;
using ModEndpoints.TestServer.Features.Books.Configuration;

namespace ModEndpoints.TestServer.Features.Books;

[MapToGroup<BooksRouteGroup>()]
internal class ListBooksSse
  : WebResultEndpointWithEmptyRequest<IAsyncEnumerable<SseItem<ListBooksResponseItem>>>
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    EndpointConfigurationContext configurationContext)
  {
    builder.MapGet("/listSse/")
      .Produces<ListBooksResponse>();
  }

  protected override async Task<WebResult<IAsyncEnumerable<SseItem<ListBooksResponseItem>>>> HandleAsync(
    CancellationToken ct)
  {
    return WebResults.ServerSentEvents(GetBooks(ct));

    async IAsyncEnumerable<ListBooksResponseItem> GetBooks(
      [EnumeratorCancellation] CancellationToken ct)
    {
      List<ListBooksResponseItem> books =
        [
          new ListBooksResponseItem(Guid.NewGuid(), "Book 1", "Author 1", 19.99m),
          new ListBooksResponseItem(Guid.NewGuid(), "Book 2", "Author 2", 29.99m)
        ];
      foreach (var book in books)
      {
        yield return book;
        await Task.Delay(1000, ct);
      }
    }
  }
}
#endif
