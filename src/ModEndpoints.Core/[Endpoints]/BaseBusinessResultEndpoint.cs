using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ModEndpoints.Core;

public abstract class BaseBusinessResultEndpoint<TRequest, TResponse>
  : EndpointConfigurator, IBusinessResultEndpoint
  where TRequest : notnull
{
  protected sealed override Delegate ExecuteDelegate => ExecuteAsync;

  private async Task<TResponse> ExecuteAsync(
    [AsParameters] TRequest req,
    HttpContext context)
  {
    var baseHandler = context.RequestServices.GetRequiredKeyedService(typeof(IEndpointConfigurator), GetType());
    var handler = baseHandler as BaseBusinessResultEndpoint<TRequest, TResponse>
      ?? throw new InvalidOperationException(Constants.RequiredServiceIsInvalidMessage);
    var ct = context.RequestAborted;

    //Request validation
    var validator = context.RequestServices.GetService<IRequestValidator>();
    if (validator is not null)
    {
      var validationResult = await validator.ValidateAsync(req, context, ct);
      if (validationResult.IsFailed)
      {
        return await HandleInvalidValidationResultAsync(validationResult, context, ct);
      }
    }

    //Handler
    return await handler.HandleAsync(req, ct);
  }

  /// <summary>
  /// Contains endpoint's logic to handle request. Input validation is completed before this method is called.
  /// </summary>
  /// <param name="req"></param>
  /// <param name="ct"></param>
  /// <returns></returns>
  protected abstract Task<TResponse> HandleAsync(
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

public abstract class BaseBusinessResultEndpoint<TResponse>
  : EndpointConfigurator, IBusinessResultEndpoint
{
  protected sealed override Delegate ExecuteDelegate => ExecuteAsync;

  private async Task<TResponse> ExecuteAsync(
    HttpContext context)
  {
    var baseHandler = context.RequestServices.GetRequiredKeyedService(typeof(IEndpointConfigurator), GetType());
    var handler = baseHandler as BaseBusinessResultEndpoint<TResponse>
      ?? throw new InvalidOperationException(Constants.RequiredServiceIsInvalidMessage);
    var ct = context.RequestAborted;

    //Handler
    return await handler.HandleAsync(ct);
  }

  /// <summary>
  /// Contains endpoint's logic to handle request.
  /// </summary>
  /// <param name="ct"></param>
  /// <returns></returns>
  protected abstract Task<TResponse> HandleAsync(
    CancellationToken ct);
}
