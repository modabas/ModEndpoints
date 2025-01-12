using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using ModEndpoints.RemoteServices.Core;
using ModResults;

namespace ModEndpoints.RemoteServices;

public class ServiceChannel(
  IHttpClientFactory clientFactory,
  IServiceProvider serviceProvider)
  : IServiceChannel
{
  private const string NoChannelRegistrationFound = "No channel registration found for request type {0}.";

  public async Task<Result<TResponse>> SendAsync<TRequest, TResponse>(
    TRequest req,
    CancellationToken ct,
    MediaTypeHeaderValue? mediaType = null,
    JsonSerializerOptions? jsonSerializerOptions = null,
    Action<HttpRequestHeaders>? configureRequestHeaders = null,
    string? uriResolverName = null)
    where TRequest : IServiceRequest<TResponse>
    where TResponse : notnull
  {
    try
    {
      using (var scope = serviceProvider.CreateScope())
      {
        var uriResolver = scope.ServiceProvider.GetRequiredKeyedService<IServiceEndpointUriResolver>(
          uriResolverName ?? ServiceEndpointDefinitions.DefaultUriResolverName);
        var requestUriResult = uriResolver.Resolve(req);
        if (requestUriResult.IsFailed)
        {
          return Result<TResponse>.Fail(requestUriResult);
        }
        if (!ServiceChannelRegistry.Instance.IsRequestRegistered<TRequest>(out var clientName))
        {
          return Result<TResponse>.CriticalError(string.Format(NoChannelRegistrationFound, typeof(TRequest)));
        }
        using (HttpRequestMessage httpReq = new(HttpMethod.Post, requestUriResult.Value))
        {
          httpReq.Content = JsonContent.Create(req, mediaType, jsonSerializerOptions);
          configureRequestHeaders?.Invoke(httpReq.Headers);
          var client = clientFactory.CreateClient(clientName);
          using (var httpResponse = await client.SendAsync(httpReq, HttpCompletionOption.ResponseHeadersRead, ct))
          {
            return await httpResponse.DeserializeResultAsync<TResponse>(ct);
          }
        }
      }
    }
    catch (Exception ex)
    {
      return ex;
    }
  }

  public async Task<Result> SendAsync<TRequest>(
    TRequest req,
    CancellationToken ct,
    MediaTypeHeaderValue? mediaType = null,
    JsonSerializerOptions? jsonSerializerOptions = null,
    Action<HttpRequestHeaders>? configureRequestHeaders = null,
    string? uriResolverName = null)
    where TRequest : IServiceRequest
  {
    try
    {
      using (var scope = serviceProvider.CreateScope())
      {
        var uriResolver = scope.ServiceProvider.GetRequiredKeyedService<IServiceEndpointUriResolver>(
          uriResolverName ?? ServiceEndpointDefinitions.DefaultUriResolverName);
        var requestUriResult = uriResolver.Resolve(req);
        if (requestUriResult.IsFailed)
        {
          return Result.Fail(requestUriResult);
        }
        if (!ServiceChannelRegistry.Instance.IsRequestRegistered<TRequest>(out var clientName))
        {
          return Result.CriticalError(string.Format(NoChannelRegistrationFound, typeof(TRequest)));
        }
        using (HttpRequestMessage httpReq = new(HttpMethod.Post, requestUriResult.Value))
        {
          httpReq.Content = JsonContent.Create(req, mediaType, jsonSerializerOptions);
          configureRequestHeaders?.Invoke(httpReq.Headers);
          var client = clientFactory.CreateClient(clientName);
          using (var httpResponse = await client.SendAsync(httpReq, HttpCompletionOption.ResponseHeadersRead, ct))
          {
            return await httpResponse.DeserializeResultAsync(ct);
          }
        }
      }
    }
    catch (Exception ex)
    {
      return ex;
    }
  }
}
