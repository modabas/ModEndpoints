namespace ModEndpoints.Core;

public interface IRouteGroupConfigurationSettings
{
  IRouteGroupConfigurationSettings? ParentRouteGroup { get; }

  Dictionary<string, object?> ConfigurationPropertyBag { get; set; }
}
