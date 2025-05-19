using Asp.Versioning;
using ModEndpoints.Core;

namespace ShowcaseWebApi.Features;

internal class FeaturesRouteGroup : RouteGroupConfigurator
{
  protected override void Configure(
    ConfigurationContext<IRouteGroupConfiguration> configurationContext)
  {
    var builder = MapGroup("/api/v{version:apiVersion}");
    var apiVersionSet = builder.NewApiVersionSet()
      .HasApiVersion(new ApiVersion(1))
      .HasApiVersion(new ApiVersion(2))
      .ReportApiVersions()
      .Build();
    builder.WithApiVersionSet(apiVersionSet);
  }
}
