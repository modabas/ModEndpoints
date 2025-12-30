

namespace ModEndpoints.Core;

internal class RequestValidationOptions : IEquatable<RequestValidationOptions?>
{
  public required bool IsEnabled { get; set; }
  public required string ServiceName { get; set; }
  public required bool IsPerEndpointCustomizationEnabled { get; set; }

  public override bool Equals(object? obj)
  {
    return Equals(obj as RequestValidationOptions);
  }

  public bool Equals(RequestValidationOptions? other)
  {
    return other is not null &&
           IsEnabled == other.IsEnabled &&
           ServiceName == other.ServiceName &&
           IsPerEndpointCustomizationEnabled == other.IsPerEndpointCustomizationEnabled;
  }

  public override int GetHashCode()
  {
    return HashCode.Combine(IsEnabled, ServiceName, IsPerEndpointCustomizationEnabled);
  }

  public static bool operator ==(RequestValidationOptions? left, RequestValidationOptions? right)
  {
    return EqualityComparer<RequestValidationOptions>.Default.Equals(left, right);
  }

  public static bool operator !=(RequestValidationOptions? left, RequestValidationOptions? right)
  {
    return !(left == right);
  }
}
