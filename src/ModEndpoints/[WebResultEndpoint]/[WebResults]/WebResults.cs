using Microsoft.AspNetCore.Routing;
using ModResults;

namespace ModEndpoints;

public static class WebResults
{
  #region "FromResult"
  /// <summary>
  /// Creates a new instance of the WebResult class from the specified Result object.
  /// </summary>
  /// <param name="result">The Result object to wrap in a WebResult. Cannot be null.</param>
  /// <returns>A WebResult instance that encapsulates the specified Result object.</returns>
  public static WebResult FromResult(Result result)
  {
    return new WebResult(result);
  }

  /// <summary>
  /// Creates a new <see cref="WebResult{TValue}"/> instance from the specified result.
  /// </summary>
  /// <typeparam name="TValue">The type of the value contained in the result. Must not be null.</typeparam>
  /// <param name="result">The result to wrap in a <see cref="WebResult{TValue}"/>. Cannot be null.</param>
  /// <returns>A <see cref="WebResult{TValue}"/> that encapsulates the specified result.</returns>
  public static WebResult<TValue> FromResult<TValue>(Result<TValue> result)
    where TValue : notnull
  {
    return new WebResult<TValue>(result);
  }

  /// <summary>
  /// Creates a new <see cref="WebResult{TValue}"/> that represents a successful result containing the specified value.
  /// </summary>
  /// <typeparam name="TValue">The type of the value to be wrapped in the result. Must be non-null.</typeparam>
  /// <param name="value">The value to include in the successful result. Cannot be null.</param>
  /// <returns>A <see cref="WebResult{TValue}"/> instance containing the specified value as a successful result.</returns>
  public static WebResult<TValue> FromValue<TValue>(TValue value)
    where TValue : notnull
  {
    return new WebResult<TValue>(value);
  }
  #endregion

  #region "AtUriOnOk"

  /// <summary>
  /// Creates a new <see cref="WebResultAtUriOnOk"/> instance that represents the specified result and associates it
  /// with a URI to be used when the operation is successful.
  /// </summary>
  /// <param name="result">The result to be encapsulated in the response. Represents the outcome of the operation.</param>
  /// <param name="uri">The URI to associate with the response if the operation is successful. Can be null if no URI should be set.</param>
  /// <returns>A <see cref="WebResultAtUriOnOk"/> containing the specified result and associated URI.</returns>
  public static WebResultAtUriOnOk AtUriOnOk(Result result, string? uri)
  {
    return new WebResultAtUriOnOk(result, uri);
  }

  /// <summary>
  /// Creates a new <see cref="WebResultAtUriOnOk"/> instance that represents the specified result and associates it
  /// with a URI to be used when the operation is successful.
  /// </summary>
  /// <param name="result">The result to be encapsulated in the response. Represents the outcome of the operation.</param>
  /// <param name="uri">The URI to associate with the response if the operation is successful. Can be null if no URI should be set.</param>
  /// <returns>A <see cref="WebResultAtUriOnOk"/> containing the specified result and associated URI.</returns>
  public static WebResultAtUriOnOk AtUriOnOk(Result result, Uri? uri)
  {
    return new WebResultAtUriOnOk(result, uri);
  }

  /// <summary>
  /// Creates a new <see cref="WebResultAtUriOnOk{TValue}"/> instance that represents the specified result and associates it
  /// with a URI to be used when the operation is successful.
  /// </summary>
  /// <typeparam name="TValue"></typeparam>
  /// <param name="result">The result to be encapsulated in the response. Represents the outcome of the operation.</param>
  /// <param name="uri">The URI to associate with the response if the operation is successful. Can be null if no URI should be set.</param>
  /// <returns>A <see cref="WebResultAtUriOnOk{TValue}"/> containing the specified result and associated URI.</returns>
  public static WebResultAtUriOnOk<TValue> AtUriOnOk<TValue>(Result<TValue> result, string? uri)
    where TValue : notnull
  {
    return new WebResultAtUriOnOk<TValue>(result, uri);
  }

