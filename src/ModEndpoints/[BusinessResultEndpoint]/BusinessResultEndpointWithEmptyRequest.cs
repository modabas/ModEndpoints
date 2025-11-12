using ModEndpoints.Core;
using ModResults;

namespace ModEndpoints;

public abstract class BusinessResultEndpointWithEmptyRequest<TResultValue>
  : BaseBusinessResultEndpoint<Result<TResultValue>>
  where TResultValue : notnull
{
}

public abstract class BusinessResultEndpointWithEmptyRequest
  : BaseBusinessResultEndpoint<Result>
{
}
