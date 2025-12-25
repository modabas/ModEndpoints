using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ModEndpoints.Core;

internal sealed class RequestValidationController : IRequestValidationController
{
  private readonly IOptions<RequestValidationOptions> _options;
  private readonly IEndpointNameResolver _endpointNameResolver;
  private static readonly RequestValidationMetadata _defaultMetadata = new();
  private const string ServiceNotRegistered =
    "Request validation service with name '{0}' is not registered.";
  private readonly ConcurrentDictionary<string, RequestValidationMetadata> _metadataCache = new();

  public RequestValidationController(
    IOptions<RequestValidationOptions> options,
    IEndpointNameResolver endpointNameResolver)
  {
    _options = options;
    _endpointNameResolver = endpointNameResolver;
  }

  private RequestValidationMetadata GetEndpointMetadata(HttpContext context)
  {
    var endpoint = context.GetEndpoint();
    if (endpoint is null)
    {
      return _defaultMetadata;
    }
    var endpointName = _endpointNameResolver.GetName(endpoint);
    if (string.IsNullOrWhiteSpace(endpointName))
    {
      return _defaultMetadata;
    }
    return _metadataCache.GetOrAdd(
      endpointName,
      (_, state) =>
      {
        return state.Endpoint.Metadata.GetMetadata<RequestValidationMetadata>()
          ?? state.DefaultValue;
      },
      new { Endpoint = endpoint, DefaultValue = _defaultMetadata });
  }

  public async Task<RequestValidationResult?> ValidateAsync<TRequest>(
    TRequest req,
    HttpContext context,
    CancellationToken ct)
    where TRequest : notnull
  {
    if (_options.Value.IsEnabled)
    {
      var validationMetadata = GetEndpointMetadata(context);
      if (validationMetadata.IsEnabled)
      {
        var validationServiceName = validationMetadata.ServiceName ?? _options.Value.DefaultServiceName;
        var validationService = context.RequestServices.GetKeyedService<IRequestValidationService>(validationServiceName);
        if (validationService is null)
        {
          throw new RequestValidationException(string.Format(
            ServiceNotRegistered, validationServiceName));
        }
        return await validationService.ValidateAsync(req, context, ct);
      }
      return null;
    }
    return null;
  }
}
