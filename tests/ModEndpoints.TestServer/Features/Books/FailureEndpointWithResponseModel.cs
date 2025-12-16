using ModEndpoints.Core;
using ModResults;

namespace ModEndpoints.TestServer.Features.Books;

public record FailureEndpointWithoutResponseModelResponse(Guid Id);

internal class FailureEndpointWithResponseModel
  : WebResultEndpointWithEmptyRequest<FailureEndpointWithoutResponseModelResponse>
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    EndpointConfigurationContext configurationContext)
  {
    builder.MapPost("/failure/withResponseModel");
  }

  protected override async Task<WebResult<FailureEndpointWithoutResponseModelResponse>> HandleAsync(CancellationToken ct)
  {
    return Result<FailureEndpointWithoutResponseModelResponse>.NotFound("Item not found.");
  }
}
