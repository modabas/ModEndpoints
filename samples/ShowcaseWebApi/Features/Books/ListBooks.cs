using Microsoft.EntityFrameworkCore;
using ModEndpoints;
using ModEndpoints.Core;
using ShowcaseWebApi.Data;
using ShowcaseWebApi.Features.Books.Configuration;

namespace ShowcaseWebApi.Features.Books;

public record ListBooksResponse(List<ListBooksResponseItem> Books);
public record ListBooksResponseItem(Guid Id, string Title, string Author, decimal Price);

[MapToGroup<BooksV1RouteGroup>()]
internal class ListBooks(ServiceDbContext db)
  : WebResultEndpointWithEmptyRequest<ListBooksResponse>
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    EndpointConfigurationContext configurationContext)
  {
    builder.MapGet("/")
      .Produces<ListBooksResponse>();
  }

  protected override async Task<WebResult<ListBooksResponse>> HandleAsync(
    CancellationToken ct)
  {
    var books = await db.Books
      .Select(b => new ListBooksResponseItem(
        b.Id,
        b.Title,
        b.Author,
        b.Price))
      .ToListAsync(ct);

    return new ListBooksResponse(Books: books);
  }
}
