using System.Collections.Concurrent;

namespace ModEndpoints.Core;

internal class ComponentDiscriminator : IComponentDiscriminator
{
  private readonly ConcurrentDictionary<Type, int> _registry = new();

  public int GetDiscriminator<T>(T component)
    where T : notnull
  {
    var type = component.GetType();
    return _registry.GetOrAdd(type, 0);
  }

  public int IncrementDiscriminator<T>(T component)
    where T : notnull
  {
    var type = component.GetType();
    return _registry.AddOrUpdate(
      type,
      0,
      (key, oldValue) => oldValue + 1);
  }
}
