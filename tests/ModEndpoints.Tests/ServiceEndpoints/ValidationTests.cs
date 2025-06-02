using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using ModEndpoints.RemoteServices;
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

  [Fact]
  public async Task InvalidParameters_ForStreamingEndpointOfTRequestAndTResponse_ReturnsInvalidAsync()
  {
    var endpointUri = $"{typeof(FilterAndStreamStoreListRequest).FullName}.Endpoint";
    var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/storesWithServiceEndpoint/{endpointUri}")
    {
      Content = JsonContent.Create(new FilterAndStreamStoreListRequest(""))
    };

    var httpResponse = await _testClient.SendAsync(httpRequest);
    Assert.True(httpResponse.IsSuccessStatusCode);
    Assert.Equal(StatusCodes.Status200OK, (int)httpResponse.StatusCode);

    var responseContent = await httpResponse.Content.ReadAsStringAsync();
    var response = await JsonSerializer.DeserializeAsync<List<StreamingResponseItem<FilterAndStreamStoreListResponse>>>(
      await httpResponse.Content.ReadAsStreamAsync(),
      _defaultJsonDeserializationOptions);

    Assert.NotNull(response);
    Assert.Single(response);
    Assert.False(response[0].Result.IsOk);
    Assert.True(response[0].Result.IsFailed);
    var failure = response[0].Result.Failure;
    Assert.NotNull(failure);
    Assert.Equal(FailureType.Invalid, failure.Type);
  }

  [Fact]
  public async Task InvalidParameters_ForStreamingEndpointOfTRequest_ReturnsInvalidAsync()
  {
    var endpointUri = $"{typeof(StreamStoreStatusListRequest).FullName}.Endpoint";
    var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/storesWithServiceEndpoint/{endpointUri}")
    {
      Content = JsonContent.Create(new StreamStoreStatusListRequest(""))
    };

    var httpResponse = await _testClient.SendAsync(httpRequest);
    Assert.True(httpResponse.IsSuccessStatusCode);
    Assert.Equal(StatusCodes.Status200OK, (int)httpResponse.StatusCode);

    var responseContent = await httpResponse.Content.ReadAsStringAsync();
    var response = await JsonSerializer.DeserializeAsync<List<StreamingResponseItem>>(
      await httpResponse.Content.ReadAsStreamAsync(),
      _defaultJsonDeserializationOptions);

    Assert.NotNull(response);
    Assert.Single(response);
    Assert.False(response[0].Result.IsOk);
    Assert.True(response[0].Result.IsFailed);
    var failure = response[0].Result.Failure;
    Assert.NotNull(failure);
    Assert.Equal(FailureType.Invalid, failure.Type);
  }

}
