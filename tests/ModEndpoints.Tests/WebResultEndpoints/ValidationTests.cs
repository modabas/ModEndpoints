using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using ModEndpoints.TestServer.Features.Books;

namespace ModEndpoints.Tests.WebResultEndpoints;

public class ValidationTests
{
  private readonly HttpClient _testClient;

  public ValidationTests()
  {
    _testClient = TestServerFixture.Instance.Client;
  }

  [Fact]
  public async Task InvalidParameter_ForEndpointOfTRequestAndTResponse_Returns_InvalidAsync()
  {
    var bookId = Guid.Empty;
    var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/books/{bookId}");
    var httpResponse = await _testClient.SendAsync(httpRequest);

    Assert.False(httpResponse.IsSuccessStatusCode);
    Assert.Equal(StatusCodes.Status400BadRequest, (int)httpResponse.StatusCode);
  }

  [Fact]
  public async Task InvalidParameter_ForEndpointOfTRequest_Returns_InvalidAsync()
  {
    var bookId = Guid.Empty;
    var httpRequest = new HttpRequestMessage(HttpMethod.Put, $"/api/books/{bookId}")
    {
      Content = JsonContent.Create(new UpdateBookRequestBody("", "", 0))
    };

    var httpResponse = await _testClient.SendAsync(httpRequest);
    Assert.False(httpResponse.IsSuccessStatusCode);
    Assert.Equal(StatusCodes.Status400BadRequest, (int)httpResponse.StatusCode);
  }
}
