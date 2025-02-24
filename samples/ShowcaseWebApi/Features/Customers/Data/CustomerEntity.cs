using ShowcaseWebApi.Data;

namespace ShowcaseWebApi.Features.Customers.Data;

internal class CustomerEntity : BaseEntity
{
  public string FirstName { get; set; } = string.Empty;
  public string? MiddleName { get; set; }
  public string LastName { get; set; } = string.Empty;
}
