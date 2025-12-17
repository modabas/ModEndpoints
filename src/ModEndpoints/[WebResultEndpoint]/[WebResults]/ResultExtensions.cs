using ModResults;

namespace ModEndpoints;

public static partial class ResultExtensions
{
  extension<TValue>(Result<TValue> result)
    where TValue : notnull
  {
    public WebResult ToWebResult()
    {
      return new DefaultWebResult(result.ToResult());
    }

    public WebResult<TValue> ToWebResultOfTValue()
    {
      return new DefaultWebResult<TValue>(result);
    }
  }

  extension(Result result)
  {
    public WebResult ToWebResult()
    {
      return new DefaultWebResult(result);
    }
  }
}
