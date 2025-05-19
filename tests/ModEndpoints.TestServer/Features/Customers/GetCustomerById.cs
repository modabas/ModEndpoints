using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using ModEndpoints.Core;
using ModEndpoints.TestServer.Features.Customers.Configuration;

namespace ModEndpoints.TestServer.Features.Customers;

public record GetCustomerByIdRequest(Guid Id);

public record GetCustomerByIdResponse(Guid Id, string FirstName, string? MiddleName, string LastName);

internal class GetCustomerByIdRequestValidator : AbstractValidator<GetCustomerByIdRequest>
{
  public GetCustomerByIdRequestValidator()
  {
    RuleFor(x => x.Id).NotEmpty();
  }
}

[MapToGroup<CustomersRouteGroup>()]
internal class GetCustomerById
  : MinimalEndpoint<GetCustomerByIdRequest, Results<Ok<GetCustomerByIdResponse>, NotFound, ValidationProblem>>
{
  protected override void Configure(
    EndpointRegistrationBuilder builder,
    ConfigurationContext<IEndpointConfiguration> configurationContext)
  {
    builder.MapGet("/{Id}");
  }

  protected override async Task<Results<Ok<GetCustomerByIdResponse>, NotFound, ValidationProblem>> HandleAsync(
    GetCustomerByIdRequest req,
    CancellationToken ct)
  {
    await Task.CompletedTask; // Simulate async work

    return TypedResults.Ok(new GetCustomerByIdResponse(
      Id: req.Id,
      FirstName: "FirstName",
      MiddleName: "MiddleName",
      LastName: "LastName"));
  }
}
