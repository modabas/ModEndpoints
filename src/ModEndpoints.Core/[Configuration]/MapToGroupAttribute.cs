namespace ModEndpoints.Core;

/// <summary>
/// 
/// </summary>
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
