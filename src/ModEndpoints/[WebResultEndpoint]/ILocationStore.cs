namespace ModEndpoints;

public interface ILocationStore
{
  ValueTask<string?> GetValueAsync(CancellationToken ct);
  ValueTask SetValueAsync(string? uri, CancellationToken ct);
  ValueTask SetValueAsync(string? routeName, object? routeValues, CancellationToken ct);
  ValueTask SetValueAsync(Uri? uri, CancellationToken ct);
}
