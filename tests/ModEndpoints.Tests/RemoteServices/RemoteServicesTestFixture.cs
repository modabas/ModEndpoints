using Microsoft.Extensions.DependencyInjection;
using ModEndpoints.RemoteServices;
using ModEndpoints.TestServer.Features.StoresWithServiceEndpoint;

namespace ModEndpoints.Tests.RemoteServices;

public class RemoteServicesTestFixture
{
  public IServiceProvider ServiceProvider { get; init; }

  private static Lazy<RemoteServicesTestFixture> _instance =
    new Lazy<RemoteServicesTestFixture>(
      () => new RemoteServicesTestFixture(),
      LazyThreadSafetyMode.ExecutionAndPublication);

  public static RemoteServicesTestFixture Instance => _instance.Value;

  private RemoteServicesTestFixture()
  {
    var testClient = TestServerFixture.Instance.Client;

    var baseAddress = testClient.BaseAddress;
    var clientName = "IntegrationTests.Client";

    var services = new ServiceCollection();
    services.AddRemoteServicesWithNewClient(
      typeof(GetStoreByIdRequest).Assembly,
      clientName,
      (sp, client) =>
      {
        client.BaseAddress = baseAddress;
        client.Timeout = TimeSpan.FromSeconds(5);
      },
      builder =>
      {
        builder.ConfigurePrimaryHttpMessageHandler(() => testClient.GetHandler());
      });

    ServiceProvider = services.BuildServiceProvider();
  }
}
