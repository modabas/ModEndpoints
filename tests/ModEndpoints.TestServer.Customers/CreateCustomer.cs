using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ModEndpoints.Core;
using ModEndpoints.TestServer.Customers.Configuration;

namespace ModEndpoints.TestServer.Customers;
public record CreateCustomerRequest([FromBody] CreateCustomerRequestBody Body);

public record CreateCustomerRequestBody(string FirstName, string? MiddleName, string LastName);

public record CreateCustomerResponse(Guid Id);

internal class CreateCustomerRequestValidator : AbstractValidator<CreateCustomerRequest>
{
  public CreateCustomerRequestValidator()
  {
    RuleFor(x => x.Body.FirstName).NotEmpty();
    RuleFor(x => x.Body.LastName).NotEmpty();
  }
}

[MapToGroup<CustomersRouteGroup>()]
internal class CreateCustomer
  : MinimalEndpoint<CreateCustomerRequest, Results<CreatedAtRoute<CreateCustomerResponse>, ValidationProblem, ProblemHttpResult>>
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    ConfigurationContext<EndpointConfigurationParameters> configurationContext)
  {
    builder.MapPost("/");
  }

  protected override async Task<Results<CreatedAtRoute<CreateCustomerResponse>, ValidationProblem, ProblemHttpResult>> HandleAsync(
    CreateCustomerRequest req,
    CancellationToken ct)
  {
    await Task.CompletedTask; // Simulate async work

    var customerId = Guid.NewGuid();
    return TypedResults.CreatedAtRoute(
      new CreateCustomerResponse(customerId),
      typeof(GetCustomerById).FullName,
      new { id = customerId });
  }
}
