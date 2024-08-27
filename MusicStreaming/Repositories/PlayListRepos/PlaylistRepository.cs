using BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.GenericRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.PlayListRepos
{
    public class PlaylistRepository : GenericRepository<Playlist>, IPlaylistRepository
    {
        private readonly MusicStreamingContext _context;

        public PlaylistRepository(MusicStreamingContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Playlist>> GetPlaylistsByUserId(int userId, int? page, int? size)
        {
            var pageIndex = (page.HasValue && page > 0) ? page.Value : 1;
            var sizeIndex = (size.HasValue && size > 0) ? size.Value : 10;

            return await _context.Playlists.Include(x => x.User).Where(x => x.UserId == userId)
                .Skip((pageIndex - 1) * sizeIndex)
                .Take(sizeIndex)
                .ToListAsync();
        }
    }
}
