using ModEndpoints.Core;
using ModResults;

namespace ModEndpoints;
public abstract class ServiceResultEndpointWithEmptyRequest<TResultValue>
  : BaseServiceResultEndpoint<Result<TResultValue>>
{
}

public abstract class ServiceResultEndpointWithEmptyRequest
  : BaseServiceResultEndpoint<Result>
{
}
