using System.Net.Http.Headers;
using System.Text.Json;
using ModEndpoints.RemoteServices.Core;
using ModResults;

namespace ModEndpoints.RemoteServices;
public interface IServiceChannel
{
  Task<Result<TResponse>> SendAsync<TRequest, TResponse>(
    TRequest req,
    CancellationToken ct,
    string? endpointUriPrefix = null,
    MediaTypeHeaderValue? mediaType = null,
    JsonSerializerOptions? jsonSerializerOptions = null,
    Action<HttpRequestHeaders>? configureRequestHeaders = null,
    string? uriResolverName = null)
    where TRequest : IServiceRequest<TResponse>
    where TResponse : notnull;
  Task<Result> SendAsync<TRequest>(
    TRequest req,
    CancellationToken ct,
    string? endpointUriPrefix = null,
    MediaTypeHeaderValue? mediaType = null,
    JsonSerializerOptions? jsonSerializerOptions = null,
    Action<HttpRequestHeaders>? configureRequestHeaders = null,
    string? uriResolverName = null)
    where TRequest : IServiceRequest;
}
