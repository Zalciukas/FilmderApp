using Microsoft.EntityFrameworkCore;
using Filmder.Models;

namespace Filmder.Data;

public class AppDbContext : DbContext
{
    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<Movie> Movies { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=filmder.db");
    }
}