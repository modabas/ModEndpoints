namespace BenchmarkWebApi.Features.RegularEndpoints;

internal static class BasicTest
{
  public static RouteHandlerBuilder MapMinimalApiForBasicTest(this IEndpointRouteBuilder builder)
  {
    return builder.MapGet("/MinimalApis/BasicTest",
      IResult () =>
    {
      return Results.Ok("Hello World");
    }).Produces<string>();
  }
}
