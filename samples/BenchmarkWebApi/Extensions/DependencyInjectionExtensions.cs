using BenchmarkWebApi.Features.MinimalEndpoints;
using BenchmarkWebApi.Services;
using FluentValidation;
using ModEndpoints;

namespace BenchmarkWebApi.Extensions;

internal static class DependencyInjectionExtensions
{
  extension(WebApplicationBuilder builder)
  {
    public WebApplicationBuilder AddFeatures()
    {
      // A service for more real world example
      builder.Services.AddScoped<IGetMeAStringService, GetMeAStringService>();
      builder.Services.AddModEndpointsFromAssemblyContaining<BasicTest>();
      builder.Services.AddValidatorsFromAssemblyContaining<InProcessTestRequestValidator>(includeInternalTypes: true);

      return builder;
    }
  }
}
