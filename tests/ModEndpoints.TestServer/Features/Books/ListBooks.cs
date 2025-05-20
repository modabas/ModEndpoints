using ModEndpoints.Core;
using ModEndpoints.TestServer.Features.Books.Configuration;
using ModResults;

namespace ModEndpoints.TestServer.Features.Books;

public record ListBooksResponse(List<ListBooksResponseItem> Books);
public record ListBooksResponseItem(Guid Id, string Title, string Author, decimal Price);

[MapToGroup<BooksRouteGroup>()]
internal class ListBooks
  : WebResultEndpointWithEmptyRequest<ListBooksResponse>
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    ConfigurationContext<IEndpointConfigurationSettings> configurationContext)
  {
    builder.MapGet("/")
      .Produces<ListBooksResponse>();
  }

  protected override async Task<Result<ListBooksResponse>> HandleAsync(
    CancellationToken ct)
  {
    await Task.CompletedTask; // Simulate async work

    return new ListBooksResponse(Books:
      [
        new ListBooksResponseItem(Guid.NewGuid(), "Book 1", "Author 1", 19.99m),
        new ListBooksResponseItem(Guid.NewGuid(), "Book 2", "Author 2", 29.99m)
      ]);

  }
}
