using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using ModEndpoints.TestServer.Features.Books;
using ModResults;

namespace ModEndpoints.Tests.WebResultEndpoints;

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
    var bookId = Guid.NewGuid();
    var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/books/{bookId}");
    var httpResponse = await _testClient.SendAsync(httpRequest);

    Assert.True(httpResponse.IsSuccessStatusCode);
    Assert.Equal(StatusCodes.Status200OK, (int)httpResponse.StatusCode);

    var responseContent = await httpResponse.Content.ReadAsStringAsync();
    var response = await JsonSerializer.DeserializeAsync<GetBookByIdResponse>(
      await httpResponse.Content.ReadAsStreamAsync(),
      _defaultJsonDeserializationOptions);

    Assert.NotNull(response);
    Assert.Equal(bookId, response.Id);
  }

  [Fact]
  public async Task EndpointWithEmptyRequestOfTResponse_Returns_SuccessAsync()
  {
    var httpRequest = new HttpRequestMessage(HttpMethod.Get, "/api/books");
    var httpResponse = await _testClient.SendAsync(httpRequest);

    Assert.True(httpResponse.IsSuccessStatusCode);
    Assert.Equal(StatusCodes.Status200OK, (int)httpResponse.StatusCode);

    var responseContent = await httpResponse.Content.ReadAsStringAsync();
    var response = await JsonSerializer.DeserializeAsync<ListBooksResponse>(
      await httpResponse.Content.ReadAsStreamAsync(),
      _defaultJsonDeserializationOptions);

    Assert.NotNull(response);
    Assert.NotNull(response.Books);
    Assert.Equal(2, response.Books.Count);
  }

  [Fact]
  public async Task EndpointWithEmptyRequest_Returns_SuccessAsync()
  {
    var httpRequest = new HttpRequestMessage(HttpMethod.Delete, "/api/books");

    var httpResponse = await _testClient.SendAsync(httpRequest);
    Assert.True(httpResponse.IsSuccessStatusCode);
    Assert.Equal(StatusCodes.Status204NoContent, (int)httpResponse.StatusCode);
  }

  [Fact]
  public async Task EndpointOfTRequest_Returns_SuccessAsync()
  {
    var bookId = Guid.NewGuid();
    var httpRequest = new HttpRequestMessage(HttpMethod.Put, $"/api/books/{bookId}")
    {
      Content = JsonContent.Create(new UpdateBookRequestBody("Title 1", "Author 1", 19.99m))
    };

    var httpResponse = await _testClient.SendAsync(httpRequest);
    Assert.True(httpResponse.IsSuccessStatusCode);
    Assert.Equal(StatusCodes.Status204NoContent, (int)httpResponse.StatusCode);
  }
}
