using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using ModEndpoints.RemoteServices.Core;
using ModResults;

namespace ModEndpoints.RemoteServices;

public class ServiceChannel(
  IHttpClientFactory clientFactory,
  IServiceEndpointUriResolver uriResolver)
  : IServiceChannel
{
  public async Task<Result<TResponse>> SendAsync<TRequest, TResponse>(
    TRequest req,
    CancellationToken ct,
    MediaTypeHeaderValue? mediaType = null,
    JsonSerializerOptions? jsonSerializerOptions = null,
    Action<HttpRequestHeaders>? configureRequestHeaders = null)
    where TRequest : IServiceRequest<TResponse>
    where TResponse : notnull
  {
    try
    {
      var requestUri = uriResolver.Resolve(req);
      if (string.IsNullOrWhiteSpace(requestUri))
      {
        return Result<TResponse>.CriticalError("Cannot resolve uri for service endpoint.");
      }
      if (!ServiceChannelRegistry.Instance.IsRegistered<TRequest>(out var clientName))
      {
        return Result<TResponse>.CriticalError($"No channel registration found for request type {typeof(TRequest)}");
      }
      using (HttpRequestMessage httpReq = new(HttpMethod.Post, requestUri))
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
    Action<HttpRequestHeaders>? configureRequestHeaders = null)
    where TRequest : IServiceRequest
  {
    try
    {
      var requestUri = uriResolver.Resolve(req);
      if (string.IsNullOrWhiteSpace(requestUri))
      {
        return Result.CriticalError("Cannot resolve uri for service endpoint.");
      }
      if (!ServiceChannelRegistry.Instance.IsRegistered<TRequest>(out var clientName))
      {
        return Result.CriticalError($"No channel registration found for request type {typeof(TRequest)}");
      }
      using (HttpRequestMessage httpReq = new(HttpMethod.Post, requestUri))
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
    catch (Exception ex)
    {
      return ex;
    }
  }
}
