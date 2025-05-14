using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ModEndpoints.RemoteServices.Core;

namespace ModEndpoints.RemoteServices;
public static class DependencyInjectionExtensions
{
  private const string ClientAlreadyExists = "A client with name {0} already exists.";
  private const string ClientDoesNotExist = "A client with name {0} does not exist.";
  private const string ChannelAlreadyRegistered = "A channel for request type {0} is already registered.";
  private const string ChannelCannotBeRegistered = "Channel couldn't be registered for request type {0} and client name {1}.";
  private const string RequestTypeFlaggedAsDoNotRegister = "Request type {0} is flagged as 'DoNotRegister'.";

  /// <summary>
  /// Adds and configures a new client for specified ServiceEndpoint request.
  /// </summary>
  /// <typeparam name="TRequest">ServiceEndpoint request type to be processed.</typeparam>
  /// <param name="services"></param>
  /// <param name="baseAddress">Base address of Uri of the Internet resource used when sending requests.</param>
  /// <param name="timeout">Timespan to wait before the request times out.</param>
  /// <param name="configureClientBuilder">Delegate to further customize <see cref="IHttpClientBuilder"/> for added <see cref="HttpClient"/>.</param>
  /// <returns></returns>
  public static IServiceCollection AddRemoteServiceWithNewClient<TRequest>(
    this IServiceCollection services,
    string baseAddress,
    TimeSpan timeout = default,
    Action<IHttpClientBuilder>? configureClientBuilder = null)
    where TRequest : IServiceRequestMarker
  {
    var clientName = ServiceClientNameResolver.GetDefaultName<TRequest>();
    return services.AddRemoteServiceWithNewClient<TRequest>(
      clientName,
      baseAddress,
      timeout,
      configureClientBuilder);
  }

  /// <summary>
  /// Adds and configures a new client for specified ServiceEndpoint request.
  /// </summary>
  /// <typeparam name="TRequest">ServiceEndpoint request type to be processed.</typeparam>
  /// <param name="services"></param>
  /// <param name="clientName">The logical name for the client to configure.</param>
  /// <param name="baseAddress">Base address of Uri of the Internet resource used when sending requests.</param>
  /// <param name="timeout">Timespan to wait before the request times out.</param>
  /// <param name="configureClientBuilder">Delegate to further customize <see cref="IHttpClientBuilder"/> for added <see cref="HttpClient"/>.</param>
  /// <returns></returns>
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

  /// <summary>
  /// Adds and configures a new client for specified ServiceEndpoint request.
  /// </summary>
  /// <typeparam name="TRequest">ServiceEndpoint request type to be processed.</typeparam>
  /// <param name="services"></param>
  /// <param name="configureClient">Delegate that will be used to configure <see cref="HttpClient"/>.</param>
  /// <param name="configureClientBuilder">Delegate to further customize <see cref="IHttpClientBuilder"/> for added <see cref="HttpClient"/>.</param>
  /// <returns></returns>
  public static IServiceCollection AddRemoteServiceWithNewClient<TRequest>(
    this IServiceCollection services,
    Action<IServiceProvider, HttpClient> configureClient,
    Action<IHttpClientBuilder>? configureClientBuilder = null)
    where TRequest : IServiceRequestMarker
  {
    var clientName = ServiceClientNameResolver.GetDefaultName<TRequest>();
    return services.AddRemoteServiceWithNewClient<TRequest>(
      clientName,
      configureClient,
      configureClientBuilder);
  }

  /// <summary>
  /// Adds and configures a new client for specified ServiceEndpoint request.
  /// </summary>
  /// <typeparam name="TRequest">ServiceEndpoint request type to be processed.</typeparam>
  /// <param name="services"></param>
  /// <param name="clientName">The logical name for the client to configure.</param>
  /// <param name="configureClient">Delegate that will be used to configure <see cref="HttpClient"/>.</param>
  /// <param name="configureClientBuilder">Delegate to further customize <see cref="IHttpClientBuilder"/> for added <see cref="HttpClient"/>.</param>
  /// <returns></returns>
  /// <exception cref="InvalidOperationException"></exception>
  public static IServiceCollection AddRemoteServiceWithNewClient<TRequest>(
    this IServiceCollection services,
    string clientName,
    Action<IServiceProvider, HttpClient> configureClient,
    Action<IHttpClientBuilder>? configureClientBuilder = null)
    where TRequest : IServiceRequestMarker
  {
    if (ServiceChannelRegistry.Instance.DoesClientExist(clientName))
    {
      throw new InvalidOperationException(string.Format(ClientAlreadyExists, clientName));
    }

    return services.AddRemoteServiceWithNewClientInternal(
      typeof(TRequest),
      clientName,
      configureClient,
      configureClientBuilder);
  }

