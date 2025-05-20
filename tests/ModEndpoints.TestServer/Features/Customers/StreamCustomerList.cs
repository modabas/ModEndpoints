using System.Runtime.CompilerServices;
using ModEndpoints.Core;
using ModEndpoints.TestServer.Features.Customers.Configuration;

namespace ModEndpoints.TestServer.Features.Customers;

public record StreamCustomerListResponse(
  Guid Id,
  string FirstName,
  string? MiddleName,
  string LastName);

[MapToGroup<CustomersRouteGroup>()]
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
