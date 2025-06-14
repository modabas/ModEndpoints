﻿using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using ModEndpoints.RemoteServices;
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

    var response = await JsonSerializer.DeserializeAsync<Result<GetStoreByIdResponse>>(
      await httpResponse.Content.ReadAsStreamAsync(),
      _defaultJsonDeserializationOptions);

    Assert.NotNull(response);
    Assert.True(response.IsOk);
    Assert.False(response.IsFailed);
    Assert.NotNull(response.Value);
    Assert.Equal(storeId, response.Value.Id);
    Assert.Equal("Name 1", response.Value.Name);
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

    var response = await JsonSerializer.DeserializeAsync<Result>(
      await httpResponse.Content.ReadAsStreamAsync(),
      _defaultJsonDeserializationOptions);

    Assert.NotNull(response);
    Assert.True(response.IsOk);
    Assert.False(response.IsFailed);
  }

  [Fact]
  public async Task StreamingEndpointOfTRequestAndTResponse_Returns_SuccessAsync()
  {
    var endpointUri = $"{typeof(FilterAndStreamStoreListRequest).FullName}.Endpoint";
    var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/storesWithServiceEndpoint/{endpointUri}")
    {
      Content = JsonContent.Create(new FilterAndStreamStoreListRequest("Name 2"))
    };

    var httpResponse = await _testClient.SendAsync(httpRequest);

    Assert.True(httpResponse.IsSuccessStatusCode);
    Assert.Equal(StatusCodes.Status200OK, (int)httpResponse.StatusCode);

    var response = await JsonSerializer.DeserializeAsync<List<StreamingResponseItem<FilterAndStreamStoreListResponse>>>(
      await httpResponse.Content.ReadAsStreamAsync(),
      _defaultJsonDeserializationOptions);

    Assert.NotNull(response);
    Assert.Single(response);
    var store = response[0].Result.Value;
    Assert.NotNull(store);
    Assert.Equal("Name 2", store.Name);
  }

  [Fact]
  public async Task StreamingEndpointOfTRequest_Returns_SuccessAsync()
  {
    var endpointUri = $"{typeof(StreamStoreStatusListRequest).FullName}.Endpoint";
    var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/storesWithServiceEndpoint/{endpointUri}")
    {
      Content = JsonContent.Create(new StreamStoreStatusListRequest("Name"))
    };

    var httpResponse = await _testClient.SendAsync(httpRequest);

    Assert.True(httpResponse.IsSuccessStatusCode);
    Assert.Equal(StatusCodes.Status200OK, (int)httpResponse.StatusCode);

    var response = await JsonSerializer.DeserializeAsync<List<StreamingResponseItem>>(
      await httpResponse.Content.ReadAsStreamAsync(),
      _defaultJsonDeserializationOptions);

    Assert.NotNull(response);
    Assert.Equal(2, response.Count);
    Assert.True(response[0].Result.IsOk);
    Assert.True(response[1].Result.IsOk);
  }
}
