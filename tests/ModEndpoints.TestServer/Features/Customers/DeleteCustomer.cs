using FluentValidation;
using ModEndpoints.Core;
using ModEndpoints.TestServer.Features.Customers.Configuration;

namespace ModEndpoints.TestServer.Features.Customers;
public record DeleteCustomerRequest(Guid Id);

internal class DeleteCustomerRequestValidator : AbstractValidator<DeleteCustomerRequest>
{
  public DeleteCustomerRequestValidator()
  {
    RuleFor(x => x.Id).NotEmpty();
  }
}

[MapToGroup<CustomersRouteGroup>()]
internal class DeleteCustomer
  : MinimalEndpoint<DeleteCustomerRequest, IResult>
{
  protected override void Configure(
    ConfigurationContext<IEndpointConfiguration> configurationContext)
  {
    MapDelete("/{Id}")
      .Produces(StatusCodes.Status204NoContent);
  }

  protected override async Task<IResult> HandleAsync(
    DeleteCustomerRequest req,
    CancellationToken ct)
  {
    await Task.CompletedTask; // Simulate async work

    return Results.NoContent();
  }
}
