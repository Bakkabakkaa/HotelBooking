using System.Linq.Expressions;
using WhiteLagoon.Application.Common.Interfaces;

namespace WhiteLagoon.Infrastructure.Repository;

public class Repository<T> : IRepository<T> where T : class
{
    private IRepository<T> _repositoryImplementation;
    public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
    {
        return _repositoryImplementation.GetAll(filter, includeProperties);
    }

    public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null)
    {
        return _repositoryImplementation.Get(filter, includeProperties);
    }

    public void Add(T entity)
    {
        _repositoryImplementation.Add(entity);
    }

    public void Remove(T entity)
    {
        _repositoryImplementation.Remove(entity);
    }
}