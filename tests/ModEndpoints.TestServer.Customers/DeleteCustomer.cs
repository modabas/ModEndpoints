﻿using FluentValidation;
using Microsoft.AspNetCore.Http;
using ModEndpoints.Core;
using ModEndpoints.TestServer.Customers.Configuration;

namespace ModEndpoints.TestServer.Customers;
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
    await Task.CompletedTask; // Simulate async work

    return Results.NoContent();
  }
}
