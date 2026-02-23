using ModEndpoints.Core;
using ModResults;

namespace ModEndpoints;

public static class RequestValidationResultExtensions
{
  extension(RequestValidationResult validationResult)
  {
    /// <summary>
    /// Converts a <see cref="RequestValidationResult"/> to a Failed <see cref="Result"/> with failure type <see cref="FailureType.Invalid"/>.
    /// </summary>
    /// <returns></returns>
    public Result ToInvalidResult()
    {
      return Result.Invalid(
        validationResult.GetValidationErrors().ToArray());

    }

    /// <summary>
    /// Converts a <see cref="RequestValidationResult"/> to a Failed <see cref="Result{TValue}"/> with failure type <see cref="FailureType.Invalid"/>.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public Result<TValue> ToInvalidResult<TValue>()
      where TValue : notnull
    {
      return Result<TValue>.Invalid(
        validationResult.GetValidationErrors().ToArray());
    }

    /// <summary>
    /// Converts <see cref="RequestValidationFailure"/>s of a <see cref="RequestValidationResult"/> to a collection of <see cref="Error"/>.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Error> GetValidationErrors()
    {
      return validationResult.Errors?
        .Select(
          e => new Error(
            errorMessage: e.ErrorMessage,
            code: e.ErrorCode,
            propertyName: e.PropertyName)) ?? [];
    }
  }
}
