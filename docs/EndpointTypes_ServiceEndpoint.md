# ServiceEndpoint

This is a very specialized endpoint which is intended to abstract away all HTTP client and request setup, consumption and response handling when used together with its client implementation. Aim is to enable developers to easily consume remote services with a strongly typed request and response model only by sharing said models between the service and client projects.

Request model defined for a ServiceEndpoint is bound with [FromBody] attribute.

A ServiceEndpoint implementation, similar to BusinessResultEntpoint, encapsulates the response [business result](https://github.com/modabas/ModResults) of HandleAsync method in a HTTP 200 Minimal API IResult and sends to client. The [business result](https://github.com/modabas/ModResults) returned may be in Ok or Failed state.

- **ServiceEndpoint&lt;TRequest, TResultValue&gt;**: Has a request model, supports request validation and returns a [Result&lt;TResultValue&gt;](https://github.com/modabas/ModResults) within HTTP 200 IResult.
``` csharp
public record GetStoreByIdRequest(Guid Id) : IServiceRequest<GetStoreByIdResponse>;

public record GetStoreByIdResponse(Guid Id, string Name);

internal class GetStoreByIdRequestValidator : AbstractValidator<GetStoreByIdRequest>
{
  public GetStoreByIdRequestValidator()
  {
    RuleFor(x => x.Id).NotEmpty();
  }
}

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
```

- **ServiceEndpoint&lt;TRequest&gt;**: Has a request model, supports request validation and returns a [Result](https://github.com/modabas/ModResults) within HTTP 200 IResult.
``` csharp
public record DeleteStoreRequest(Guid Id) : IServiceRequest;
internal class DeleteStoreRequestValidator : AbstractValidator<DeleteStoreRequest>
{
  public DeleteStoreRequestValidator()
  {
    RuleFor(x => x.Id).NotEmpty();
  }
}

internal class DeleteStore
  : ServiceEndpoint<DeleteStoreRequest>
{
  protected override Task<Result> HandleAsync(
    DeleteStoreRequest req,
    CancellationToken ct)
  {
    // Handle request
  }
}
```

- **ServiceEndpointWithStreamingResponse&lt;TRequest, TResultValue&gt;**: Has a request model, supports request validation and returns `IAsyncEnumerable<StreamingResponseItem<TResultValue>>`.
``` csharp
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
```

- **ServiceEndpointWithStreamingResponse&lt;TRequest&gt;**: Has a request model, supports request validation and returns `IAsyncEnumerable<StreamingResponseItem>`.
``` csharp
public record StreamStoreStatusListRequest(string Name) : IServiceRequestWithStreamingResponse;

internal class StreamStoreStatusListRequestValidator
  : AbstractValidator<StreamStoreStatusListRequest>
{
  public StreamStoreStatusListRequestValidator()
  {
    RuleFor(x => x.Name).NotEmpty();
  }
}

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
```

>**Note**: `StreamingResponseItem` is a specialized type that contains a `Result` object and also Response Type and Id fields. It is used for streaming responses to allow clients to process each item as it arrives.

A ServiceEndpoint has following special traits and constraints:
- A ServiceEndpoint is always registered as a POST method, and its bound pattern is determined accourding to its request type.
- Request model defined for a ServiceEndpoint is bound with [FromBody] attribute.
- A ServiceEndpoint's request must implement either IServiceRequest (for endpoints implementing ServiceEndpoint&lt;TRequest&gt;) or IServiceRequest&lt;TResultValue&gt; (for endpoints implementing ServiceEndpoint&lt;TRequest, TResultValue&gt;)
- A ServiceEndpoint's request is specific to that endpoint. Each endpoint must have its unique request type.
- To utilize the advantages of a ServiceEndpoint over other endpoint types, its request and response models have to be shared with clients and therefore has to be in a seperate class library.

These restrictions enable clients to call ServiceEndpoints by utilizing a specialized message channel resolved from dependency injection, see [ServiceEndpoint Clients](ServiceEndpointClients.md) documentation for details.