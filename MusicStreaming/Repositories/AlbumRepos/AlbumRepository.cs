using Azure;
using BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.GenericRepos;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.AlbumRepos
{
    public class AlbumRepository : GenericRepository<Album>, IAlbumRepository
    {
        private readonly MusicStreamingContext _context;

        public AlbumRepository(MusicStreamingContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Album>> GetAlbums(int? page, int? size)
        {
            try
            {
                var pageIndex = (page.HasValue && page > 0) ? page.Value : 1;
                var sizeIndex = (size.HasValue && size > 0) ? size.Value : 10;

                return await _context.Albums.Include(x => x.Artist)
                    .Skip((pageIndex - 1) * sizeIndex)
                    .Take(sizeIndex)
                    .ToListAsync();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Album> GetAlbumsByAlbumId(int albumId)
        {
            return await _context.Albums.Include(x => x.Artist).ThenInclude(x => x.User).Where(x => x.AlbumId == albumId).FirstOrDefaultAsync();
        }

        public async Task<List<Album>> GetAlbumsByAlbumIds(List<int> albumIds)
        {
            return await _context.Albums.Include(x => x.Artist).Where(x => albumIds.Contains(x.AlbumId)).ToListAsync();
        }

        public async Task<List<Album>> GetAlbumsByArtist(int artistId, int? page, int? size)
        {
            try
            {
                var pageIndex = (page.HasValue && page > 0) ? page.Value : 1;
                var sizeIndex = (size.HasValue && size > 0) ? size.Value : 10;

                return await _context.Albums.Include(x => x.Artist)
                    .Where(x => x.ArtistId == artistId)
                    .Skip((pageIndex - 1) * sizeIndex)
                    .Take(sizeIndex)
                    .ToListAsync();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Album>> GetAlbumsByArtist(int artistId)
        {
            try
            {
                return await _context.Albums.Include(x => x.Artist)
                    .Where(x => x.ArtistId == artistId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Album>> GetAlbumsByGenre(string genre, int? page, int? size)
        {
            try
            {
                var pageIndex = (page.HasValue && page > 0) ? page.Value : 1;
                var sizeIndex = (size.HasValue && size > 0) ? size.Value : 10;

                return await _context.Albums.Include(x => x.Artist)
                    .Where(x => x.Genre.ToLower().Equals(genre.ToLower()))
                    .Skip((pageIndex - 1) * sizeIndex)
                    .Take(sizeIndex)
                    .ToListAsync();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Album>> GetRecommendAlbumsForUser(int userId, int? page, int? size)
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

                var recommendAlbum = await _context.Albums
                    .Include(x => x.Artist)
                    .Where(x => genre.Contains(x.Genre) ||
                artistId.Contains((int)x.ArtistId)).ToListAsync();

                return recommendAlbum;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Album>> SearchByAlbumName(string albumName, int? page, int? size)
        {
            try
            {
                var pageIndex = (page.HasValue && page > 0) ? page.Value : 1;
                var sizeIndex = (size.HasValue && size > 0) ? size.Value : 10;

                return await _context.Albums.Include(x => x.Artist).Where(x => x.Title.ToLower().Contains(albumName.ToLower()))
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
