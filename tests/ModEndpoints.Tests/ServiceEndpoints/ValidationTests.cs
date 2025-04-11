using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using ModEndpoints.TestServer.Features.StoresWithServiceEndpoint;
using ModResults;

namespace ModEndpoints.Tests.ServiceEndpoints;

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
    var endpointUri = $"{typeof(GetStoreByIdRequest).FullName}.Endpoint";
    var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/storesWithServiceEndpoint/{endpointUri}")
    {
      Content = JsonContent.Create(new GetStoreByIdRequest(storeId))
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

  [Fact]
  public async Task InvalidParameter_ForEndpointOfTRequest_Returns_InvalidAsync()
  {
    var storeId = Guid.Empty;
    var endpointUri = $"{typeof(DeleteStoreRequest).FullName}.Endpoint";
    var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/storesWithServiceEndpoint/{endpointUri}")
    {
      Content = JsonContent.Create(new DeleteStoreRequest(storeId))
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
