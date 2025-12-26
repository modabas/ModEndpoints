using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using ModEndpoints.RemoteServices.Contracts;

namespace ModEndpoints.Core;

public static class DependencyInjectionExtensions
{
  /// <summary>
  /// Registers route groups and endpoints from the assembly containing the specified type into the dependency injection container. 
  /// </summary>
  /// <remarks>This method scans the assembly containing the specified type for components and registers them with the
  /// dependency injection container.</remarks>
  /// <typeparam name="T"></typeparam>
  /// <param name="services">The <see cref="IServiceCollection"/> to which the services will be added.</param>
  /// <param name="configure">An optional delegate to configure <see cref="ModEndpointsCoreOptions"/> for customizing the behavior of the
  /// registration process.</param>
  /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
  public static IServiceCollection AddModEndpointsCoreFromAssemblyContaining<T>(
    this IServiceCollection services,
    Action<ModEndpointsCoreOptions>? configure = null)
  {
    return services.AddModEndpointsCoreFromAssembly(typeof(T).Assembly, configure);
  }

  /// <summary>
  /// Registers route groups and endpoints from the specified assembly into the dependency injection container.
  /// </summary>
  /// <remarks>This method scans the specified assembly for components and registers them with the
  /// dependency injection container.</remarks>
  /// <param name="services">The <see cref="IServiceCollection"/> to which the services will be added.</param>
  /// <param name="assembly">The assembly containing components to be registered.</param>
  /// <param name="configure">An optional delegate to configure <see cref="ModEndpointsCoreOptions"/> for customizing the behavior of the
  /// registration process.</param>
  /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
  public static IServiceCollection AddModEndpointsCoreFromAssembly(
    this IServiceCollection services,
    Assembly assembly,
    Action<ModEndpointsCoreOptions>? configure = null)
  {
    ModEndpointsCoreOptions options = new();
    configure?.Invoke(options);

    //Request validation
    services.Configure<RequestValidationOptions>(config =>
    {
      config.IsEnabled = options.EnableRequestValidation;
      config.ServiceName = options.RequestValidationServiceName;
    });
    services.TryAddSingleton<IRequestValidationController, RequestValidationController>();
    services.TryAddKeyedSingleton<IRequestValidationService, FluentValidationRequestValidationService>(
      RequestValidationDefinitions.DefaultServiceName);

    //Component registration
    services.TryAddScoped<IComponentDiscriminator, ComponentDiscriminator>();

    ComponentRegistryAccessor.Instance.Initialize();

    return services
      .AddRouteGroupsCoreFromAssembly(assembly, options)
      .AddEndpointsCoreFromAssembly(assembly, options);
  }

  private static IServiceCollection AddRouteGroupsCoreFromAssembly(
    this IServiceCollection services,
    Assembly assembly,
    ModEndpointsCoreOptions options)
  {
    var routeGroupTypes = assembly
      .DefinedTypes
      .Where(type => type is { IsAbstract: false, IsInterface: false } &&
        type.IsAssignableTo(typeof(IRouteGroupConfigurator)) &&
        !type.GetCustomAttributes<DoNotRegisterAttribute>().Any());

    var serviceDescriptors = routeGroupTypes
      .Select(type => ServiceDescriptor.DescribeKeyed(
        typeof(IRouteGroupConfigurator),
        type,
        type,
        options.RouteGroupConfiguratorLifetime))
      .ToArray();

    services.TryAddEnumerable(serviceDescriptors);
    ComponentRegistryAccessor.Instance.Registry?.TryAddRouteGroups(routeGroupTypes);

    return services;
  }

  private static IServiceCollection AddEndpointsCoreFromAssembly(
    this IServiceCollection services,
    Assembly assembly,
    ModEndpointsCoreOptions options)
  {
    var endpointTypes = assembly
      .DefinedTypes
      .Where(type => type is { IsAbstract: false, IsInterface: false } &&
        type.IsAssignableTo(typeof(IEndpointConfigurator)) &&
        !type.GetCustomAttributes<DoNotRegisterAttribute>().Any());

    AddEndpoints(services, endpointTypes, options);

    return services;
  }

