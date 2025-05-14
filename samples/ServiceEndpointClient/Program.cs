// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModEndpoints.RemoteServices;
using ModResults;
using Polly;
using ShowcaseWebApi.FeatureContracts.Features.StoresWithServiceEndpoint;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

var baseAddress = "https://localhost:7012/api/";
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
  List<ListStoresResponse> stores = new();
  Console.WriteLine("***********************");
  //send request over channel to remote ServiceResultEndpoint
  await foreach (var listResult in channel.SendAsync(
    new ListStoresRequest(),
    "v1/storesWithServiceEndpoint/",
    default))
  {
    Console.WriteLine($"Streaming response type: {listResult.ResponseType}");
    if (listResult.Result.IsOk)
    {
      stores.Add(listResult.Result.Value);
    }
    else
    {
      Console.WriteLine(listResult.Result.DumpMessages());
    }
  }

  Console.WriteLine("***********************");
  Console.WriteLine($"ListStores complete. Total count: {stores.Count}");
  Console.WriteLine("***********************");
  var id = stores.FirstOrDefault()?.Id;
  if (id is not null)
  {
    //send request over channel to remote ServiceResultEndpoint
    var getResult = await channel.SendAsync(
      new GetStoreByIdRequest(Id: id.Value),
      "v1/storesWithServiceEndpoint/",
      default);
    Console.WriteLine("***********************");
    Console.WriteLine("GetStoreById response BEFORE delete:");
    if (getResult.IsOk)
    {
      Console.WriteLine(getResult.Value);
    }
    else
    {
      Console.WriteLine(getResult.DumpMessages());
    }
    Console.WriteLine("***********************");

    //send request over channel to remote ServiceResultEndpoint
    var deleteResult = await channel.SendAsync(
      new DeleteStoreRequest(Id: id.Value),
      "v1/storesWithServiceEndpoint/",
      default);
    Console.WriteLine("***********************");
    Console.WriteLine("DeleteStore response:");
    Console.WriteLine(deleteResult.DumpMessages());
    Console.WriteLine("***********************");

    //send request over channel to remote ServiceResultEndpoint
    getResult = await channel.SendAsync(
      new GetStoreByIdRequest(Id: id.Value),
      "v1/storesWithServiceEndpoint/",
      default);
    Console.WriteLine("***********************");
    Console.WriteLine("GetStoreById response AFTER delete:");
    if (getResult.IsOk)
    {
      Console.WriteLine(getResult.Value);
    }
    else
    {
      Console.WriteLine(getResult.DumpMessages());
    }
    Console.WriteLine("***********************");
  }

  Console.WriteLine();
}
