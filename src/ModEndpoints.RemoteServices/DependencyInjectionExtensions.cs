using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ModEndpoints.RemoteServices.Core;

namespace ModEndpoints.RemoteServices;
public static class DependencyInjectionExtensions
{
  public static IHttpClientBuilder AddRemoteServiceWithNewClient<TRequest>(
    this IServiceCollection services,
    Action<IServiceProvider, HttpClient> configureClient)
    where TRequest : IServiceRequestMarker
  {
    var clientName = DefaultClientName.Resolve<TRequest>();
    return services.AddRemoteServiceWithNewClient<TRequest>(clientName, configureClient);
  }

  public static IHttpClientBuilder AddRemoteServiceWithNewClient<TRequest>(
    this IServiceCollection services,
    string clientName,
    Action<IServiceProvider, HttpClient> configureClient)
    where TRequest : IServiceRequestMarker
  {
    if (ServiceChannelRegistry.Instance.IsRegistered<TRequest>())
    {
      throw new InvalidOperationException($"A channel for request type {typeof(TRequest)} is already registered.");
    }
    if (ServiceChannelRegistry.Instance.IsRegistered(clientName))
    {
      throw new InvalidOperationException($"A channel with client name {clientName} is already registered.");
    }

    services.AddRemoteServicesCore();

    if (!ServiceChannelRegistry.Instance.Register<TRequest>(clientName))
    {
      throw new InvalidOperationException($"Channel couldn't be registered for request type {typeof(TRequest)} and client name {clientName}.");
    }
    var clientBuilder = services.AddHttpClient(clientName);
    clientBuilder.ConfigureHttpClient(configureClient);
    return clientBuilder;
  }

  public static IHttpClientBuilder AddRemoteServiceWithNewClient<TRequest>(
    this IServiceCollection services,
    string baseAddress,
    TimeSpan timeout = default)
    where TRequest : IServiceRequestMarker
  {
    var clientName = DefaultClientName.Resolve<TRequest>();
    return services.AddRemoteServiceWithNewClient<TRequest>(clientName, baseAddress, timeout);
  }

  public static IHttpClientBuilder AddRemoteServiceWithNewClient<TRequest>(
    this IServiceCollection services,
    string clientName,
    string baseAddress,
    TimeSpan timeout = default)
    where TRequest : IServiceRequestMarker
  {
    Action<IServiceProvider, HttpClient> configureClient = (sp, client) =>
    {
      client.BaseAddress = new Uri(baseAddress);
      if (timeout != default)
      {
        client.Timeout = timeout;
      }
    };
    return services.AddRemoteServiceWithNewClient<TRequest>(clientName, configureClient);
  }

  public static IServiceCollection AddRemoteServiceToExistingClient<TRequest>(
    this IServiceCollection services,
    string clientName)
    where TRequest : IServiceRequestMarker
  {
    if (ServiceChannelRegistry.Instance.IsRegistered<TRequest>())
    {
      throw new InvalidOperationException($"A channel for request type {typeof(TRequest)} is already registered.");
    }
    if (!ServiceChannelRegistry.Instance.IsRegistered(clientName))
    {
      throw new InvalidOperationException($"A channel with client name {clientName} is not registered.");
    }
    if (!ServiceChannelRegistry.Instance.Register<TRequest>(clientName))
    {
      throw new InvalidOperationException($"Channel couldn't be registered for request type {typeof(TRequest)} and client name {clientName}.");
    }
    return services;
  }

  public static IHttpClientBuilder AddRemoteServicesWithNewClient(
    this IServiceCollection services,
    Assembly fromAssembly,
    string clientName,
    Action<IServiceProvider, HttpClient> configureClient,
    Func<Type, bool>? filterPredicate = null)
  {
    if (ServiceChannelRegistry.Instance.IsRegistered(clientName))
    {
      throw new InvalidOperationException($"A channel with client name {clientName} is already registered.");
    }
    var requestTypes = fromAssembly
        .DefinedTypes
        .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                       type.IsAssignableTo(typeof(IServiceRequestMarker)));

    if (filterPredicate is not null)
    {
      requestTypes = requestTypes.Where(type => filterPredicate(type));
    }

    IHttpClientBuilder? clientBuilder = null;
    var requestTypeList = requestTypes.ToList();
    for (var i = 0; i < requestTypeList.Count; i++)
    {
      //first element
      if (i == 0)
      {
        clientBuilder = services.AddRemoteServiceWithNewClientInternal(
          requestTypeList[i],
          clientName,
          configureClient);
      }
      //rest
      else
      {
        services.AddRemoteServiceToExistingClientInternal(
          requestTypeList[i],
          clientName);
      }
    }
    if (clientBuilder is null)
    {
      throw new InvalidOperationException($"Cannot create HttpClient with client name {clientName}.");
    }
    return clientBuilder;
  }

  public static IServiceCollection AddRemoteServicesToExistingClient(
    this IServiceCollection services,
    Assembly fromAssembly,
    string clientName,
    Func<Type, bool>? filterPredicate = null)
  {
    if (!ServiceChannelRegistry.Instance.IsRegistered(clientName))
    {
      throw new InvalidOperationException($"A channel with client name {clientName} is not registered.");
    }
    var requestTypes = fromAssembly
        .DefinedTypes
        .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                       type.IsAssignableTo(typeof(IServiceRequestMarker)));

    if (filterPredicate is not null)
    {
      requestTypes = requestTypes.Where(type => filterPredicate(type));
    }

    foreach (var requestType in requestTypes)
    {
      services.AddRemoteServiceToExistingClientInternal(
        requestType,
        clientName);
    }
    return services;
  }

  private static IHttpClientBuilder AddRemoteServiceWithNewClientInternal(
    this IServiceCollection services,
    Type requestType,
    string clientName,
    Action<IServiceProvider, HttpClient> configureClient)
  {
    if (ServiceChannelRegistry.Instance.IsRegistered(requestType))
    {
      throw new InvalidOperationException($"A channel for request type {requestType} is already registered.");
    }

    services.AddRemoteServicesCore();

    if (!ServiceChannelRegistry.Instance.Register(requestType, clientName))
    {
      throw new InvalidOperationException($"Channel couldn't be registered for request type {requestType} and client name {clientName}.");
    }
    var clientBuilder = services.AddHttpClient(clientName);
    clientBuilder.ConfigureHttpClient(configureClient);
    return clientBuilder;
  }

  private static IServiceCollection AddRemoteServiceToExistingClientInternal(
    this IServiceCollection services,
    Type requestType,
    string clientName)
  {
    if (ServiceChannelRegistry.Instance.IsRegistered(requestType))
    {
      throw new InvalidOperationException($"A channel for request type {requestType} is already registered.");
    }
    if (!ServiceChannelRegistry.Instance.Register(requestType, clientName))
    {
      throw new InvalidOperationException($"Channel couldn't be registered for request type {requestType} and client name {clientName}.");
    }
    return services;
  }

  private static IServiceCollection AddRemoteServicesCore(
    this IServiceCollection services)
  {
    services.TryAddTransient<IServiceEndpointUriResolver, ServiceEndpointUriResolver>();
    services.TryAddTransient<IServiceChannel, ServiceChannel>();
    return services;
  }

}
