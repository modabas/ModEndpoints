# MinimalEndpoint

>**Package**: `ModEndpoints.Core`

MinimalEndpoint within `ModEndpoints.Core` package is closest to barebones Minimal API. Request model (if any) defined for a MinimalEndpoint is bound with [AsParameters] attribute. Its 'HandleAsync' method supports the following types of return values:

- string
- T (Any other type)
- Minimal API IResult based (Including TypedResults with Results<TResult1, TResultN> return value)

See [How to create responses in Minimal API apps](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/responses?view=aspnetcore-8.0) for detailed information.

A MinimalEndpoint implementation, after handling request, returns the response model.

- **MinimalEndpoint&lt;TRequest, TResponse&gt;**: Has a request model, supports request validation and returns a response model.
``` csharp
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

internal class CreateCustomer
  : MinimalEndpoint<CreateCustomerRequest, Results<CreatedAtRoute<CreateCustomerResponse>, ValidationProblem, ProblemHttpResult>>
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    ConfigurationContext<EndpointConfigurationParameters> configurationContext)
  {
    builder.MapPost("/");
  }

  protected override async Task<Results<CreatedAtRoute<CreateCustomerResponse>, ValidationProblem, ProblemHttpResult>> HandleAsync(
    CreateCustomerRequest req,
    CancellationToken ct)
  {
    await Task.CompletedTask; // Simulate async work

    var customerId = Guid.NewGuid();
    return TypedResults.CreatedAtRoute(
      new CreateCustomerResponse(customerId),
      typeof(GetCustomerById).FullName,
      new { id = customerId });
  }
}
```

- **MinimalEndpoint&lt;TResponse&gt;**: Doesn't have a request model and returns a response model.
``` csharp
public record ListCustomersResponse(List<ListCustomersResponseItem> Customers);
public record ListCustomersResponseItem(
  Guid Id,
  string FirstName,
  string? MiddleName,
  string LastName);

internal class ListCustomers
  : MinimalEndpoint<ListCustomersResponse>
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    ConfigurationContext<EndpointConfigurationParameters> configurationContext)
  {
    builder.MapGet("/");
  }

  protected override async Task<ListCustomersResponse> HandleAsync(
    CancellationToken ct)
  {
    await Task.CompletedTask; // Simulate async work

    return new ListCustomersResponse(Customers:
      [
        new ListCustomersResponseItem(
            Id: Guid.NewGuid(),
            FirstName: "John",
            MiddleName: "Doe",
            LastName: "Smith"),
        new ListCustomersResponseItem(
          Id: Guid.NewGuid(),
          FirstName: "Jane",
          MiddleName: null,
          LastName: "Doe")
      ]);
  }
}
```

- **MinimalEndpointWithStreamingResponse&lt;TRequest, TResponse&gt;**: Has a request model, supports request validation and returns `IAsyncEnumerable<TResponse>`.
``` csharp
public record FilterAndStreamCustomerListRequest([FromBody] FilterAndStreamCustomerListRequestBody Body);

public record FilterAndStreamCustomerListRequestBody(string FirstName);

public record FilterAndStreamCustomerListResponse(
  Guid Id,
  string FirstName,
  string? MiddleName,
  string LastName);

internal class FilterAndStreamCustomerListRequestValidator : AbstractValidator<FilterAndStreamCustomerListRequest>
{
  public FilterAndStreamCustomerListRequestValidator()
  {
    RuleFor(x => x.Body.FirstName).NotEmpty();
  }
}

internal class FilterAndStreamCustomerList
  : MinimalEndpointWithStreamingResponse<FilterAndStreamCustomerListRequest, FilterAndStreamCustomerListResponse>
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    ConfigurationContext<EndpointConfigurationParameters> configurationContext)
  {
    builder.MapMethods("/filter-and-stream-list", [HttpMethod.Post.Method]);
  }

  protected override async IAsyncEnumerable<FilterAndStreamCustomerListResponse> HandleAsync(
    FilterAndStreamCustomerListRequest req,
    [EnumeratorCancellation] CancellationToken ct)
  {
    List<FilterAndStreamCustomerListResponse> customers =
      [
        new FilterAndStreamCustomerListResponse(
            Id: Guid.NewGuid(),
            FirstName: "John",
            MiddleName: "Doe",
            LastName: "Smith"),
        new FilterAndStreamCustomerListResponse(
          Id: Guid.NewGuid(),
          FirstName: "Jane",
          MiddleName: null,
          LastName: "Doe")
      ];

    foreach (var customer in customers.Where(c => c.FirstName == req.Body.FirstName))
    {
      yield return customer;
      await Task.Delay(1000, ct); // Simulate async work
    }
  }
}
```

- **MinimalEndpointWithStreamingResponse&lt;TResponse&gt;**: Doesn't have a request model and returns `IAsyncEnumerable<TResponse>`.
``` csharp
public record StreamCustomerListResponse(
  Guid Id,
  string FirstName,
  string? MiddleName,
  string LastName);

internal class StreamCustomerList
  : MinimalEndpointWithStreamingResponse<StreamCustomerListResponse>
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    ConfigurationContext<EndpointConfigurationParameters> configurationContext)
  {
    builder.MapMethods("/stream-list", [HttpMethod.Get.Method]);
  }

  protected override async IAsyncEnumerable<StreamCustomerListResponse> HandleAsync(
    [EnumeratorCancellation] CancellationToken ct)
  {
    List<StreamCustomerListResponse> customers =
      [
        new StreamCustomerListResponse(
            Id: Guid.NewGuid(),
            FirstName: "John",
            MiddleName: "Doe",
            LastName: "Smith"),
        new StreamCustomerListResponse(
          Id: Guid.NewGuid(),
          FirstName: "Jane",
          MiddleName: null,
          LastName: "Doe")
      ];

    foreach (var customer in customers)
    {
      yield return customer;
      await Task.Delay(1000, ct); // Simulate async work
    }
  }
}
```
