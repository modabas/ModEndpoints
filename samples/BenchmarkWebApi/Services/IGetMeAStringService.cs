using ModResults;

namespace BenchmarkWebApi.Services;

internal interface IGetMeAStringService
{
  Task<Result<string>> GetMeAResultOfStringAsync(string name, CancellationToken ct);
  Task<string> GetMeAStringAsync(CancellationToken ct);
}
