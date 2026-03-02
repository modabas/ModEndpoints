using System.Runtime.CompilerServices;
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
    EndpointConfigurationBuilder builder,
    EndpointConfigurationContext configurationContext)
  {
    builder.MapGet("/");
  }

  protected override IAsyncEnumerable<ListCustomersResponse> HandleAsync(CancellationToken ct)
  {
    return GetCustomers(ct);

    async IAsyncEnumerable<ListCustomersResponse> GetCustomers(
      [EnumeratorCancellation] CancellationToken ct)
    {
      await foreach (var customer in db.Customers
        .Select(c => new ListCustomersResponse(
          c.Id,
          c.FirstName,
          c.MiddleName,
          c.LastName))
        .AsAsyncEnumerable()
        .WithCancellation(ct))
      {
        await Task.Delay(1000, ct);
        yield return customer;
      }
    }
  }
}
