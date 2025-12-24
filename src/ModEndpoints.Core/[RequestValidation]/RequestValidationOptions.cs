namespace ModEndpoints.Core;

internal class RequestValidationOptions
{
  public required bool IsEnabled { get; set; }
  public required string DefaultServiceName { get; set; }
}
