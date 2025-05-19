using Microsoft.EntityFrameworkCore;
using ModEndpoints.Core;
using ShowcaseWebApi.Data;
using ShowcaseWebApi.Features.Customers.Configuration;

namespace ShowcaseWebApi.Features.Customers;

public record ListCustomersResponse(
  Guid Id,
  string FirstName,
  string? MiddleName,
  string LastName);

[MapToGroup<CustomersV1RouteGroup>()]
internal class ListCustomers(ServiceDbContext db)
  : MinimalEndpointWithStreamingResponse<ListCustomersResponse>
{
  protected override void Configure(
    ConfigurationContext<IEndpointConfiguration> configurationContext)
  {
    MapGet("/");
  }

  protected override IAsyncEnumerable<ListCustomersResponse> HandleAsync(CancellationToken ct)
  {
    var customers = db.Customers
      .Select(c => new ListCustomersResponse(
        c.Id,
        c.FirstName,
        c.MiddleName,
        c.LastName))
      .AsAsyncEnumerable();

    return customers;
  }
}
