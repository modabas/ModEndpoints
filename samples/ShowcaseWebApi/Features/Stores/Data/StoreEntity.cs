using ShowcaseWebApi.Data;

namespace ShowcaseWebApi.Features.Stores.Data;
internal class StoreEntity : BaseEntity
{
  public string Name { get; set; } = string.Empty;
}
