# Disabling Components

DoNotRegister attribute can be used to prevent targeted route group (including its children) or endpoint to be registered during server application startup.

Also prevents targeted service endpoint request to be registered during service endpoint client application startup.

```csharp
[MapToGroup<CustomersV1RouteGroup>()]
[DoNotRegister]
internal class DisabledCustomerFeature
  : MinimalEndpoint<IResult>
{
  protected override void Configure(
    IServiceProvider serviceProvider,
    IRouteGroupConfigurator? parentRouteGroup)
  {
    MapGet("/disabled/");
  }

  protected override Task<IResult> HandleAsync(
    CancellationToken ct)
  {
    throw new NotImplementedException();
  }
}
```
