namespace ModEndpoints.Core;

/// <summary>
/// Attribute to map targeted endpoint or route group under root route group.
/// By default all groups and endpoints are members under root.
/// However if a group membership is defined on them via <see cref="MapToGroupAttribute{TGroup}"/>,
/// causing them to not be a member of root anymore,
/// this attribute is here to enable them being member of root directly
/// in addition to their other memberships.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class MapToRootGroupAttribute : Attribute;
