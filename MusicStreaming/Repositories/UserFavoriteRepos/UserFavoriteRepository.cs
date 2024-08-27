using BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.GenericRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.UserFavoriteRepos
{
    public class UserFavoriteRepository : GenericRepository<UserFavorite>, IUserFavoriteRepository
    {
        private readonly MusicStreamingContext _context;

        public UserFavoriteRepository(MusicStreamingContext context) : base(context)
        {
            _context = context;
        }

        public async Task<UserFavorite> CheckSongInUserFavorite(int songId, int userId)
        {
            return await _context.UserFavorites.Where(x => x.SongId == songId && x.UserId == userId).FirstOrDefaultAsync();
        }

        public async Task<List<UserFavorite>> GetUserFavoriteBySongId(int songId)
        {
            return await _context.UserFavorites.Where(x => x.SongId == songId).ToListAsync();
        }

        public async Task<List<UserFavorite>> GetUserFavorites(int userId, int? page, int? size)
        {
            var pageIndex = (page.HasValue && page > 0) ? page.Value : 1;
            var sizeIndex = (size.HasValue && size > 0) ? size.Value : 10;

            return await _context.UserFavorites.Where(x => x.UserId == userId)
                .Skip((pageIndex - 1) * sizeIndex)
                .Take(sizeIndex)
                .ToListAsync();
        }
    }
}
