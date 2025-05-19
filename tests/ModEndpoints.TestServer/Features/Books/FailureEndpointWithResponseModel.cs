using ModEndpoints.Core;
using ModResults;

namespace ModEndpoints.TestServer.Features.Books;

public record FailureEndpointWithoutResponseModelResponse(Guid Id);

internal class FailureEndpointWithResponseModel
  : WebResultEndpointWithEmptyRequest<FailureEndpointWithoutResponseModelResponse>
{
  protected override void Configure(
    EndpointRegistrationBuilder builder,
    ConfigurationContext<IEndpointConfiguration> configurationContext)
  {
    builder.MapPost("/failure/withResponseModel");
  }

  protected override Task<Result<FailureEndpointWithoutResponseModelResponse>> HandleAsync(CancellationToken ct)
  {
    return Task.FromResult(Result<FailureEndpointWithoutResponseModelResponse>.NotFound("Item not found."));
  }
}
