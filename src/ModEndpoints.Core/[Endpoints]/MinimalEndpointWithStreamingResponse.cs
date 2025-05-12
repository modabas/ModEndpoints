using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ModEndpoints.Core;

public abstract class MinimalEndpointWithStreamingResponse<TRequest, TResponse>
  : EndpointConfigurator, IMinimalEndpoint
  where TRequest : notnull
{
  protected sealed override Delegate ExecuteDelegate => ExecuteAsync;

  private async IAsyncEnumerable<TResponse> ExecuteAsync(
    [AsParameters] TRequest req,
    HttpContext context)
  {
    var baseHandler = context.RequestServices.GetRequiredKeyedService(typeof(IEndpointConfigurator), GetType());
    var handler = baseHandler as MinimalEndpointWithStreamingResponse<TRequest, TResponse>
      ?? throw new InvalidOperationException(Constants.RequiredServiceIsInvalidMessage);
    var ct = context.RequestAborted;

    //Request validation
    var validator = context.RequestServices.GetService<IValidator<TRequest>>();
    if (validator is not null)
    {
      var validationResult = await validator.ValidateAsync(req, ct);
      if (!validationResult.IsValid)
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
  /// This method is called if request validation fails, and is responsible for handling failed <see cref="ValidationResult"/>.
  /// Throws <see cref="ValidationException"/> otherwise.
  /// </summary>
  /// <param name="validationResult"></param>
  /// <param name="context"></param>
  /// <param name="ct"></param>
  /// <returns>Endpoint's <typeparamref name="TResponse"/> type validation failed response to caller.</returns>
  /// <exception cref="ValidationException"></exception>
  protected virtual ValueTask<TResponse> HandleInvalidValidationResultAsync(
    ValidationResult validationResult,
    HttpContext context,
    CancellationToken ct)
  {
    throw new ValidationException(validationResult.Errors);
  }
}

public abstract class MinimalEndpointWithStreamingResponse<TResponse>
  : EndpointConfigurator, IMinimalEndpoint
{
  protected sealed override Delegate ExecuteDelegate => ExecuteAsync;

  private async IAsyncEnumerable<TResponse> ExecuteAsync(
    HttpContext context)
  {
    var baseHandler = context.RequestServices.GetRequiredKeyedService(typeof(IEndpointConfigurator), GetType());
    var handler = baseHandler as MinimalEndpointWithStreamingResponse<TResponse>
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
