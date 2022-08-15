using Contracts.Common.Interfaces;
using Contracts.Domains;
using Contracts.Domains.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;


namespace Infrastructure.Common
{
    public class RepositoryBase<T, K, TContext> : RepositoryQueryBase<T, K, TContext>,
     IRepositoryBaseAsync<T, K, TContext>
     where T : EntityBase<K>
     where TContext : DbContext
    {
        private readonly TContext _dbContext;
        private readonly IUnitOfWork<TContext> _unitOfWork;

        public RepositoryBase(TContext dbContext, IUnitOfWork<TContext> unitOfWork) : base(dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public Task<IDbContextTransaction> BeginTransactionAsync() => _dbContext.Database.BeginTransactionAsync();

        public async Task EndTransactionAsync()
        {
            await SaveChangesAsync();
            await _dbContext.Database.CommitTransactionAsync();
        }

        public Task RollbackTransactionAsync() => _dbContext.Database.RollbackTransactionAsync();

        public async Task<K> CreateAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
            await SaveChangesAsync();
            return entity.Id;
        }

        public async Task<IList<K>> CreateListAsync(IEnumerable<T> entities)
        {
            await _dbContext.Set<T>().AddRangeAsync(entities);
            await SaveChangesAsync();
            return entities.Select(x => x.Id).ToList();
        }

        public async Task UpdateAsync(T entity)
        {
            if (_dbContext.Entry(entity).State == EntityState.Unchanged) return;

            T exist = _dbContext.Set<T>().Find(entity.Id);
            _dbContext.Entry(exist).CurrentValues.SetValues(entity);
            await SaveChangesAsync();
            return;
        }

        public Task UpdateListAsync(IEnumerable<T> entities) => _dbContext.Set<T>().AddRangeAsync(entities);

        public async Task DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            await SaveChangesAsync();
            return;
        }
        public void Delete(T entity) => _dbContext.Set<T>().Remove(entity);

        public async Task DeleteListAsync(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().RemoveRange(entities);
            await SaveChangesAsync();
            return;
        }

        public Task<int> SaveChangesAsync() => _unitOfWork.CommitAsync();

        public void Create(T entity)
        {
            _dbContext.Set<T>().Add(entity);

        }

        public IList<K> CreateList(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().AddRange(entities);
            return entities.Select(x => x.Id).ToList();
        }

        public void Update(T entity)
        {
            if (_dbContext.Entry(entity).State == EntityState.Unchanged) return;

            T exist = _dbContext.Set<T>().Find(entity.Id);
            _dbContext.Entry(exist).CurrentValues.SetValues(entity);

        }
    }
}
