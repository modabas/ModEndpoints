using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ModEndpoints.Core;
using ModEndpoints.TestServer.Features.Stores.Configuration;
using ModResults;

namespace ModEndpoints.TestServer.Features.Stores;

public record UpdateStoreRequest(Guid Id, [FromBody] UpdateStoreRequestBody Body);

public record UpdateStoreRequestBody(string Name);

internal class UpdateStoreRequestValidator : AbstractValidator<UpdateStoreRequest>
{
  public UpdateStoreRequestValidator()
  {
    RuleFor(x => x.Id).NotEmpty();
    RuleFor(x => x.Body.Name).NotEmpty();
  }
}

[MapToGroup<StoresRouteGroup>()]
internal class UpdateStore
  : BusinessResultEndpoint<UpdateStoreRequest>
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    EndpointConfigurationContext configurationContext)
  {
    builder.MapPut("/{Id}");
  }

  protected override Task<Result> HandleAsync(
    UpdateStoreRequest req,
    CancellationToken ct)
  {
    return Task.FromResult(Result.Ok());
  }
}
