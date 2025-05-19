using FluentValidation;
using ModEndpoints.Core;
using ModEndpoints.TestServer.Features.Stores.Configuration;
using ModResults;

namespace ModEndpoints.TestServer.Features.Stores;
public record GetStoreByIdRequest(Guid Id);

public record GetStoreByIdResponse(Guid Id, string Name);

internal class GetStoreByIdRequestValidator : AbstractValidator<GetStoreByIdRequest>
{
  public GetStoreByIdRequestValidator()
  {
    RuleFor(x => x.Id).NotEmpty();
  }
}

[MapToGroup<StoresRouteGroup>()]
internal class GetStoreById
  : BusinessResultEndpoint<GetStoreByIdRequest, GetStoreByIdResponse>
{
  protected override void Configure(
    ConfigurationContext<IEndpointConfiguration> configurationContext)
  {
    MapGet("/{Id}");
  }

  protected override async Task<Result<GetStoreByIdResponse>> HandleAsync(
    GetStoreByIdRequest req,
    CancellationToken ct)
  {
    await Task.CompletedTask; // Simulate async work

    return new GetStoreByIdResponse(
        Id: req.Id,
        Name: "Name 1");
  }
}

