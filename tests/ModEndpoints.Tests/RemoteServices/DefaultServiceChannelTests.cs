using System;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ModEndpoints.RemoteServices;
using ModEndpoints.RemoteServices.Core;
using ModResults;
using NSubstitute;

namespace ModEndpoints.Tests.RemoteServices;

public class DefaultServiceChannelTests
{
  private class DummyRequest : IServiceRequest<string> { }
  private class DummyStreamingRequest : IServiceRequestWithStreamingResponse<string> { }
  private class DummyRequestNoResponse : IServiceRequest { }
  private class DummyStreamingRequestNoResponse : IServiceRequestWithStreamingResponse { }

  private readonly IHttpClientFactory _clientFactory = Substitute.For<IHttpClientFactory>();
  private readonly IServiceEndpointUriResolver _uriResolver = Substitute.For<IServiceEndpointUriResolver>();
  private readonly IServiceChannelSerializer _serializer = Substitute.For<IServiceChannelSerializer>();

  [Fact]
  public async Task SendAsync_ReturnsFail_WhenUriResolverFailsAsync()
  {
    var req = new DummyRequest();
    _uriResolver.Resolve(req).Returns(Result<string>.CriticalError("fail"));

    var serviceProvider = CreateServiceProvider();

    var channel = new DefaultServiceChannel(_clientFactory, serviceProvider);

    var result = await channel.SendAsync(req, null, CancellationToken.None);

    Assert.True(result.IsFailed);
  }

  private ServiceProvider CreateServiceProvider()
  {
    // Create service collection
    var services = new ServiceCollection();
    services.AddSingleton<IHttpClientFactory>(_clientFactory);
    services.TryAddKeyedSingleton<IServiceEndpointUriResolver>(
      RemoteServiceDefinitions.DefaultUriResolverName,
      _uriResolver);
    services.AddKeyedTransient<IServiceChannelSerializer>(
      RemoteServiceDefinitions.DefaultSerializerName,
      (_, _) =>
      {
        return _serializer;
      });

    var serviceProvider = services.BuildServiceProvider();
    return serviceProvider;
  }

  [Fact]
  public async Task SendAsync_ReturnsCriticalError_WhenNotRegisteredAsync()
  {
    var req = new DummyRequest();
    _uriResolver.Resolve(req).Returns(Result<string>.Ok("http://localhost/endpoint"));
    var serviceProvider = CreateServiceProvider();
    ServiceChannelRegistry.Instance.Clear();

    var channel = new DefaultServiceChannel(_clientFactory, serviceProvider);

    var result = await channel.SendAsync(req, null, CancellationToken.None);

    Assert.True(result.IsFailed);
    Assert.Equal(FailureType.CriticalError, result.Failure.Type);
    Assert.Contains(result.Failure.Errors, e => e.Message.Contains("No channel registration found"));
  }

  [Fact]
  public async Task SendAsync_CallsSerializerAndReturnsResultAsync()
  {
    var req = new DummyRequest();
    _uriResolver.Resolve(req).Returns(Result<string>.Ok("http://localhost/endpoint"));
    ServiceChannelRegistry.Instance.Clear();
    ServiceChannelRegistry.Instance.RegisterRequest(typeof(DummyRequest), "client");

    var messageHandler = new FakeHttpMessageHandler(HttpStatusCode.OK);
    _serializer.CreateContentAsync(req, Arg.Any<CancellationToken>())
        .Returns(new StringContent("test"));
    _serializer.DeserializeResultAsync<string>(messageHandler.Response, Arg.Any<CancellationToken>())
        .Returns(Result<string>.Ok("ok"));
    var httpClient = new HttpClient(messageHandler);
    _clientFactory.CreateClient(Arg.Any<string>()).Returns(httpClient);

    var serviceProvider = CreateServiceProvider();

    var channel = new DefaultServiceChannel(_clientFactory, serviceProvider);

    var result = await channel.SendAsync(req, null, CancellationToken.None);

    Assert.True(result.IsOk);
    Assert.Equal("ok", result.Value);
  }

