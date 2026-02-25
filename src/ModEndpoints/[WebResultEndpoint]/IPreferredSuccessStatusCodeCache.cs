using Microsoft.AspNetCore.Http;

namespace ModEndpoints;

internal interface IPreferredSuccessStatusCodeCache
{
  int? GetStatusCode(HttpContext context);
}
