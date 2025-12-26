using ModEndpoints.RemoteServices.Contracts;
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
  /// <typeparam name="TResponse">ServiceEndpoint response type.</typeparam>
  /// <param name="req">Request to be sent.</param>
  /// <param name="ct">The <see cref="CancellationToken"/> to cancel operation.</param>
  /// <returns>Response of remote service endpoint or failure result.</returns>
  Task<Result<TResponse>> SendAsync<TResponse>(
    IServiceRequest<TResponse> req,
    CancellationToken ct)
    where TResponse : notnull
  {
    return SendAsync(req, null, ct);
  }

  /// <summary>
  /// Sends request to remote service endpoint.
  /// </summary>
  /// <typeparam name="TResponse">ServiceEndpoint response type.</typeparam>
  /// <param name="req">Request to be sent.</param>
  /// <param name="endpointUriPrefix">Path to append as prefix to resolved enpoint uri. Usually used to add path segments to configured client's base address.</param>
  /// <param name="ct">The <see cref="CancellationToken"/> to cancel operation.</param>
  /// <param name="httpRequestInterceptor">Delegate to further configure created HTTP request message (headers, etc) before sending to ServiceEndpoint.</param>
  /// <param name="httpResponseInterceptor">Delegate to process received HTTP response message of ServiceEndpoint before deserialization.</param>
  /// <param name="uriResolverName"><see cref="IServiceEndpointUriResolver"/> name to be used to resolve ServiceEnpoint Uri.</param>
  /// <param name="serializerName"><see cref="IServiceChannelSerializer"/> name to be used to resolve ServiceEnpoint Uri.</param>
  /// <returns>Response of remote service endpoint or failure result.</returns>
  Task<Result<TResponse>> SendAsync<TResponse>(
    IServiceRequest<TResponse> req,
    string? endpointUriPrefix,
    CancellationToken ct,
    Func<IServiceProvider, HttpRequestMessage, CancellationToken, Task>? httpRequestInterceptor = null,
    Func<IServiceProvider, HttpResponseMessage, CancellationToken, Task>? httpResponseInterceptor = null,
    string? uriResolverName = null,
    string? serializerName = null)
    where TResponse : notnull;

  /// <summary>
  /// Sends request to remote service endpoint.
  /// </summary>
  /// <param name="req">Request to be sent.</param>
  /// <param name="ct">The <see cref="CancellationToken"/> to cancel operation.</param>
  /// <returns>Response of remote service endpoint or failure result.</returns>
  Task<Result> SendAsync(
    IServiceRequest req,
    CancellationToken ct)
  {
    return SendAsync(req, null, ct);
  }

  /// <summary>
  /// Sends request to remote service endpoint.
  /// </summary>
  /// <param name="req">Request to be sent.</param>
  /// <param name="endpointUriPrefix">Path to append as prefix to resolved enpoint uri. Usually used to add path segments to configured client's base address.</param>
  /// <param name="ct">The <see cref="CancellationToken"/> to cancel operation.</param>
  /// <param name="httpRequestInterceptor">Delegate to further configure created HTTP request message (headers, etc) before sending to ServiceEndpoint.</param>
  /// <param name="httpResponseInterceptor">Delegate to process received HTTP response message of ServiceEndpoint before deserialization.</param>
  /// <param name="uriResolverName"><see cref="IServiceEndpointUriResolver"/> name to be used to resolve ServiceEnpoint Uri.</param>
  /// <param name="serializerName"><see cref="IServiceChannelSerializer"/> name to be used to resolve ServiceEnpoint Uri.</param>
  /// <returns>Response of remote service endpoint or failure result.</returns>
  Task<Result> SendAsync(
    IServiceRequest req,
    string? endpointUriPrefix,
    CancellationToken ct,
    Func<IServiceProvider, HttpRequestMessage, CancellationToken, Task>? httpRequestInterceptor = null,
    Func<IServiceProvider, HttpResponseMessage, CancellationToken, Task>? httpResponseInterceptor = null,
    string? uriResolverName = null,
    string? serializerName = null);

  /// <summary>
  /// Sends request to remote service endpoint.
  /// </summary>
  /// <typeparam name="TResponse">ServiceEndpoint response type.</typeparam>
  /// <param name="req">Request to be sent.</param>
  /// <param name="ct">The <see cref="CancellationToken"/> to cancel operation.</param>
  /// <returns>Response stream of remote service endpoint or failure result as AsyncEnumerable.</returns>
  IAsyncEnumerable<StreamingResponseItem<TResponse>> SendAsync<TResponse>(
    IServiceRequestWithStreamingResponse<TResponse> req,
    CancellationToken ct)
    where TResponse : notnull
  {
    return SendAsync(req, null, ct);
  }

  /// <summary>
  /// Sends request to remote service endpoint.
  /// </summary>
  /// <typeparam name="TResponse">ServiceEndpoint response type.</typeparam>
  /// <param name="req">Request to be sent.</param>
  /// <param name="endpointUriPrefix">Path to append as prefix to resolved enpoint uri. Usually used to add path segments to configured client's base address.</param>
  /// <param name="ct">The <see cref="CancellationToken"/> to cancel operation.</param>
  /// <param name="httpRequestInterceptor">Delegate to further configure created HTTP request message (headers, etc) before sending to ServiceEndpoint.</param>
  /// <param name="httpResponseInterceptor">Delegate to process received HTTP response message of ServiceEndpoint before deserialization.</param>
  /// <param name="uriResolverName"><see cref="IServiceEndpointUriResolver"/> name to be used to resolve ServiceEnpoint Uri.</param>
  /// <param name="serializerName"><see cref="IServiceChannelSerializer"/> name to be used to resolve ServiceEnpoint Uri.</param>
  /// <returns>Response stream of remote service endpoint or failure result as IAsyncEnumerable.</returns>
  IAsyncEnumerable<StreamingResponseItem<TResponse>> SendAsync<TResponse>(
    IServiceRequestWithStreamingResponse<TResponse> req,
    string? endpointUriPrefix,
    CancellationToken ct,
    Func<IServiceProvider, HttpRequestMessage, CancellationToken, Task>? httpRequestInterceptor = null,
    Func<IServiceProvider, HttpResponseMessage, CancellationToken, Task>? httpResponseInterceptor = null,
    string? uriResolverName = null,
    string? serializerName = null)
    where TResponse : notnull;

  /// <summary>
  /// Sends request to remote service endpoint.
  /// </summary>
  /// <param name="req">Request to be sent.</param>
  /// <param name="ct">The <see cref="CancellationToken"/> to cancel operation.</param>
  /// <returns>Response stream of remote service endpoint or failure result as IAsyncEnumerable.</returns>
  IAsyncEnumerable<StreamingResponseItem> SendAsync(
    IServiceRequestWithStreamingResponse req,
    CancellationToken ct)
  {
    return SendAsync(req, null, ct);
  }


  /// <summary>
  /// Sends request to remote service endpoint.
  /// </summary>
  /// <param name="req">Request to be sent.</param>
  /// <param name="endpointUriPrefix">Path to append as prefix to resolved enpoint uri. Usually used to add path segments to configured client's base address.</param>
  /// <param name="ct">The <see cref="CancellationToken"/> to cancel operation.</param>
  /// <param name="httpRequestInterceptor">Delegate to further configure created HTTP request message (headers, etc) before sending to ServiceEndpoint.</param>
  /// <param name="httpResponseInterceptor">Delegate to process received HTTP response message of ServiceEndpoint before deserialization.</param>
  /// <param name="uriResolverName"><see cref="IServiceEndpointUriResolver"/> name to be used to resolve ServiceEnpoint Uri.</param>
  /// <param name="serializerName"><see cref="IServiceChannelSerializer"/> name to be used to resolve ServiceEnpoint Uri.</param>
  /// <returns>Response stream of remote service endpoint or failure result as IAsyncEnumerable.</returns>
  IAsyncEnumerable<StreamingResponseItem> SendAsync(
    IServiceRequestWithStreamingResponse req,
    string? endpointUriPrefix,
    CancellationToken ct,
    Func<IServiceProvider, HttpRequestMessage, CancellationToken, Task>? httpRequestInterceptor = null,
    Func<IServiceProvider, HttpResponseMessage, CancellationToken, Task>? httpResponseInterceptor = null,
    string? uriResolverName = null,
    string? serializerName = null);
}
