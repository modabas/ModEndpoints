using Microsoft.AspNetCore.Http;
using ModResults;

namespace ModEndpoints;

internal static class WebResultWithLocationUriOnSuccessExtensions
{
  extension(WebResultWithLocationUriOnSuccess)
  {
    public static string? ParseUri(Uri? uri)
    {
      if (uri != null)
      {
        if (uri.IsAbsoluteUri)
        {
          return uri.AbsoluteUri;
        }
        else
        {
          return uri.GetComponents(UriComponents.SerializationInfoString, UriFormat.UriEscaped);
        }
      }
      return null;
    }
  }
}

internal sealed class WebResultWithLocationUriOnSuccess : WebResult
{
  private readonly string? _location;
  public string? Location => _location;

  internal WebResultWithLocationUriOnSuccess(Result result, string? uri) : base(result)
  {
    _location = uri;
  }

  internal WebResultWithLocationUriOnSuccess(Result result, Uri? uri) : base(result)
  {
    _location = WebResultWithLocationUriOnSuccess.ParseUri(uri);
  }

  public override ValueTask<IResult> ExecuteAsync(HttpContext context, CancellationToken ct)
  {
    return ValueTask.FromResult(this.ExecuteInternal(context, Location));
  }
}

internal sealed class WebResultWithLocationUriOnSuccess<TValue> : WebResult<TValue> where TValue : notnull
{
  private readonly string? _location;
  public string? Location => _location;

  internal WebResultWithLocationUriOnSuccess(Result<TValue> result, string? uri) : base(result)
  {
    _location = uri;
  }

  internal WebResultWithLocationUriOnSuccess(Result<TValue> result, Uri? uri) : base(result)
  {
    _location = WebResultWithLocationUriOnSuccess.ParseUri(uri);
  }

  public override ValueTask<IResult> ExecuteAsync(HttpContext context, CancellationToken ct)
  {
    return ValueTask.FromResult(this.ExecuteInternal(context, Location));
  }
}
