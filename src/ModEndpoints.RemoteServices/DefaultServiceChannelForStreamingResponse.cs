using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using ModEndpoints.RemoteServices.Core;
using ModResults;

namespace ModEndpoints.RemoteServices;

public class DefaultServiceChannelForStreamingResponse(
  IHttpClientFactory clientFactory,
  IServiceProvider serviceProvider)
  : IServiceChannelForStreamingResponse
{
  private const string NoChannelRegistrationFound = "No channel registration found for request type {0}.";

  public IAsyncEnumerable<Result<TResponse>> SendAsync<TResponse>(IServiceRequestWithStreamingResponse<TResponse> req, CancellationToken ct)
    where TResponse : notnull
  {
    return SendAsync(req, null, ct);
  }

  public async IAsyncEnumerable<Result<TResponse>> SendAsync<TResponse>(
    IServiceRequestWithStreamingResponse<TResponse> req,
    string? endpointUriPrefix,
    [EnumeratorCancellation] CancellationToken ct,
    Func<IServiceProvider, HttpRequestMessage, CancellationToken, Task>? httpRequestInterceptor = null,
    Func<IServiceProvider, HttpResponseMessage, CancellationToken, Task>? httpResponseInterceptor = null,
    string? uriResolverName = null,
    string? serializerName = null)
    where TResponse : notnull
  {
    using (var scope = serviceProvider.CreateScope())
    {
      var uriResolver = scope.ServiceProvider.GetRequiredKeyedService<IServiceEndpointUriResolver>(
        uriResolverName ?? ServiceEndpointDefinitions.DefaultUriResolverName);
      var serializer = scope.ServiceProvider.GetRequiredKeyedService<IServiceChannelSerializerForStreamingResponse>(
        serializerName ?? ServiceEndpointDefinitions.DefaultSerializerForStreamingResponseName);
      var requestUriResult = uriResolver.Resolve(req);
      if (requestUriResult.IsFailed)
      {
        yield return Result<TResponse>.Fail(requestUriResult);
        yield break;
      }
      var requestType = req.GetType();
      if (!ServiceChannelRegistry.Instance.IsRequestRegistered(requestType, out var clientName))
      {
        yield return Result<TResponse>.CriticalError(string.Format(NoChannelRegistrationFound, requestType));
        yield break;
      }
      using (HttpRequestMessage httpReq = new(
        HttpMethod.Post,
        Combine(endpointUriPrefix, requestUriResult.Value)))
      {
        httpReq.Content = await serializer.CreateContentAsync(req, ct);
        if (httpRequestInterceptor is not null)
        {
          await httpRequestInterceptor(scope.ServiceProvider, httpReq, ct);
        }
        var client = clientFactory.CreateClient(clientName);
        using (var httpResponse = await client.SendAsync(httpReq, ct))
        {
          if (httpResponseInterceptor is not null)
          {
            await httpResponseInterceptor(scope.ServiceProvider, httpResponse, ct);
          }
          await foreach (var resultObject in serializer.DeserializeResultAsync<TResponse>(httpResponse, ct))
          {
            ct.ThrowIfCancellationRequested();
            yield return resultObject;
          }
        }
      }
    }
  }

  public IAsyncEnumerable<Result> SendAsync(IServiceRequestWithStreamingResponse req, CancellationToken ct)
  {
    return SendAsync(req, null, ct);
  }

  public async IAsyncEnumerable<Result> SendAsync(
    IServiceRequestWithStreamingResponse req,
    string? endpointUriPrefix,
    [EnumeratorCancellation] CancellationToken ct,
    Func<IServiceProvider, HttpRequestMessage, CancellationToken, Task>? httpRequestInterceptor = null,
    Func<IServiceProvider, HttpResponseMessage, CancellationToken, Task>? httpResponseInterceptor = null,
    string? uriResolverName = null,
    string? serializerName = null)
  {
    using (var scope = serviceProvider.CreateScope())
    {
      var uriResolver = scope.ServiceProvider.GetRequiredKeyedService<IServiceEndpointUriResolver>(
        uriResolverName ?? ServiceEndpointDefinitions.DefaultUriResolverName);
      var serializer = scope.ServiceProvider.GetRequiredKeyedService<IServiceChannelSerializerForStreamingResponse>(
        serializerName ?? ServiceEndpointDefinitions.DefaultSerializerForStreamingResponseName);
      var requestUriResult = uriResolver.Resolve(req);
      if (requestUriResult.IsFailed)
      {
        yield return Result.Fail(requestUriResult);
        yield break;
      }
      var requestType = req.GetType();
      if (!ServiceChannelRegistry.Instance.IsRequestRegistered(requestType, out var clientName))
      {
        yield return Result.CriticalError(string.Format(NoChannelRegistrationFound, requestType));
        yield break;
      }
      using (HttpRequestMessage httpReq = new(
        HttpMethod.Post,
        Combine(endpointUriPrefix, requestUriResult.Value)))
      {
        httpReq.Content = await serializer.CreateContentAsync(req, ct);
        if (httpRequestInterceptor is not null)
        {
          await httpRequestInterceptor(scope.ServiceProvider, httpReq, ct);
        }
        var client = clientFactory.CreateClient(clientName);
        using (var httpResponse = await client.SendAsync(httpReq, ct))
        {
          if (httpResponseInterceptor is not null)
          {
            await httpResponseInterceptor(scope.ServiceProvider, httpResponse, ct);
          }
          await foreach (var resultObject in serializer.DeserializeResultAsync(httpResponse, ct))
          {
            ct.ThrowIfCancellationRequested();
            yield return resultObject;
          }
        }
      }
    }
  }

  private static string Combine(string? endpointUriPrefix, string endpointUri)
  {
    if (string.IsNullOrWhiteSpace(endpointUriPrefix))
    {
      return endpointUri;
    }
    return string.Format(
      "{0}/{1}",
      endpointUriPrefix.TrimEnd('/'),
      endpointUri.TrimStart('/'));
  }
}
