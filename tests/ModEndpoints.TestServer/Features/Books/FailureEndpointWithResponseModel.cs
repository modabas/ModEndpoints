﻿using ModEndpoints.Core;
using ModResults;

namespace ModEndpoints.TestServer.Features.Books;

public record FailureEndpointWithoutResponseModelResponse(Guid Id);

internal class FailureEndpointWithResponseModel
  : WebResultEndpointWithEmptyRequest<FailureEndpointWithoutResponseModelResponse>
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    ConfigurationContext<EndpointConfigurationParameters> configurationContext)
  {
    builder.MapPost("/failure/withResponseModel");
  }

  protected override Task<Result<FailureEndpointWithoutResponseModelResponse>> HandleAsync(CancellationToken ct)
  {
    return Task.FromResult(Result<FailureEndpointWithoutResponseModelResponse>.NotFound("Item not found."));
  }
}
