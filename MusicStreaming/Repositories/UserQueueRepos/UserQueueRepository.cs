using BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.GenericRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.UserQueueRepos
{
    public class UserQueueRepository : GenericRepository<UserQueue>, IUserQueueRepository
    {
        private readonly MusicStreamingContext _context;

        public UserQueueRepository(MusicStreamingContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<UserQueue>> GetUserQueue(int userId, int? page, int? size)
        {
            var pageIndex = (page.HasValue && page > 0) ? page.Value : 1;
            var sizeIndex = (size.HasValue && size > 0) ? size.Value : 10;

            return await _context.UserQueues.Where(x => x.UserId == userId)
                .Skip((pageIndex - 1) * sizeIndex)
                .Take(sizeIndex)
                .ToListAsync();
        }

        public async Task<List<UserQueue>> GetUserQueue(int userId)
        {
            return await _context.UserQueues.Where(x => x.UserId == userId)
                .ToListAsync();
        }

        public async Task<List<UserQueue>> GetUserQueueBySongIds(List<int> songIds)
        {
            return await _context.UserQueues.Where(x => songIds.Contains(x.SongId)).ToListAsync();
        }
    }
}
