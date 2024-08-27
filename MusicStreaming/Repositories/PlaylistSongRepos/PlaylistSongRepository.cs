using BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.GenericRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.PlaylistSongRepos
{
    public class PlaylistSongRepository : GenericRepository<PlaylistSong>, IPlaylistSongRepository
    {
        private readonly MusicStreamingContext _context;

        public PlaylistSongRepository(MusicStreamingContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PlaylistSong> CheckSongExistingInPlaylist(int songId, int playlistId)
        {
            try
            {
                var currPlaylistSong = await _context.PlaylistSongs.Where(x => x.SongId == songId && x.PlaylistId == playlistId).FirstOrDefaultAsync();

                return currPlaylistSong;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<PlaylistSong>> GetPlaylistSongsByPlaylistId(int playlistId)
        {
            try
            {
                return await _context.PlaylistSongs.Where(x => x.PlaylistId == playlistId).ToListAsync();
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<PlaylistSong>> GetPlaylistSongsBySongId(int songId)
        {
            try
            {
                return await _context.PlaylistSongs.Where(x => x.SongId == songId).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
