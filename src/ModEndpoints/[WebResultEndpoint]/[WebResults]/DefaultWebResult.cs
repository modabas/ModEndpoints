using Microsoft.AspNetCore.Http;
using ModResults;

namespace ModEndpoints;

internal sealed class DefaultWebResult : WebResult
{
  public DefaultWebResult(Result result) : base(result)
  {
  }

  public override ValueTask<IResult> ExecuteAsync(HttpContext context, CancellationToken ct)
  {
    return ValueTask.FromResult(this.ExecuteInternal(context, null));
  }
}

internal sealed class DefaultWebResult<TValue> : WebResult<TValue>
  where TValue : notnull
{
  public DefaultWebResult(Result<TValue> result) : base(result)
  {
  }

  public override ValueTask<IResult> ExecuteAsync(HttpContext context, CancellationToken ct)
  {
    return ValueTask.FromResult(this.ExecuteInternal(context, null));
  }
}
