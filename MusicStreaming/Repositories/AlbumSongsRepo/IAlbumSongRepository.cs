using BusinessObjects.Entities;
using Repositories.GenericRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.AlbumSongsRepo
{
    public interface IAlbumSongRepository : IGenericRepository<AlbumSong>
    {
        Task<List<int>> GetAlbumsOfSong(int songId);
        Task<int> GetLatestTrackNumberOfAnAlbum(int albumId);
        Task<AlbumSong> CheckSongExistingInAlbum(int songId, int albumId);
        Task<List<AlbumSong>> GetAlbumSongsByAlbumId(int albumId);
        Task<List<AlbumSong>> GetAlbumSongsBySongId(int songId);
    }
}
