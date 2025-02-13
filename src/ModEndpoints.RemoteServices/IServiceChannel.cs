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
  /// <returns>Response of remote service endpoint or failure result.</returns>
  Task<Result<TResponse>> SendAsync<TRequest, TResponse>(
    TRequest req,
    CancellationToken ct)
    where TRequest : IServiceRequest<TResponse>
    where TResponse : notnull;

  /// <summary>
  /// Sends request to remote service endpoint.
  /// </summary>
  /// <typeparam name="TRequest">Type of ServiceEndpoint request that will be sent.</typeparam>
  /// <typeparam name="TResponse">ServiceEndpoint response type.</typeparam>
  /// <param name="req">Request to be sent.</param>
  /// <param name="endpointUriPrefix">Path to append as prefix to resolved enpoint uri. Usually used to add path segments to configured client's base address.</param>
  /// <param name="ct">The <see cref="CancellationToken"/> to cancel operation.</param>
  /// <param name="httpRequestInterceptor">Delegate to further configure created HTTP request message (headers, etc) before sending to ServiceEndpoint.</param>
  /// <param name="httpResponseInterceptor">Delegate to process received HTTP response message of ServiceEndpoint before deserialization.</param>
  /// <param name="uriResolverName"><see cref="IServiceEndpointUriResolver"/> name to be used to resolve ServiceEnpoint Uri.</param>
  /// <param name="serializerName"><see cref="IServiceChannelSerializer"/> name to be used to resolve ServiceEnpoint Uri.</param>
  /// <returns>Response of remote service endpoint or failure result.</returns>
  Task<Result<TResponse>> SendAsync<TRequest, TResponse>(
    TRequest req,
    string? endpointUriPrefix,
    CancellationToken ct,
    Func<IServiceProvider, HttpRequestMessage, CancellationToken, Task>? httpRequestInterceptor = null,
    Func<IServiceProvider, HttpResponseMessage, CancellationToken, Task>? httpResponseInterceptor = null,
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
  /// <returns>Response of remote service endpoint or failure result.</returns>
  Task<Result> SendAsync<TRequest>(
    TRequest req,
    CancellationToken ct)
    where TRequest : IServiceRequest;

  /// <summary>
  /// Sends request to remote service endpoint.
  /// </summary>
  /// <typeparam name="TRequest">Type of ServiceEndpoint request that will be sent.</typeparam>
  /// <param name="req">Request to be sent.</param>
  /// <param name="endpointUriPrefix">Path to append as prefix to resolved enpoint uri. Usually used to add path segments to configured client's base address.</param>
  /// <param name="ct">The <see cref="CancellationToken"/> to cancel operation.</param>
  /// <param name="httpRequestInterceptor">Delegate to further configure created HTTP request message (headers, etc) before sending to ServiceEndpoint.</param>
  /// <param name="httpResponseInterceptor">Delegate to process received HTTP response message of ServiceEndpoint before deserialization.</param>
  /// <param name="uriResolverName"><see cref="IServiceEndpointUriResolver"/> name to be used to resolve ServiceEnpoint Uri.</param>
  /// <param name="serializerName"><see cref="IServiceChannelSerializer"/> name to be used to resolve ServiceEnpoint Uri.</param>
  /// <returns>Response of remote service endpoint or failure result.</returns>
  Task<Result> SendAsync<TRequest>(
    TRequest req,
    string? endpointUriPrefix,
    CancellationToken ct,
    Func<IServiceProvider, HttpRequestMessage, CancellationToken, Task>? httpRequestInterceptor = null,
    Func<IServiceProvider, HttpResponseMessage, CancellationToken, Task>? httpResponseInterceptor = null,
    string? uriResolverName = null,
    string? serializerName = null)
    where TRequest : IServiceRequest;
}
