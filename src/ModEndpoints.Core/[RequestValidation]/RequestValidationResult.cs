namespace ModEndpoints.Core;

public record RequestValidationResult(bool IsOk, List<RequestValidationFailure>? Errors)
{
  public bool IsFailed => !IsOk;
}

public record RequestValidationFailure(
  string PropertyName,
  string ErrorMessage,
  string ErrorCode,
  object? AttemptedValue);
