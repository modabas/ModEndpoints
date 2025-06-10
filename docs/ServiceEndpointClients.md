# ServiceEndpoint Clients

>**Package**: `ModEndpoints.RemoteServices`

A client application can consume ServiceEndpoints by utilizing a specialized message channel resolved from dependency injection. Only service's base address and endpoint's request/response model information are needed. No other client implementation or service specific knowledge is required.

## ⚙️ Workflow

The workflow for consuming ServiceEndpoints from a client application consists of two main steps: **registration** and **consumption**. 

### Registration

A client application registers service endpoints with a specific HttpClient configuration (such as base address, timeout, handlers, resilience policies, etc.) during application startup. This can be performed for all service requests within an assembly or for each request type individually.

To register all service requests in an assembly at once:

```csharp
var baseAddress = "https://...";
var clientName = "MyClient";
builder.Services.AddRemoteServicesWithNewClient(
  typeof(ListStoresRequest).Assembly,
  clientName,
  (sp, client) =>
  {
    client.BaseAddress = new Uri(baseAddress);
    client.Timeout = TimeSpan.FromSeconds(5);
  },
  clientBuilder => clientBuilder.AddTransientHttpErrorPolicy(
    policyBuilder => policyBuilder.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30))));
```

...or to register service requests individually:

```csharp
var baseAddress = "https://...";
var clientName = "MyClient";
builder.Services.AddRemoteServiceWithNewClient<ListStoresRequest>(clientName,
  (sp, client) =>
  {
    client.BaseAddress = new Uri(baseAddress);
    client.Timeout = TimeSpan.FromSeconds(5);
  },
  clientBuilder => clientBuilder.AddTransientHttpErrorPolicy(
    policyBuilder => policyBuilder.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30))));
builder.Services.AddRemoteServiceToExistingClient<GetStoreByIdRequest>(clientName);
builder.Services.AddRemoteServiceToExistingClient<DeleteStoreRequest>(clientName);
builder.Services.AddRemoteServiceToExistingClient<CreateStoreRequest>(clientName);
builder.Services.AddRemoteServiceToExistingClient<UpdateStoreRequest>(clientName);
```
>**Notes**:
>1. The client name is used to resolve the correct HttpClient configuration for each request type.
>2. Service channel utilizes IHttpClientFactory and HttpClient underneath and is configured similarly.

### Comsumption

Remote `ServiceEndpoints` can be called by resolving an `IServiceChannel` instance from dependency injection and invoking the `SendAsync` method with the desired request object.

```csharp
  using IServiceScope serviceScope = hostProvider.CreateScope();
  IServiceProvider provider = serviceScope.ServiceProvider;

  //resolve service channel from DI
  var channel = provider.GetRequiredService<IServiceChannel>();
  //send request over channel to remote service
  var listResult = await channel.SendAsync(new ListStoresRequest(), ct);

```

## 📚 Samples
1. [ServiceEndpoint implementations](../samples/ShowcaseWebApi/Features/StoresWithServiceEndpoint)
2. [Client implementation](../samples/ServiceEndpointClient)
3. [Shared library for request/response models](../samples/ShowcaseWebApi.FeatureContracts).

