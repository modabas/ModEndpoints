namespace BenchmarkWebApi.Features.MinimalApis;

internal static class BasicTest
{
  extension(IEndpointRouteBuilder builder)
  {
    public RouteHandlerBuilder MapMinimalApiForBasicTest()
    {
      return builder.MapGet("/MinimalApis/BasicTest",
        IResult () =>
      {
        return Results.Ok("Hello World");
      }).Produces<string>();
    }
  }
}
