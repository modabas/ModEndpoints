using FluentValidation;
using ModEndpoints.Core;
using ModEndpoints.RemoteServices.Contracts;
using ModEndpoints.TestServer.Features.StoresWithServiceEndpoint.Configuration;
using ModResults;

namespace ModEndpoints.TestServer.Features.StoresWithServiceEndpoint;

public record GetStoreByIdRequest(Guid Id) : IServiceRequest<GetStoreByIdResponse>;
public record GetStoreByIdResponse(Guid Id, string Name);

internal class GetStoreByIdRequestValidator : AbstractValidator<GetStoreByIdRequest>
{
  public GetStoreByIdRequestValidator()
  {
    RuleFor(x => x.Id).NotEmpty();
  }
}

[MapToGroup<StoresWithServiceEndpointRouteGroup>()]
[UriResolver("Default")]
internal class GetStoreById
  : ServiceEndpoint<GetStoreByIdRequest, GetStoreByIdResponse>
{
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

