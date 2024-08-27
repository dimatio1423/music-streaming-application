using BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.GenericRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.ArtistSongRepos
{
    public class ArtistSongRepository : GenericRepository<ArtistSong>, IArtistSongRepository
    {
        private readonly MusicStreamingContext _context;

        public ArtistSongRepository(MusicStreamingContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ArtistSong> CheckArtistSongExisting(int artistId, int songId)
        {
            return await _context.ArtistSongs.Where(x => x.ArtistId == artistId && x.SongId == songId).FirstOrDefaultAsync();
        }

        public async Task<List<ArtistSong>> GetArtistSongBySongId(int songId)
        {
            return await _context.ArtistSongs.Where(x => x.SongId == songId).ToListAsync();
        }
    }
}
