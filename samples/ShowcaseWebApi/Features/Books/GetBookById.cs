using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ModEndpoints;
using ModEndpoints.Core;
using ModResults;
using ShowcaseWebApi.Data;
using ShowcaseWebApi.Features.Books.Configuration;

namespace ShowcaseWebApi.Features.Books;

public record GetBookByIdRequest(Guid Id);

public record GetBookByIdResponse(Guid Id, string Title, string Author, decimal Price);

internal class GetBookByIdRequestValidator : AbstractValidator<GetBookByIdRequest>
{
  public GetBookByIdRequestValidator()
  {
    RuleFor(x => x.Id).NotEmpty();
  }
}

[MapToGroup(typeof(BooksV1RouteGroup))]
internal class GetBookById(ServiceDbContext db)
  : WebResultEndpoint<GetBookByIdRequest, GetBookByIdResponse>
{
  protected override void Configure(
    IServiceProvider serviceProvider,
    IRouteGroupConfigurator? parentRouteGroup)
  {
    MapGet("/{Id}")
      .Produces<GetBookByIdResponse>();
  }

  protected override async Task<Result<GetBookByIdResponse>> HandleAsync(
    GetBookByIdRequest req,
    CancellationToken ct)
  {
    var entity = await db.Books.FirstOrDefaultAsync(b => b.Id == req.Id, ct);

    var result = entity is null ?
      Result<GetBookByIdResponse>.NotFound() :
      Result.Ok(new GetBookByIdResponse(
        Id: entity.Id,
        Title: entity.Title,
        Author: entity.Author,
        Price: entity.Price));

    return result;
  }
}
