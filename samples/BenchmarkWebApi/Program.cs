using BenchmarkWebApi.Extensions;
using BenchmarkWebApi.Features.RegularEndpoints;
using ModEndpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddFeatures();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapModEndpoints();
app.MapMinimalApiForBasicTest();
app.MapMinimalApiForInProcessTest();
app.Run();

