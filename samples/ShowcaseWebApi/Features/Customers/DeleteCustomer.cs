using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ModEndpoints.Core;
using ShowcaseWebApi.Data;
using ShowcaseWebApi.Features.Customers.Configuration;

namespace ShowcaseWebApi.Features.Customers;

public record DeleteCustomerRequest(Guid Id);

internal class DeleteCustomerRequestValidator : AbstractValidator<DeleteCustomerRequest>
{
  public DeleteCustomerRequestValidator()
  {
    RuleFor(x => x.Id).NotEmpty();
  }
}

[MapToGroup<CustomersV1RouteGroup>()]
internal class DeleteCustomer(ServiceDbContext db)
  : MinimalEndpoint<DeleteCustomerRequest, IResult>
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    ConfigurationContext<EndpointConfigurationParameters> configurationContext)
  {
    builder.MapDelete("/{Id}")
      .Produces(StatusCodes.Status204NoContent);
  }

  protected override async Task<IResult> HandleAsync(
    DeleteCustomerRequest req,
    CancellationToken ct)
  {
    var entity = await db.Customers.FirstOrDefaultAsync(b => b.Id == req.Id, ct);

    if (entity is null)
    {
      return Results.NotFound();
    }

    db.Customers.Remove(entity);
    var deleted = await db.SaveChangesAsync(ct);
    return deleted > 0 ? Results.NoContent() : Results.NotFound();
  }
}
