using Microsoft.EntityFrameworkCore;
using TestGFL.Models;
using System.Collections.Generic;

public class ApplicationDbContext : DbContext
{
    public DbSet<Catalog> Catalogs { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Catalog>()
            .HasMany(c => c.Children)
            .WithOne()
            .HasForeignKey(c => c.ParentId);

        // Додавання базових сутностей до БД при створенні міграції
        modelBuilder.Entity<Catalog>().HasData(
            new Catalog { Id = 1, Name = "Creating Digital Images", ParentId = null },
            new Catalog { Id = 2, Name = "Resources", ParentId = 1 },
            new Catalog { Id = 3, Name = "Evidence", ParentId = 1 },
            new Catalog { Id = 4, Name = "Graphic Products", ParentId = 1 },
            new Catalog { Id = 5, Name = "Primary Sources", ParentId = 2 },
            new Catalog { Id = 6, Name = "Secondary Sources", ParentId = 2 },
            new Catalog { Id = 7, Name = "Process", ParentId = 4 },
            new Catalog { Id = 8, Name = "Final Product", ParentId = 4 }
        );
    }
}
