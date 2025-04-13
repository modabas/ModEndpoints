﻿namespace ModEndpoints;
public static class WebResultEndpointDefinitions
{
  public const string DefaultResultToResponseMapperName = "DefaultResultToResponseMapper";
  public const string InvalidRouteMessage = "No route matches the supplied values.";
  public const string HttpContextIsInvalidMessage = "Http context resolved from dependency injection is invalid.";

  internal const string PreferredSuccessStatusCodeCacheNameForResult = "PreferredSuccessStatusCodeCacheForResult";
  internal const string PreferredSuccessStatusCodeCacheNameForResultOfT = "PreferredSuccessStatusCodeCacheForResultOfT";
}
