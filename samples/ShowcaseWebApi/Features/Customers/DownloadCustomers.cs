using FluentValidation;
using ModEndpoints.Core;
using ShowcaseWebApi.Data;
using ShowcaseWebApi.Features.Customers.Configuration;

namespace ShowcaseWebApi.Features.Customers;
public record DownloadCustomersRequest(string FileName);

internal class DownloadCustomersRequestValidator : AbstractValidator<DownloadCustomersRequest>
{
  public DownloadCustomersRequestValidator()
  {
    RuleFor(x => x.FileName)
      .NotEmpty()
      .Must(x => Path.GetExtension(x).Equals(".txt", StringComparison.OrdinalIgnoreCase))
      .WithMessage("{PropertyName} must have .txt extension.");
  }
}

[MapToGroup<CustomersV1RouteGroup>()]
internal class DownloadCustomers(ServiceDbContext db)
  : MinimalEndpoint<DownloadCustomersRequest, IResult>
{
  protected override void Configure(
    EndpointRegistrationBuilder builder,
    ConfigurationContext<IEndpointConfiguration> configurationContext)
  {
    builder.MapPost("/download/{FileName}");
  }

  protected override async Task<IResult> HandleAsync(
    DownloadCustomersRequest req,
    CancellationToken ct)
  {
    await Task.CompletedTask; // Simulate some async work
    var customers = db.Customers.AsAsyncEnumerable();
    return Results.Stream(async stream =>
    {
      await foreach (var customer in customers.WithCancellation(ct))
      {
        var line = $"{customer.Id},{customer.FirstName},{customer.MiddleName},{customer.LastName}\n";
        var lineBytes = System.Text.Encoding.UTF8.GetBytes(line);
        await stream.WriteAsync(lineBytes, ct);
      }
    },
    fileDownloadName: Path.GetFileName(req.FileName));
  }
}
