using Microsoft.EntityFrameworkCore;
using PurchaseApi.Infrastructure.Data;
using PurchaseApi.Infrastructure.Data.Interfaces;
using System.Linq.Expressions;

namespace PurchaseApi.Data.Repositories
{
    public class Repository<T> : IRepository<T>, IDisposable where T : class //Dispose kullanmamıza gerek yok DI ile bu otomatik olarak gerçekleştiriliyor sadece örnek amaçlı..
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;
        private bool _disposed = false;
        
        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();  // Generic entity set'i oluştur bu sayede tek bir yapı ile birden fazla tabloya ulaşıbabilir.
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await SaveChangesAsync();
        }

        public async Task Update(T entity)
        {
            _dbSet.Update(entity);
            await SaveChangesAsync();
        }

        public async Task Delete(T entity)
        {
            _dbSet.Remove(entity);
            await SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) // koşullu sorgu bu sayede tek bir metot ile her türlü sorguyu gerçekleştirebiliriz, tablo farketmeksizin.
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        // IDisposable implementasyonu
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Managed kaynakları serbest bırak
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
