namespace ModEndpoints.Core;

/// <summary>
/// An exception that represents failed validation
/// </summary>
[Serializable]
public class RequestValidationException : Exception
{
  /// <summary>
  /// Validation errors
  /// </summary>
  public IEnumerable<RequestValidationFailure> Errors { get; private set; }

  /// <summary>
  /// Creates a new ValidationException
  /// </summary>
  /// <param name="errors"></param>
  public RequestValidationException(IEnumerable<RequestValidationFailure> errors) : base(BuildErrorMessage(errors))
  {
    Errors = errors;
  }

  private static string BuildErrorMessage(IEnumerable<RequestValidationFailure> errors)
  {
    var arr = errors.Select(x => $"{Environment.NewLine} -- {x.PropertyName}: {x.ErrorMessage}");
    return "Validation failed: " + string.Join(string.Empty, arr);
  }
}
