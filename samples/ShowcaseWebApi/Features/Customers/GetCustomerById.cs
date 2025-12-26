using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ModEndpoints.Core;
using ShowcaseWebApi.Data;
using ShowcaseWebApi.Features.Customers.Configuration;

namespace ShowcaseWebApi.Features.Customers;

public record GetCustomerByIdRequest(Guid Id);

public record GetCustomerByIdResponse(Guid Id, string FirstName, string? MiddleName, string LastName);

internal class GetCustomerByIdRequestValidator : AbstractValidator<GetCustomerByIdRequest>
{
  public GetCustomerByIdRequestValidator()
  {
    RuleFor(x => x.Id).NotEmpty();
  }
}

[MapToGroup<CustomersV1RouteGroup>()]
internal class GetCustomerById(ServiceDbContext db)
  : MinimalEndpoint<GetCustomerByIdRequest, Results<Ok<GetCustomerByIdResponse>, NotFound, ValidationProblem, ProblemHttpResult>>
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    EndpointConfigurationContext configurationContext)
  {
    builder.MapGet("/{Id}");
  }

  protected override async Task<Results<Ok<GetCustomerByIdResponse>, NotFound, ValidationProblem, ProblemHttpResult>> HandleAsync(
    GetCustomerByIdRequest req,
    CancellationToken ct)
  {
    var entity = await db.Customers.FirstOrDefaultAsync(b => b.Id == req.Id, ct);

    return entity is null ?
      TypedResults.NotFound() :
      TypedResults.Ok(new GetCustomerByIdResponse(
        Id: entity.Id,
        FirstName: entity.FirstName,
        MiddleName: entity.MiddleName,
        LastName: entity.LastName));
  }
}
