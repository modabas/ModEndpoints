namespace ModEndpoints;

public static partial class WebResultExtensions
{
  extension<TValue>(WebResult<TValue> webResult)
    where TValue : notnull
  {
    public WebResult ToWebResult()
    {
      return webResult.Result.ToWebResult();
    }
  }
}
