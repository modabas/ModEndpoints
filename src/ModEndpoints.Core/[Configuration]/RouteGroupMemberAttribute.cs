namespace ModEndpoints.Core;

/// <summary>
/// 
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class RouteGroupMemberAttribute : Attribute
{
  public Type ParentGroupType { get; init; }
  public RouteGroupMemberAttribute(Type ParentGroupType)
  {
    if (!ParentGroupType.IsAssignableTo(typeof(IRouteGroupConfigurator)))
    {
      throw new ArgumentException(
        Constants.ParentRouteGroupInvalidMessage,
        nameof(ParentGroupType));
    }
    this.ParentGroupType = ParentGroupType;
  }
}
