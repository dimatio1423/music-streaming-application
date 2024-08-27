using BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.GenericRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.UserRepos
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly MusicStreamingContext _context;

        public UserRepository(MusicStreamingContext context) : base(context)
        {
            _context = context;
        }

        public async Task<User> GetUserByEmail(string Email)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Email.Equals(Email));
        }

        public async Task<User> GetUserByUsername(string Username)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Username.Equals(Username));
        }
    }
}
