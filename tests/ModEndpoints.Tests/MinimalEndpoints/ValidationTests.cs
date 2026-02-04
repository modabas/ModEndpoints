using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using ModEndpoints.TestServer.Customers;

namespace ModEndpoints.Tests.MinimalEndpoints;

public class ValidationTests
{
  private readonly HttpClient _testClient;

  public ValidationTests()
  {
    _testClient = TestServerFixture.Instance.Client;
  }

  [Fact]
  public async Task InvalidRouteParameter_ForTypedResultEndpoint_Returns_InvalidAsync()
  {
    var customerId = Guid.Empty;
    var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/customers/{customerId}");
    var httpResponse = await _testClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

    Assert.False(httpResponse.IsSuccessStatusCode);
    Assert.Equal(StatusCodes.Status400BadRequest, (int)httpResponse.StatusCode);
  }

  [Fact]
  public async Task InvalidBodyParameter_ForTypedResultEndpoint_Returns_InvalidAsync()
  {
    var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/customers")
    {
      Content = JsonContent.Create(new CreateCustomerRequestBody("", null, ""))
    };

    var httpResponse = await _testClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
    Assert.False(httpResponse.IsSuccessStatusCode);
    Assert.Equal(StatusCodes.Status400BadRequest, (int)httpResponse.StatusCode);
  }

  [Fact]
  public async Task InvalidParameters_ForIResultEndpoint_Returns_InvalidAsync()
  {
    var customerId = Guid.Empty;
    var httpRequest = new HttpRequestMessage(HttpMethod.Put, $"/api/customers/{customerId}")
    {
      Content = JsonContent.Create(new UpdateCustomerRequestBody("", null, ""))
    };

    var httpResponse = await _testClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
    Assert.False(httpResponse.IsSuccessStatusCode);
    Assert.Equal(StatusCodes.Status400BadRequest, (int)httpResponse.StatusCode);
  }

  [Fact]
  public async Task InvalidParameters_ForTypedResponseEndpoint_ThrowsAsync()
  {
    var customerId = Guid.Empty;
    var httpRequest = new HttpRequestMessage(HttpMethod.Patch, $"/api/customers/{customerId}")
    {
      Content = JsonContent.Create(new PartialUpdateCustomerRequestBody(""))
    };

    var httpResponse = await _testClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
    Assert.False(httpResponse.IsSuccessStatusCode);
    Assert.Equal(StatusCodes.Status500InternalServerError, (int)httpResponse.StatusCode);
  }

  [Fact]
  public async Task InvalidParameters_ForStreamingResponseEndpoint_ThrowsAsync()
  {
    var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/customers/filter-and-stream-list")
    {
      Content = JsonContent.Create(new FilterAndStreamCustomerListRequestBody(""))
    };

    var httpResponse = await _testClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
    Assert.False(httpResponse.IsSuccessStatusCode);
    Assert.Equal(StatusCodes.Status500InternalServerError, (int)httpResponse.StatusCode);
  }
}
