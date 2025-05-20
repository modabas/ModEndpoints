namespace ModEndpoints.Core;

public class ConfigurationContext<T>
{
  public IServiceProvider ServiceProvider { get; init; }

  public T Parameters { get; init; }

  public ConfigurationContext(
    IServiceProvider serviceProvider,
    T parameters)
  {
    ServiceProvider = serviceProvider;
    Parameters = parameters;
  }
}
