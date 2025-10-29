using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Persistence
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            optionsBuilder.UseNpgsql("Host=ep-holy-field-agyc3h7m-pooler.c-2.eu-central-1.aws.neon.tech;Port=5432;Database=neondb;Username=neondb_owner;Password=npg_PfWg5hHm1MzZ;SSL Mode=Require;Trust Server Certificate=true");

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
