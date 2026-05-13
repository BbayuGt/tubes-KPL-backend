using System.Linq.Expressions;

namespace tubes_KPL_backend.Repositories;

public interface IGenericRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<T?> GetByExpression(Expression<Func<T, bool>>  query);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> query);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task SaveChangesAsync();
}