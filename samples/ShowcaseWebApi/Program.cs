using ModEndpoints;
using ShowcaseWebApi.Data;
using ShowcaseWebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddWebServices();
builder.AddFeatures();
builder.Services.AddDbContext<ServiceDbContext>();
builder.AddSwagger();

var app = builder.Build();

app.SeedData();

app.UseHttpsRedirection();

app.MapModEndpoints(
  (builder, configurationContext) =>
  {
    var endpoint = configurationContext.CurrentComponent;
    builder.WithSummary(endpoint.GetType().Name);
    var endpointFullName = endpoint.GetType().FullName;
    if (!string.IsNullOrWhiteSpace(endpointFullName))
    {
      builder.WithName(endpointFullName);
    }
  });

//do this after endpoints are configured
//so swagger ui will correctly show all versions of apis
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI(options =>
  {
    var descriptions = app.DescribeApiVersions();

    // build a swagger endpoint for each discovered API version
    foreach (var description in descriptions)
    {
      var url = $"/swagger/{description.GroupName}/swagger.json";
      var name = description.GroupName.ToUpperInvariant();
      options.SwaggerEndpoint(url, name);
    }
  });
}

app.Run();
