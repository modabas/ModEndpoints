namespace ModEndpoints.Core;

/// <summary>
/// 
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class MapToGroupAttribute : Attribute
{
  public Type GroupType { get; init; }
  public MapToGroupAttribute(Type GroupType)
  {
    if (!GroupType.IsAssignableTo(typeof(IRouteGroupConfigurator)) ||
      GroupType.IsAbstract ||
      GroupType.IsInterface)
    {
      throw new ArgumentException(
        Constants.ParentRouteGroupInvalidMessage,
        nameof(GroupType));
    }
    this.GroupType = GroupType;
  }
}
