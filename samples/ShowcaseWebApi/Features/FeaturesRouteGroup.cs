using Asp.Versioning;
using ModEndpoints.Core;

namespace ShowcaseWebApi.Features;

internal class FeaturesRouteGroup : RouteGroupConfigurator
{
  protected override void Configure(
    RouteGroupRegistrationBuilder builder,
    ConfigurationContext<IRouteGroupConfiguration> configurationContext)
  {
    var groupBuilder = builder.MapGroup("/api/v{version:apiVersion}");
    var apiVersionSet = groupBuilder.NewApiVersionSet()
      .HasApiVersion(new ApiVersion(1))
      .HasApiVersion(new ApiVersion(2))
      .ReportApiVersions()
      .Build();
    groupBuilder.WithApiVersionSet(apiVersionSet);
  }
}
