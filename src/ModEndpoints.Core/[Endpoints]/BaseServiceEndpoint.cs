using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using ModEndpoints.RemoteServices.Core;

namespace ModEndpoints.Core;
public abstract class BaseServiceEndpoint<TRequest, TResponse>
  : ServiceEndpointConfigurator, IServiceEndpoint
  where TRequest : IServiceRequestMarker
{
  protected sealed override Delegate ExecuteDelegate => ExecuteAsync;

  private async Task<TResponse> ExecuteAsync(
    [FromBody] TRequest req,
    HttpContext context)
  {
    var baseHandler = context.RequestServices.GetRequiredKeyedService(typeof(IEndpointConfigurator), GetType());
    var handler = baseHandler as BaseServiceEndpoint<TRequest, TResponse>
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
    return result;
  }

  /// <summary>
  /// Contains endpoint's logic to handle request. Input validation is completed before this method is called.
  /// </summary>
  /// <param name="req"></param>
  /// <param name="context"></param>
  /// <param name="ct"></param>
  /// <returns></returns>
  protected abstract Task<TResponse> HandleAsync(
    TRequest req,
    CancellationToken ct);

  /// <summary>
  /// This method is called if request validation fails, and is responsible for mapping <see cref="ValidationResult"/> to <typeparamref name="TResponse"/>.
  /// </summary>
  /// <param name="validationResult"></param>
  /// <param name="context"></param>
  /// <param name="ct"></param>
  /// <returns>Endpoint's <typeparamref name="TResponse"/> type validation failed response to caller.</returns>
  protected abstract ValueTask<TResponse> HandleInvalidValidationResultAsync(
    ValidationResult validationResult,
    HttpContext context,
    CancellationToken ct);
}
