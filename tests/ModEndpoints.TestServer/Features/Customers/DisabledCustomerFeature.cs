using ModEndpoints.Core;
using ModEndpoints.RemoteServices.Core;
using ModEndpoints.TestServer.Features.Customers.Configuration;

namespace ModEndpoints.TestServer.Features.Customers;

[MapToGroup<CustomersRouteGroup>()]
[DoNotRegister]
internal class DisabledCustomerFeature
  : MinimalEndpoint<IResult>
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    ConfigurationContext<EndpointConfigurationParameters> configurationContext)
  {
    builder.MapGet("/disabled");
  }

  protected override Task<IResult> HandleAsync(
    CancellationToken ct)
  {
    throw new NotImplementedException();
  }
}
