namespace ModEndpoints.Core;

public interface IEndpointConfigurationSettings
{
  Dictionary<string, object?>? ConfigurationPropertyBag { get; set; }
}
