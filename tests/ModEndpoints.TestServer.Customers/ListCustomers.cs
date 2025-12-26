using ModEndpoints.Core;
using ModEndpoints.TestServer.Customers.Configuration;

namespace ModEndpoints.TestServer.Customers;

public record ListCustomersResponse(List<ListCustomersResponseItem> Customers);
public record ListCustomersResponseItem(
  Guid Id,
  string FirstName,
  string? MiddleName,
  string LastName);

[MapToGroup<CustomersRouteGroup>()]
internal class ListCustomers
  : MinimalEndpoint<ListCustomersResponse>
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    EndpointConfigurationContext configurationContext)
  {
    builder.MapMethods("/", [HttpMethod.Get.Method]);
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
