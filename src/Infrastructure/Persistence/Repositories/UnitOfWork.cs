using Application.Common.Interfaces;

namespace Infrastructure.Persistence.Repositories;
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _db;
    public UnitOfWork(ApplicationDbContext db) => _db = db;
    public Task<int> SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);

    public bool IsTracked(object entity)
    {
        return _db.ChangeTracker.Entries().Any(e => e.Entity == entity && e.State != Microsoft.EntityFrameworkCore.EntityState.Detached);
    }
}