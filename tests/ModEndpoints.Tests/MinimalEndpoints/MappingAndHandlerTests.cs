﻿using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using ModEndpoints.TestServer.Customers;

namespace ModEndpoints.Tests.MinimalEndpoints;

public class MappingAndHandlerTests
{
  private readonly HttpClient _testClient;

  private static readonly JsonSerializerOptions _defaultJsonDeserializationOptions = new()
  {
    PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    NumberHandling = JsonNumberHandling.AllowReadingFromString
  };

  public MappingAndHandlerTests()
  {
    _testClient = TestServerFixture.Instance.Client;
  }

  [Fact]
  public async Task Get_Returns_SuccessAsync()
  {
    var customerId = Guid.NewGuid();
    var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/customers/{customerId}");
    var httpResponse = await _testClient.SendAsync(httpRequest);

    Assert.True(httpResponse.IsSuccessStatusCode);
    Assert.Equal(StatusCodes.Status200OK, (int)httpResponse.StatusCode);

    var response = await JsonSerializer.DeserializeAsync<GetCustomerByIdResponse>(
      await httpResponse.Content.ReadAsStreamAsync(),
      _defaultJsonDeserializationOptions);

    Assert.NotNull(response);
    Assert.Equal(customerId, response.Id);
    Assert.Equal("FirstName", response.FirstName);
    Assert.Equal("MiddleName", response.MiddleName);
    Assert.Equal("LastName", response.LastName);
  }

  [Fact]
  public async Task GetWithEmptyRequest_Returns_SuccessAsync()
  {
    var httpRequest = new HttpRequestMessage(HttpMethod.Get, "/api/customers");
    var httpResponse = await _testClient.SendAsync(httpRequest);

    Assert.True(httpResponse.IsSuccessStatusCode);
    Assert.Equal(StatusCodes.Status200OK, (int)httpResponse.StatusCode);

    var response = await JsonSerializer.DeserializeAsync<ListCustomersResponse>(
      await httpResponse.Content.ReadAsStreamAsync(),
      _defaultJsonDeserializationOptions);

    Assert.NotNull(response);
    Assert.NotNull(response.Customers);
    Assert.Equal(2, response.Customers.Count);
    var customer2 = response.Customers[1];
    Assert.NotNull(customer2);
    Assert.Equal("Jane", customer2.FirstName);
    Assert.Null(customer2.MiddleName);
    Assert.Equal("Doe", customer2.LastName);
  }

  [Fact]
  public async Task GetStreamingResponseWithEmptyRequest_Returns_SuccessAsync()
  {
    var httpRequest = new HttpRequestMessage(HttpMethod.Get, "/api/customers/stream-list");
    var httpResponse = await _testClient.SendAsync(httpRequest);

    Assert.True(httpResponse.IsSuccessStatusCode);
    Assert.Equal(StatusCodes.Status200OK, (int)httpResponse.StatusCode);

    var response = await JsonSerializer.DeserializeAsync<List<StreamCustomerListResponse>>(
      await httpResponse.Content.ReadAsStreamAsync(),
      _defaultJsonDeserializationOptions);

    Assert.NotNull(response);
    Assert.Equal(2, response.Count);
    var customer2 = response[1];
    Assert.NotNull(customer2);
    Assert.Equal("Jane", customer2.FirstName);
    Assert.Null(customer2.MiddleName);
    Assert.Equal("Doe", customer2.LastName);
  }

  [Fact]
  public async Task Post_Returns_SuccessAsync()
  {
    var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/customers")
    {
      Content = JsonContent.Create(new CreateCustomerRequestBody("John", "Doe", "Smith"))
    };

    var httpResponse = await _testClient.SendAsync(httpRequest);
    Assert.True(httpResponse.IsSuccessStatusCode);
    Assert.Equal(StatusCodes.Status201Created, (int)httpResponse.StatusCode);

    var locationHeader = httpResponse.Headers.Location;
    Assert.NotNull(locationHeader);
    Assert.NotEmpty(locationHeader.ToString());

    var response = await JsonSerializer.DeserializeAsync<CreateCustomerResponse>(
      await httpResponse.Content.ReadAsStreamAsync(),
      _defaultJsonDeserializationOptions);

    Assert.NotNull(response);
    Assert.NotEqual(Guid.Empty, response.Id);
  }

