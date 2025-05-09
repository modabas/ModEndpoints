﻿using ModEndpoints;
using ModEndpoints.Core;
using ModResults;

namespace BenchmarkWebApi.Features.WebResultEndpoints;

internal class BasicTest
  : WebResultEndpointWithEmptyRequest<string>
{
  protected override void Configure(
    IServiceProvider serviceProvider,
    IRouteGroupConfigurator? parentRouteGroup)
  {
    MapGet("/WebResultEndpoints/BasicTest")
      .Produces<string>();
  }

  protected override Task<Result<string>> HandleAsync(
    CancellationToken ct)
  {
    return Task.FromResult(Result.Ok("Hello World"));
  }
}

