using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ModEndpoints.RemoteServices;
using ModEndpoints.RemoteServices.Contracts;
using ModResults;
using NSubstitute;

namespace ModEndpoints.Tests.RemoteServices;

public class DefaultServiceChannelTests
{
  private class UnregisteredDummyRequest : IServiceRequest<string> { }
  private class UnregisteredDummyStreamingRequest : IServiceRequestWithStreamingResponse<string> { }
  private class UnregisteredDummyRequestNoResponse : IServiceRequest { }
  private class UnregisteredDummyStreamingRequestNoResponse : IServiceRequestWithStreamingResponse { }

  private readonly IServiceEndpointUriResolver _uriResolver = Substitute.For<IServiceEndpointUriResolver>();
  private readonly IServiceChannelSerializer _serializer = Substitute.For<IServiceChannelSerializer>();

  private ServiceProvider CreateServiceProvider()
  {
    // Create service collection
    var services = new ServiceCollection();
    services.AddHttpClient();
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
  public async Task SendAsync_ReturnsFail_WhenUriResolverFailsAsync()
  {
    var req = new UnregisteredDummyRequest();
    _uriResolver.Resolve(req).Returns(Result<string>.CriticalError("fail"));

    var serviceProvider = CreateServiceProvider();

    using (var scope = serviceProvider.CreateScope())
    {
      var channel = new DefaultServiceChannel(
        scope.ServiceProvider.GetRequiredService<IHttpClientFactory>(),
        serviceProvider);

      var result = await channel.SendAsync(req, null, TestContext.Current.CancellationToken);

      Assert.True(result.IsFailed);
    }
  }

  [Fact]
  public async Task SendAsync_ReturnsCriticalError_WhenNotRegisteredAsync()
  {
    var req = new UnregisteredDummyRequest();
    _uriResolver.Resolve(req).Returns(Result<string>.Ok("http://localhost/endpoint"));
    var serviceProvider = CreateServiceProvider();

    using (var scope = serviceProvider.CreateScope())
    {
      var channel = new DefaultServiceChannel(
        scope.ServiceProvider.GetRequiredService<IHttpClientFactory>(),
        serviceProvider);
      var result = await channel.SendAsync(req, null, TestContext.Current.CancellationToken);

      Assert.True(result.IsFailed);
      Assert.Equal(FailureType.CriticalError, result.Failure.Type);
      Assert.Contains(result.Failure.Errors, e => e.Message.Contains("No channel registration found"));
    }
  }

  [Fact]
  public async Task SendAsync_NoResponse_ReturnsFail_WhenUriResolverFailsAsync()
  {
    var req = new UnregisteredDummyRequestNoResponse();
    _uriResolver.Resolve(req).Returns(Result<string>.CriticalError("fail"));

    var serviceProvider = CreateServiceProvider();

    using (var scope = serviceProvider.CreateScope())
    {
      var channel = new DefaultServiceChannel(
        scope.ServiceProvider.GetRequiredService<IHttpClientFactory>(),
        serviceProvider);
      var result = await channel.SendAsync(req, null, TestContext.Current.CancellationToken);

      Assert.True(result.IsFailed);
    }
  }

  [Fact]
  public async Task SendAsync_NoResponse_ReturnsCriticalError_WhenNotRegisteredAsync()
  {
    var req = new UnregisteredDummyRequestNoResponse();
    _uriResolver.Resolve(req).Returns(Result<string>.Ok("http://localhost/endpoint"));

    var serviceProvider = CreateServiceProvider();

    using (var scope = serviceProvider.CreateScope())
    {
      var channel = new DefaultServiceChannel(
        scope.ServiceProvider.GetRequiredService<IHttpClientFactory>(),
        serviceProvider);
      var result = await channel.SendAsync(req, null, TestContext.Current.CancellationToken);

      Assert.True(result.IsFailed);
      Assert.Equal(FailureType.CriticalError, result.Failure.Type);
      Assert.Contains(result.Failure.Errors, e => e.Message.Contains("No channel registration found"));
    }
  }

  [Fact]
  public async Task SendAsync_Streaming_ReturnsFail_WhenUriResolverFailsAsync()
  {
    var req = new UnregisteredDummyStreamingRequest();
    _uriResolver.Resolve(req).Returns(Result<string>.CriticalError("fail"));

    var serviceProvider = CreateServiceProvider();

    using (var scope = serviceProvider.CreateScope())
    {
      var channel = new DefaultServiceChannel(
        scope.ServiceProvider.GetRequiredService<IHttpClientFactory>(),
        serviceProvider);
      var results = await channel.SendAsync(req, null, TestContext.Current.CancellationToken).ToListAsync(TestContext.Current.CancellationToken);

      Assert.Single(results);
      Assert.True(results[0].Result.IsFailed);
    }
  }

  [Fact]
  public async Task SendAsync_Streaming_ReturnsCriticalError_WhenNotRegisteredAsync()
  {
    var req = new UnregisteredDummyStreamingRequest();
    _uriResolver.Resolve(req).Returns(Result<string>.Ok("http://localhost/endpoint"));

    var serviceProvider = CreateServiceProvider();

    using (var scope = serviceProvider.CreateScope())
    {
      var channel = new DefaultServiceChannel(
        scope.ServiceProvider.GetRequiredService<IHttpClientFactory>(),
        serviceProvider);

      var results = await channel.SendAsync(req, null, TestContext.Current.CancellationToken).ToListAsync(TestContext.Current.CancellationToken);

      Assert.Single(results);
      Assert.True(results[0].Result.IsFailed);
    }
  }

  [Fact]
  public async Task SendAsync_Streaming_NoResponse_ReturnsFail_WhenUriResolverFailsAsync()
  {
    var req = new UnregisteredDummyStreamingRequestNoResponse();
    _uriResolver.Resolve(req).Returns(Result<string>.CriticalError("fail"));

    var serviceProvider = CreateServiceProvider();

    using (var scope = serviceProvider.CreateScope())
    {
      var channel = new DefaultServiceChannel(
        scope.ServiceProvider.GetRequiredService<IHttpClientFactory>(),
        serviceProvider);

      var results = await channel.SendAsync(req, null, TestContext.Current.CancellationToken).ToListAsync(TestContext.Current.CancellationToken);

      Assert.Single(results);
      Assert.True(results[0].Result.IsFailed);
    }
  }

  [Fact]
  public async Task SendAsync_Streaming_NoResponse_ReturnsCriticalError_WhenNotRegisteredAsync()
  {
    var req = new UnregisteredDummyStreamingRequestNoResponse();
    _uriResolver.Resolve(req).Returns(Result<string>.Ok("http://localhost/endpoint"));

    var serviceProvider = CreateServiceProvider();

    using (var scope = serviceProvider.CreateScope())
    {
      var channel = new DefaultServiceChannel(
        scope.ServiceProvider.GetRequiredService<IHttpClientFactory>(),
        serviceProvider);
      var results = await channel.SendAsync(req, null, TestContext.Current.CancellationToken).ToListAsync(TestContext.Current.CancellationToken);

      Assert.Single(results);
      Assert.True(results[0].Result.IsFailed);
    }
  }
}
