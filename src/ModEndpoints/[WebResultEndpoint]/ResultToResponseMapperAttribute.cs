namespace ModEndpoints;

using ModEndpoints.Core;

/// <summary>
/// Sets the result to response mapper name for that <see cref="IWebResultEndpoint"/> implementation.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class ResultToResponseMapperAttribute : Attribute
{
  public string Name { get; init; }
  public ResultToResponseMapperAttribute(string Name)
  {
    this.Name = Name;
  }
}
