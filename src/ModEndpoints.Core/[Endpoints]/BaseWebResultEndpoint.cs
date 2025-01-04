using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ModEndpoints.Core;

public abstract class BaseWebResultEndpoint<TRequest, THandlerResult>
  : EndpointConfigurator, IWebResultEndpoint
  where TRequest : notnull
{
  protected sealed override Delegate ExecuteDelegate => ExecuteAsync;

  private async Task<IResult> ExecuteAsync(
    [AsParameters] TRequest req,
    HttpContext context)
  {
    var baseHandler = context.RequestServices.GetRequiredKeyedService(typeof(IEndpointConfigurator), GetType());
    var handler = baseHandler as BaseWebResultEndpoint<TRequest, THandlerResult>
      ?? throw new InvalidOperationException(Constants.RequiredServiceIsInvalidMessage);
    var ct = context.RequestAborted;

    //Request validation
    var validator = context.RequestServices.GetService<IValidator<TRequest>>();
    if (validator is not null)
    {
      var validationResult = await validator.ValidateAsync(req, ct);
      if (!validationResult.IsValid)
      {
        return await HandleInvalidValidationResultAsync(validationResult, context, ct);
      }
    }

    //Handler
    var result = await handler.HandleAsync(req, ct);
    //Post handler mapping
    return await handler.ConvertResultToResponseAsync(result, context, ct);
  }

  /// <summary>
  /// This method is called if request validation fails, and is responsible for mapping <see cref="ValidationResult"/> to <see cref="IResult"/>.
  /// </summary>
  /// <param name="validationResult"></param>
  /// <param name="context"></param>
  /// <param name="ct"></param>
  /// <returns>Endpoint's <see cref="IResult"/> type validation failed response to caller.</returns>
  protected virtual ValueTask<IResult> HandleInvalidValidationResultAsync(
    ValidationResult validationResult,
    HttpContext context,
    CancellationToken ct)
  {
    return new ValueTask<IResult>(validationResult.ToMinimalApiResult());
  }

  /// <summary>
  /// Runs just before returning response to caller, and is responsible for mapping result parameter of type <typeparamref name="THandlerResult"/> to <see cref="IResult"/>.
  /// </summary>
  /// <param name="result"></param>
  /// <param name="context"></param>
  /// <param name="ct"></param>
  /// <returns>Endpoint's <see cref="IResult"/> type response to caller.</returns>
  protected abstract ValueTask<IResult> ConvertResultToResponseAsync(
    THandlerResult result,
    HttpContext context,
    CancellationToken ct);

  /// <summary>
  /// Contains endpoint's logic to handle request. Input validation is completed before this method is called.
  /// </summary>
  /// <param name="req"></param>
  /// <param name="ct"></param>
  /// <returns></returns>
  protected abstract Task<THandlerResult> HandleAsync(
    TRequest req,
    CancellationToken ct);
}

public abstract class BaseWebResultEndpoint<THandlerResult>
  : EndpointConfigurator, IWebResultEndpoint
{
  protected sealed override Delegate ExecuteDelegate => ExecuteAsync;

  private async Task<IResult> ExecuteAsync(
    HttpContext context)
  {
    var baseHandler = context.RequestServices.GetRequiredKeyedService(typeof(IEndpointConfigurator), GetType());
    var handler = baseHandler as BaseWebResultEndpoint<THandlerResult>
      ?? throw new InvalidOperationException(Constants.RequiredServiceIsInvalidMessage);
    var ct = context.RequestAborted;

    //Handler
    var result = await handler.HandleAsync(ct);
    //Post handler mapping
    return await handler.ConvertResultToResponseAsync(result, context, ct);
  }

  /// <summary>
  /// Runs just before returning response to caller, and is responsible for mapping result parameter of type <typeparamref name="THandlerResult"/> to <see cref="IResult"/>.
  /// </summary>
  /// <param name="result"></param>
  /// <param name="context"></param>
  /// <param name="ct"></param>
  /// <returns>Endpoint's <see cref="IResult"/> response to caller.</returns>
  protected abstract ValueTask<IResult> ConvertResultToResponseAsync(
    THandlerResult result,
    HttpContext context,
    CancellationToken ct);

  /// <summary>
  /// Contains endpoint's logic to handle request.
  /// </summary>
  /// <param name="ct"></param>
  /// <returns></returns>
  protected abstract Task<THandlerResult> HandleAsync(
    CancellationToken ct);
}
