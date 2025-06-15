// Data/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Usuario { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure the User entity to match the table and enforce unique email
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Usuario");
            entity.HasIndex(e => e.Email).IsUnique();
        });
    }
}