  /// <summary>
  /// Adds specified ServiceEndpoint request to an already added and configured client.
  /// </summary>
  /// <typeparam name="TRequest">ServiceEndpoint request type to be processed.</typeparam>
  /// <param name="services"></param>
  /// <param name="clientName">The logical name for the client to configure.</param>
  /// <returns></returns>
  /// <exception cref="InvalidOperationException"></exception>
  public static IServiceCollection AddRemoteServiceToExistingClient<TRequest>(
    this IServiceCollection services,
    string clientName)
    where TRequest : IServiceRequestMarker
  {
    if (!ServiceChannelRegistry.Instance.DoesClientExist(clientName))
    {
      throw new InvalidOperationException(string.Format(ClientDoesNotExist, clientName));
    }
    return services.AddRemoteServiceToExistingClientInternal(typeof(TRequest), clientName);
  }

  /// <summary>
  /// Adds and configures a new client for ServiceEndpoint requests in specified assembly.
  /// </summary>
  /// <param name="services"></param>
  /// <param name="fromAssembly">Name of the assembly that will be scanned for ServiceEndpoint requests.</param>
  /// <param name="clientName">The logical name for the client to configure.</param>
  /// <param name="configureClient">Delegate that will be used to configure <see cref="HttpClient"/>.</param>
  /// <param name="configureClientBuilder">Delegate to further customize <see cref="IHttpClientBuilder"/> for added <see cref="HttpClient"/>.</param>
  /// <param name="requestFilterPredicate">A predicate to filter ServiceEndpoint requests found, before being processed.</param>
  /// <returns></returns>
  /// <exception cref="InvalidOperationException"></exception>
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
      throw new InvalidOperationException(string.Format(ClientAlreadyExists, clientName));
    }
    var requestTypes = fromAssembly
        .DefinedTypes
        .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                       type.IsAssignableTo(typeof(IServiceRequestMarker)) &&
                       !type.GetCustomAttributes<DoNotRegisterAttribute>().Any());

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

  /// <summary>
  /// Adds ServiceEndpoint requests in specified assembly to an already added and configured client.
  /// </summary>
  /// <param name="services"></param>
  /// <param name="fromAssembly">Name of the assembly that will be scanned for ServiceEndpoint requests.</param>
  /// <param name="clientName">The logical name for the client to configure.</param>
  /// <param name="requestFilterPredicate">A predicate to filter ServiceEndpoint requests found, before being processed.</param>
  /// <returns></returns>
  /// <exception cref="InvalidOperationException"></exception>
  public static IServiceCollection AddRemoteServicesToExistingClient(
    this IServiceCollection services,
    Assembly fromAssembly,
    string clientName,
    Func<Type, bool>? requestFilterPredicate = null)
  {
    if (!ServiceChannelRegistry.Instance.DoesClientExist(clientName))
    {
      throw new InvalidOperationException(string.Format(ClientDoesNotExist, clientName));
    }
    var requestTypes = fromAssembly
        .DefinedTypes
        .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                       type.IsAssignableTo(typeof(IServiceRequestMarker)) &&
                       !type.GetCustomAttributes<DoNotRegisterAttribute>().Any());

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
    if (requestType.GetCustomAttributes<DoNotRegisterAttribute>().Any())
    {
      throw new InvalidOperationException(string.Format(RequestTypeFlaggedAsDoNotRegister, requestType));
    }
    if (ServiceChannelRegistry.Instance.IsRequestRegistered(requestType))
    {
      throw new InvalidOperationException(string.Format(ChannelAlreadyRegistered, requestType));
    }
    services.AddRemoteServicesCore();
    services.AddClientInternal(
      clientName,
      configureClient,
      configureClientBuilder);
    if (!ServiceChannelRegistry.Instance.RegisterRequest(requestType, clientName))
    {
      throw new InvalidOperationException(string.Format(ChannelCannotBeRegistered, requestType, clientName));
    }
    return services;
  }

  private static IServiceCollection AddRemoteServiceToExistingClientInternal(
    this IServiceCollection services,
    Type requestType,
    string clientName)
  {
    if (requestType.GetCustomAttributes<DoNotRegisterAttribute>().Any())
    {
      throw new InvalidOperationException(string.Format(RequestTypeFlaggedAsDoNotRegister, requestType));
    }
    if (ServiceChannelRegistry.Instance.IsRequestRegistered(requestType))
    {
      throw new InvalidOperationException(string.Format(ChannelAlreadyRegistered, requestType));
    }
    if (!ServiceChannelRegistry.Instance.RegisterRequest(requestType, clientName))
    {
      throw new InvalidOperationException(string.Format(ChannelCannotBeRegistered, requestType, clientName));
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
    services.TryAddKeyedSingleton<IServiceEndpointUriResolver, DefaultServiceEndpointUriResolver>(
      ServiceEndpointDefinitions.DefaultUriResolverName);
    services.AddKeyedTransient<IServiceChannelSerializer, DefaultServiceChannelSerializer>(
      ServiceEndpointDefinitions.DefaultSerializerName,
      (_, _) =>
      {
        return new DefaultServiceChannelSerializer(new ServiceChannelSerializerOptions()
        {
          SerializationOptions = null,
          DeserializationOptions = ServiceEndpointDefinitions.DefaultJsonDeserializationOptions,
          StreamingDeserializationOptions = ServiceEndpointDefinitions.DefaultJsonDeserializationOptionsForStreamingResponse
        });
      });
    services.TryAddTransient<IServiceChannel, DefaultServiceChannel>();
    return services;
  }
}
