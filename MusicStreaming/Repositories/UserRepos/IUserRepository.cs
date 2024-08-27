using BusinessObjects.Entities;
using Repositories.GenericRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.UserRepos
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User> GetUserByEmail(string Email);
        Task<User> GetUserByUsername(string Username);
    }
}
