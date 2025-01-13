using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ModEndpoints;
using ModEndpoints.Core;
using ModResults;
using ShowcaseWebApi.Features.Books.Configuration;

namespace ShowcaseWebApi.Features.Books;
public record UploadBookRequest(string Title, [FromForm] string Author, IFormFile BookFile);

public record UploadBookResponse(string FileName, long FileSize);

internal class UploadBookRequestValidator : AbstractValidator<UploadBookRequest>
{
  public UploadBookRequestValidator()
  {
    RuleFor(x => x.Title).NotEmpty();
    RuleFor(x => x.Author).NotEmpty();
    RuleFor(x => x.BookFile).NotEmpty();
  }
}

[MapToGroup<BooksV2RouteGroup>()]
internal class UploadBook
  : WebResultEndpoint<UploadBookRequest, UploadBookResponse>
{
  protected override void Configure(
    IServiceProvider serviceProvider,
    IRouteGroupConfigurator? parentRouteGroup)
  {
    MapPost("/upload/{Title}")
      .DisableAntiforgery()
      .Produces<UploadBookResponse>();
  }

  protected override Task<Result<UploadBookResponse>> HandleAsync(
    UploadBookRequest req,
    CancellationToken ct)
  {
    return Task.FromResult(Result.Ok(new UploadBookResponse(
      req.BookFile.FileName,
      req.BookFile.Length)));
  }
}
