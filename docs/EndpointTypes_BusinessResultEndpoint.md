# BusinessResultEndpoint

>**Package**: `ModEndpoints`

A BusinessResultEndpoint implementation, after handling request, encapsulates the [business result](https://github.com/modabas/ModResults) of HandleAsync method in a HTTP 200 Minimal API IResult and sends to client. The [business result](https://github.com/modabas/ModResults) returned may be in Ok or Failed state. This behaviour makes BusinessResultEndpoints more suitable for cases where clients are aware of Result or Result&lt;TValue&gt; implementations.

Request model (if any) defined for a BusinessResultEndpoint is bound with [AsParameters] attribute.

- **BusinessResultEndpoint&lt;TRequest, TResultValue&gt;**: Has a request model, supports request validation and returns a [Result&lt;TResultValue&gt;](https://github.com/modabas/ModResults) within HTTP 200 IResult.
``` csharp
public record GetStoreByIdRequest(Guid Id);

public record GetStoreByIdResponse(Guid Id, string Name);

internal class GetStoreByIdRequestValidator : AbstractValidator<GetStoreByIdRequest>
{
  public GetStoreByIdRequestValidator()
  {
    RuleFor(x => x.Id).NotEmpty();
  }
}

internal class GetStoreById
  : BusinessResultEndpoint<GetStoreByIdRequest, GetStoreByIdResponse>
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    ConfigurationContext<EndpointConfigurationParameters> configurationContext)
  {
    builder.MapGet("/{Id}");
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
```

- **BusinessResultEndpoint&lt;TRequest&gt;**: Has a request model, supports request validation and returns a [Result](https://github.com/modabas/ModResults) within HTTP 200 IResult.
``` csharp
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

internal class UpdateStore
  : BusinessResultEndpoint<UpdateStoreRequest>
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    ConfigurationContext<EndpointConfigurationParameters> configurationContext)
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
```

- **BusinessResultEndpointWithEmptyRequest&lt;TResultValue&gt;**: Doesn't have a request model and returns a [Result&lt;TResultValue&gt;](https://github.com/modabas/ModResults) within HTTP 200 IResult.
``` csharp
public record ListStoresResponse(List<ListStoresResponseItem> Stores);
public record ListStoresResponseItem(Guid Id, string Name);

internal class ListStores
  : BusinessResultEndpointWithEmptyRequest<ListStoresResponse>
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    ConfigurationContext<EndpointConfigurationParameters> configurationContext)
  {
    builder.MapGet("/");
  }

  protected override async Task<Result<ListStoresResponse>> HandleAsync(
    CancellationToken ct)
  {
    await Task.CompletedTask; // Simulate async work

    return new ListStoresResponse(Stores:
      [
        new ListStoresResponseItem(Guid.NewGuid(), "Name 1"),
        new ListStoresResponseItem(Guid.NewGuid(), "Name 2")
      ]);
  }
}
```

- **BusinessResultEndpointWithEmptyRequest**: Doesn't have a request model and returns a [Result](https://github.com/modabas/ModResults) within HTTP 200 IResult.
``` csharp
internal class ResultEndpointWithEmptyRequest
  : BusinessResultEndpointWithEmptyRequest
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    ConfigurationContext<EndpointConfigurationParameters> configurationContext)
  {
    builder.MapDelete("/");
  }

  protected override async Task<Result> HandleAsync(
    CancellationToken ct)
  {
    await Task.CompletedTask; // Simulate async work
    return Result.Ok();
  }
}
```
