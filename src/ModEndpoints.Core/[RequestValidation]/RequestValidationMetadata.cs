namespace ModEndpoints.Core;

internal record RequestValidationMetadata(
  bool IsEnabled = true,
  string? ServiceName = null);