  [Fact]
  public async Task SendAsync_NoResponse_ReturnsFail_WhenUriResolverFailsAsync()
  {
    var req = new DummyRequestNoResponse();
    _uriResolver.Resolve(req).Returns(Result<string>.CriticalError("fail"));

    var serviceProvider = CreateServiceProvider();

    var channel = new DefaultServiceChannel(_clientFactory, serviceProvider);

    var result = await channel.SendAsync(req, null, CancellationToken.None);

    Assert.True(result.IsFailed);
  }

  [Fact]
  public async Task SendAsync_NoResponse_ReturnsCriticalError_WhenNotRegisteredAsync()
  {
    var req = new DummyRequestNoResponse();
    _uriResolver.Resolve(req).Returns(Result<string>.Ok("http://localhost/endpoint"));
    ServiceChannelRegistry.Instance.Clear();

    var serviceProvider = CreateServiceProvider();

    var channel = new DefaultServiceChannel(_clientFactory, serviceProvider);

    var result = await channel.SendAsync(req, null, CancellationToken.None);

    Assert.True(result.IsFailed);
    Assert.Equal(FailureType.CriticalError, result.Failure.Type);
    Assert.Contains(result.Failure.Errors, e => e.Message.Contains("No channel registration found"));
  }

  [Fact]
  public async Task SendAsync_NoResponse_CallsSerializerAndReturnsResultAsync()
  {
    var req = new DummyRequestNoResponse();
    _uriResolver.Resolve(req).Returns(Result<string>.Ok("http://localhost/endpoint"));
    ServiceChannelRegistry.Instance.Clear();
    ServiceChannelRegistry.Instance.RegisterRequest(typeof(DummyRequestNoResponse), "client");

    var messageHandler = new FakeHttpMessageHandler(HttpStatusCode.OK);
    _serializer.CreateContentAsync(req, Arg.Any<CancellationToken>())
        .Returns(new StringContent("test"));
    _serializer.DeserializeResultAsync(messageHandler.Response, Arg.Any<CancellationToken>())
        .Returns(Result.Ok());
    var httpClient = new HttpClient(messageHandler);
    _clientFactory.CreateClient(Arg.Any<string>()).Returns(httpClient);

    var serviceProvider = CreateServiceProvider();

    var channel = new DefaultServiceChannel(_clientFactory, serviceProvider);

    var result = await channel.SendAsync(req, null, CancellationToken.None);

    Assert.True(result.IsOk);
  }

  [Fact]
  public async Task SendAsync_Streaming_ReturnsFail_WhenUriResolverFailsAsync()
  {
    var req = new DummyStreamingRequest();
    _uriResolver.Resolve(req).Returns(Result<string>.CriticalError("fail"));

    var serviceProvider = CreateServiceProvider();

    var channel = new DefaultServiceChannel(_clientFactory, serviceProvider);

    var results = new List<StreamingResponseItem<string>>();
    await foreach (var item in channel.SendAsync(req, null, CancellationToken.None))
    {
      results.Add(item);
    }

    Assert.Single(results);
    Assert.True(results[0].Result.IsFailed);
  }

  [Fact]
  public async Task SendAsync_Streaming_ReturnsCriticalError_WhenNotRegisteredAsync()
  {
    var req = new DummyStreamingRequest();
    _uriResolver.Resolve(req).Returns(Result<string>.Ok("http://localhost/endpoint"));
    ServiceChannelRegistry.Instance.Clear();

    var serviceProvider = CreateServiceProvider();

    var channel = new DefaultServiceChannel(_clientFactory, serviceProvider);

    var results = new List<StreamingResponseItem<string>>();
    await foreach (var item in channel.SendAsync(req, null, CancellationToken.None))
    {
      results.Add(item);
    }

    Assert.Single(results);
    Assert.True(results[0].Result.IsFailed);
  }

