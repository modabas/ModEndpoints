using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ModEndpoints.Core;

internal sealed class RequestValidationController : IRequestValidationController
{
  //private readonly ConcurrentDictionary<string, RequestValidationMetadata> _endpointMetadataCache
  //  = new();

  //private RequestValidationMetadata GetRequestValidationMetadata(HttpContext context)
  //{
  //  var endpoint = context.GetEndpoint();
  //  if (endpoint is null)
  //  {
  //    return RequestValidationDefinitions.DefaultMetadata;
  //  }
  //  string? endpointName = null;
  //  //FirstOrDefault to get the first added metadata (in case of multiple)
  //  var list = endpoint.Metadata.GetOrderedMetadata<EndpointConfigurationMetadata>();
  //  if (list is not null && list.Count > 0)
  //  {
  //    endpointName = list[0].EndpointUniqueName;
  //  }
  //  if (string.IsNullOrWhiteSpace(endpointName))
  //  {
  //    endpointName = endpoint.ToString();
  //  }
  //  if (string.IsNullOrWhiteSpace(endpointName))
  //  {
  //    return RequestValidationDefinitions.DefaultMetadata;
  //  }
  //  return _endpointMetadataCache.GetOrAdd(
  //    endpointName,
  //    (_, state) =>
  //    {
  //      var metadata = state.Endpoint.Metadata.GetMetadata<RequestValidationMetadata>();
  //      return metadata ?? state.DefaultValue;
  //    },
  //    new { Endpoint = endpoint, DefaultValue = RequestValidationDefinitions.DefaultMetadata });
  //}

  public async Task<RequestValidationResult> ValidateAsync<TRequest>(
    TRequest req,
    HttpContext context,
    CancellationToken ct)
    where TRequest : notnull
  {
    //var validationMetadata = GetRequestValidationMetadata(context);
    var validationMetadata = 
      context.GetEndpoint()?.Metadata.GetMetadata<RequestValidationMetadata>()
      ?? RequestValidationDefinitions.DefaultMetadata;
    if (validationMetadata.IsEnabled)
    {
      var validationServiceName = validationMetadata.ServiceName;
      var validationService = context.RequestServices.GetRequiredKeyedService<IRequestValidationService>(validationServiceName);
      return await validationService.ValidateAsync(req, context, ct);
    }
    return new RequestValidationResult { IsOk = true };
  }
}
