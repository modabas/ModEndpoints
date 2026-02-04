using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using ModEndpoints.RemoteServices.Contracts;

namespace ModEndpoints.Core;

/// <summary>
/// Abstract base class for service endpoints that return a streaming response of type <see cref="IAsyncEnumerable{TResponse}"/> from HandleAsync method.<br/>
/// </summary>
/// <typeparam name="TRequest">Request type.</typeparam>
/// <typeparam name="TResponse">Response type containing business result.</typeparam>
public abstract class BaseServiceEndpointWithStreamingResponse<TRequest, TResponse>
  : ServiceEndpointConfigurator, IServiceEndpoint
  where TRequest : IServiceRequestMarker
{
  protected sealed override Delegate ExecuteDelegate => ExecuteAsync;

  private async IAsyncEnumerable<TResponse> ExecuteAsync(
    [FromBody] TRequest req,
    HttpContext context)
  {
    var handler = context.RequestServices.GetRequiredKeyedService(
        typeof(IEndpointConfigurator),
        typeof(TRequest))
      as BaseServiceEndpointWithStreamingResponse<TRequest, TResponse>
      ?? throw new InvalidOperationException(Constants.RequiredServiceIsInvalidMessage);
    var ct = context.RequestAborted;

    //Request validation
    {
      var validationController = context.RequestServices.GetRequiredService<IRequestValidationController>();
      var validationResult = await validationController.ValidateAsync(req, context, ct).ConfigureAwait(false);
      if (validationResult?.IsFailed == true)
      {
        yield return await HandleInvalidValidationResultAsync(validationResult, context, ct).ConfigureAwait(false);
        yield break;
      }
    }
    //Handler
    await foreach (var item in handler.HandleAsync(req, ct).WithCancellation(ct).ConfigureAwait(false))
    {
      yield return item;
    }
  }

  /// <summary>
  /// Contains endpoint's logic to handle request. Input validation is completed before this method is called.
  /// </summary>
  /// <param name="req"></param>
  /// <param name="ct"></param>
  /// <returns></returns>
  protected abstract IAsyncEnumerable<TResponse> HandleAsync(
    TRequest req,
    CancellationToken ct);

  /// <summary>
  /// This method is called if request validation fails, and is responsible for mapping <see cref="RequestValidationResult"/> to <typeparamref name="TResponse"/>.
  /// </summary>
  /// <param name="validationResult"></param>
  /// <param name="context"></param>
  /// <param name="ct"></param>
  /// <returns>Endpoint's <typeparamref name="TResponse"/> type validation failed response to caller.</returns>
  protected abstract ValueTask<TResponse> HandleInvalidValidationResultAsync(
    RequestValidationResult validationResult,
    HttpContext context,
    CancellationToken ct);
}