  [Fact]
  public async Task SendAsync_Streaming_CallsSerializerAndYieldsResultsAsync()
  {
    var req = new DummyStreamingRequest();
    _uriResolver.Resolve(req).Returns(Result<string>.Ok("http://localhost/endpoint"));
    ServiceChannelRegistry.Instance.Clear();
    ServiceChannelRegistry.Instance.RegisterRequest(typeof(DummyStreamingRequest), "client");

    var messageHandler = new FakeHttpMessageHandler(HttpStatusCode.OK);
    _serializer.CreateContentAsync(req, Arg.Any<CancellationToken>())
        .Returns(new StringContent("test"));

    var streamingItems = new List<StreamingResponseItem<string>>
          {
              new(Result<string>.Ok("stream1")),
              new(Result<string>.Ok("stream2"))
          };
    _serializer.DeserializeStreamingResponseItemAsync<string>(messageHandler.Response, Arg.Any<CancellationToken>())
        .Returns(streamingItems.ToAsyncEnumerable());

    var httpClient = new HttpClient(messageHandler);
    _clientFactory.CreateClient(Arg.Any<string>()).Returns(httpClient);

    var serviceProvider = CreateServiceProvider();

    var channel = new DefaultServiceChannel(_clientFactory, serviceProvider);

    var results = new List<StreamingResponseItem<string>>();
    await foreach (var item in channel.SendAsync(req, null, CancellationToken.None))
    {
      results.Add(item);
    }

    Assert.Equal(2, results.Count);
    Assert.Equal("stream1", results[0].Result.Value);
    Assert.Equal("stream2", results[1].Result.Value);
  }

  [Fact]
  public async Task SendAsync_Streaming_NoResponse_ReturnsFail_WhenUriResolverFailsAsync()
  {
    var req = new DummyStreamingRequestNoResponse();
    _uriResolver.Resolve(req).Returns(Result<string>.CriticalError("fail"));

    var serviceProvider = CreateServiceProvider();

    var channel = new DefaultServiceChannel(_clientFactory, serviceProvider);

    var results = new List<StreamingResponseItem>();
    await foreach (var item in channel.SendAsync(req, null, CancellationToken.None))
    {
      results.Add(item);
    }

    Assert.Single(results);
    Assert.True(results[0].Result.IsFailed);
  }

  [Fact]
  public async Task SendAsync_Streaming_NoResponse_ReturnsCriticalError_WhenNotRegisteredAsync()
  {
    var req = new DummyStreamingRequestNoResponse();
    _uriResolver.Resolve(req).Returns(Result<string>.Ok("http://localhost/endpoint"));
    ServiceChannelRegistry.Instance.Clear();

    var serviceProvider = CreateServiceProvider();

    var channel = new DefaultServiceChannel(_clientFactory, serviceProvider);

    var results = new List<StreamingResponseItem>();
    await foreach (var item in channel.SendAsync(req, null, CancellationToken.None))
    {
      results.Add(item);
    }

    Assert.Single(results);
    Assert.True(results[0].Result.IsFailed);
  }

  [Fact]
  public async Task SendAsync_Streaming_NoResponse_CallsSerializerAndYieldsResultsAsync()
  {
    var req = new DummyStreamingRequestNoResponse();
    _uriResolver.Resolve(req).Returns(Result<string>.Ok("http://localhost/endpoint"));
    ServiceChannelRegistry.Instance.Clear();
    ServiceChannelRegistry.Instance.RegisterRequest(typeof(DummyStreamingRequestNoResponse), "client");

    var messageHandler = new FakeHttpMessageHandler(HttpStatusCode.OK);
    _serializer.CreateContentAsync(req, Arg.Any<CancellationToken>())
        .Returns(new StringContent("test"));

    var streamingItems = new List<StreamingResponseItem>
          {
              new(Result.Ok()),
              new(Result.Ok())
          };
    _serializer.DeserializeStreamingResponseItemAsync(messageHandler.Response, Arg.Any<CancellationToken>())
        .Returns(streamingItems.ToAsyncEnumerable());

    var httpClient = new HttpClient(messageHandler);
    _clientFactory.CreateClient(Arg.Any<string>()).Returns(httpClient);

    var serviceProvider = CreateServiceProvider();

    var channel = new DefaultServiceChannel(_clientFactory, serviceProvider);

    var results = new List<StreamingResponseItem>();
    await foreach (var item in channel.SendAsync(req, null, CancellationToken.None))
    {
      results.Add(item);
    }

    Assert.Equal(2, results.Count);
    Assert.True(results[0].Result.IsOk);
    Assert.True(results[1].Result.IsOk);
  }

  // Helper for faking HttpMessageHandler
  public class FakeHttpMessageHandler : HttpMessageHandler
  {
    public HttpResponseMessage Response { get; init; }

    public FakeHttpMessageHandler(HttpStatusCode statusCode)
    {
      Response = new HttpResponseMessage(statusCode);
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
      return Task.FromResult(Response);
    }
  }
}