  private static void AddEndpoints(
    IServiceCollection services,
    IEnumerable<TypeInfo> endpointTypes,
    ModEndpointsCoreOptions options)
  {
    foreach (var endpointType in endpointTypes)
    {
      if (!TryAddServiceEndpoint(services, endpointType, typeof(BaseServiceEndpoint<,>), options) &&
        !TryAddServiceEndpoint(services, endpointType, typeof(BaseServiceEndpointWithStreamingResponse<,>), options))
      {
        AddEndpoint(services, endpointType, options);
      }
    }
    return;

    static bool TryAddServiceEndpoint(
      IServiceCollection services,
      TypeInfo endpointType,
      Type genericServiceEndpointBaseType,
      ModEndpointsCoreOptions options)
    {
      if (IsAssignableFrom(endpointType, genericServiceEndpointBaseType))
      {
        AddServiceEndpoint(services, endpointType, genericServiceEndpointBaseType, options);
        return true;
      }
      return false;
    }

    static void AddServiceEndpoint(
      IServiceCollection services,
      TypeInfo endpointType,
      Type genericServiceEndpointBaseType,
      ModEndpointsCoreOptions options)
    {
      var requestType = GetGenericArgumentsOfBase(
        endpointType,
        genericServiceEndpointBaseType)
        .Single(type => type.IsAssignableTo(typeof(IServiceRequestMarker)));

      var descriptor = ServiceDescriptor.DescribeKeyed(
        typeof(IEndpointConfigurator),
        requestType,
        endpointType,
        options.EndpointLifetime);

      if (options.ThrowOnDuplicateUseOfServiceEndpointRequest)
      {
        int count = services.Count;
        for (int i = 0; i < count; i++)
        {
          if (Equals(services[i].ServiceKey, descriptor.ServiceKey))
          {
            // Already added
            throw new InvalidOperationException(string.Format(
              Constants.ServiceEndpointAlreadyRegisteredMessage,
              requestType));
          }
        }
        services.Add(descriptor);
        ComponentRegistryAccessor.Instance.Registry?.AddEndpoint(requestType, endpointType);
      }
      else
      {
        services.TryAdd(descriptor);
        ComponentRegistryAccessor.Instance.Registry?.TryAddEndpoint(requestType, endpointType);
      }
    }

    static void AddEndpoint(
      IServiceCollection services,
      TypeInfo endpointType,
      ModEndpointsCoreOptions options)
    {
      services.TryAdd(ServiceDescriptor.DescribeKeyed(
        typeof(IEndpointConfigurator),
        endpointType,
        endpointType,
        options.EndpointLifetime));
      ComponentRegistryAccessor.Instance.Registry?.TryAddEndpoint(endpointType, endpointType);
    }

    static Type[] GetGenericArgumentsOfBase(Type derivedType, Type baseType)
    {
      while (derivedType.BaseType != null)
      {
        derivedType = derivedType.BaseType;
        if (derivedType.IsGenericType && derivedType.GetGenericTypeDefinition() == baseType)
        {
          return derivedType.GetGenericArguments();
        }
      }
      throw new InvalidOperationException("Base type was not found");
    }

    static bool IsAssignableFrom(Type extendType, Type baseType)
    {
      while (!baseType.IsAssignableFrom(extendType))
      {
        if (extendType.Equals(typeof(object)))
        {
          return false;
        }
        if (extendType.IsGenericType && !extendType.IsGenericTypeDefinition)
        {
          extendType = extendType.GetGenericTypeDefinition();
        }
        else
        {
          if (extendType.BaseType is null)
          {
            return false;
          }
          extendType = extendType.BaseType;
        }
      }
      return true;
    }
  }

