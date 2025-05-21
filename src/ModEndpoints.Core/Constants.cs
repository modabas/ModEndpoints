namespace ModEndpoints.Core;
internal static class Constants
{
  public const string RouteBuilderIsNullForEndpointMessage = 
    "Route builder is null for {0} endpoint.";
  public const string RequiredServiceIsInvalidMessage = 
    "Service resolved from dependency injection is invalid.";
  public const string MissingRouteGroupConfigurationMessage = 
    "Missing route group configuration! Start configuring {0} route group by calling the MapGroup method of the builder.";
  public const string MissingRouteGroupConfigurationLogMessage =
    "Missing route group configuration! Start configuring {routeGroupType} route group by calling the MapGroup method of the builder.";
  public const string MissingEndpointConfigurationMessage =
    "Missing endpoint configuration! Start configuring {0} endpoint by calling one of the Map[HttpVerb] methods of the builder.";
  public const string MissingEndpointConfigurationLogMessage =
    "Missing endpoint configuration! Start configuring {endpointType} endpoint by calling one of the Map[HttpVerb] methods of the builder.";
  public const string ServiceEndpointAlreadyRegisteredMessage =
    "An endpoint for request type {0} is already registered.";
  public const string ServiceEndpointCannotBeRegisteredMessage =
    "An endpoint of type {0} couldn't be registered for request type {1}.";
}
