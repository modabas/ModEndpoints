using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModEndpoints.Core;
using ShowcaseWebApi.Data;
using ShowcaseWebApi.Features.Customers.Configuration;

namespace ShowcaseWebApi.Features.Customers;

public record UpdateCustomerRequest(Guid Id, [FromBody] UpdateCustomerRequestBody Body);

public record UpdateCustomerRequestBody(string FirstName, string? MiddleName, string LastName);

public record UpdateCustomerResponse(Guid Id, string FirstName, string? MiddleName, string LastName);

internal class UpdateCustomerRequestValidator : AbstractValidator<UpdateCustomerRequest>
{
  public UpdateCustomerRequestValidator()
  {
    RuleFor(x => x.Id).NotEmpty();
    RuleFor(x => x.Body.FirstName).NotEmpty();
    RuleFor(x => x.Body.LastName).NotEmpty();

  }
}

[MapToGroup<CustomersV1RouteGroup>()]
internal class UpdateCustomer(ServiceDbContext db)
  : MinimalEndpoint<UpdateCustomerRequest, IResult>
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    ConfigurationContext<EndpointConfigurationParameters> configurationContext)
  {
    builder.MapPut("/{Id}")
      .Produces<UpdateCustomerResponse>();
  }

  protected override async Task<IResult> HandleAsync(
    UpdateCustomerRequest req,
    CancellationToken ct)
  {
    var entity = await db.Customers.FirstOrDefaultAsync(b => b.Id == req.Id, ct);

    if (entity is null)
    {
      return Results.NotFound();
    }

    entity.FirstName = req.Body.FirstName;
    entity.MiddleName = req.Body.MiddleName;
    entity.LastName = req.Body.LastName;

    var updated = await db.SaveChangesAsync(ct);
    return updated > 0 ?
      Results.Ok(new UpdateCustomerResponse(
      Id: req.Id,
      FirstName: req.Body.FirstName,
      MiddleName: req.Body.MiddleName,
      LastName: req.Body.LastName))
      : Results.NotFound();
  }
}
