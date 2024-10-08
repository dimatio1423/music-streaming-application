﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.GenericRepos
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> Get(int id);
        Task<List<T>> GetAll(int? page, int? size);
        Task Insert(T entity);
        Task Update(T entity);
        Task Remove(T entity);
        Task AddRange(List<T> entities);
        Task UpdateRange(List<T> entities);
        Task DeleteRange(List<T> entities);
    }
}
