using Microsoft.AspNetCore.Http;
using ModEndpoints.Core;
using ModResults;

namespace ModEndpoints;

/// <summary>
/// Abstract base class for endpoints that return a <see cref="Result{TResultValue}"/> business result from HandleAsync method wrapped in an HTTP 200 <see cref="IResult"/>.
/// </summary>
/// <typeparam name="TResultValue">Type of the value contained by business result response.</typeparam>
public abstract class BusinessResultEndpointWithEmptyRequest<TResultValue>
  : BaseBusinessResultEndpoint<Result<TResultValue>>
  where TResultValue : notnull
{
}

/// <summary>
/// Abstract base class for endpoints that return a <see cref="Result"/> business result from HandleAsync method wrapped in an HTTP 200 <see cref="IResult"/>.
/// </summary>
public abstract class BusinessResultEndpointWithEmptyRequest
  : BaseBusinessResultEndpoint<Result>
{
}
