﻿using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModEndpoints.Core;
using ModEndpoints.TestServer.Customers.Configuration;

namespace ModEndpoints.TestServer.Customers;
public record PartialUpdateCustomerRequest(Guid Id, [FromBody] PartialUpdateCustomerRequestBody Body);

public record PartialUpdateCustomerRequestBody(string FirstName);

public record PartialUpdateCustomerResponse(Guid Id, string FirstName);

internal class PartialUpdateCustomerRequestValidator : AbstractValidator<PartialUpdateCustomerRequest>
{
  public PartialUpdateCustomerRequestValidator()
  {
    RuleFor(x => x.Id).NotEmpty();
    RuleFor(x => x.Body.FirstName).NotEmpty();
  }
}

[MapToGroup<CustomersRouteGroup>()]
internal class PartialUpdateCustomer
  : MinimalEndpoint<PartialUpdateCustomerRequest, PartialUpdateCustomerResponse>
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    ConfigurationContext<EndpointConfigurationParameters> configurationContext)
  {
    builder.MapPatch("/{Id}")
      .Produces<PartialUpdateCustomerResponse>();
  }

  protected override async Task<PartialUpdateCustomerResponse> HandleAsync(
    PartialUpdateCustomerRequest req,
    CancellationToken ct)
  {
    await Task.CompletedTask; // Simulate async work

    return new PartialUpdateCustomerResponse(
      Id: req.Id,
      FirstName: req.Body.FirstName);
  }
}
