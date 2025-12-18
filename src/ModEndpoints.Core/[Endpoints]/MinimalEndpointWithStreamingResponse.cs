using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ModEndpoints.Core;

/// <summary>
/// Abstract base class for endpoints that return a streaming response of type <see cref="IAsyncEnumerable{TResponse}"/> from HandleAsync method.<br/>
/// > **Note:** .Net 10 introduced `ServerSentEventsResult` IResult type to return streaming responses, which can be used as response type of a `MinimalEndpoint`.
/// </summary>
/// <typeparam name="TRequest">Request type.</typeparam>
/// <typeparam name="TResponse">Response type.</typeparam>
public abstract class MinimalEndpointWithStreamingResponse<TRequest, TResponse>
  : EndpointConfigurator, IMinimalEndpoint
  where TRequest : notnull
{
  protected sealed override Delegate ExecuteDelegate => ExecuteAsync;

  private async IAsyncEnumerable<TResponse> ExecuteAsync(
    [AsParameters] TRequest req,
    HttpContext context)
  {
    var handler = context.RequestServices.GetRequiredKeyedService(
        typeof(IEndpointConfigurator),
        GetType())
      as MinimalEndpointWithStreamingResponse<TRequest, TResponse>
      ?? throw new InvalidOperationException(Constants.RequiredServiceIsInvalidMessage);
    var ct = context.RequestAborted;

    //Request validation
    {
      var validator = context.RequestServices.GetService<IRequestValidatorService>();
      if (validator is not null)
      {
        var validationResult = await validator.ValidateAsync(req, context, ct);
        if (validationResult.IsFailed)
        {
          yield return await HandleInvalidValidationResultAsync(validationResult, context, ct);
          yield break;
        }
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
  /// This method is called if request validation fails, and is responsible for handling failed <see cref="RequestValidationResult"/>.
  /// Throws <see cref="RequestValidationException"/> otherwise.
  /// </summary>
  /// <param name="validationResult"></param>
  /// <param name="context"></param>
  /// <param name="ct"></param>
  /// <returns>Endpoint's <typeparamref name="TResponse"/> type validation failed response to caller.</returns>
  /// <exception cref="RequestValidationException"></exception>
  protected virtual ValueTask<TResponse> HandleInvalidValidationResultAsync(
    RequestValidationResult validationResult,
    HttpContext context,
    CancellationToken ct)
  {
    throw new RequestValidationException(validationResult.Errors ?? []);
  }
}

/// <summary>
/// Abstract base class for endpoints that return a streaming response of type <see cref="IAsyncEnumerable{TResponse}"/> from HandleAsync method.<br/>
/// > **Note:** .Net 10 introduced `ServerSentEventsResult` IResult type to return streaming responses, which can be used as response type of a `MinimalEndpoint`.
/// </summary>
/// <typeparam name="TResponse">Response type.</typeparam>
public abstract class MinimalEndpointWithStreamingResponse<TResponse>
  : EndpointConfigurator, IMinimalEndpoint
{
  protected sealed override Delegate ExecuteDelegate => ExecuteAsync;

  private async IAsyncEnumerable<TResponse> ExecuteAsync(
    HttpContext context)
  {
    var handler = context.RequestServices.GetRequiredKeyedService(
        typeof(IEndpointConfigurator),
        GetType())
      as MinimalEndpointWithStreamingResponse<TResponse>
      ?? throw new InvalidOperationException(Constants.RequiredServiceIsInvalidMessage);

    var ct = context.RequestAborted;

    //Handler
    await foreach (var item in handler.HandleAsync(ct).WithCancellation(ct))
    {
      yield return item;
    }
  }

  /// <summary>
  /// Contains endpoint's logic to handle request.
  /// </summary>
  /// <param name="ct"></param>
  /// <returns></returns>
  protected abstract IAsyncEnumerable<TResponse> HandleAsync(
    CancellationToken ct);
}
