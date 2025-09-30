using Microsoft.EntityFrameworkCore;
using Filmder.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Filmder.Data;

public class AppDbContext(DbContextOptions options) : IdentityDbContext<AppUser>(options)
{
    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Group> Groups { get; set; }
}