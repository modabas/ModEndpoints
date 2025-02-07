using System.Net.Http.Headers;
using ModEndpoints.RemoteServices.Core;
using ModResults;

namespace ModEndpoints.RemoteServices;

/// <summary>
/// ServiceEndpoint channel for sending requests to remote services via configured clients.
/// </summary>
public interface IServiceChannel
{
  /// <summary>
  /// Sends request to remote service endpoint.
  /// </summary>
  /// <typeparam name="TRequest">Type of ServiceEndpoint request that will be sent.</typeparam>
  /// <typeparam name="TResponse">ServiceEndpoint response type.</typeparam>
  /// <param name="req">Request to be sent.</param>
  /// <param name="ct">The <see cref="CancellationToken"/> to cancel operation.</param>
  /// <param name="endpointUriPrefix">Path to append as prefix to resolved enpoint uri. Usually used to add path segments to configured client's base address.</param>
  /// <param name="mediaType">The media type to use for the content.</param>
  /// <param name="configureRequestHeaders">Delegate to configure HTTP request headers.</param>
  /// <param name="uriResolverName"><see cref="IServiceEndpointUriResolver"/> name to be used to resolve ServiceEnpoint Uri.</param>
  /// <param name="serializerName"><see cref="IServiceChannelSerializer"/> name to be used to resolve ServiceEnpoint Uri.</param>
  /// <returns>Response of remote service endpoint or failure result.</returns>
  Task<Result<TResponse>> SendAsync<TRequest, TResponse>(
    TRequest req,
    CancellationToken ct,
    string? endpointUriPrefix = null,
    MediaTypeHeaderValue? mediaType = null,
    Action<HttpRequestHeaders>? configureRequestHeaders = null,
    string? uriResolverName = null,
    string? serializerName = null)
    where TRequest : IServiceRequest<TResponse>
    where TResponse : notnull;

  /// <summary>
  /// Sends request to remote service endpoint.
  /// </summary>
  /// <typeparam name="TRequest">Type of ServiceEndpoint request that will be sent.</typeparam>
  /// <param name="req">Request to be sent.</param>
  /// <param name="ct">The <see cref="CancellationToken"/> to cancel operation.</param>
  /// <param name="endpointUriPrefix">Path to append as prefix to resolved enpoint uri. Usually used to add path segments to configured client's base address.</param>
  /// <param name="mediaType">The media type to use for the content.</param>
  /// <param name="configureRequestHeaders">Delegate to configure HTTP request headers.</param>
  /// <param name="uriResolverName"><see cref="IServiceEndpointUriResolver"/> name to be used to resolve ServiceEnpoint Uri.</param>
  /// <param name="serializerName"><see cref="IServiceChannelSerializer"/> name to be used to resolve ServiceEnpoint Uri.</param>
  /// <returns>Response of remote service endpoint or failure result.</returns>
  Task<Result> SendAsync<TRequest>(
    TRequest req,
    CancellationToken ct,
    string? endpointUriPrefix = null,
    MediaTypeHeaderValue? mediaType = null,
    Action<HttpRequestHeaders>? configureRequestHeaders = null,
    string? uriResolverName = null,
    string? serializerName = null)
    where TRequest : IServiceRequest;
}
