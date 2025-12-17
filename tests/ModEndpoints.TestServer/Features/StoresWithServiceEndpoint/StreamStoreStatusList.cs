using System.Runtime.CompilerServices;
using FluentValidation;
using ModEndpoints.Core;
using ModEndpoints.RemoteServices;
using ModEndpoints.RemoteServices.Shared;
using ModEndpoints.TestServer.Features.StoresWithServiceEndpoint.Configuration;
using ModResults;

namespace ModEndpoints.TestServer.Features.StoresWithServiceEndpoint;

public record StreamStoreStatusListRequest(string Name) : IServiceRequestWithStreamingResponse;

internal class StreamStoreStatusListRequestValidator
  : AbstractValidator<StreamStoreStatusListRequest>
{
  public StreamStoreStatusListRequestValidator()
  {
    RuleFor(x => x.Name).NotEmpty();
  }
}

[MapToGroup<StoresWithServiceEndpointRouteGroup>()]
internal class StreamStoreStatusList
  : ServiceEndpointWithStreamingResponse<StreamStoreStatusListRequest>
{
  protected override async IAsyncEnumerable<StreamingResponseItem> HandleAsync(
    StreamStoreStatusListRequest req,
    [EnumeratorCancellation] CancellationToken ct)
  {
    for (int i = 0; i < 2; i++)
    {
      yield return new StreamingResponseItem(Result.Ok());
      await Task.Delay(1000, ct); // Simulate async work
    }
  }
}
