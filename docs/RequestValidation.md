# Request Validation

Every endpoint type in ModEndpoints packages supports request validation. By default, request validation utilizes FluentValidation and if a validator is registered for a request model, the request will be automatically validated before reaching the endpoint handler.

Each endpoint type has its own default behavior when request validation fails:
- `MinimalEndpoint` attempts to return an `IResult` with a 400 Bad Request response if validation fails and the response model is compatible with `IResult` for representing a bad request; otherwise, it throws a `RequestValidationException`.
- `WebResultEndpoint` will return an IResult with a 400 Bad Request response if validation fails.
- `BusinessResultEndpoint` and `ServiceEndpoint` will return a business result indicating failure with an invalid status if validation does not pass.

You can customize this behavior for individual endpoints by overriding the `HandleInvalidValidationResultAsync` method. This method is invoked by the internals of the endpoint implementation when request validation fails and receives a failed `RequestValidationResult` containing errors and the `HttpContext` as parameters. Response type varies depending on the endpoint type and/or response model.

>**Note**: If request validation fails, the endpoint handler method `HandleAsync` will not be called.

```csharp
public record GetBookByIdRequest(Guid Id);

public record GetBookByIdResponse(Guid Id, string Title, string Author, decimal Price);

internal class GetBookByIdRequestValidator : AbstractValidator<GetBookByIdRequest>
{
  public GetBookByIdRequestValidator()
  {
    RuleFor(x => x.Id).NotEmpty();
  }
}

internal class GetBookById(ServiceDbContext db)
  : WebResultEndpoint<GetBookByIdRequest, GetBookByIdResponse>
{
  protected override void Configure(
    IServiceProvider serviceProvider,
    IRouteGroupConfigurator? parentRouteGroup)
  {
    MapGet("/{Id}")
      .Produces<GetBookByIdResponse>();
  }

  protected override ValueTask<IResult> HandleInvalidValidationResultAsync(
    RequestValidationResult validationResult,
    HttpContext context,
    CancellationToken ct)
  {
    // change the default behavior of the endpoint when request validation has failed

  }

  protected override async Task<Result<GetBookByIdResponse>> HandleAsync(
    GetBookByIdRequest req,
    CancellationToken ct)
  {
    //implement the endpoint logic here

  }
}
```

## Disabling Default Request Validation
Default request validation can be disabled during dependency injection by setting UseDefaultRequestValidation to false. This can be useful if you want to implement your own request validation logic or if you want to use a different validation library.

If you are using `ModEndpoints.Core` package:
```csharp
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddModEndpointsFromAssemblyContaining<MyEndpoint>(conf =>
{
  conf.CoreOptions.UseDefaultRequestValidation = false;
});

// ... add other services
```

If you are using `ModEndpoints` package:
```csharp
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddModEndpointsCoreFromAssemblyContaining<MyEndpoint>(conf =>
{
  conf.UseDefaultRequestValidation = false;
});

// ... add other services
```