  /// <summary>
  /// Maps and configures route groups and endpoints. Configuration processing order:
  /// Group's Configuration -> Endpoint's Configuration -> Global Endpoint Configuration -> Endpoint's PostConfigure -> Group's EndpointPostConfigure -> Group's PostConfigure.
  /// Global Endpoint Configuration apply to all endpoints.
  /// Group's EndpointPostConfigure apply to endpoints directly under that group (not if they are under a child group of current group).
  /// </summary>
  /// <param name="app"></param>
  /// <param name="globalEndpointConfiguration">Endpoint configuration to be applied to all endpoints.</param>
  /// <param name="throwOnMissingConfiguration"></param>
  /// <returns></returns>
  public static IEndpointRouteBuilder MapModEndpointsCore(
    this IEndpointRouteBuilder app,
    Action<RouteHandlerBuilder, EndpointConfigurationContext>? globalEndpointConfiguration = null,
    bool throwOnMissingConfiguration = false)
  {
    using (var scope = app.ServiceProvider.CreateScope())
    {
      var routeGroups = ComponentRegistryAccessor.Instance.Registry?.GetRouteGroups()
        .Select(t => RuntimeHelpers.GetUninitializedObject(t) as IRouteGroupConfigurator)
        .Where(i => i is not null)
        .Select(i => i!) ?? [];
      var endpoints = ComponentRegistryAccessor.Instance.Registry?.GetEndpoints()
        .Select(t => RuntimeHelpers.GetUninitializedObject(t) as IEndpointConfigurator)
        .Where(i => i is not null)
        .Select(i => i!) ?? [];

      //Items that don't have a membership to any route group or
      //items that have a membership to root route group (items at root)
      Func<Type, bool> typeIsNotMemberOfAnyRouteGroupPredicate =
        x => !x.GetCustomAttributes(typeof(MapToGroupAttribute<>)).Any() ||
              x.GetCustomAttributes<MapToRootGroupAttribute>().Any();

      _ = MapComponents(
        scope.ServiceProvider,
        app,
        typeIsNotMemberOfAnyRouteGroupPredicate,
        null, //we are at root, so no current route group
        null, //we are at root, so no current configuration context
        routeGroups,
        endpoints,
        globalEndpointConfiguration,
        throwOnMissingConfiguration);

      ComponentRegistryAccessor.Instance.Clear();

      return app;
    }
  }

  private static IEndpointRouteBuilder MapComponents(
    IServiceProvider serviceProvider,
    IEndpointRouteBuilder builder,
    Func<Type, bool> filterPredicate,
    IRouteGroupConfigurator? currentRouteGroup,
    RouteGroupConfigurationContext? currentConfigurationContext,
    IEnumerable<IRouteGroupConfigurator> routeGroups,
    IEnumerable<IEndpointConfigurator> endpoints,
    Action<RouteHandlerBuilder, EndpointConfigurationContext>? globalEndpointConfiguration,
    bool throwOnMissingConfiguration)
  {
    //Process groups matching filter predicate
    foreach (var childRouteGroup in routeGroups.Where(
      x => filterPredicate(x.GetType())))
    {
      var componentDiscriminator = serviceProvider.GetRequiredService<IComponentDiscriminator>();
      DefaultRouteGroupConfigurationContext childConfigurationContext = new(
        serviceProvider,
        new DefaultRouteGroupConfigurationParameters(
          childRouteGroup,
          componentDiscriminator.GetDiscriminator(childRouteGroup),
          currentConfigurationContext?.Parameters));
      var childRouteGroupBuilders = childRouteGroup.Configure(builder, childConfigurationContext);
      if (childRouteGroupBuilders.Length == 0)
      {
        if (throwOnMissingConfiguration)
        {
          throw new InvalidOperationException(string.Format(
            Constants.MissingRouteGroupConfigurationMessage,
            childRouteGroup.GetType()));
        }
        else
        {
          var logger = serviceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger<RouteGroupConfigurator>();
          logger.LogWarning(
            Constants.MissingRouteGroupConfigurationLogMessage,
            childRouteGroup.GetType());
        }
      }

      foreach (var childRouteGroupBuilder in childRouteGroupBuilders)
      {
        _ = MapRouteGroup(
          childRouteGroup,
          serviceProvider,
          childRouteGroupBuilder,
          childConfigurationContext,
          routeGroups,
          endpoints,
          globalEndpointConfiguration,
          throwOnMissingConfiguration);

        childRouteGroup.PostConfigure(
          childRouteGroupBuilder,
          childConfigurationContext);

        childConfigurationContext.Parameters.SelfDiscriminator =
          componentDiscriminator.IncrementDiscriminator(childRouteGroup);
      }
    }

    //Process endpoints matching filter predicate
    foreach (var endpoint in endpoints.Where(
      x => filterPredicate(x.GetType())))
    {
      _ = MapEndpoint(
        endpoint,
        serviceProvider,
        builder,
        currentRouteGroup,
        currentConfigurationContext,
        globalEndpointConfiguration,
        throwOnMissingConfiguration);
    }
    return builder;
  }

