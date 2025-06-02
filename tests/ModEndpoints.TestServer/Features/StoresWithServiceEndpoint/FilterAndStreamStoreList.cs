using System.Runtime.CompilerServices;
using FluentValidation;
using ModEndpoints.Core;
using ModEndpoints.RemoteServices;
using ModEndpoints.RemoteServices.Core;
using ModEndpoints.TestServer.Features.StoresWithServiceEndpoint.Configuration;

namespace ModEndpoints.TestServer.Features.StoresWithServiceEndpoint;

public record FilterAndStreamStoreListRequest(string Name)
  : IServiceRequestWithStreamingResponse<FilterAndStreamStoreListResponse>;

public record FilterAndStreamStoreListResponse(
  Guid Id,
  string Name);

internal class FilterAndStreamStoreListRequestValidator : AbstractValidator<FilterAndStreamStoreListRequest>
{
  public FilterAndStreamStoreListRequestValidator()
  {
    RuleFor(x => x.Name).NotEmpty();
  }
}

[MapToGroup<StoresWithServiceEndpointRouteGroup>()]
internal class FilterAndStreamStoreList
  : ServiceEndpointWithStreamingResponse<FilterAndStreamStoreListRequest, FilterAndStreamStoreListResponse>
{
  protected override async IAsyncEnumerable<StreamingResponseItem<FilterAndStreamStoreListResponse>> HandleAsync(
    FilterAndStreamStoreListRequest req,
    [EnumeratorCancellation] CancellationToken ct)
  {
    List<FilterAndStreamStoreListResponse> stores =
      [
        new FilterAndStreamStoreListResponse(
            Id: Guid.NewGuid(),
            Name: "Name 1"),
        new FilterAndStreamStoreListResponse(
          Id: Guid.NewGuid(),
          Name: "Name 2")
      ];

    foreach (var store in stores.Where(c => c.Name == req.Name))
    {
      yield return new StreamingResponseItem<FilterAndStreamStoreListResponse>(store);
      await Task.Delay(1000, ct); // Simulate async work
    }
  }
}
