namespace ModEndpoints.Core;

public class RequestValidationResult
{
  public bool IsFailed { get; set; } = false;
  public List<RequestValidationFailure> Errors { get; set; } = [];
}

public class RequestValidationFailure
{
  public string PropertyName { get; set; } = string.Empty;
  public string ErrorMessage { get; set; } = string.Empty;
  public string ErrorCode { get; set; } = string.Empty;
  public object? AttemptedValue { get; set; }
}
