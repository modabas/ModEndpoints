using FluentValidation;
using ModEndpoints.Core;
using ModEndpoints.TestServer.Features.Books.Configuration;

namespace ModEndpoints.TestServer.Features.Books;

public record GetBookByIdRequest(Guid Id);

public record GetBookByIdResponse(Guid Id, string Title, string Author, decimal Price);

internal class GetBookByIdRequestValidator : AbstractValidator<GetBookByIdRequest>
{
  public GetBookByIdRequestValidator()
  {
    RuleFor(x => x.Id).NotEmpty();
  }
}

[MapToGroup<BooksRouteGroup>()]
internal class GetBookById
  : WebResultEndpoint<GetBookByIdRequest, GetBookByIdResponse>
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    EndpointConfigurationContext configurationContext)
  {
    builder.MapGet("/{Id}")
      .Produces<GetBookByIdResponse>();
  }

  protected override async Task<WebResult<GetBookByIdResponse>> HandleAsync(
    GetBookByIdRequest req,
    CancellationToken ct)
  {
    await Task.CompletedTask; // Simulate async work

    return new GetBookByIdResponse(
      req.Id,
      "Book 1",
      "Author 1",
      19.99m);
  }
}
