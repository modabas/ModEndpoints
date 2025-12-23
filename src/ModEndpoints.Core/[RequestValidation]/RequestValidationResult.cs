using System.Collections.ObjectModel;

namespace ModEndpoints.Core;

public class RequestValidationResult
{
  public bool IsOk { get; init; }
  public bool IsFailed => !IsOk;

  private readonly List<RequestValidationFailure>? _errors;
  public ReadOnlyCollection<RequestValidationFailure>? Errors => _errors?.AsReadOnly();
  public RequestValidationResult(bool isOk)
  {
    IsOk = isOk;
  }
  public RequestValidationResult(bool isOk, IEnumerable<RequestValidationFailure> errors)
  {
    IsOk = isOk;
    _errors = [.. errors];
  }
}

public class RequestValidationFailure
{
  public string PropertyName { get; set; } = string.Empty;
  public string ErrorMessage { get; set; } = string.Empty;
  public string ErrorCode { get; set; } = string.Empty;
  public object? AttemptedValue { get; set; }
}
