using System.Runtime.CompilerServices;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ModEndpoints.Core;
using ModEndpoints.TestServer.Features.Customers.Configuration;

namespace ModEndpoints.TestServer.Features.Customers;

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

[MapToGroup<CustomersRouteGroup>()]
internal class FilterAndStreamCustomerList
  : MinimalEndpointWithStreamingResponse<FilterAndStreamCustomerListRequest, FilterAndStreamCustomerListResponse>
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    ConfigurationContext<IEndpointConfigurationSettings> configurationContext)
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