  /// <summary>
  /// Creates a new <see cref="WebResultAtUriOnOk{TValue}"/> instance that represents the specified result and associates it
  /// with a URI to be used when the operation is successful.
  /// </summary>
  /// <typeparam name="TValue"></typeparam>
  /// <param name="result">The result to be encapsulated in the response. Represents the outcome of the operation.</param>
  /// <param name="uri">The URI to associate with the response if the operation is successful. Can be null if no URI should be set.</param>
  /// <returns>A <see cref="WebResultAtUriOnOk{TValue}"/> containing the specified result and associated URI.</returns>
  public static WebResultAtUriOnOk<TValue> AtUriOnOk<TValue>(Result<TValue> result, Uri? uri)
    where TValue : notnull
  {
    return new WebResultAtUriOnOk<TValue>(result, uri);
  }

  /// <summary>
  /// Creates a new <see cref="WebResultAtUriOnOk{TValue}"/> that represents a successful result containing the specified value and associates it
  /// with a URI to be used when the operation is successful.
  /// </summary>
  /// <typeparam name="TValue">The type of the value to include in the response. Must not be null.</typeparam>
  /// <param name="value">The value to include in the successful result. Cannot be null.</param>
  /// <param name="uri">The URI to set in the Location header of the response. Can be null if no Location header is required.</param>
  /// <returns>A successful <see cref="WebResultAtUriOnOk{TValue}"/> containing the specified value and Location URI.</returns>
  public static WebResultAtUriOnOk<TValue> AtUriOnOk<TValue>(TValue value, string? uri)
    where TValue : notnull
  {
    return new WebResultAtUriOnOk<TValue>(value, uri);
  }

  /// <summary>
  /// Creates a new <see cref="WebResultAtUriOnOk{TValue}"/> that represents a successful result containing the specified value and associates it
  /// with a URI to be used when the operation is successful.
  /// </summary>
  /// <typeparam name="TValue">The type of the value to include in the response. Must not be null.</typeparam>
  /// <param name="value">The value to include in the successful result. Cannot be null.</param>
  /// <param name="uri">The URI to set in the Location header of the response. Can be null if no Location header is required.</param>
  /// <returns>A successful <see cref="WebResultAtUriOnOk{TValue}"/> containing the specified value and Location URI.</returns>
  public static WebResultAtUriOnOk<TValue> AtUriOnOk<TValue>(TValue value, Uri? uri)
    where TValue : notnull
  {
    return new WebResultAtUriOnOk<TValue>(value, uri);
  }
  #endregion

  #region "AtRouteOnOk"

  /// <summary>
  /// Creates a new <see cref="WebResultAtRouteOnOk"/> instance that represents the specified result and associates it
  /// with a URI to be used when the operation is successful.
  /// </summary>
  /// <param name="result">The result containing the value to include in the response. Must not be null.</param>
  /// <param name="routeName">The name of the route to use for generating the Location header.</param>
  /// <param name="routeValues">An object containing the route values to use for URL generation.</param>
  /// <returns>A <see cref="WebResultAtRouteOnOk"/> containing the specified result and associated URI.</returns>
  public static WebResultAtRouteOnOk AtRouteOnOk(Result result, string? routeName, object? routeValues)
  {
    return new WebResultAtRouteOnOk(result, routeName, routeValues);
  }

  /// <summary>
  /// Creates a new <see cref="WebResultAtRouteOnOk"/> instance that represents the specified result and associates it
  /// with a URI to be used when the operation is successful.
  /// </summary>
  /// <param name="result">The result containing the value to include in the response. Must not be null.</param>
  /// <param name="routeName">The name of the route to use for generating the Location header.</param>
  /// <param name="routeValues">An object containing the route values to use for URL generation.</param>
  /// <returns>A <see cref="WebResultAtRouteOnOk"/> containing the specified result and associated URI.</returns>
  public static WebResultAtRouteOnOk AtRouteOnOk(Result result, string? routeName, RouteValueDictionary? routeValues)
  {
    return new WebResultAtRouteOnOk(result, routeName, routeValues);
  }

