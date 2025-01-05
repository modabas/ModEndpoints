// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModEndpoints.RemoteServices;
using ModResults;
using Polly;
using ShowcaseWebApi.FeatureContracts.Features.StoresWithServiceEndpoint;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

var baseAddress = "https://localhost:7012/api/v1/storesWithServiceEndpoint/";
var clientName = "ShowcaseApi.Client";
//builder.Services.AddRemoteServiceWithNewClient<ListStoresRequest>(clientName,
//  (sp, client) =>
//  {
//    client.BaseAddress = new Uri(baseAddress);
//    client.Timeout = TimeSpan.FromSeconds(5);
//  },
//  clientBuilder => clientBuilder.AddTransientHttpErrorPolicy(
//    policyBuilder => policyBuilder.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30))));
//builder.Services.AddRemoteServiceToExistingClient<GetStoreByIdRequest>(clientName);
//builder.Services.AddRemoteServiceToExistingClient<DeleteStoreRequest>(clientName);
//builder.Services.AddRemoteServiceToExistingClient<CreateStoreRequest>(clientName);
//builder.Services.AddRemoteServiceToExistingClient<UpdateStoreRequest>(clientName);
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

using IHost host = builder.Build();

await CallRemoteServicesAsync(host.Services);

await host.RunAsync();

static async Task CallRemoteServicesAsync(IServiceProvider hostProvider)
{
  using IServiceScope serviceScope = hostProvider.CreateScope();
  IServiceProvider provider = serviceScope.ServiceProvider;

  //resolve service channel from DI
  var channel = provider.GetRequiredService<IServiceChannel>();
  //send request over channel to remote ServiceResultEndpoint
  var listResult = await channel.SendAsync<ListStoresRequest, ListStoresResponse>(new ListStoresRequest(), default);

  if (listResult.IsOk)
  {
    Console.WriteLine($"ListStores complete. Total count: {listResult.Value.Stores.Count}");
    var id = listResult.Value.Stores.FirstOrDefault()?.Id;
    if (id is not null)
    {
      //send request over channel to remote ServiceResultEndpoint
      var getResult = await channel.SendAsync<GetStoreByIdRequest, GetStoreByIdResponse>(new GetStoreByIdRequest(Id: id.Value), default);
      if (getResult.IsOk)
      {
        Console.WriteLine(getResult.Value);
      }
      else
      {
        Console.WriteLine(getResult.DumpMessages());
      }
    }
  }
  else
  {
    Console.WriteLine(listResult.DumpMessages());
  }

  Console.WriteLine();
}
