# Handling Files

## File Uploads
`IFormFile` or `IFormFileCollection` in ASP.NET Core are used to handle file uploads with form parameter binding. The IFormFile interface represents a file sent with the HttpRequest, and IFormFileCollection is a collection of IFormFile objects.

``` csharp
public record UploadBookRequest(string Title, [FromForm] string Author, IFormFile BookFile);

public record UploadBookResponse(string FileName, long FileSize);

internal class UploadBookRequestValidator : AbstractValidator<UploadBookRequest>
{
  public UploadBookRequestValidator()
  {
    RuleFor(x => x.Title).NotEmpty();
    RuleFor(x => x.Author).NotEmpty();
    RuleFor(x => x.BookFile).NotEmpty();
  }
}

internal class UploadBook
  : WebResultEndpoint<UploadBookRequest, UploadBookResponse>
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    EndpointConfigurationContext configurationContext)
  {
    builder.MapPost("/upload/{Title}")
      .DisableAntiforgery()
      .Produces<UploadBookResponse>();
  }

  protected override Task<WebResult<UploadBookResponse>> HandleAsync(
    UploadBookRequest req,
    CancellationToken ct)
  {
    // Process file upload
    // ...
    //

    return Task.FromResult(
      WebResults.FromResult(
        new UploadBookResponse(
          req.BookFile.FileName,
          req.BookFile.Length))
      .AsBase());
  }
}
```
In this example, the `UploadBookRequest` record includes a file upload parameter `IFormFile BookFile`. The `UploadBook` endpoint handles the file upload and returns a response with the file name and size.

>**Note**: The `DisableAntiforgery` method is used to disable CSRF protection for this endpoint. The default behavior of ASP.NET Core is to require an antiforgery token for Minimal API endpoints that bind a parameter from the form via `IFormFile` or `IFormFileCollection` and an exception is thrown at startup if the anti-forgery middleware isn't registered for an API that defines these input types. You should be cautious when disabling CSRF protection and ensure that your application is secure against CSRF attacks.

## File Downloads
Returning `Results.File()` or `Results.Stream()` from a `MinimalEndpoint` can be used to return files from an endpoint. The file can be a physical file on disk or a stream of data.

``` csharp
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

internal class DownloadCustomers(ServiceDbContext db)
  : MinimalEndpoint<DownloadCustomersRequest, IResult>
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    EndpointConfigurationContext configurationContext)
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
```
In this example, the `DownloadCustomers` endpoint returns a stream of customer data as a text file. The `Results.Stream()` method is used to write the data to the response stream, and the `fileDownloadName` parameter specifies the name of the file to be downloaded.