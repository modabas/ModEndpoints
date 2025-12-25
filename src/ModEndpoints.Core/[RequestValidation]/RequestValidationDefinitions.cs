namespace ModEndpoints.Core;

public static class RequestValidationDefinitions
{
  public const string DefaultServiceName = "Default";

  internal static readonly RequestValidationResult SuccessfulValidationResult = new(IsOk: true, Errors: null);
}
