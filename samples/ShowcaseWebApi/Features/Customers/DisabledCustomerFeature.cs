using ModEndpoints.Core;
using ModEndpoints.RemoteServices.Shared;
using ShowcaseWebApi.Features.Customers.Configuration;

namespace ShowcaseWebApi.Features.Customers;

[MapToGroup<CustomersV1RouteGroup>()]
[DoNotRegister]
internal class DisabledCustomerFeature
  : MinimalEndpoint<IResult>
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    EndpointConfigurationContext configurationContext)
  {
    builder.MapGet("/disabled/");
  }

  protected override Task<IResult> HandleAsync(
    CancellationToken ct)
  {
    throw new NotImplementedException();
  }
}
