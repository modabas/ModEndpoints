using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ModEndpoints.Core;
using ShowcaseWebApi.Data;
using ShowcaseWebApi.Features.Customers.Configuration;
using ShowcaseWebApi.Features.Customers.Data;

namespace ShowcaseWebApi.Features.Customers;

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

[MapToGroup<CustomersV1RouteGroup>()]
internal class CreateCustomer(ServiceDbContext db)
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
    var customer = new CustomerEntity()
    {
      FirstName = req.Body.FirstName,
      MiddleName = req.Body.MiddleName,
      LastName = req.Body.LastName
    };

    db.Customers.Add(customer);
    await db.SaveChangesAsync(ct);

    return TypedResults.CreatedAtRoute(
      new CreateCustomerResponse(customer.Id),
      typeof(GetCustomerById).FullName,
      new { id = customer.Id });
  }
}