  /// <summary>
  /// Creates a new <see cref="WebResultAtRouteOnOk{TValue}"/> instance that represents the specified result and associates it
  /// with a URI to be used when the operation is successful.
  /// </summary>
  /// <typeparam name="TValue">The type of the value contained in the result. Must be non-null.</typeparam>
  /// <param name="result">The result containing the value to include in the response. Must not be null.</param>
  /// <param name="routeName">The name of the route to use for generating the Location header.</param>
  /// <param name="routeValues">An object containing the route values to use for URL generation.</param>
  /// <returns>A <see cref="WebResultAtRouteOnOk{TValue}"/> containing the specified result and associated URI.</returns>
  public static WebResultAtRouteOnOk<TValue> AtRouteOnOk<TValue>(Result<TValue> result, string? routeName, object? routeValues)
    where TValue : notnull
  {
    return new WebResultAtRouteOnOk<TValue>(result, routeName, routeValues);
  }

  /// <summary>
  /// Creates a new <see cref="WebResultAtRouteOnOk{TValue}"/> instance that represents the specified result and associates it
  /// with a URI to be used when the operation is successful.
  /// </summary>
  /// <typeparam name="TValue">The type of the value contained in the result. Must be non-null.</typeparam>
  /// <param name="result">The result containing the value to include in the response. Must not be null.</param>
  /// <param name="routeName">The name of the route to use for generating the Location header.</param>
  /// <param name="routeValues">An object containing the route values to use for URL generation.</param>
  /// <returns>A <see cref="WebResultAtRouteOnOk{TValue}"/> containing the specified result and associated URI.</returns>
  public static WebResultAtRouteOnOk<TValue> AtRouteOnOk<TValue>(Result<TValue> result, string? routeName, RouteValueDictionary? routeValues)
    where TValue : notnull
  {
    return new WebResultAtRouteOnOk<TValue>(result, routeName, routeValues);
  }

  /// <summary>
  /// Creates a new <see cref="WebResultAtRouteOnOk{TValue}"/> instance that represents the specified result and associates it
  /// with a URI to be used when the operation is successful.
  /// </summary>
  /// <typeparam name="TValue">The type of the value contained in the result. Must be non-null.</typeparam>
  /// <param name="value">The value to include in the successful result. Cannot be null.</param>
  /// <param name="routeName">The name of the route to use for generating the Location header.</param>
  /// <param name="routeValues">An object containing the route values to use for URL generation.</param>
  /// <returns>A successful <see cref="WebResultAtRouteOnOk{TValue}"/> containing the specified value and associated URI.</returns>
  public static WebResultAtRouteOnOk<TValue> AtRouteOnOk<TValue>(TValue value, string? routeName, object? routeValues)
    where TValue : notnull
  {
    return new WebResultAtRouteOnOk<TValue>(value, routeName, routeValues);
  }

  /// <summary>
  /// Creates a new <see cref="WebResultAtRouteOnOk{TValue}"/> instance that represents the specified result and associates it
  /// with a URI to be used when the operation is successful.
  /// </summary>
  /// <typeparam name="TValue">The type of the value contained in the result. Must be non-null.</typeparam>
  /// <param name="value">The value to include in the successful result. Cannot be null.</param>
  /// <param name="routeName">The name of the route to use for generating the Location header.</param>
  /// <param name="routeValues">An object containing the route values to use for URL generation.</param>
  /// <returns>A successful <see cref="WebResultAtRouteOnOk{TValue}"/> containing the specified value and associated URI.</returns>
  public static WebResultAtRouteOnOk<TValue> AtRouteOnOk<TValue>(TValue value, string? routeName, RouteValueDictionary? routeValues)
    where TValue : notnull
  {
    return new WebResultAtRouteOnOk<TValue>(value, routeName, routeValues);
  }
  #endregion
}
