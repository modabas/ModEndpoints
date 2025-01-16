namespace ModEndpoints.Core;

/// <summary>
/// Attribute to map targeted endpoint or route group under specified parent route group.
/// </summary>
/// <typeparam name="TGroup"></typeparam>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class MapToGroupAttribute<TGroup> : Attribute
  where TGroup : IRouteGroupConfigurator, new()
{
  public Type GroupType { get; init; }
  public MapToGroupAttribute()
  {
    this.GroupType = typeof(TGroup);
  }
}
