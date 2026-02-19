using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Infrastructure.Data;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

//Repo Implementation
public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
{
    //injecting and implementing based on DB provider
    private StoreDatabaseContext _context = null;
    public GenericRepository(StoreDatabaseContext context)
    {
        _context = context;
    }
   
    //public int SaveChanges() => _context.SaveChanges();

    public async Task<IReadOnlyList<TEntity>> ListAllAsync() 
        => await _context.Set<TEntity>().ToListAsync();
    

    public async Task<TEntity> GetByIdAsync(int id)
        => await _context.Set<TEntity>().FindAsync(id);
    

    public async Task<TEntity> GetEntityWithSpec(ISpecification<TEntity> spec)
        => await ApplySpecification(spec).FirstOrDefaultAsync();
    

    public async Task<IReadOnlyList<TEntity>> ListAsync(ISpecification<TEntity> spec)
        => await ApplySpecification(spec).ToListAsync();

    public async Task<int> CountAsync(ISpecification<TEntity> spec)
        => await ApplySpecification(spec).CountAsync();

    private IQueryable<TEntity> ApplySpecification (ISpecification<TEntity> specification)
        => SpecificationEvaluator<TEntity>.GetQuery(_context.Set<TEntity>().AsQueryable(), specification);

    public void Add(TEntity entity)
    {
        _context.Set<TEntity>().Add(entity);
        _context.SaveChanges();
    }

    public void Update(TEntity entity)
    {
        _context.Set<TEntity>().Attach(entity);
        _context.Entry(entity).State= EntityState.Modified;
        _context.SaveChanges();
    }

    public void Delete(TEntity entity)
    {
        _context.Set<TEntity>().Remove(entity);
        _context.SaveChanges();

    }
}



