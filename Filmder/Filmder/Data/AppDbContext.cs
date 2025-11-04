using Microsoft.EntityFrameworkCore;
using Filmder.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Filmder.Data;

public class AppDbContext(DbContextOptions options) : IdentityDbContext<AppUser>(options)
{
    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<GroupMember> GroupMembers { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Rating> Ratings { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<MovieScore> MovieScores { get; set; }
    public DbSet<SwipeHistory> SwipeHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .Entity<Movie>()
            .Property(m => m.Genre)
            .HasConversion<string>();
    }
}