using Microsoft.AspNetCore.Http;
using ModResults;

namespace ModEndpoints;

public sealed class DefaultWebResult : WebResult
{
  internal DefaultWebResult(Result result) : base(result)
  {
  }

  public override ValueTask<IResult> ExecuteAsync(HttpContext context, CancellationToken ct)
  {
    return this.ExecuteInternalAsync(context, null);
  }
}

public sealed class DefaultWebResult<TValue> : WebResult<TValue>
  where TValue : notnull
{
  internal DefaultWebResult(Result<TValue> result) : base(result)
  {
  }

  public override ValueTask<IResult> ExecuteAsync(HttpContext context, CancellationToken ct)
  {
    return this.ExecuteInternalAsync(context, null);
  }
}
