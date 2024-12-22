using ModResults;

namespace BenchmarkWebApi.Services;
internal class GetMeAStringService : IGetMeAStringService
{
  /// <summary>
  /// This purely exists to simulate some work.
  /// </summary>
  /// <returns>"Hello, World!"</returns>
  public async Task<string> GetMeAStringAsync(CancellationToken ct)
  {
    using var str = new MemoryStream();
    await using var writer = new StreamWriter(str);
    using var reader = new StreamReader(str);

    await writer.WriteAsync("Hello, ");
    await writer.WriteAsync("World!");
    await writer.FlushAsync(ct);
    str.Seek(0, SeekOrigin.Begin);
    return await reader.ReadToEndAsync(ct);
  }

  public async Task<Result<string>> GetMeAResultOfStringAsync(string name, CancellationToken ct)
  {
    using var str = new MemoryStream();
    await using var writer = new StreamWriter(str);
    using var reader = new StreamReader(str);

    await writer.WriteAsync("Hello, ");
    await writer.WriteAsync(name);
    await writer.FlushAsync(ct);
    str.Seek(0, SeekOrigin.Begin);
    return await reader.ReadToEndAsync(ct);
  }
}
