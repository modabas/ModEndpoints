using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using ModEndpoints.RemoteServices.Core;

namespace ModEndpoints.Core;
public abstract class BaseServiceEndpointWithStreamingResponse<TRequest, TResponse>
  : ServiceEndpointConfigurator, IServiceEndpoint
  where TRequest : IServiceRequestMarker
{
  protected sealed override Delegate ExecuteDelegate => ExecuteAsync;

  private async IAsyncEnumerable<TResponse> ExecuteAsync(
    [FromBody] TRequest req,
    HttpContext context)
  {
    var baseHandler = context.RequestServices.GetRequiredKeyedService(typeof(IEndpointConfigurator), GetType());
    var handler = baseHandler as BaseServiceEndpointWithStreamingResponse<TRequest, TResponse>
      ?? throw new InvalidOperationException(Constants.RequiredServiceIsInvalidMessage);
    var ct = context.RequestAborted;

    //Request validation
    var validator = context.RequestServices.GetService<IRequestValidator>();
    if (validator is not null)
    {
      var validationResult = await validator.ValidateAsync(req, context, ct);
      if (validationResult.IsFailed)
      {
        yield return await HandleInvalidValidationResultAsync(validationResult, context, ct);
        yield break;
      }
    }

    //Handler
    await foreach (var item in handler.HandleAsync(req, ct).WithCancellation(ct))
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
