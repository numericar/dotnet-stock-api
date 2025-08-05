using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using StockAPI.Contexts;
using StockAPI.Repositories.Intefaces;

namespace StockAPI.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext dbContext;

    public DbSet<T> DbSet { get; private set; }

    public Repository(AppDbContext mySQLDbContext)
    {
        this.dbContext = mySQLDbContext;
        this.DbSet = mySQLDbContext.Set<T>();
    }

    public IQueryable<T> Find(Expression<Func<T, bool>> expression)
    {
        try
        {
            return this.DbSet.Where(expression);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<T?> FindById<N>(N id)
    {
        try
        {
            return await this.DbSet.FindAsync(id);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task Add(T entity)
    {
        try
        {
            await this.DbSet.AddAsync(entity);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task AddRange(IEnumerable<T> entities)
    {
        try
        {
            await this.DbSet.AddRangeAsync(entities);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public void Update(T entity)
    {
        try
        {
            this.DbSet.Update(entity);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public void Remove(T entity)
    {
        try
        {
            this.DbSet.Remove(entity);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public void RemoveRange(IEnumerable<T> entities)
    {
        try
        {
            this.DbSet.RemoveRange(entities);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task SaveChangeAsync()
    {
        try
        {
            await this.dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
