using Microsoft.EntityFrameworkCore;
using InvestmentControl.Domain.Interfaces;

namespace InvestmentControl.Infrastructure.Repositories;

public abstract class GenericRepository<TEntity, TDomain, TContext> : IRepository<TDomain>
    where TEntity : class
    where TDomain : class
    where TContext : DbContext
{
    protected readonly TContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    protected GenericRepository(TContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public virtual async Task<TDomain?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        return entity == null ? null : MapToDomain(entity);
    }

    public virtual async Task<IEnumerable<TDomain>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _dbSet.ToListAsync(cancellationToken);
        return entities.Select(MapToDomain);
    }

    public virtual async Task AddAsync(TDomain domain, CancellationToken cancellationToken = default)
    {
        var entity = MapToEntity(domain);
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public virtual void Update(TDomain domain)
    {
        var entity = MapToEntity(domain);
        _dbSet.Update(entity);
    }

    public virtual void Delete(TDomain domain)
    {
        var entity = MapToEntity(domain);
        _dbSet.Remove(entity);
    }

    public virtual async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    protected abstract TDomain MapToDomain(TEntity entity);
    protected abstract TEntity MapToEntity(TDomain domain);
}