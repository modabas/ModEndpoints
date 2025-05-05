using ModEndpoints.Core;
using Scalar.AspNetCore;
using WeatherForecastWebApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddModEndpointsCoreFromAssemblyContaining<GetWeatherForecast>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
  app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.MapModEndpointsCore();

app.Run();

