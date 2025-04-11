using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using ModEndpoints.TestServer.Features.Stores;
using ModResults;

namespace ModEndpoints.Tests.BusinessResultEndpoints;

public class ValidationTests
{
  private readonly HttpClient _testClient;

  private static readonly JsonSerializerOptions _defaultJsonDeserializationOptions = new()
  {
    PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    NumberHandling = JsonNumberHandling.AllowReadingFromString
  };

  public ValidationTests()
  {
    _testClient = TestServerFixture.Instance.Client;
  }

  [Fact]
  public async Task InvalidParameter_ForEndpointOfTRequestAndTResponse_Returns_InvalidAsync()
  {
    var storeId = Guid.Empty;
    var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/stores/{storeId}");
    var httpResponse = await _testClient.SendAsync(httpRequest);

    Assert.True(httpResponse.IsSuccessStatusCode);
    Assert.Equal(StatusCodes.Status200OK, (int)httpResponse.StatusCode);

    var responseContent = await httpResponse.Content.ReadAsStringAsync();
    var response = await JsonSerializer.DeserializeAsync<Result>(
      await httpResponse.Content.ReadAsStreamAsync(),
      _defaultJsonDeserializationOptions);

    Assert.NotNull(response);
    Assert.False(response.IsOk);
    Assert.True(response.IsFailed);
    Assert.NotNull(response.Failure);
    Assert.Equal(FailureType.Invalid, response.Failure.Type);
  }

  [Fact]
  public async Task InvalidParameter_ForEndpointOfTRequest_Returns_InvalidAsync()
  {
    var storeId = Guid.Empty;
    var httpRequest = new HttpRequestMessage(HttpMethod.Put, $"/api/stores/{storeId}")
    {
      Content = JsonContent.Create(new UpdateStoreRequestBody(""))
    };

    var httpResponse = await _testClient.SendAsync(httpRequest);
    Assert.True(httpResponse.IsSuccessStatusCode);
    Assert.Equal(StatusCodes.Status200OK, (int)httpResponse.StatusCode);

    var responseContent = await httpResponse.Content.ReadAsStringAsync();
    var response = await JsonSerializer.DeserializeAsync<Result>(
      await httpResponse.Content.ReadAsStreamAsync(),
      _defaultJsonDeserializationOptions);

    Assert.NotNull(response);
    Assert.False(response.IsOk);
    Assert.True(response.IsFailed);
    Assert.NotNull(response.Failure);
    Assert.Equal(FailureType.Invalid, response.Failure.Type);
  }
}
