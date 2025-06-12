namespace ModEndpoints.Core;

internal class ComponentRegistryAccessor
{
  private static readonly AsyncLocal<ComponentRegistryHolder> _registry = new();

  /// <inheritdoc/>
  public ComponentRegistry? Registry
  {
    get
    {
      return _registry.Value?.Registry;
    }
    set
    {
      // Clear current ComponentRegistry trapped in the AsyncLocals, as its done.
      if (_registry.Value is not null)
      {
        _registry.Value.Registry = null;
      }

      if (value != null)
      {
        // Use an object indirection to hold the ComponentRegistry in the AsyncLocal,
        // so it can be cleared in all ExecutionContexts when its cleared.
        _registry.Value = new ComponentRegistryHolder { Registry = value };
      }
    }
  }

  private sealed class ComponentRegistryHolder
  {
    public ComponentRegistry? Registry;
  }

  private static Lazy<ComponentRegistryAccessor> _instance =
    new Lazy<ComponentRegistryAccessor>(
      () => new ComponentRegistryAccessor(),
      LazyThreadSafetyMode.ExecutionAndPublication);

  public static ComponentRegistryAccessor Instance => _instance.Value;

  private ComponentRegistryAccessor()
  {
  }
}
