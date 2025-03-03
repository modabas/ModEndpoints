using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace ModEndpoints.Core;

public abstract class MinimalEndpoint<TRequest, TResponse>
  : EndpointConfigurator, IMinimalEndpoint
  where TRequest : notnull
{
  protected sealed override Delegate ExecuteDelegate => ExecuteAsync;

  private async Task<TResponse> ExecuteAsync(
    [AsParameters] TRequest req,
    HttpContext context)
  {
    var baseHandler = context.RequestServices.GetRequiredKeyedService(typeof(IEndpointConfigurator), GetType());
    var handler = baseHandler as MinimalEndpoint<TRequest, TResponse>
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
  /// <param name="ct"></param>
  /// <returns></returns>
  protected abstract Task<TResponse> HandleAsync(
    TRequest req,
    CancellationToken ct);

  /// <summary>
  /// This method is called if request validation fails, and is responsible for mapping <see cref="ValidationResult"/> to <typeparamref name="TResponse"/> if <typeparamref name="TResponse"/> is assignable from <see cref="IResult"/>.
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
    var responseType = typeof(TResponse);

    //Is using TypedResults
    if (responseType.IsGenericType &&
      responseType.Name.StartsWith("Results`") &&
      (responseType.Namespace?.Equals("Microsoft.AspNetCore.Http.HttpResults") ?? false))
    {
      if (TryUseImplicitOperatorFor<ProblemHttpResult>(
        responseType,
        validationResult,
        vr => vr.ToTypedProblem(),
        out var problem))
      {
        return new ValueTask<TResponse>(problem);
      }
      if (TryUseImplicitOperatorFor<ValidationProblem>(
        responseType,
        validationResult,
        vr => vr.ToTypedValidationProblem(),
        out var validationProblem))
      {
        return new ValueTask<TResponse>(validationProblem);
      }
      if (TryUseImplicitOperatorFor<BadRequest<HttpValidationProblemDetails>>(
        responseType,
        validationResult,
        vr => vr.ToTypedBadRequestWithValidationProblem(),
        out var badRequestWithValidationProblem))
      {
        return new ValueTask<TResponse>(badRequestWithValidationProblem);
      }
      if (TryUseImplicitOperatorFor<BadRequest<ProblemDetails>>(
        responseType,
        validationResult,
        vr => vr.ToTypedBadRequestWithProblem(),
        out var badRequestWithProblem))
      {
        return new ValueTask<TResponse>(badRequestWithProblem);
      }
      if (TryUseImplicitOperatorFor<BadRequest>(
        responseType,
        validationResult,
        _ => TypedResults.BadRequest(),
        out var badRequest))
      {
        return new ValueTask<TResponse>(badRequest);
      }
    }

    if (responseType.IsAssignableFrom(typeof(IResult)))
    {
      return new ValueTask<TResponse>((TResponse)validationResult.ToMinimalApiResult());
    }
    else
    {
      throw new ValidationException(validationResult.Errors);
    }
  }

  private static bool TryUseImplicitOperatorFor<T>(
    Type responseType,
    ValidationResult validationResult,
    Func<ValidationResult, T> conversionFunc,
    [NotNullWhen(true)] out TResponse? response)
  {
    var converter = responseType.GetMethod("op_Implicit", new[] { typeof(T) });

    if (converter is not null)
    {
      var result = converter.Invoke(null, new[] { (object?)conversionFunc(validationResult) });
      if (result is not null)
      {
        response = (TResponse)result;
        return true;
      }
    }
    response = default;
    return false;
  }
}

public abstract class MinimalEndpoint<TResponse>
  : EndpointConfigurator, IMinimalEndpoint
{
  protected sealed override Delegate ExecuteDelegate => ExecuteAsync;

  private async Task<TResponse> ExecuteAsync(
    HttpContext context)
  {
    var baseHandler = context.RequestServices.GetRequiredKeyedService(typeof(IEndpointConfigurator), GetType());
    var handler = baseHandler as MinimalEndpoint<TResponse>
      ?? throw new InvalidOperationException(Constants.RequiredServiceIsInvalidMessage);
    var ct = context.RequestAborted;

    //Handler
    var result = await handler.HandleAsync(ct);
    return result;
  }

  /// <summary>
  /// Contains endpoint's logic to handle request.
  /// </summary>
  /// <param name="ct"></param>
  /// <returns></returns>
  protected abstract Task<TResponse> HandleAsync(
    CancellationToken ct);
}
