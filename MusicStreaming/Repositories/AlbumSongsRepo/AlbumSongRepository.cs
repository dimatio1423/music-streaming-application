using BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.GenericRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.AlbumSongsRepo
{
    public class AlbumSongRepository : GenericRepository<AlbumSong>, IAlbumSongRepository
    {
        private readonly MusicStreamingContext _context;

        public AlbumSongRepository(MusicStreamingContext context) : base (context)
        {
            _context = context;
        }

        public async Task<AlbumSong> CheckSongExistingInAlbum(int songId, int albumId)
        {
            try
            {
                return await _context.AlbumSongs.Where(x => x.AlbumId == albumId && x.SongId == songId).FirstOrDefaultAsync();
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<int>> GetAlbumsOfSong(int songId)
        {
            try
            {
                return await _context.AlbumSongs.Where(x => x.SongId == songId).Select(x => x.AlbumId).ToListAsync();

            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<AlbumSong>> GetAlbumSongsByAlbumId(int albumId)
        {
            try
            {
                return await _context.AlbumSongs.Where(x => x.AlbumId == albumId).ToListAsync();

            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<AlbumSong>> GetAlbumSongsBySongId(int songId)
        {
            try
            {
                return await _context.AlbumSongs.Where(x => x.SongId == songId).ToListAsync();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> GetLatestTrackNumberOfAnAlbum(int albumId)
        {
            try
            {
                return await _context.AlbumSongs
                    .Where(x => x.AlbumId == albumId)
                    .MaxAsync(x => (int?) x.TrackNumber ?? 0);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
