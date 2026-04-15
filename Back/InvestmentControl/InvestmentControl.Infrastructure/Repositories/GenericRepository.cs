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
        var trackedEntity = GetTrackedEntity(entity);

        if (trackedEntity != null)
        {
            // Обновляем уже отслеживаемую сущность
            _context.Entry(trackedEntity).CurrentValues.SetValues(entity);
        }
        else
        {
            // Если нет отслеживаемой, прикрепляем новую как Modified
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }
    }

    public virtual void Delete(TDomain domain)
    {
        var entity = MapToEntity(domain);
        var trackedEntity = GetTrackedEntity(entity);

        if (trackedEntity != null)
        {
            _dbSet.Remove(trackedEntity);
        }
        else
        {
            _dbSet.Attach(entity);
            _dbSet.Remove(entity);
        }
    }

    public virtual async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    protected abstract TDomain MapToDomain(TEntity entity);
    protected abstract TEntity MapToEntity(TDomain domain);

    // Находит уже отслеживаемую сущность по Id
    private TEntity? GetTrackedEntity(TEntity entity)
    {
        var entityId = GetEntityId(entity);
        if (entityId == null) return null;

        // Ищем в локальном кэше
        var tracked = _dbSet.Local.FirstOrDefault(e => GetEntityId(e)?.Equals(entityId) == true);
        if (tracked != null) return tracked;

        // Если не нашли, пробуем найти через ChangeTracker (более надёжно)
        foreach (var entry in _context.ChangeTracker.Entries<TEntity>())
        {
            var id = GetEntityId(entry.Entity);
            if (id?.Equals(entityId) == true)
                return entry.Entity;
        }

        return null;
    }

    private static object? GetEntityId(TEntity entity)
    {
        var property = typeof(TEntity).GetProperty("Id");
        return property?.GetValue(entity);
    }
}