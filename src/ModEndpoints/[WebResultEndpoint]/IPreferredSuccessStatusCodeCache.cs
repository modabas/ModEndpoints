using Microsoft.AspNetCore.Http;

namespace ModEndpoints;

public interface IPreferredSuccessStatusCodeCache
{
  int? GetStatusCode(HttpContext context);
}
