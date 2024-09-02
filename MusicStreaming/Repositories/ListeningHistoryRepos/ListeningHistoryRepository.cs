using BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.GenericRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.ListeningHistoryRepos
{
    public class ListeningHistoryRepository : GenericRepository<ListeningHistory>, IListeningHistoryRepository
    {
        private readonly MusicStreamingContext _context;

        public ListeningHistoryRepository(MusicStreamingContext context) : base(context)
        {
            _context = context;

        }

        public async Task<List<ListeningHistory>> GetListeningHistoriesBySongId(int songId)
        {
            return await _context.ListeningHistories.Where(x => x.SongId == songId).ToListAsync();
        }

        public async Task<List<ListeningHistory>> GetListeningHistoriesBySongIds(List<int> songIds)
        {
            return await _context.ListeningHistories.Where(x => songIds.Contains((int)x.SongId)).ToListAsync();
        }

        public async Task<List<ListeningHistory>> GetListeningHistoriesByUser(int userId, int? page, int? size)
        {
            var pageIndex = (page.HasValue && page > 0) ? page.Value : 1;
            var sizeIndex = (size.HasValue && size > 0) ? size.Value : 10;

            return await _context.ListeningHistories.Where(x => x.UserId == userId)
                .Skip((pageIndex - 1) * sizeIndex)
                .Take(sizeIndex)
                .ToListAsync();
        }

        public async Task<List<ListeningHistory>> GetListeningHistoriesByUser(int userId)
        {
            return await _context.ListeningHistories.Where(x => x.UserId == userId)
                .ToListAsync();
        }

        public async Task<ListeningHistory> GetListeningHistoryByUserIdAndSongId(int userId, int songId)
        {
            return await _context.ListeningHistories.Where(x => x.UserId == userId && x.SongId == songId).FirstOrDefaultAsync();
        }
    }
}
