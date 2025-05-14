using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComputerStore.Domain.Common;
using ComputerStore.Domain.Entities;
using ComputerStore.Domain.Entities.Base;
using System.Linq.Expressions;

namespace ComputerStore.Tests.Unit.Common
{
    public class MockRepository<T> : IRepository<T> where T : Entity
    {
        protected readonly List<T> _entities = new();
        protected int _nextId = 1;

        public Task<T?> GetByIdAsync(int id)
        {
            return Task.FromResult(_entities.FirstOrDefault(e => e.Id == id));
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<T>>(_entities.ToList());
        }

        public Task AddAsync(T entity)
        {
            entity.Id = _nextId++;
            _entities.Add(entity);
            return Task.CompletedTask;
        }

        public void Update(T entity)
        {
            var existing = _entities.FirstOrDefault(e => e.Id == entity.Id);
            if (existing != null)
            {
                _entities.Remove(existing);
                _entities.Add(entity);
            }
        }

        public void Delete(T entity)
        {
            _entities.Remove(entity);
        }

        public void SeedData(IEnumerable<T> entities)
        {
            _entities.Clear();
            _entities.AddRange(entities);
            _nextId = _entities.Max(e => e.Id) + 1;
        }

        public int Count => _entities.Count;

        public Task<T?> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return Task.FromResult(_entities.AsQueryable().FirstOrDefault(predicate));
        }

        public Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> predicate)
        {
            return Task.FromResult(_entities.AsQueryable().Where(predicate).AsEnumerable());
        }
    }
}