# Request Validation

Every endpoint type in ModEndpoints packages supports request validation. By default, request validation utilizes FluentValidation and if a validator is registered for a request model, the request will be automatically validated before reaching the endpoint handler.

Each endpoint type has its own default behavior when request validation fails:
- `MinimalEndpoint` attempts to return an `IResult` with a 400 Bad Request response if validation fails and the response model is compatible with `IResult` for representing a bad request; otherwise, it throws a `RequestValidationException`.
- `WebResultEndpoint` will return an IResult with a 400 Bad Request response if validation fails.
- `BusinessResultEndpoint` and `ServiceEndpoint` will return a business result indicating failure with an invalid status if validation does not pass.

You can customize invalid request responses for individual endpoints by overriding the `HandleInvalidValidationResultAsync` method. This method is invoked by the internals of the endpoint implementation when request validation fails and receives a failed `RequestValidationResult` containing errors and the `HttpContext` as parameters. Response type varies depending on the endpoint type and/or response model.

>**Note**: If request validation fails, the endpoint handler method `HandleAsync` will not be called.

>**Note**: `WebResultEndpoint` implementations have to override `HandleValidationFailureAsync` method instead to customize the behavior when request validation fails.

```csharp
internal class GetBookById(ServiceDbContext db)
  : WebResultEndpoint<GetBookByIdRequest, GetBookByIdResponse>
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    EndpointConfigurationContext configurationContext)
  {
    builder.MapGet("/{Id}")
      .Produces<GetBookByIdResponse>();
  }

  protected override ValueTask<WebResult<GetBookByIdResponse>> HandleValidationFailureAsync(
    RequestValidationResult validationResult,
    HttpContext context,
    CancellationToken ct)
  {
    // change the default behavior of the endpoint when request validation has failed

  }

  protected override async Task<WebResult<GetBookByIdResponse>> HandleAsync(
    GetBookByIdRequest req,
    CancellationToken ct)
  {
    //implement the endpoint logic here

  }
}

public record GetBookByIdRequest(Guid Id);

public record GetBookByIdResponse(Guid Id, string Title, string Author, decimal Price);

internal class GetBookByIdRequestValidator : AbstractValidator<GetBookByIdRequest>
{
  public GetBookByIdRequestValidator()
  {
    RuleFor(x => x.Id).NotEmpty();
  }
}
```

## Customizing Request Validation Behavior Globally

Global request validation behavior can be customized during application startup:
- `EnableRequestValidation` property (default value: true) turns on/off request validation globally for all endpoints, 
- `DefaultRequestValidationServiceName` property changes the default service used for request validation for all endpoints. Default values for these settings make use of built-in FluentValidation request validation service.

By implementing your own request validation service that adheres to `IRequestValidationService` interface, you can register it in the DI container with a custom service name and set `DefaultRequestValidationServiceName` to that name, so that all endpoints will use your custom request validation service by default.

If you are using `ModEndpoints.Core` package:
```csharp
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddKeyedSingleton<IRequestValidationService, MyValidationService>(
  "MyValidationServiceName");

builder.Services.AddModEndpointsCoreFromAssemblyContaining<MyEndpoint>(conf =>
{
  conf.DefaultRequestValidationServiceName = "MyValidationServiceName";
});

// ... add other services
```

If you are using `ModEndpoints` package:
```csharp
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddKeyedSingleton<IRequestValidationService, MyValidationService>(
  "MyValidationServiceName");

builder.Services.AddModEndpointsFromAssemblyContaining<MyEndpoint>(conf =>
{
  conf.CoreOptions.DefaultRequestValidationServiceName = "MyValidationServiceName";
});

// ... add other services
```

## Custom Request Validation Behavior per Endpoint

Individual endpoint request validation behaviour can be set during endpoint configuration via route builder extension methods:
- **`DisableRequestValidation()` method:** Disables request validation for that endpoint.
- **`ValidateRequestWithOptions(string? validationServiceName)` method:** Use this method to require that incoming requests to the endpoint are validated according to the rules defined by the specified validation service instead of the default validation service set by global options. Has no effect if request validation is globally turned off.

```csharp
internal class CreateCustomer(ServiceDbContext db)
  : MinimalEndpoint<CreateCustomerRequest, Results<CreatedAtRoute<CreateCustomerResponse>, ValidationProblem, ProblemHttpResult>>
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    EndpointConfigurationContext configurationContext)
  {
    builder
      .MapPost("/")
      .DisableRequestValidation();
  }

  protected override async Task<Results<CreatedAtRoute<CreateCustomerResponse>, ValidationProblem, ProblemHttpResult>> HandleAsync(
    CreateCustomerRequest req,
    CancellationToken ct)
  {
    //Handler implementation
  }
}

public record CreateCustomerRequest([FromBody] CreateCustomerRequestBody Body);

public record CreateCustomerRequestBody(string FirstName, string? MiddleName, string LastName);

public record CreateCustomerResponse(Guid Id);

internal class CreateCustomerRequestValidator : AbstractValidator<CreateCustomerRequest>
{
  public CreateCustomerRequestValidator()
  {
    RuleFor(x => x.Body.FirstName).NotEmpty();
    RuleFor(x => x.Body.LastName).NotEmpty();
  }
}
```

