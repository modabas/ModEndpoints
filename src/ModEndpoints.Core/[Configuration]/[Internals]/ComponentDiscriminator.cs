namespace ModEndpoints.Core;

internal sealed class ComponentDiscriminator : IComponentDiscriminator
{
  private readonly Dictionary<Type, int> _registry = new();

  public int GetDiscriminator<T>(T component)
    where T : notnull
  {
    var type = component.GetType();
    //GetOrAdd
    if (_registry.TryGetValue(type, out var discriminator))
    {
      return discriminator;
    }
    _registry[type] = 0;
    return 0;
  }

  public int IncrementDiscriminator<T>(T component)
    where T : notnull
  {
    var type = component.GetType();
    //AddOrUpdate
    if (_registry.TryGetValue(type, out var discriminator))
    {
      _registry[type] = ++discriminator;
      return discriminator;
    }
    _registry[type] = 0;
    return 0;
  }
}
