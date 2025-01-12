namespace ModEndpoints;

using ModEndpoints.Core;

/// <summary>
/// Sets the uri resolver name for that <see cref="IServiceEndpoint"/> implementation.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class UriResolverAttribute : Attribute
{
  public string Name { get; init; }
  public UriResolverAttribute(string Name)
  {
    this.Name = Name;
  }
}
