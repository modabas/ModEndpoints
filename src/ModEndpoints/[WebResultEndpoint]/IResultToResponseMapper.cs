using Microsoft.AspNetCore.Http;
using ModResults;

namespace ModEndpoints;
public interface IResultToResponseMapper
{
  ValueTask<IResult> ToResponseAsync(
    Result result,
    HttpContext context,
    CancellationToken ct);
  ValueTask<IResult> ToResponseAsync<TValue>(
    Result<TValue> result,
    HttpContext context,
    CancellationToken ct)
    where TValue : notnull;
}
