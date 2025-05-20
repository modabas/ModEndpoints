using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ModEndpoints.Core;
using ModEndpoints.TestServer.Features.Books.Configuration;
using ModResults;

namespace ModEndpoints.TestServer.Features.Books;
public record UpdateBookRequest(Guid Id, [FromBody] UpdateBookRequestBody Body);

public record UpdateBookRequestBody(string Title, string Author, decimal Price);

internal class UpdateBookRequestValidator : AbstractValidator<UpdateBookRequest>
{
  public UpdateBookRequestValidator()
  {
    RuleFor(x => x.Id).NotEmpty();
    RuleFor(x => x.Body.Title).NotEmpty();
    RuleFor(x => x.Body.Author).NotEmpty();
    RuleFor(x => x.Body.Price).GreaterThan(0);
  }
}

[MapToGroup<BooksRouteGroup>()]
internal class UpdateBook
  : WebResultEndpoint<UpdateBookRequest>
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    ConfigurationContext<EndpointConfigurationParameters> configurationContext)
  {
    builder.MapPut("/{Id}");
  }

  protected override Task<Result> HandleAsync(
    UpdateBookRequest req,
    CancellationToken ct)
  {
    return Task.FromResult(Result.Ok());
  }
}
