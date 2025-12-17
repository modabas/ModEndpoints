using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;
using ModEndpoints.RemoteServices;
using ModEndpoints.RemoteServices.Shared;
using ModResults;

namespace ModEndpoints.Tests.RemoteServices;

public class DefaultServiceChannelSerializerTests
{
  private readonly ServiceChannelSerializerOptions _options = new()
  {
    SerializationOptions = new JsonSerializerOptions(),
    DeserializationOptions = new JsonSerializerOptions(),
    StreamingDeserializationOptions = new JsonSerializerOptions()
  };

  private readonly DefaultServiceChannelSerializer _serializer;

  public DefaultServiceChannelSerializerTests()
  {
    _serializer = new DefaultServiceChannelSerializer(_options);
  }

  private class DummyRequest : IServiceRequestMarker
  {
    public string Name { get; set; } = "Test";
  }

  private class DummyResponse
  {
    public string Value { get; set; } = "Hello";
  }

  [Fact]
  public async Task CreateContentAsync_SerializesRequestAsync()
  {
    var request = new DummyRequest { Name = "TestName" };
    var content = await _serializer.CreateContentAsync(request, CancellationToken.None);

    Assert.NotNull(content);
    var json = await content.ReadAsStringAsync();
    Assert.Contains("TestName", json);
  }

  [Fact]
  public async Task DeserializeResultAsyncTResponse_ReturnsCriticalError_OnNonSuccessStatusAsync()
  {
    var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
    {
      ReasonPhrase = "Bad request",
      RequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://localhost/test")
    };
    var result = await _serializer.DeserializeResultAsync<DummyResponse>(response, CancellationToken.None);

    Assert.True(result.IsFailed);
    Assert.Equal(FailureType.CriticalError, result.Failure.Type);
    Assert.Contains(result.Failure.Errors, e => e.Message.Contains("400"));
    Assert.Contains(result.Failure.Errors, e => e.Message.Contains("Bad request"));
    Assert.Contains("Instance: GET http://localhost/test", result.Statements.Facts[0].Message);
  }

  [Fact]
  public async Task DeserializeResultAsyncTResponse_ThrowsCriticalError_OnNullDeserializationAsync()
  {
    var response = new HttpResponseMessage(HttpStatusCode.OK)
    {
      Content = new StringContent("", Encoding.UTF8, "application/json"),
      RequestMessage = new HttpRequestMessage(HttpMethod.Post, "http://localhost/test2")
    };

    await Assert.ThrowsAsync<JsonException>(async () => await _serializer.DeserializeResultAsync<DummyResponse>(response, CancellationToken.None));
  }

  [Fact]
  public async Task DeserializeResultAsync_ReturnsCriticalError_OnNonSuccessStatusAsync()
  {
    var response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
    {
      ReasonPhrase = "Server error",
      RequestMessage = new HttpRequestMessage(HttpMethod.Put, "http://localhost/test3")
    };
    var result = await _serializer.DeserializeResultAsync(response, CancellationToken.None);

    Assert.True(result.IsFailed);
    Assert.Equal(FailureType.CriticalError, result.Failure.Type);
    Assert.Contains(result.Failure.Errors, e => e.Message.Contains("500"));
    Assert.Contains(result.Failure.Errors, e => e.Message.Contains("Server error"));
    Assert.Contains("Instance: PUT http://localhost/test3", result.Statements.Facts[0].Message);
  }

  [Fact]
  public async Task DeserializeResultAsync_ReturnsCriticalError_OnNullDeserializationAsync()
  {
    var response = new HttpResponseMessage(HttpStatusCode.OK)
    {
      Content = new StringContent("", Encoding.UTF8, "application/json"),
      RequestMessage = new HttpRequestMessage(HttpMethod.Delete, "http://localhost/test4")
    };

    await Assert.ThrowsAsync<JsonException>(async () => await _serializer.DeserializeResultAsync(response, CancellationToken.None));
  }

  [Fact]
  public async Task DeserializeStreamingResponseItemAsyncTResponse_ReturnsCriticalError_OnNonSuccessStatusAsync()
  {
    var response = new HttpResponseMessage(HttpStatusCode.Forbidden)
    {
      ReasonPhrase = "Forbidden",
      RequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://localhost/stream")
    };
    var items = _serializer.DeserializeStreamingResponseItemAsync<DummyResponse>(response, CancellationToken.None);

    var enumerator = items.GetAsyncEnumerator();
    Assert.True(await enumerator.MoveNextAsync());
    var item = enumerator.Current;
    Assert.True(item.Result.IsFailed);
    Assert.Equal(FailureType.CriticalError, item.Result.Failure.Type);
    Assert.Contains(item.Result.Failure.Errors, e => e.Message.Contains("403"));
    Assert.Contains(item.Result.Failure.Errors, e => e.Message.Contains("Forbidden"));
    Assert.Contains("Instance: GET http://localhost/stream", item.Result.Statements.Facts[0].Message);
  }

  [Fact]
  public async Task DeserializeStreamingResponseItemAsyncTResponse_ThrowsCriticalError_OnNullDeserializationAsync()
  {
    var response = new HttpResponseMessage(HttpStatusCode.OK)
    {
      Content = new StringContent("", Encoding.UTF8, "application/json"),
      RequestMessage = new HttpRequestMessage(HttpMethod.Post, "http://localhost/stream2")
    };

    await Assert.ThrowsAsync<JsonException>(async () =>
    {
      await foreach (var item in _serializer.DeserializeStreamingResponseItemAsync<DummyResponse>(response, CancellationToken.None))
      {
        // This should not be reached, as an exception is expected
        throw new UnreachableException("This code should not be reached due to expected exception.");
      }
    });
  }

  [Fact]
  public async Task DeserializeStreamingResponseItemAsync_ReturnsCriticalError_OnNonSuccessStatusAsync()
  {
    var response = new HttpResponseMessage(HttpStatusCode.NotFound)
    {
      ReasonPhrase = "Not Found",
      RequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://localhost/stream3")
    };
    var items = _serializer.DeserializeStreamingResponseItemAsync(response, CancellationToken.None);

    var enumerator = items.GetAsyncEnumerator();
    Assert.True(await enumerator.MoveNextAsync());
    var item = enumerator.Current;
    Assert.True(item.Result.IsFailed);
    Assert.Equal(FailureType.CriticalError, item.Result.Failure.Type);
    Assert.Contains(item.Result.Failure.Errors, e => e.Message.Contains("404"));
    Assert.Contains(item.Result.Failure.Errors, e => e.Message.Contains("Not Found"));
    Assert.Contains("Instance: GET http://localhost/stream3", item.Result.Statements.Facts[0].Message);
  }

  [Fact]
  public async Task DeserializeStreamingResponseItemAsync_ThrowsCriticalError_OnNullDeserializationAsync()
  {
    var response = new HttpResponseMessage(HttpStatusCode.OK)
    {
      Content = new StringContent("", Encoding.UTF8, "application/json"),
      RequestMessage = new HttpRequestMessage(HttpMethod.Post, "http://localhost/stream4")
    };

    await Assert.ThrowsAsync<JsonException>(async () =>
    {
      await foreach (var item in _serializer.DeserializeStreamingResponseItemAsync(response, CancellationToken.None))
      {
        // This should not be reached, as an exception is expected
        throw new UnreachableException("This code should not be reached due to expected exception.");
      }
    });
  }

}
