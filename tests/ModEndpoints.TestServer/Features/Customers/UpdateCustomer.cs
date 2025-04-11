using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ModEndpoints.Core;
using ModEndpoints.TestServer.Features.Customers.Configuration;

namespace ModEndpoints.TestServer.Features.Customers;
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

[MapToGroup<CustomersRouteGroup>()]
internal class UpdateCustomer
  : MinimalEndpoint<UpdateCustomerRequest, IResult>
{
  protected override void Configure(
    IServiceProvider serviceProvider,
    IRouteGroupConfigurator? parentRouteGroup)
  {
    MapPut("/{Id}")
      .Produces<UpdateCustomerResponse>();
  }

  protected override async Task<IResult> HandleAsync(
    UpdateCustomerRequest req,
    CancellationToken ct)
  {
    await Task.CompletedTask; // Simulate async work

    return Results.Ok(new UpdateCustomerResponse(
      Id: req.Id,
      FirstName: req.Body.FirstName,
      MiddleName: req.Body.MiddleName,
      LastName: req.Body.LastName));
  }
}
