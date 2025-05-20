using FluentValidation;
using ModEndpoints.Core;
using ModEndpoints.TestServer.Features.Books.Configuration;
using ModResults;

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
[ResultToResponseMapper("DefaultResultToResponseMapper")]
internal class GetBookById
  : WebResultEndpoint<GetBookByIdRequest, GetBookByIdResponse>
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    ConfigurationContext<IEndpointConfigurationSettings> configurationContext)
  {
    builder.MapGet("/{Id}")
      .Produces<GetBookByIdResponse>();
  }

  protected override async Task<Result<GetBookByIdResponse>> HandleAsync(
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
