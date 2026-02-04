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
    var httpResponse = await _testClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

    Assert.True(httpResponse.IsSuccessStatusCode);
    Assert.Equal(StatusCodes.Status200OK, (int)httpResponse.StatusCode);

    var response = await JsonSerializer.DeserializeAsync<GetBookByIdResponse>(
      await httpResponse.Content.ReadAsStreamAsync(TestContext.Current.CancellationToken),
      _defaultJsonDeserializationOptions, TestContext.Current.CancellationToken);

    Assert.NotNull(response);
    Assert.Equal(bookId, response.Id);
    Assert.Equal("Book 1", response.Title);
    Assert.Equal("Author 1", response.Author);
    Assert.Equal(19.99m, response.Price);
  }

  [Fact]
  public async Task FailureEndpointWithEmptyRequestOfTResponse_Returns_ErrorAsync()
  {
    var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/books/failure/withResponseModel");
    var httpResponse = await _testClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

    Assert.False(httpResponse.IsSuccessStatusCode);
    Assert.Equal(StatusCodes.Status404NotFound, (int)httpResponse.StatusCode);
  }

  [Fact]
  public async Task FailureEndpointWithEmptyRequest_Returns_ErrorAsync()
  {
    var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/books/failure/withoutResponseModel");

    var httpResponse = await _testClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
    Assert.False(httpResponse.IsSuccessStatusCode);
    Assert.Equal(StatusCodes.Status404NotFound, (int)httpResponse.StatusCode);
  }

  [Fact]
  public async Task EndpointOfTRequest_Returns_SuccessAsync()
  {
    var bookId = Guid.NewGuid();
    var httpRequest = new HttpRequestMessage(HttpMethod.Put, $"/api/books/{bookId}")
    {
      Content = JsonContent.Create(new UpdateBookRequestBody("Title 1", "Author 1", 19.99m))
    };

    var httpResponse = await _testClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
    Assert.True(httpResponse.IsSuccessStatusCode);
    Assert.Equal(StatusCodes.Status204NoContent, (int)httpResponse.StatusCode);
  }

  [Fact]
  public async Task EndpointWithEmptyRequestOfTResponse_Returns_SuccessAsync()
  {
    var httpRequest = new HttpRequestMessage(HttpMethod.Get, "/api/books");
    var httpResponse = await _testClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

    Assert.True(httpResponse.IsSuccessStatusCode);
    Assert.Equal(StatusCodes.Status200OK, (int)httpResponse.StatusCode);

    var response = await JsonSerializer.DeserializeAsync<ListBooksResponse>(
      await httpResponse.Content.ReadAsStreamAsync(TestContext.Current.CancellationToken),
      _defaultJsonDeserializationOptions, TestContext.Current.CancellationToken);

    Assert.NotNull(response);
    Assert.NotNull(response.Books);
    Assert.Equal(2, response.Books.Count);
    var book2 = response.Books[1];
    Assert.NotNull(book2);
    Assert.Equal("Book 2", book2.Title);
    Assert.Equal("Author 2", book2.Author);
    Assert.Equal(29.99m, book2.Price);
  }

  [Fact]
  public async Task EndpointWithEmptyRequest_Returns_SuccessAsync()
  {
    var httpRequest = new HttpRequestMessage(HttpMethod.Delete, "/api/books");

    var httpResponse = await _testClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
    Assert.True(httpResponse.IsSuccessStatusCode);
    Assert.Equal(StatusCodes.Status204NoContent, (int)httpResponse.StatusCode);
  }

#if NET10_0_OR_GREATER
  [Fact]
  public async Task EndpointWithSseResponse_Returns_SuccessAsync()
  {
    var httpRequest = new HttpRequestMessage(HttpMethod.Get, "/api/books/listSse");
    var httpResponse = await _testClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

    Assert.True(httpResponse.IsSuccessStatusCode);
    Assert.Equal(StatusCodes.Status200OK, (int)httpResponse.StatusCode);

    List<System.Net.ServerSentEvents.SseItem<ListBooksResponseItem?>> response = new();
    await foreach (var item in System.Net.ServerSentEvents.SseParser.Create(
      await httpResponse.Content.ReadAsStreamAsync(TestContext.Current.CancellationToken),
      (_, bytes) => JsonSerializer.Deserialize<ListBooksResponseItem>(bytes, _defaultJsonDeserializationOptions))
      .EnumerateAsync(TestContext.Current.CancellationToken))
    {
      response.Add(item);
    }

    Assert.NotNull(response);
    Assert.Equal(2, response.Count);
    var book2 = response[1].Data;
    Assert.NotNull(book2);
    Assert.Equal("Book 2", book2.Title);
    Assert.Equal("Author 2", book2.Author);
    Assert.Equal(29.99m, book2.Price);
  }
#endif

  [Fact]
  public async Task EndpointWithStreamingResponse_Returns_SuccessAsync()
  {
    var httpRequest = new HttpRequestMessage(HttpMethod.Get, "/api/books/listWithStreamingResponse");
    var httpResponse = await _testClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

    Assert.True(httpResponse.IsSuccessStatusCode);
    Assert.Equal(StatusCodes.Status200OK, (int)httpResponse.StatusCode);

    var response = await JsonSerializer.DeserializeAsync<List<ListBooksResponseItem>>(
      await httpResponse.Content.ReadAsStreamAsync(TestContext.Current.CancellationToken),
      _defaultJsonDeserializationOptions, TestContext.Current.CancellationToken);

    Assert.NotNull(response);
    Assert.Equal(2, response.Count);
    var book2 = response[1];
    Assert.NotNull(book2);
    Assert.Equal("Book 2", book2.Title);
    Assert.Equal("Author 2", book2.Author);
    Assert.Equal(29.99m, book2.Price);
  }
}