  [Fact]
  public async Task PostStreamingResponse_Returns_SuccessAsync()
  {
    var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/customers/filter-and-stream-list")
    {
      Content = JsonContent.Create(new FilterAndStreamCustomerListRequestBody("Jane"))
    };
    var httpResponse = await _testClient.SendAsync(httpRequest);

    Assert.True(httpResponse.IsSuccessStatusCode);
    Assert.Equal(StatusCodes.Status200OK, (int)httpResponse.StatusCode);

    var response = await JsonSerializer.DeserializeAsync<List<FilterAndStreamCustomerListResponse>>(
      await httpResponse.Content.ReadAsStreamAsync(),
      _defaultJsonDeserializationOptions);

    Assert.NotNull(response);
    Assert.Single(response);
    var customer = response[0];
    Assert.NotNull(customer);
    Assert.Equal("Jane", customer.FirstName);
    Assert.Null(customer.MiddleName);
    Assert.Equal("Doe", customer.LastName);
  }

  [Fact]
  public async Task Put_Returns_SuccessAsync()
  {
    var customerId = Guid.NewGuid();
    var httpRequest = new HttpRequestMessage(HttpMethod.Put, $"/api/customers/{customerId}")
    {
      Content = JsonContent.Create(new UpdateCustomerRequestBody("John", "Doe", "Smith"))
    };

    var httpResponse = await _testClient.SendAsync(httpRequest);
    Assert.True(httpResponse.IsSuccessStatusCode);
    Assert.Equal(StatusCodes.Status200OK, (int)httpResponse.StatusCode);

    var response = await JsonSerializer.DeserializeAsync<UpdateCustomerResponse>(
      await httpResponse.Content.ReadAsStreamAsync(),
      _defaultJsonDeserializationOptions);

    Assert.NotNull(response);
    Assert.Equal(customerId, response.Id);
    Assert.Equal("John", response.FirstName);
    Assert.Equal("Doe", response.MiddleName);
    Assert.Equal("Smith", response.LastName);
  }

  [Fact]
  public async Task Patch_Returns_SuccessAsync()
  {
    var customerId = Guid.NewGuid();
    var httpRequest = new HttpRequestMessage(HttpMethod.Patch, $"/api/customers/{customerId}")
    {
      Content = JsonContent.Create(new PartialUpdateCustomerRequestBody("John"))
    };

    var httpResponse = await _testClient.SendAsync(httpRequest);
    Assert.True(httpResponse.IsSuccessStatusCode);
    Assert.Equal(StatusCodes.Status200OK, (int)httpResponse.StatusCode);

    var response = await JsonSerializer.DeserializeAsync<PartialUpdateCustomerResponse>(
      await httpResponse.Content.ReadAsStreamAsync(),
      _defaultJsonDeserializationOptions);

    Assert.NotNull(response);
    Assert.Equal(customerId, response.Id);
    Assert.Equal("John", response.FirstName);
  }

  [Fact]
  public async Task Delete_Returns_SuccessAsync()
  {
    var customerId = Guid.NewGuid();
    var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"/api/customers/{customerId}");

    var httpResponse = await _testClient.SendAsync(httpRequest);
    Assert.True(httpResponse.IsSuccessStatusCode);
    Assert.Equal(StatusCodes.Status204NoContent, (int)httpResponse.StatusCode);
  }

  [Fact]
  public async Task DisabledEndpoint_IsNot_MappedAsync()
  {
    var httpRequest = new HttpRequestMessage(HttpMethod.Get, "/api/customers/disabled");

    var httpResponse = await _testClient.SendAsync(httpRequest);
    Assert.False(httpResponse.IsSuccessStatusCode);
    Assert.Equal(StatusCodes.Status400BadRequest, (int)httpResponse.StatusCode);
  }
}
