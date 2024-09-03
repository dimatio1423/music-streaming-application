using BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.GenericRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.SongRepos
{
    public class SongRepository : GenericRepository<Song>, ISongRepository
    {
        private readonly MusicStreamingContext _context;

        public SongRepository(MusicStreamingContext context) : base(context)
        {
            _context = context;
        }

        public async Task<int> AddSong(Song song)
        {
            try
            {
                await _context.Songs.AddAsync(song);
                await _context.SaveChangesAsync();

                return song.SongId;
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Song>> GetRecommendSongsForUser(int userId, int? page, int? size)
        {
            try
            {
                var pageIndex = (page.HasValue && page > 0) ? page.Value : 1;
                var sizeIndex = (size.HasValue && size > 0) ? size.Value : 10;

                var artistId = await _context.ListeningHistories
                    .Join(_context.Songs,
                        history => history.SongId,
                        song => song.SongId,
                        (history, song) => new { history, song })
                    .Join(_context.AlbumSongs,
                        hs => hs.song.SongId,
                        albumSong => albumSong.SongId,
                        (hs, albumSong) => new { hs.history, hs.song, albumSong })
                    .Join(_context.Albums,
                        hsa => hsa.albumSong.AlbumId,
                        album => album.AlbumId,
                        (hsa, album) => new { hsa.history, hsa.song, hsa.albumSong, album })
                    .Where(x => x.history.UserId == userId)
                    .Select(x => x.album.ArtistId)
                    .Distinct()
                    .ToListAsync();

                var genre = await _context.ListeningHistories
                    .Join(_context.Songs,
                        history => history.SongId,
                        song => song.SongId,
                        (history, song) => new { history, song })
                    .Join(_context.AlbumSongs,
                        hs => hs.song.SongId,
                        albumSong => albumSong.SongId,
                        (hs, albumSong) => new { hs.history, hs.song, albumSong })
                    .Join(_context.Albums,
                        hsa => hsa.albumSong.AlbumId,
                        album => album.AlbumId,
                        (hsa, album) => new { hsa.history, hsa.song, hsa.albumSong, album })
                    .Where(x => x.history.UserId == userId)
                    .Select(x => x.album.Genre)
                    .Distinct()
                    .ToListAsync();

                var recommendAlbums = await _context.Albums
                    .Include(x => x.Artist)
                    .Where(x => genre.Contains(x.Genre) ||
                artistId.Contains(x.ArtistId))
                    .Select(x => x.AlbumId)
                    .ToListAsync();

                return await _context.Songs.Include(x => x.AlbumSongs)
                    .Where(x => x.AlbumSongs.Any(x => recommendAlbums.Contains(x.AlbumId)))
                    .ToListAsync();


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Song>> GetSongs(int? page, int? size)
        {
            try
            {

                var pageIndex = (page.HasValue && page > 0) ? page.Value : 1;
                var sizeIndex = (size.HasValue && size > 0) ? size.Value : 10;

                    return await _context.Songs
                    .Include(x => x.ArtistSongs).ThenInclude(x => x.Artist)
                        .Skip((pageIndex - 1) * sizeIndex).Take(sizeIndex).ToListAsync();
                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Song>> GetSongsByAlbum(int albumId, int? page, int? size)
        {
            try
            {
                var pageIndex = (page.HasValue && page > 0) ? page.Value : 1;
                var sizeIndex = (size.HasValue && size > 0) ? size.Value : 10;

                return await _context.Songs.Include(x => x.AlbumSongs)
                    .Where(x => x.AlbumSongs.Any(x => x.AlbumId == albumId))
                    .Skip((pageIndex - 1) * sizeIndex)
                    .Take(sizeIndex)
                    .ToListAsync();
                
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Song>> GetSongsByArtist(int artistId, int? page, int? size)
        {
            try
            {
                var pageIndex = (page.HasValue && page > 0) ? page.Value : 1;
                var sizeIndex = (size.HasValue && size > 0) ? size.Value : 10;

                //return await _context.Songs.Include(x => x.AlbumSongs)
                //    .Join(_context.AlbumSongs,
                //        song => song.SongId,
                //        albumSong => albumSong.SongId,
                //        (song, albumSong) => new { song, albumSong })
                //    .Join(_context.Albums,
                //        sa => sa.albumSong.AlbumId,
                //        album => album.AlbumId,
                //        (sa, album ) => new {sa.song, sa.albumSong, album})
                //    //.Join(_context.Artists,
                //    //    sa => sa.album.ArtistId,
                //    //    artist => artist.ArtistId,
                //    //    (sa, artist) => new {sa.song, sa.album, artist})
                //    .Where(x => x.album.ArtistId == artistId)
                //    .Skip((pageIndex - 1) * sizeIndex)
                //    .Take(sizeIndex)
                //    .Select(x => x.song)
                //    .ToListAsync();

                return await _context.Songs.Include(x => x.ArtistSongs)
                             .Join(_context.ArtistSongs,
                                song => song.SongId,
                                artistSong => artistSong.SongId,
                                (song, artistSong) => new {song, artistSong})
                             .Join(_context.Artists,
                                sas => sas.artistSong.ArtistId,
                                artist => artist.ArtistId,
                                (sas, artist) => new {sas, artist})
                  .Where(x => x.artist.ArtistId == artistId)
                  .Skip((pageIndex - 1) * sizeIndex)
                  .Take(sizeIndex)
                  .Select(x => x.sas.song)
                  .ToListAsync();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Song>> GetSongsByArtist(int artistId)
        {
            try
            {
                return await _context.Songs.Include(x => x.ArtistSongs)
                             .Join(_context.ArtistSongs,
                                song => song.SongId,
                                artistSong => artistSong.SongId,
                                (song, artistSong) => new { song, artistSong })
                             .Join(_context.Artists,
                                sas => sas.artistSong.ArtistId,
                                artist => artist.ArtistId,
                                (sas, artist) => new { sas, artist })
                  .Where(x => x.artist.ArtistId == artistId)
                  .Select(x => x.sas.song)
                  .ToListAsync();
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Song>> GetUserFavoriteSongs(List<int> songIds, int? page, int? size)
        {
            try
            {
                var pageIndex = (page.HasValue && page > 0) ? page.Value : 1;
                var sizeIndex = (size.HasValue && size > 0) ? size.Value : 10;

                return await _context.Songs.Include(x => x.AlbumSongs)
                    .Where(x => songIds.Contains(x.SongId))
                    .Skip((pageIndex - 1) * sizeIndex)
                    .Take(sizeIndex)
                    .ToListAsync();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Song>> GetUserListeningHisotry(List<int> songIds, int? page, int? size)
        {
            try
            {
                var pageIndex = (page.HasValue && page > 0) ? page.Value : 1;
                var sizeIndex = (size.HasValue && size > 0) ? size.Value : 10;

                return await _context.Songs.Include(x => x.AlbumSongs)
                    .Where(x => songIds.Contains(x.SongId))
                    .Skip((pageIndex - 1) * sizeIndex)
                    .Take(sizeIndex)
                    .ToListAsync();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Song>> GetUserQueueSongs(List<int> songIds, int? page, int? size)
        {
            try
            {
                var pageIndex = (page.HasValue && page > 0) ? page.Value : 1;
                var sizeIndex = (size.HasValue && size > 0) ? size.Value : 10;

                return await _context.Songs.Include(x => x.AlbumSongs)
                    .Where(x => songIds.Contains(x.SongId))
                    .Skip((pageIndex - 1) * sizeIndex)
                    .Take(sizeIndex)
                    .ToListAsync();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Song>> SearchBySongName(string songName, int? page, int? size)
        {
            try
            {
                var pageIndex = (page.HasValue && page > 0) ? page.Value : 1;
                var sizeIndex = (size.HasValue && size > 0) ? size.Value : 10;

                return await _context.Songs
                    .Include(x => x.ArtistSongs).ThenInclude(x => x.Artist)
                    .Include(x => x.AlbumSongs).Where(x => x.Title.ToLower().Contains(songName.ToLower()))
                    .Skip((pageIndex - 1) * sizeIndex)
                    .Take(sizeIndex)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
