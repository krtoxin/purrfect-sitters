using System.Reflection;
using Domain.Users;
using Domain.Owners;
using Domain.Sitters;
using Domain.Pets;
using Domain.Bookings;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; init; }
    public DbSet<OwnerProfile> OwnerProfiles { get; init; }
    public DbSet<SitterProfile> SitterProfiles { get; init; }
    public DbSet<Pet> Pets { get; init; }
    public DbSet<Booking> Bookings { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}