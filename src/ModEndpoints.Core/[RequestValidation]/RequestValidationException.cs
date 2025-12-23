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
  /// Initializes a new instance of the RequestValidationException class with the specified collection of validation
  /// failures.
  /// </summary>
  /// <remarks>Use this constructor to create an exception that encapsulates multiple validation errors encountered
  /// during a request. The provided errors are accessible through the Errors property.</remarks>
  /// <param name="errors">A collection of RequestValidationFailure objects that describe the validation errors that caused the exception.
  /// Cannot be null.</param>
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
