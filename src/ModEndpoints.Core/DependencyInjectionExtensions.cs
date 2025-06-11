using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using ModEndpoints.RemoteServices.Core;

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

    if (options.AddDefaultRequestValidatorService)
    {
      services.TryAddSingleton<IRequestValidator, FluentValidationRequestValidator>();
    }

    services.TryAddScoped<IComponentDiscriminator, ComponentDiscriminator>();

    return services
      .AddRouteGroupsCoreFromAssembly(assembly, options)
      .AddEndpointsCoreFromAssembly(assembly, options);
  }

  private static IServiceCollection AddRouteGroupsCoreFromAssembly(
    this IServiceCollection services,
    Assembly assembly,
    ModEndpointsCoreOptions options)
  {
    //Don't add RootRouteGroup, it's just a marker class to define root
    //Normally its assembly won't be loaded with this method anyway but just in case
    var routeGroupTypes = assembly
      .DefinedTypes
      .Where(type => type is { IsAbstract: false, IsInterface: false } &&
        type.IsAssignableTo(typeof(IRouteGroupConfigurator)) &&
        type != typeof(RootRouteGroup) &&
        !type.GetCustomAttributes<DoNotRegisterAttribute>().Any());

    var serviceDescriptors = routeGroupTypes
      .Select(type => ServiceDescriptor.DescribeKeyed(
        typeof(IRouteGroupConfigurator),
        type,
        type,
        options.RouteGroupConfiguratorLifetime))
      .ToArray();

    services.TryAddEnumerable(serviceDescriptors);
    ComponentRegistry.Instance.TryAddRouteGroups(routeGroupTypes);

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
      }
      else
      {
        services.TryAdd(descriptor);
      }
      ComponentRegistry.Instance.TryAddEndpoint(requestType, endpointType);
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
      ComponentRegistry.Instance.TryAddEndpoint(endpointType, endpointType);
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
  public static WebApplication MapModEndpointsCore(
    this WebApplication app,
    Action<RouteHandlerBuilder, ConfigurationContext<EndpointConfigurationParameters>>? globalEndpointConfiguration = null,
    bool throwOnMissingConfiguration = false)
  {
    IEndpointRouteBuilder builder = app;
    using (var scope = builder.ServiceProvider.CreateScope())
    {
      var routeGroups = ComponentRegistry.Instance.GetRouteGroups().Select(t => RuntimeHelpers.GetUninitializedObject(t) as IRouteGroupConfigurator).Where(i => i is not null).Select(i => i!); ;
      var endpoints = ComponentRegistry.Instance.GetEndpoints().Select(t => RuntimeHelpers.GetUninitializedObject(t) as IEndpointConfigurator).Where(i => i is not null).Select(i => i!);

      //Items that don't have a membership to any route group or
      //items that have a membership to root route group (items at root)
      Func<Type, bool> typeIsNotMemberOfAnyRouteGroupPredicate =
        x => !x.GetCustomAttributes(typeof(MapToGroupAttribute<>)).Any() ||
              x.GetCustomAttributes<MapToGroupAttribute<RootRouteGroup>>().Any();

      _ = MapComponents(
        scope.ServiceProvider,
        builder,
        typeIsNotMemberOfAnyRouteGroupPredicate,
        null, //we are at root, so no current route group
        null, //we are at root, so no current configuration context
        routeGroups,
        endpoints,
        globalEndpointConfiguration,
        throwOnMissingConfiguration);

      return app;
    }
  }

  private static IEndpointRouteBuilder MapComponents(
    IServiceProvider serviceProvider,
    IEndpointRouteBuilder builder,
    Func<Type, bool> filterPredicate,
    IRouteGroupConfigurator? currentRouteGroup,
    ConfigurationContext<RouteGroupConfigurationParameters>? currentConfigurationContext,
    IEnumerable<IRouteGroupConfigurator> routeGroups,
    IEnumerable<IEndpointConfigurator> endpoints,
    Action<RouteHandlerBuilder, ConfigurationContext<EndpointConfigurationParameters>>? globalEndpointConfiguration,
    bool throwOnMissingConfiguration)
  {
    //Process groups matching filter predicate
    foreach (var childRouteGroup in routeGroups.Where(
      x => filterPredicate(x.GetType())))
    {
      var componentDiscriminator = serviceProvider.GetRequiredService<IComponentDiscriminator>();
      ConfigurationContext<RouteGroupConfigurationParameters> childConfigurationContext = new(
        serviceProvider,
        new(
          childRouteGroup,
          componentDiscriminator.GetDiscriminator(childRouteGroup),
          currentConfigurationContext?.Parameters));
      var routeGroupBuilders = childRouteGroup.Configure(builder, childConfigurationContext);
      if (routeGroupBuilders.Length == 0)
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

      foreach (var routeGroupBuilder in routeGroupBuilders)
      {
        _ = MapRouteGroup(
          childRouteGroup,
          serviceProvider,
          routeGroupBuilder,
          childRouteGroup,
          childConfigurationContext,
          routeGroups,
          endpoints,
          globalEndpointConfiguration,
          throwOnMissingConfiguration);

        childRouteGroup.PostConfigure(
          routeGroupBuilder,
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
    RouteGroupBuilder builder,
    IRouteGroupConfigurator parentRouteGroup,
    ConfigurationContext<RouteGroupConfigurationParameters> parentConfigurationContext,
    IEnumerable<IRouteGroupConfigurator> routeGroups,
    IEnumerable<IEndpointConfigurator> endpoints,
    Action<RouteHandlerBuilder, ConfigurationContext<EndpointConfigurationParameters>>? globalEndpointConfiguration,
    bool throwOnMissingConfiguration)
  {
    //Items having membership to this route group
    Func<Type, bool> typeIsMemberOfCurrentGroupPredicate =
      x => x.GetCustomAttributes(typeof(MapToGroupAttribute<>)).Any(
        a => (Type?)a.GetType().GetProperty("GroupType")?.GetValue(a) == routeGroup.GetType());

    //Pass this group as current route group
    _ = MapComponents(
      serviceProvider,
      builder,
      typeIsMemberOfCurrentGroupPredicate,
      parentRouteGroup, //process items under this group's hierarchy
      parentConfigurationContext, //process items under this group's hierarchy
      routeGroups,
      endpoints,
      globalEndpointConfiguration,
      throwOnMissingConfiguration);

    return builder;
  }

  private static RouteHandlerBuilder[] MapEndpoint(
    IEndpointConfigurator endpoint,
    IServiceProvider serviceProvider,
    IEndpointRouteBuilder builder,
    IRouteGroupConfigurator? parentRouteGroup,
    ConfigurationContext<RouteGroupConfigurationParameters>? parentConfigurationContext,
    Action<RouteHandlerBuilder, ConfigurationContext<EndpointConfigurationParameters>>? globalEndpointConfiguration,
    bool throwOnMissingConfiguration)
  {
    var componentDiscriminator = serviceProvider.GetRequiredService<IComponentDiscriminator>();
    ConfigurationContext<EndpointConfigurationParameters> endpointConfigurationContext = new(
      serviceProvider,
      new(
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
