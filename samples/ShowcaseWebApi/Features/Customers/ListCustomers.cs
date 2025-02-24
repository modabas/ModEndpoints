using Microsoft.EntityFrameworkCore;
using ModEndpoints.Core;
using ShowcaseWebApi.Data;
using ShowcaseWebApi.Features.Customers.Configuration;

namespace ShowcaseWebApi.Features.Customers;

public record ListCustomersResponse(List<ListCustomersResponseItem> Customers);
public record ListCustomersResponseItem(
  Guid Id,
  string FirstName,
  string? MiddleName,
  string LastName);

[MapToGroup<CustomersV1RouteGroup>()]
internal class ListCustomers(ServiceDbContext db)
  : MinimalEndpoint<ListCustomersResponse>
{
  protected override void Configure(
    IServiceProvider serviceProvider,
    IRouteGroupConfigurator? parentRouteGroup)
  {
    MapGet("/");
  }

  protected override async Task<ListCustomersResponse> HandleAsync(
    CancellationToken ct)
  {
    var customers = await db.Customers
      .Select(c => new ListCustomersResponseItem(
        c.Id,
        c.FirstName,
        c.MiddleName,
        c.LastName))
      .ToListAsync(ct);

    return new ListCustomersResponse(Customers: customers);
  }
}
