using Microsoft.AspNetCore.Mvc.Testing;

namespace ModEndpoints.Tests;

public class TestServerFixture
{
  public HttpClient Client { get; init; }

  private static Lazy<TestServerFixture> _instance =
    new Lazy<TestServerFixture>(
      () => new TestServerFixture(),
      LazyThreadSafetyMode.ExecutionAndPublication);

  public static TestServerFixture Instance => _instance.Value;

  private TestServerFixture()
  {
    var factory = new WebApplicationFactory<Program>();
    Client = factory.CreateClient();
  }
}
