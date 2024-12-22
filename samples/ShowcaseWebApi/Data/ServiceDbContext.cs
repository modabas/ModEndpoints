using Microsoft.EntityFrameworkCore;
using ShowcaseWebApi.Features.Books.Data;
using ShowcaseWebApi.Features.Stores.Data;

namespace ShowcaseWebApi.Data;

internal class ServiceDbContext(DbContextOptions<ServiceDbContext> options)
    : DbContext(options)
{
  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);

    builder.ApplyConfigurationsFromAssembly(typeof(ServiceDbContext).Assembly);
  }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    base.OnConfiguring(optionsBuilder);
    optionsBuilder.UseInMemoryDatabase("SampleDb");
  }

  #region DbSets
  public DbSet<BookEntity> Books => Set<BookEntity>();
  public DbSet<StoreEntity> Stores => Set<StoreEntity>();
  #endregion
}
