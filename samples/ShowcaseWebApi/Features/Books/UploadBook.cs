using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ModEndpoints;
using ModEndpoints.Core;
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
    EndpointConfigurationBuilder builder,
    EndpointConfigurationContext configurationContext)
  {
    builder.MapPost("/upload/{Title}")
      .DisableAntiforgery()
      .Produces<UploadBookResponse>();
  }

  protected override Task<WebResult<UploadBookResponse>> HandleAsync(
    UploadBookRequest req,
    CancellationToken ct)
  {
    return Task.FromResult(
      WebResults.FromResult(
        new UploadBookResponse(
          req.BookFile.FileName,
          req.BookFile.Length))
      .AsBase());
  }
}
