using FluentValidation;
using ModEndpoints.Core;
using ModEndpoints.RemoteServices.Core;
using ModEndpoints.TestServer.Features.StoresWithServiceEndpoint.Configuration;
using ModResults;

namespace ModEndpoints.TestServer.Features.StoresWithServiceEndpoint;

public record DeleteStoreRequest(Guid Id) : IServiceRequest;
internal class DeleteStoreRequestValidator : AbstractValidator<DeleteStoreRequest>
{
  public DeleteStoreRequestValidator()
  {
    RuleFor(x => x.Id).NotEmpty();
  }
}

[MapToGroup<StoresWithServiceEndpointRouteGroup>()]
internal class DeleteStore
  : ServiceEndpoint<DeleteStoreRequest>
{
  protected override Task<Result> HandleAsync(
    DeleteStoreRequest req,
    CancellationToken ct)
  {
    return Task.FromResult(Result.Ok());
  }
}
