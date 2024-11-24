using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace GraphQlDotNetCore;

public class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Existing configurations (e.g., connection string) go here
        optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var ids = new Guid[] { Guid.NewGuid(), Guid.NewGuid() };

        modelBuilder.ApplyConfiguration(new OwnerContextConfiguration(ids));
        modelBuilder.ApplyConfiguration(new AccountContextConfiguration(ids));
    }

    public DbSet<Owner> Owners { get; set; }
    public DbSet<Account> Accounts { get; set; }

}