  //Recursive
  private static RouteGroupBuilder MapRouteGroup(
    IRouteGroupConfigurator routeGroup,
    IServiceProvider serviceProvider,
    RouteGroupBuilder routeGroupBuilder,
    RouteGroupConfigurationContext routeGroupConfigurationContext,
    IEnumerable<IRouteGroupConfigurator> routeGroups,
    IEnumerable<IEndpointConfigurator> endpoints,
    Action<RouteHandlerBuilder, EndpointConfigurationContext>? globalEndpointConfiguration,
    bool throwOnMissingConfiguration)
  {
    //Items having membership to this route group
    Func<Type, bool> typeIsMemberOfCurrentGroupPredicate =
      x => x.GetCustomAttributes(typeof(MapToGroupAttribute<>)).Any(
        a => (Type?)a.GetType().GetProperty("GroupType")?.GetValue(a) == routeGroup.GetType());

    //Pass this group as current route group
    _ = MapComponents(
      serviceProvider,
      routeGroupBuilder,
      typeIsMemberOfCurrentGroupPredicate,
      routeGroup, //process items under this group's hierarchy
      routeGroupConfigurationContext, //process items under this group's hierarchy
      routeGroups,
      endpoints,
      globalEndpointConfiguration,
      throwOnMissingConfiguration);

    return routeGroupBuilder;
  }

  private static RouteHandlerBuilder[] MapEndpoint(
    IEndpointConfigurator endpoint,
    IServiceProvider serviceProvider,
    IEndpointRouteBuilder builder,
    IRouteGroupConfigurator? parentRouteGroup,
    RouteGroupConfigurationContext? parentConfigurationContext,
    Action<RouteHandlerBuilder, EndpointConfigurationContext>? globalEndpointConfiguration,
    bool throwOnMissingConfiguration)
  {
    var componentDiscriminator = serviceProvider.GetRequiredService<IComponentDiscriminator>();
    DefaultEndpointConfigurationContext endpointConfigurationContext = new(
      serviceProvider,
      new DefaultEndpointConfigurationParameters(
        endpoint,
        componentDiscriminator.GetDiscriminator(endpoint),
        parentConfigurationContext?.Parameters));
    var routeHandlerBuilders = endpoint.Configure(builder, endpointConfigurationContext);
    if (routeHandlerBuilders.Length == 0)
    {
      if (throwOnMissingConfiguration)
      {
        throw new InvalidOperationException(string.Format(
          Constants.MissingEndpointConfigurationMessage,
          endpoint.GetType()));
      }
      else
      {
        var logger = serviceProvider.GetRequiredService<ILoggerFactory>()
          .CreateLogger<EndpointConfigurator>();
        logger.LogWarning(
          Constants.MissingEndpointConfigurationLogMessage,
          endpoint.GetType());
      }
    }
    foreach (var routeHandlerBuilder in routeHandlerBuilders)
    {
      globalEndpointConfiguration?.Invoke(
        routeHandlerBuilder,
        endpointConfigurationContext);
      endpoint.PostConfigure(
        routeHandlerBuilder,
        endpointConfigurationContext);
      parentRouteGroup?.EndpointPostConfigure(
        routeHandlerBuilder,
        endpointConfigurationContext);

      endpointConfigurationContext.Parameters.SelfDiscriminator =
        componentDiscriminator.IncrementDiscriminator(endpoint);
    }
    return routeHandlerBuilders;
  }
}
