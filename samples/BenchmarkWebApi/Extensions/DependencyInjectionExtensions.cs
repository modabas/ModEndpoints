using BenchmarkWebApi.Features.RegularEndpoints;
using BenchmarkWebApi.Services;
using FluentValidation;
using ModEndpoints;

namespace BenchmarkWebApi.Extensions;
internal static class DependencyInjectionExtensions
{
  public static WebApplicationBuilder AddFeatures(this WebApplicationBuilder builder)
  {
    // A service for more real world example
    builder.Services.AddScoped<IGetMeAStringService, GetMeAStringService>();
    builder.Services.AddModEndpointsFromAssembly(typeof(DependencyInjectionExtensions).Assembly);
    builder.Services.AddValidatorsFromAssemblyContaining<InProcessTestRequestValidator>(includeInternalTypes: true);

    return builder;
  }
}
