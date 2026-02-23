using System.Runtime.CompilerServices;

namespace ModEndpoints.Tests;

internal static class HttpClientHelpers
{
  extension(HttpClient source)
  {
    internal HttpClientHandler GetHandler()
    {
      return Unsafe.As<HttpClientHandler>(get_handler(source))
    ;
    }
  }

  [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_handler")]
  private extern static ref HttpMessageHandler get_handler(HttpMessageInvoker obj);
}
