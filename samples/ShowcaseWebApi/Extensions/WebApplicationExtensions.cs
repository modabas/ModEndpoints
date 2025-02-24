using ShowcaseWebApi.Data;
using ShowcaseWebApi.Features.Books.Data;
using ShowcaseWebApi.Features.Customers.Data;
using ShowcaseWebApi.Features.Stores.Data;

namespace ShowcaseWebApi.Extensions;

public static class WebApplicationExtensions
{
  public static void SeedData(this WebApplication app)
  {
    using (var scope = app.Services.CreateScope())
    {
      var db = scope.ServiceProvider.GetRequiredService<ServiceDbContext>();
      db.Books.Add(new BookEntity()
      {
        Author = "L. Frank Baum",
        Price = 5.99m,
        Title = "Wizard Of Oz"
      });
      db.Books.Add(new BookEntity()
      {
        Author = "Who Knows",
        Price = 7.95m,
        Title = "Endpoints for Dummies"
      });
      db.Books.Add(new BookEntity()
      {
        Author = "Satoshi Nakamoto",
        Price = 5.95m,
        Title = "Crypto for Dummies"
      });
      db.Stores.Add(new StoreEntity()
      {
        Name = "Lesser Evil Store"
      });
      db.Stores.Add(new StoreEntity()
      {
        Name = "Big Bad Store"
      });
      db.Stores.Add(new StoreEntity()
      {
        Name = "Middling Evil Store"
      });
      db.Customers.Add(new CustomerEntity()
      {
        FirstName = "Willie",
        MiddleName = "Jonathan",
        LastName = "Normand"
      });
      db.Customers.Add(new CustomerEntity()
      {
        FirstName = "Leslie",
        MiddleName = "Lois",
        LastName = "Coffman"
      });
      db.Customers.Add(new CustomerEntity()
      {
        FirstName = "Oliver",
        LastName = "Rogers"
      });
      db.SaveChanges();
    }
  }
}
