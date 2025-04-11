using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using ModEndpoints.TestServer.Features.StoresWithServiceEndpoint;
using ModResults;

namespace ModEndpoints.Tests.ServiceEndpoints;

public class HandlerTests
{
  private readonly HttpClient _testClient;

  private static readonly JsonSerializerOptions _defaultJsonDeserializationOptions = new()
  {
    PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    NumberHandling = JsonNumberHandling.AllowReadingFromString
  };

  public HandlerTests()
  {
    _testClient = TestServerFixture.Instance.Client;
  }

  [Fact]
  public async Task EndpointOfTRequestAndTResponse_Returns_SuccessAsync()
  {
    var storeId = Guid.NewGuid();
    var endpointUri = $"{typeof(GetStoreByIdRequest).FullName}.Endpoint";
    var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/storesWithServiceEndpoint/{endpointUri}")
    {
      Content = JsonContent.Create(new GetStoreByIdRequest(storeId))
    };
    var httpResponse = await _testClient.SendAsync(httpRequest);

    Assert.True(httpResponse.IsSuccessStatusCode);
    Assert.Equal(StatusCodes.Status200OK, (int)httpResponse.StatusCode);

    var responseContent = await httpResponse.Content.ReadAsStringAsync();
    var response = await JsonSerializer.DeserializeAsync<Result<GetStoreByIdResponse>>(
      await httpResponse.Content.ReadAsStreamAsync(),
      _defaultJsonDeserializationOptions);

    Assert.NotNull(response);
    Assert.True(response.IsOk);
    Assert.False(response.IsFailed);
    Assert.NotNull(response.Value);
    Assert.Equal(storeId, response.Value.Id);
  }

  [Fact]
  public async Task EndpointOfTRequest_Returns_SuccessAsync()
  {
    var storeId = Guid.NewGuid();
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
    Assert.True(response.IsOk);
    Assert.False(response.IsFailed);
  }
}
