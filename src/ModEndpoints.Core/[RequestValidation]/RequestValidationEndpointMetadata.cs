namespace ModEndpoints.Core;

internal record RequestValidationEndpointMetadata(
  bool IsEnabled = true,
  string? ServiceName = null);
