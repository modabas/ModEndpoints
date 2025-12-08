namespace ModEndpoints.Core;

public static class ConfigurationParameterExtensions
{
  /// <summary>
  /// Gets a value from the property bag, or adds it if it does not exist.
  /// </summary>
  /// <typeparam name="TValue">Type of the value.</typeparam>
  /// <param name="parameters"></param>
  /// <param name="key">Key of the value.</param>
  /// <param name="valueFactory">Factory to create the value if it does not exist.</param>
  /// <returns>The value associated with the key.</returns>
  public static TValue GetOrAddPropertyBagValue<TValue>(this EndpointConfigurationParameters parameters, string key, Func<TValue> valueFactory)
  {
    if (parameters.PropertyBag.TryGetValue(key, out var existingValue) && existingValue is TValue typedValue)
    {
      return typedValue;
    }
    var newValue = valueFactory();
    parameters.PropertyBag[key] = newValue;
    return newValue;
  }

  /// <summary>
  /// Retrieves the deepest parent route group configuration associated with the specified endpoint configuration
  /// parameters.
  /// </summary>
  /// <remarks>This method traverses the parent route group hierarchy starting from the specified parameters and
  /// returns the last ancestor in the chain. If the parameters do not have any parent route groups, the method returns
  /// null.</remarks>
  /// <param name="parameters">The endpoint configuration parameters from which to traverse parent route groups. Cannot be null.</param>
  /// <returns>The deepest parent route group configuration parameters, or null if no parent route groups exist.</returns>
  public static RouteGroupConfigurationParameters? GetRootRouteGroupParameters(this EndpointConfigurationParameters parameters)
  {
    return parameters.ParentRouteGroupParameters?.GetRootRouteGroupParameters();
  }

  /// <summary>
  /// Gets a value from the property bag, or adds it if it does not exist.
  /// </summary>
  /// <typeparam name="TValue">Type of the value.</typeparam>
  /// <param name="parameters"></param>
  /// <param name="key">Key of the value.</param>
  /// <param name="valueFactory">Factory to create the value if it does not exist.</param>
  /// <returns>The value associated with the key.</returns>
  public static TValue GetOrAddPropertyBagValue<TValue>(this RouteGroupConfigurationParameters parameters, string key, Func<TValue> valueFactory)
  {
    if (parameters.PropertyBag.TryGetValue(key, out var existingValue) && existingValue is TValue typedValue)
    {
      return typedValue;
    }
    var newValue = valueFactory();
    parameters.PropertyBag[key] = newValue;
    return newValue;
  }

  /// <summary>
  /// Retrieves the deepest parent route group configuration associated with the specified route group configuration
  /// parameters.
  /// </summary>
  /// <remarks>This method traverses the parent route group hierarchy starting from the specified parameters and
  /// returns the last ancestor in the chain. If the parameters do not have any parent route groups, the method returns
  /// null.</remarks>
  /// <param name="parameters">The route group configuration parameters from which to traverse parent route groups. Cannot be null.</param>
  /// <returns>The deepest parent route group configuration parameters, or null if no parent route groups exist.</returns>
  public static RouteGroupConfigurationParameters? GetRootRouteGroupParameters(this RouteGroupConfigurationParameters parameters)
  {
    var current = parameters.ParentRouteGroupParameters;
    RouteGroupConfigurationParameters? deepest = null;
    while (current is not null)
    {
      deepest = current;
      current = current.ParentRouteGroupParameters;
    }
    return deepest;
  }
}
