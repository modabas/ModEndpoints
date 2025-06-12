using FluentValidation;
using ModEndpoints;
using ModEndpoints.TestServer.Customers;
using ModEndpoints.TestServer.Features.Books;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddModEndpointsFromAssemblyContaining<GetCustomerById>();
builder.Services.AddValidatorsFromAssemblyContaining<GetCustomerByIdRequestValidator>(includeInternalTypes: true);
builder.Services.AddModEndpointsFromAssemblyContaining<GetBookById>();
builder.Services.AddValidatorsFromAssemblyContaining<GetBookByIdRequestValidator>(includeInternalTypes: true);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapModEndpoints(
  (builder, configurationContext) =>
  {
    var endpoint = configurationContext.Parameters.CurrentEndpoint;
    builder.WithSummary(endpoint.GetType().Name);
    var endpointFullName = endpoint.GetType().FullName;
    if (!string.IsNullOrWhiteSpace(endpointFullName))
    {
      var discriminator = configurationContext.Parameters.SelfDiscriminator;
      if (discriminator == 0)
      {
        builder.WithName(endpointFullName);
      }
      else
      {
        builder.WithName($"{endpointFullName}_{discriminator}");
      }
    }
  });

app.Run();

public partial class Program { }
