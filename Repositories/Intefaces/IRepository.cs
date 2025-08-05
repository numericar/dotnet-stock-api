using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace StockAPI.Repositories.Intefaces;

public interface IRepository<T> where T : class
{
    public DbSet<T> DbSet { get; }
    IQueryable<T> Find(Expression<Func<T, bool>> expression);
    Task<T?> FindById<N>(N id);
    Task Add(T entity);
    Task AddRange(IEnumerable<T> entities);
    void Update(T entity);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
    Task SaveChangeAsync();
}
