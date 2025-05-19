using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using ModEndpoints.RemoteServices.Core;

namespace ModEndpoints.Core;
public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddModEndpointsCoreFromAssemblyContaining<T>(
    this IServiceCollection services,
    Action<ModEndpointsCoreOptions>? configure = null)
  {
    return services.AddModEndpointsCoreFromAssembly(typeof(T).Assembly, configure);
  }

  public static IServiceCollection AddModEndpointsCoreFromAssembly(
    this IServiceCollection services,
    Assembly assembly,
    Action<ModEndpointsCoreOptions>? configure = null)
  {
    ModEndpointsCoreOptions options = new();
    configure?.Invoke(options);

    if (options.UseDefaultRequestValidation)
    {
      services.AddSingleton<IRequestValidator, FluentValidationRequestValidator>();
    }

    return services
      .AddRouteGroupsCoreFromAssembly(assembly, options.RouteGroupConfiguratorLifetime)
      .AddEndpointsCoreFromAssembly(assembly, options.EndpointLifetime);
  }

  private static IServiceCollection AddRouteGroupsCoreFromAssembly(
    this IServiceCollection services,
    Assembly assembly,
    ServiceLifetime lifetime)
  {
    //Don't add RootRouteGroup, it's just a marker class to define root
    //Normally its assembly won't be loaded with this method anyway but just in case
    var serviceDescriptors = assembly
        .DefinedTypes
        .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                       type.IsAssignableTo(typeof(IRouteGroupConfigurator)) &&
                       type != typeof(RootRouteGroup) &&
                       !type.GetCustomAttributes<DoNotRegisterAttribute>().Any())
        .Select(type => ServiceDescriptor.DescribeKeyed(typeof(IRouteGroupConfigurator), type, type, lifetime))
        .ToArray();

    services.TryAddEnumerable(serviceDescriptors);

    return services;
  }

  private static IServiceCollection AddEndpointsCoreFromAssembly(
    this IServiceCollection services,
    Assembly assembly,
    ServiceLifetime lifetime)
  {
    var endpointTypes = assembly
      .DefinedTypes
      .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                     type.IsAssignableTo(typeof(IEndpointConfigurator)) &&
                     !type.GetCustomAttributes<DoNotRegisterAttribute>().Any());

    CheckServiceEndpointRegistrations(endpointTypes);

    var serviceDescriptors = endpointTypes
        .Select(type => ServiceDescriptor.DescribeKeyed(typeof(IEndpointConfigurator), type, type, lifetime))
        .ToArray();

    services.TryAddEnumerable(serviceDescriptors);

    return services;
  }

  private static void CheckServiceEndpointRegistrations(IEnumerable<TypeInfo> endpointTypes)
  {
    var serviceEndpointTypes = endpointTypes.Where(type => IsAssignableFrom(type, typeof(BaseServiceEndpoint<,>))).ToList();
    foreach (var serviceEndpointType in serviceEndpointTypes)
    {
      var requestType = GetGenericArgumentsOfBase(serviceEndpointType, typeof(BaseServiceEndpoint<,>)).Single(type => type.IsAssignableTo(typeof(IServiceRequestMarker)));
      if (ServiceEndpointRegistry.Instance.IsRegistered(requestType))
      {
        throw new InvalidOperationException($"An endpoint for request type {requestType} is already registered.");
      }
      if (!ServiceEndpointRegistry.Instance.Register(requestType, serviceEndpointType))
      {
        throw new InvalidOperationException($"An endpoint of type {serviceEndpointType} couldn't be registered for request type {requestType}.");
      }
    }
    return;
  }

  private static Type[] GetGenericArgumentsOfBase(Type derivedType, Type baseType)
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

  private static bool IsAssignableFrom(Type extendType, Type baseType)
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

  //Configuration processing order:
  //Group Configuration -> Endpoint Configuration -> Global Endpoint Configuration -> Endpoint Override -> Group Endpoint Override -> Group Override
  //Global overrides apply to all endpoints
  //Group overrides apply to endpoints directly under that group (not if they are under another group, which is under this group)
  public static WebApplication MapModEndpointsCore(
    this WebApplication app,
    Action<IServiceProvider, RouteHandlerBuilder, IRouteGroupConfigurator?, IEndpointConfigurator>? globalEndpointConfiguration = null,
    bool throwOnMissingConfiguration = false)
  {
    IEndpointRouteBuilder builder = app;
    using (var scope = builder.ServiceProvider.CreateScope())
    {
      var routeGroups = scope.ServiceProvider.GetKeyedServices<IRouteGroupConfigurator>(KeyedService.AnyKey);
      var endpoints = scope.ServiceProvider.GetKeyedServices<IEndpointConfigurator>(KeyedService.AnyKey);

      //Items that don't have a membership to any route group or
      //items that have a membership to root route group (items at root)
      Func<Type, bool> typeIsNotMemberOfAnyRouteGroupPredicate =
        x => !x.GetCustomAttributes(typeof(MapToGroupAttribute<>)).Any() ||
              x.GetCustomAttributes<MapToGroupAttribute<RootRouteGroup>>().Any();

      _ = MapInternal(
        scope.ServiceProvider,
        builder,
        typeIsNotMemberOfAnyRouteGroupPredicate,
        null, //we are at root, so no current route group
        routeGroups,
        endpoints,
        globalEndpointConfiguration,
        throwOnMissingConfiguration);

      return app;
    }
  }

  private static IEndpointRouteBuilder MapInternal(
    IServiceProvider serviceProvider,
    IEndpointRouteBuilder builder,
    Func<Type, bool> filterPredicate,
    IRouteGroupConfigurator? currentRouteGroup,
    IEnumerable<IRouteGroupConfigurator> routeGroups,
    IEnumerable<IEndpointConfigurator> endpoints,
    Action<IServiceProvider, RouteHandlerBuilder, IRouteGroupConfigurator?, IEndpointConfigurator>? globalEndpointConfiguration,
    bool throwOnMissingConfiguration)
  {
    //Process groups matching filter predicate
    foreach (var childRouteGroup in routeGroups.Where(
      x => filterPredicate(x.GetType())))
    {
      var routeGroupBuilder = childRouteGroup.Configure(serviceProvider, builder, currentRouteGroup);
      if (routeGroupBuilder is null)
      {
        if (throwOnMissingConfiguration)
        {
          throw new InvalidOperationException(string.Format(
            "Missing route group configuration! " +
            "Start configuring {0} route group by calling the MapGroup method.",
            childRouteGroup.GetType()));
        }
        else
        {
          var logger = serviceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger<RouteGroupConfigurator>();
          logger.LogWarning(
            "Missing route group configuration! " +
            "Start configuring {routeGroupType} route group by calling the MapGroup method.",
            childRouteGroup.GetType());
        }
      }

      if (routeGroupBuilder is not null)
      {
        _ = childRouteGroup.Map(
          serviceProvider,
          routeGroupBuilder,
          routeGroups,
          endpoints,
          globalEndpointConfiguration,
          throwOnMissingConfiguration);

        childRouteGroup.ConfigurationOverrides?.Invoke(
          serviceProvider,
          routeGroupBuilder,
          currentRouteGroup,
          childRouteGroup);
      }
    }

    //Process endpoints matching filter predicate
    foreach (var endpoint in endpoints.Where(
      x => filterPredicate(x.GetType())))
    {
      _ = endpoint.Map(
        serviceProvider,
        builder,
        currentRouteGroup,
        globalEndpointConfiguration,
        throwOnMissingConfiguration);
    }
    return builder;
  }

  //Recursive
  private static RouteGroupBuilder Map(
    this IRouteGroupConfigurator routeGroup,
    IServiceProvider serviceProvider,
    RouteGroupBuilder builder,
    IEnumerable<IRouteGroupConfigurator> routeGroups,
    IEnumerable<IEndpointConfigurator> endpoints,
    Action<IServiceProvider, RouteHandlerBuilder, IRouteGroupConfigurator?, IEndpointConfigurator>? globalEndpointConfiguration,
    bool throwOnMissingConfiguration)
  {
    //Items having membership to this route group
    Func<Type, bool> typeIsMemberOfCurrentGroupPredicate =
      x => x.GetCustomAttributes(typeof(MapToGroupAttribute<>)).Any(
        a => (Type?)a.GetType().GetProperty("GroupType")?.GetValue(a) == routeGroup.GetType());

    //Pass this group as current route group
    _ = MapInternal(
      serviceProvider,
      builder,
      typeIsMemberOfCurrentGroupPredicate,
      routeGroup, //process items under this group's hierarchy
      routeGroups,
      endpoints,
      globalEndpointConfiguration,
      throwOnMissingConfiguration);

    return builder;
  }

  private static RouteHandlerBuilder? Map(
    this IEndpointConfigurator endpoint,
    IServiceProvider serviceProvider,
    IEndpointRouteBuilder builder,
    IRouteGroupConfigurator? parentRouteGroup,
    Action<IServiceProvider, RouteHandlerBuilder, IRouteGroupConfigurator?, IEndpointConfigurator>? globalEndpointConfiguration,
    bool throwOnMissingConfiguration)
  {
    var routeHandlerBuilder = endpoint.Configure(serviceProvider, builder, parentRouteGroup);
    if (routeHandlerBuilder is null)
    {
      if (throwOnMissingConfiguration)
      {
        throw new InvalidOperationException(string.Format(
          "Missing endpoint configuration! " +
          "Start configuring {0} endpoint by calling one of the Map[HttpVerb] methods.",
          endpoint.GetType()));
      }
      else
      {
        var logger = serviceProvider.GetRequiredService<ILoggerFactory>()
          .CreateLogger<EndpointConfigurator>();
        logger.LogWarning(
          "Missing endpoint configuration! " +
          "Start configuring {endpointType} endpoint by calling one of the Map[HttpVerb] methods.",
          endpoint.GetType());
      }
    }
    if (routeHandlerBuilder is not null)
    {
      globalEndpointConfiguration?.Invoke(
          serviceProvider,
          routeHandlerBuilder,
          parentRouteGroup,
          endpoint);
      endpoint.ConfigurationOverrides?.Invoke(
        serviceProvider,
        routeHandlerBuilder,
        parentRouteGroup,
        endpoint);
      parentRouteGroup?.EndpointConfigurationOverrides?.Invoke(
        serviceProvider,
        routeHandlerBuilder,
        parentRouteGroup,
        endpoint);
    }
    return routeHandlerBuilder;
  }
}
