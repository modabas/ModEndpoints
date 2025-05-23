﻿using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using ModEndpoints.RemoteServices.Core;
using ModResults;

namespace ModEndpoints.RemoteServices;

public class DefaultServiceChannel(
  IHttpClientFactory clientFactory,
  IServiceProvider serviceProvider)
  : IServiceChannel
{
  private const string NoChannelRegistrationFound = "No channel registration found for request type {0}.";

  public async Task<Result<TResponse>> SendAsync<TResponse>(
    IServiceRequest<TResponse> req,
    string? endpointUriPrefix,
    CancellationToken ct,
    Func<IServiceProvider, HttpRequestMessage, CancellationToken, Task>? httpRequestInterceptor = null,
    Func<IServiceProvider, HttpResponseMessage, CancellationToken, Task>? httpResponseInterceptor = null,
    string? uriResolverName = null,
    string? serializerName = null)
    where TResponse : notnull
  {
    using (var scope = serviceProvider.CreateScope())
    {
      var uriResolver = scope.ServiceProvider.GetRequiredKeyedService<IServiceEndpointUriResolver>(
        uriResolverName ?? RemoteServiceDefinitions.DefaultUriResolverName);
      var serializer = scope.ServiceProvider.GetRequiredKeyedService<IServiceChannelSerializer>(
        serializerName ?? RemoteServiceDefinitions.DefaultSerializerName);
      var requestUriResult = uriResolver.Resolve(req);
      if (requestUriResult.IsFailed)
      {
        return Result<TResponse>.Fail(requestUriResult);
      }
      var requestType = req.GetType();
      if (!ServiceChannelRegistry.Instance.IsRequestRegistered(requestType, out var clientName))
      {
        return Result<TResponse>.CriticalError(string.Format(NoChannelRegistrationFound, requestType));
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
          return await serializer.DeserializeResultAsync<TResponse>(httpResponse, ct);
        }
      }
    }
  }

  public async Task<Result> SendAsync(
    IServiceRequest req,
    string? endpointUriPrefix,
    CancellationToken ct,
    Func<IServiceProvider, HttpRequestMessage, CancellationToken, Task>? httpRequestInterceptor = null,
    Func<IServiceProvider, HttpResponseMessage, CancellationToken, Task>? httpResponseInterceptor = null,
    string? uriResolverName = null,
    string? serializerName = null)
  {
    using (var scope = serviceProvider.CreateScope())
    {
      var uriResolver = scope.ServiceProvider.GetRequiredKeyedService<IServiceEndpointUriResolver>(
        uriResolverName ?? RemoteServiceDefinitions.DefaultUriResolverName);
      var serializer = scope.ServiceProvider.GetRequiredKeyedService<IServiceChannelSerializer>(
        serializerName ?? RemoteServiceDefinitions.DefaultSerializerName);
      var requestUriResult = uriResolver.Resolve(req);
      if (requestUriResult.IsFailed)
      {
        return Result.Fail(requestUriResult);
      }
      var requestType = req.GetType();
      if (!ServiceChannelRegistry.Instance.IsRequestRegistered(requestType, out var clientName))
      {
        return Result.CriticalError(string.Format(NoChannelRegistrationFound, requestType));
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
          return await serializer.DeserializeResultAsync(httpResponse, ct);
        }
      }
    }
  }

  public async IAsyncEnumerable<StreamingResponseItem<TResponse>> SendAsync<TResponse>(
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
        uriResolverName ?? RemoteServiceDefinitions.DefaultUriResolverName);
      var serializer = scope.ServiceProvider.GetRequiredKeyedService<IServiceChannelSerializer>(
        serializerName ?? RemoteServiceDefinitions.DefaultSerializerName);
      var requestUriResult = uriResolver.Resolve(req);
      if (requestUriResult.IsFailed)
      {
        yield return new StreamingResponseItem<TResponse>(
          Result: Result<TResponse>.Fail(requestUriResult),
          ResponseType: StreamingResponseItemDefinitions.DefaultClientSideErrorResponseType);
        yield break;
      }
      var requestType = req.GetType();
      if (!ServiceChannelRegistry.Instance.IsRequestRegistered(requestType, out var clientName))
      {
        yield return new StreamingResponseItem<TResponse>(
          Result: Result<TResponse>.CriticalError(string.Format(NoChannelRegistrationFound, requestType)),
          ResponseType: StreamingResponseItemDefinitions.DefaultClientSideErrorResponseType);
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
        using (var httpResponse = await client.SendAsync(httpReq, HttpCompletionOption.ResponseHeadersRead, ct))
        {
          if (httpResponseInterceptor is not null)
          {
            await httpResponseInterceptor(scope.ServiceProvider, httpResponse, ct);
          }
          await foreach (var resultObject in serializer.DeserializeStreamingResponseItemAsync<TResponse>(httpResponse, ct))
          {
            ct.ThrowIfCancellationRequested();
            yield return resultObject;
          }
        }
      }
    }
  }

  public async IAsyncEnumerable<StreamingResponseItem> SendAsync(
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
        uriResolverName ?? RemoteServiceDefinitions.DefaultUriResolverName);
      var serializer = scope.ServiceProvider.GetRequiredKeyedService<IServiceChannelSerializer>(
        serializerName ?? RemoteServiceDefinitions.DefaultSerializerName);
      var requestUriResult = uriResolver.Resolve(req);
      if (requestUriResult.IsFailed)
      {
        yield return new StreamingResponseItem(
          Result: Result.Fail(requestUriResult),
          ResponseType: StreamingResponseItemDefinitions.DefaultClientSideErrorResponseType);
        yield break;
      }
      var requestType = req.GetType();
      if (!ServiceChannelRegistry.Instance.IsRequestRegistered(requestType, out var clientName))
      {
        yield return new StreamingResponseItem(
          Result: Result.CriticalError(string.Format(NoChannelRegistrationFound, requestType)),
          ResponseType: StreamingResponseItemDefinitions.DefaultClientSideErrorResponseType);
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
        using (var httpResponse = await client.SendAsync(httpReq, HttpCompletionOption.ResponseHeadersRead, ct))
        {
          if (httpResponseInterceptor is not null)
          {
            await httpResponseInterceptor(scope.ServiceProvider, httpResponse, ct);
          }
          await foreach (var resultObject in serializer.DeserializeStreamingResponseItemAsync(httpResponse, ct))
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
