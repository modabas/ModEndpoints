using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ModEndpoints.RemoteServices.Core;

namespace ModEndpoints.RemoteServices;
public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddRemoteServiceWithNewClient<TRequest>(
    this IServiceCollection services,
    string baseAddress,
    TimeSpan timeout = default,
    Action<IHttpClientBuilder>? configureClientBuilder = null)
    where TRequest : IServiceRequestMarker
  {
    var clientName = DefaultClientName.Resolve<TRequest>();
    return services.AddRemoteServiceWithNewClient<TRequest>(
      clientName,
      baseAddress,
      timeout,
      configureClientBuilder);
  }

  public static IServiceCollection AddRemoteServiceWithNewClient<TRequest>(
    this IServiceCollection services,
    string clientName,
    string baseAddress,
    TimeSpan timeout = default,
    Action<IHttpClientBuilder>? configureClientBuilder = null)
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
    return services.AddRemoteServiceWithNewClient<TRequest>(
      clientName,
      configureClient,
      configureClientBuilder);
  }

  public static IServiceCollection AddRemoteServiceWithNewClient<TRequest>(
    this IServiceCollection services,
    Action<IServiceProvider, HttpClient> configureClient,
    Action<IHttpClientBuilder>? configureClientBuilder = null)
    where TRequest : IServiceRequestMarker
  {
    var clientName = DefaultClientName.Resolve<TRequest>();
    return services.AddRemoteServiceWithNewClient<TRequest>(
      clientName,
      configureClient,
      configureClientBuilder);
  }

  public static IServiceCollection AddRemoteServiceWithNewClient<TRequest>(
    this IServiceCollection services,
    string clientName,
    Action<IServiceProvider, HttpClient> configureClient,
    Action<IHttpClientBuilder>? configureClientBuilder = null)
    where TRequest : IServiceRequestMarker
  {
    if (ServiceChannelRegistry.Instance.DoesClientExist(clientName))
    {
      throw new InvalidOperationException($"A client with name {clientName} already exists.");
    }

    return services.AddRemoteServiceWithNewClientInternal(
      typeof(TRequest),
      clientName,
      configureClient,
      configureClientBuilder);
  }

  public static IServiceCollection AddRemoteServiceToExistingClient<TRequest>(
    this IServiceCollection services,
    string clientName)
    where TRequest : IServiceRequestMarker
  {
    if (!ServiceChannelRegistry.Instance.DoesClientExist(clientName))
    {
      throw new InvalidOperationException($"A client with name {clientName} does not exist.");
    }
    return services.AddRemoteServiceToExistingClientInternal(typeof(TRequest), clientName);
  }

  public static IServiceCollection AddRemoteServicesWithNewClient(
    this IServiceCollection services,
    Assembly fromAssembly,
    string clientName,
    Action<IServiceProvider, HttpClient> configureClient,
    Action<IHttpClientBuilder>? configureClientBuilder = null,
    Func<Type, bool>? requestFilterPredicate = null)
  {
    if (ServiceChannelRegistry.Instance.DoesClientExist(clientName))
    {
      throw new InvalidOperationException($"A client with name {clientName} already exists.");
    }
    var requestTypes = fromAssembly
        .DefinedTypes
        .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                       type.IsAssignableTo(typeof(IServiceRequestMarker)));

    if (requestFilterPredicate is not null)
    {
      requestTypes = requestTypes.Where(type => requestFilterPredicate(type));
    }

    services.AddRemoteServicesCore();
    services.AddClientInternal(
      clientName,
      configureClient,
      configureClientBuilder);
    foreach (var requestType in requestTypes)
    {
      services.AddRemoteServiceToExistingClientInternal(
        requestType,
        clientName);
    }
    return services;
  }

  public static IServiceCollection AddRemoteServicesToExistingClient(
    this IServiceCollection services,
    Assembly fromAssembly,
    string clientName,
    Func<Type, bool>? requestFilterPredicate = null)
  {
    if (!ServiceChannelRegistry.Instance.DoesClientExist(clientName))
    {
      throw new InvalidOperationException($"A client with name {clientName} does not exist.");
    }
    var requestTypes = fromAssembly
        .DefinedTypes
        .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                       type.IsAssignableTo(typeof(IServiceRequestMarker)));

    if (requestFilterPredicate is not null)
    {
      requestTypes = requestTypes.Where(type => requestFilterPredicate(type));
    }

    foreach (var requestType in requestTypes)
    {
      services.AddRemoteServiceToExistingClientInternal(
        requestType,
        clientName);
    }
    return services;
  }

  private static IServiceCollection AddRemoteServiceWithNewClientInternal(
    this IServiceCollection services,
    Type requestType,
    string clientName,
    Action<IServiceProvider, HttpClient> configureClient,
    Action<IHttpClientBuilder>? configureClientBuilder)
  {
    if (ServiceChannelRegistry.Instance.IsRequestRegistered(requestType))
    {
      throw new InvalidOperationException($"A channel for request type {requestType} is already registered.");
    }
    services.AddRemoteServicesCore();
    services.AddClientInternal(
      clientName,
      configureClient,
      configureClientBuilder);
    if (!ServiceChannelRegistry.Instance.RegisterRequest(requestType, clientName))
    {
      throw new InvalidOperationException($"Channel couldn't be registered for request type {requestType} and client name {clientName}.");
    }
    return services;
  }

  private static IServiceCollection AddRemoteServiceToExistingClientInternal(
    this IServiceCollection services,
    Type requestType,
    string clientName)
  {
    if (ServiceChannelRegistry.Instance.IsRequestRegistered(requestType))
    {
      throw new InvalidOperationException($"A channel for request type {requestType} is already registered.");
    }
    if (!ServiceChannelRegistry.Instance.RegisterRequest(requestType, clientName))
    {
      throw new InvalidOperationException($"Channel couldn't be registered for request type {requestType} and client name {clientName}.");
    }
    return services;
  }

  private static IServiceCollection AddClientInternal(
    this IServiceCollection services,
    string clientName,
    Action<IServiceProvider, HttpClient> configureClient,
    Action<IHttpClientBuilder>? configureClientBuilder)
  {
    var clientBuilder = services.AddHttpClient(clientName);
    clientBuilder.ConfigureHttpClient(configureClient);
    configureClientBuilder?.Invoke(clientBuilder);
    ServiceChannelRegistry.Instance.AddClient(clientName);
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
